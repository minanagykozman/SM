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

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MemberImagesController : SMControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private const string BucketName = "amapptesting";

        public MemberImagesController(ILogger<SMControllerBase> logger, IAmazonS3 s3Client)
        : base(logger)
        {
            _s3Client = s3Client;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadSingle([FromForm] ImageParams param)
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
                    BucketName = BucketName,
                    Key = key,
                    InputStream = param.ImageFile.OpenReadStream(),
                    ContentType = param.ImageFile.ContentType
                };

                await _s3Client.PutObjectAsync(request);

                var url = $"https://{BucketName}.s3.amazonaws.com/{key}";

                using (MemberHandler handler = new MemberHandler())
                {
                    handler.UpdateMemberImage(param.MemberID, url);
                }

                return Ok(new { imageUrl = url });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        public class ImageParams
        {
           public IFormFile ImageFile { get; set; }
            public int MemberID { get; set; }
        }
        //[HttpPost("upload-zip")]
        //public async Task<IActionResult> UploadZip([FromForm] IFormFile zipFile)
        //{
        //    if (zipFile == null || zipFile.Length == 0)
        //        return BadRequest("No ZIP file uploaded.");

        //    if (!zipFile.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        //        return BadRequest("Only ZIP files are allowed.");

        //    var imageUrls = new List<string>();
        //    using var archiveStream = zipFile.OpenReadStream();
        //    using var archive = new ZipArchive(archiveStream);

        //    foreach (var entry in archive.Entries)
        //    {
        //        if (!entry.FullName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
        //            !entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
        //            !entry.FullName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
        //            continue; // skip non-images

        //        using var entryStream = entry.Open();
        //        using var ms = new MemoryStream();
        //        await entryStream.CopyToAsync(ms);
        //        ms.Position = 0;

        //        var key = $"uploads/batch/{Guid.NewGuid()}_{entry.Name}";

        //        var putRequest = new PutObjectRequest
        //        {
        //            BucketName = BucketName,
        //            Key = key,
        //            InputStream = ms,
        //            ContentType = GetMimeType(entry.Name),
        //            CannedACL = S3CannedACL.PublicRead
        //        };

        //        await _s3Client.PutObjectAsync(putRequest);

        //        var imageUrl = $"https://{BucketName}.s3.amazonaws.com/{key}";
        //        imageUrls.Add(imageUrl);

        //        // TODO: Optionally save `imageUrl` and entry.Name to DB
        //    }

        //    return Ok(new { imageUrls });
        //}

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
