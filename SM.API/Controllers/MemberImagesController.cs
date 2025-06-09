using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.API.Services;
using SM.BAL;
using SM.DAL.DataModel;
using System.Collections.Generic;
using System.IO.Compression;
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
        //private const string BucketName = "amapptesting";

        public MemberImagesController(ILogger<SMControllerBase> logger, IAmazonS3 s3Client)
        : base(logger)
        {
            _s3Client = s3Client;
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
                var fileNameOnly = Path.GetFileNameWithoutExtension(entry.FullName);
                membersImages.Add(new IamgeProperties() { Filename = fileNameOnly, ImageURL = imageUrl, Key = key });
            }
            using (MemberHandler handler = new MemberHandler())
            {
                handler.BulkUploadImages(membersImages);
            }
            return Ok(new { membersImages });
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
