using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using SM.API.Services;
using SM.BAL;
using SM.DAL.DataModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Net.Http;
using static SM.API.Controllers.EventsController;
using static SM.BAL.EventHandler;
using static SM.BAL.MemberHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MemberImagesController : SMControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly HttpClient _httpClient;
        public MemberImagesController(ILogger<SMControllerBase> logger, IAmazonS3 s3Client)
        : base(logger)
        {
            _s3Client = s3Client;
            _httpClient = new HttpClient();
        }
        [HttpPost("generate-cards")]
        public async Task<IActionResult> GenerateMemberCards([FromBody] List<int> memberIDs)
        {
            if (memberIDs == null || !memberIDs.Any())
            {
                return BadRequest("Member ID list cannot be empty.");
            }
            List<Member> members;
            using (MemberHandler handler = new MemberHandler())
            {
                members = handler.GetMembersCardData(memberIDs);
            }

            if (members == null || !members.Any())
            {
                return NotFound("No data found for the provided member IDs.");
            }

            // --- Step 2: Setup Temporary Directory for Processing ---
            // Create a unique temporary folder on the server to store the generated images.
            string tempDirectory = Path.Combine(Path.GetTempPath(), $"CardGen_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempDirectory);

            string zipFilePath = string.Empty;

            try
            {
                // Download the base card image ONCE.
                Bitmap baseCardTemplate = await DownloadImageAsync(SMConfigurationManager.BaseImageURL);
                if (baseCardTemplate == null)
                {
                    return StatusCode(500, "Failed to download the base card template from S3.");
                }

                // --- Step 3: Generate Each Card ---
                foreach (var member in members)
                {
                    await GenerateCard(member, baseCardTemplate, tempDirectory);
                }

                // --- Step 4: Zip the Generated Images ---
                zipFilePath = Path.Combine(Path.GetTempPath(), $"Generated_IDs_{Guid.NewGuid()}.zip");
                ZipFile.CreateFromDirectory(tempDirectory, zipFilePath);

                // --- Step 5: Read Zip File into Memory and Return ---
                var memoryStream = new MemoryStream();
                using (var stream = new FileStream(zipFilePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memoryStream);
                }
                memoryStream.Position = 0; // Reset stream position for reading

                return File(memoryStream, "application/zip", "MemberCards.zip");
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
            finally
            {
                // --- Step 6: Cleanup ---
                // This block executes whether the process succeeds or fails.
                // Delete the temporary directory and all its contents (the individual card images).
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
                // Delete the temporary zip file.
                if (System.IO.File.Exists(zipFilePath))
                {
                    System.IO.File.Delete(zipFilePath);
                }
            }
        }
        /// <summary>
        /// Downloads an image from a given URL and returns it as a Bitmap.
        /// </summary>
        private async Task<Bitmap> DownloadImageAsync(string imageUrl)
        {
            try
            {
                var response = await _httpClient.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return new Bitmap(stream);
                }
            }
            catch
            {
                return null; // Return null if download fails
            }
        }

        /// <summary>
        /// Generates a single card image and saves it to the specified output directory.
        /// </summary>
        private async Task GenerateCard(Member member, Bitmap baseCardTemplate, string outputDirectory)
        {
            // Download the member's personal photo
            using (Bitmap personalPhoto = await DownloadImageAsync(member.ImageURL))
            {
                if (personalPhoto == null) return; // Skip card if photo download fails

                string name = TrimName(member.FullName, 17);
                string id = member.Code;
                string outputImagePath = Path.Combine(outputDirectory, $"{id}.jpg");

                // Generate QR code with the modern QRCoder library
                byte[] qrCodeAsBytes;
                using (var qrGenerator = new QRCodeGenerator())
                using (var qrCodeData = qrGenerator.CreateQrCode(id, QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    // Using 25 pixels per module as per original code
                    qrCodeAsBytes = qrCode.GetGraphic(25, new byte[] { 0, 0, 0 }, new byte[] { 255, 255, 255 });
                }

                // Convert byte array to Bitmap for drawing
                using (var ms = new MemoryStream(qrCodeAsBytes))
                using (var qrCodeImage = new Bitmap(ms))
                // Clone the base template to create the final card for this member
                using (Bitmap finalCard = new Bitmap(baseCardTemplate))
                {
                    // --- YOUR PRECISE DRAWING LOGIC STARTS HERE ---

                    // Calculate the position to center the overlay image
                    int qrCodeX = (finalCard.Width - qrCodeImage.Width) / 2;
                    int mid = (finalCard.Height - qrCodeImage.Height) / 2;
                    int qrCodeY = mid + 1100;

                    // Create a graphics object to modify the base image
                    using (Graphics graphics = Graphics.FromImage(finalCard))
                    {
                        // Draw Personal Photo
                        int personalImageX = (finalCard.Width - 1060) / 2;
                        graphics.DrawImage(personalPhoto, personalImageX, 600, 1150, 1150);

                        // Draw the QR Code image onto the final card at the calculated position
                        graphics.DrawImage(qrCodeImage, qrCodeX, qrCodeY, qrCodeImage.Width, qrCodeImage.Height);

                        // NOTE: For custom fonts to work on a server (like your EC2 instance),
                        // they must be installed on the operating system. If they are not present,
                        // the system may substitute a default font.
                        using (Font arFont = new Font("Dubai", 150, FontStyle.Regular, GraphicsUnit.Pixel))
                        using (Font enFont = new Font("Bahnschrift", 140, FontStyle.Regular, GraphicsUnit.Pixel))
                        using (SolidBrush textBrush = new SolidBrush(Color.DarkGreen))
                        {
                            // Measure text widths to center-align
                            SizeF nameSize = graphics.MeasureString(name, arFont);
                            SizeF idSize = graphics.MeasureString(id, enFont);

                            // Calculate text positions
                            int nameX = (finalCard.Width - (int)nameSize.Width) / 2;
                            int nameY = mid + qrCodeImage.Height - 100;
                            int idX = (finalCard.Width - (int)idSize.Width) / 2;
                            int idY = nameY + (int)nameSize.Height + 15; // Place below name

                            // Draw text
                            // Note: Your original code used arFont for both. I'm preserving that.
                            // If the 'id' needs enFont, change 'arFont' to 'enFont' in the second DrawString call.
                            graphics.DrawString(name, arFont, textBrush, new PointF(nameX, nameY));
                            graphics.DrawString(id, arFont, textBrush, new PointF(idX, idY));
                        }
                    }

                    // Save the final composite image to the temporary directory
                    finalCard.Save(outputImagePath, ImageFormat.Jpeg);
                }
            }
        }

        /// <summary>
        /// Helper function to trim a name to a maximum length by removing words from the end.
        /// </summary>
        private string TrimName(string fullName, int maxLength)
        {
            if (string.IsNullOrEmpty(fullName)) return string.Empty;

            // Trim the name by removing last parts
            while (fullName.Length > maxLength)
            {
                int lastSpaceIndex = fullName.LastIndexOf(' ');
                if (lastSpaceIndex == -1)
                {
                    // If no spaces, just truncate the string
                    fullName = fullName.Substring(0, maxLength);
                    break;
                };
                fullName = fullName.Substring(0, lastSpaceIndex);
            }
            return fullName;
        }
    



        [HttpPost("UploadMemberImage")]
        public async Task<IActionResult> UploadMemberImage([FromForm] ImageParams param)
        {
            try
            {
                if (param.ImageFile == null || param.ImageFile.Length == 0)
                    return BadRequest("No file uploaded.");

                if (!param.ImageFile.ContentType.StartsWith("image/"))
                    return BadRequest("Only image files are allowed.");

                var key = $"uploads/{param.MemberID}/{Guid.NewGuid()}_{Path.GetFileName(param.ImageFile.FileName)}";

                var request = new PutObjectRequest
                {
                    BucketName = SMConfigurationManager.S3BucketName,
                    Key = key,
                    InputStream = param.ImageFile.OpenReadStream(),
                    ContentType = param.ImageFile.ContentType
                };

                await _s3Client.PutObjectAsync(request);

                var url = $"https://{SMConfigurationManager.S3BucketName}.s3.amazonaws.com/{key}";

                using (MemberHandler handler = new MemberHandler())
                {
                    handler.UpdateMemberImage(param.MemberID, url, key);
                }

                return Ok(new { imageUrl = url });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [RequestSizeLimit(100_000_000)] // 100 MB
        [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
        [HttpPost("UploadBulkMembersImages")]
        public async Task<IActionResult> UploadBulkMembersImages([FromForm] ZipParams param)
        {
            IFormFile zipFile = param.ZipFile;
            if (zipFile == null || zipFile.Length == 0)
                return BadRequest("No ZIP file uploaded.");

            if (!zipFile.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only ZIP files are allowed.");

            using var archiveStream = zipFile.OpenReadStream();
            using var archive = new ZipArchive(archiveStream);
            List<IamgeProperties> membersImages = new List<IamgeProperties>();
            foreach (var entry in archive.Entries)
            {
                if (!entry.FullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                    !entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                    !entry.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                    continue; // skip non-images

                using var entryStream = entry.Open();
                using var ms = new MemoryStream();
                await entryStream.CopyToAsync(ms);
                ms.Position = 0;
                string fileName = Path.GetFileNameWithoutExtension(entry.Name);
                var key = $"uploads/batch/{Guid.NewGuid()}_{entry.Name}";

                var putRequest = new PutObjectRequest
                {
                    BucketName = SMConfigurationManager.S3BucketName,
                    Key = key,
                    InputStream = ms,
                    ContentType = GetMimeType(entry.Name)
                };

                await _s3Client.PutObjectAsync(putRequest);

                var imageUrl = $"https://{SMConfigurationManager.S3BucketName}.s3.amazonaws.com/{key}";
                IamgeProperties memberImage = new IamgeProperties()
                {
                    Filename = fileName,
                    ImageURL = imageUrl,
                    Key = key
                };
                membersImages.Add(memberImage);
            }
            List<string> missingMembers = new List<string>();
            using (MemberHandler handler = new MemberHandler())
            {
                missingMembers = handler.BulkUploadImages(membersImages);
            }
            return Ok(new { missingMembers });
        }
        public class ImageParams
        {
            public IFormFile ImageFile { get; set; }
            public int MemberID { get; set; }
        }
        public class ZipParams
        {
            public IFormFile ZipFile { get; set; }
        }

        // Optional helper method
        private static string GetMimeType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }

    }
}
