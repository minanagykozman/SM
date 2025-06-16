using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using SixLabors.Fonts;
using SM.API.Services;
using SM.BAL;
using SM.DAL.DataModel;
using System.Collections.Generic;

using System.IO.Compression;

using static SM.BAL.MemberHandler;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;


namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MemberImagesController : SMControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly HttpClient _httpClient;
        // --- ImageSharp Font Management ---
        private readonly FontCollection _fontCollection;
        private readonly FontFamily _dubaiFamily;
        private readonly FontFamily _bahnschriftFamily;
        public MemberImagesController(ILogger<SMControllerBase> logger, IAmazonS3 s3Client)
        : base(logger)
        {
            _s3Client = s3Client;
            _httpClient = new HttpClient();

            _fontCollection = new FontCollection();
            var fontPath = Path.Combine(AppContext.BaseDirectory, "fonts");
            _dubaiFamily = _fontCollection.Add(Path.Combine(fontPath, "DUBAI-REGULAR.TTF"));
            _bahnschriftFamily = _fontCollection.Add(Path.Combine(fontPath, "bahnschrift.ttf"));
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
                using (Image baseCardTemplate = await DownloadImageAsync(SMConfigurationManager.BaseImageURL))
                {
                    if (baseCardTemplate == null)
                    {
                        return StatusCode(500, "Failed to download the base card template from S3.");
                    }

                    foreach (var member in members)
                    {
                        await GenerateCard(member, baseCardTemplate, tempDirectory);
                    }
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
        /// <summary>
        /// Downloads an image from a given URL and returns it as an ImageSharp Image object.
        /// </summary>
        private async Task<Image> DownloadImageAsync(string imageUrl)
        {
            var response = await _httpClient.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                // Use ImageSharp's loader
                return await Image.LoadAsync(stream);
            }
        }

        /// <summary>
        /// Generates a single card image using ImageSharp and saves it to the output directory.
        /// </summary>
        private async Task GenerateCard(Member member, Image baseCardTemplate, string outputDirectory)
        {
            using (Image personalPhoto = await DownloadImageAsync(member.ImageURL))
            {
                if (personalPhoto == null) return;

                string name = TrimName(member.FullName, 17);
                string id = member.Code;
                string outputImagePath = Path.Combine(outputDirectory, $"{id}.jpg");

                byte[] qrCodeAsBytes;
                using (var qrGenerator = new QRCodeGenerator())
                using (var qrCodeData = qrGenerator.CreateQrCode(id, QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    qrCodeAsBytes = qrCode.GetGraphic(25, new byte[] { 0, 0, 0 }, new byte[] { 255, 255, 255 });
                }

                using (var qrCodeImage = Image.Load(qrCodeAsBytes))
                using (var finalCard = baseCardTemplate.Clone(ctx =>
                {
                    // Resize personal photo before drawing
                    personalPhoto.Mutate(p => p.Resize(1150, 1150));

                    var arFont = _dubaiFamily.CreateFont(150, FontStyle.Regular);
                    var enFont = _bahnschriftFamily.CreateFont(140, FontStyle.Regular);

                    // --- Draw Images ---
                    int personalImageX = (baseCardTemplate.Width - 1060) / 2;
                    ctx.DrawImage(personalPhoto, new Point(personalImageX, 600), 1f);

                    int qrCodeX = (baseCardTemplate.Width - qrCodeImage.Width) / 2;
                    int qrCodeY = (baseCardTemplate.Height / 2) - (qrCodeImage.Height / 2) + 1100;
                    ctx.DrawImage(qrCodeImage, new Point(qrCodeX, qrCodeY), 1f);

                    // --- Text Measurement and Drawing ---
                    var nameBounds = TextMeasurer.MeasureBounds(name, new TextOptions(arFont));
                    var idBounds = TextMeasurer.MeasureBounds(id, new TextOptions(enFont));

                    float nameX = (baseCardTemplate.Width - nameBounds.Width) / 2;
                    float nameY = (baseCardTemplate.Height / 2) + (qrCodeImage.Height / 2);

                    float idX = (baseCardTemplate.Width - idBounds.Width) / 2;
                    float idY = nameY + nameBounds.Height + 80;

                    ctx.DrawText(name, arFont, Color.DarkGreen, new PointF(nameX, nameY));
                    ctx.DrawText(id, enFont, Color.DarkGreen, new PointF(idX, idY));
                }))
                {
                    // Save the final composite image asynchronously
                    await finalCard.SaveAsJpegAsync(outputImagePath);
                }
            }
        }

        private string TrimName(string fullName, int maxLength)
        {
            if (string.IsNullOrEmpty(fullName)) return string.Empty;

            while (fullName.Length > maxLength)
            {
                int lastSpaceIndex = fullName.LastIndexOf(' ');
                if (lastSpaceIndex == -1)
                {
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
