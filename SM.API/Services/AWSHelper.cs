using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using SM.BAL;
using static SM.API.Controllers.MemberImagesController;

namespace SM.API.Services
{
    public static class AWSHelper
    {
        public static async Task<AWSImageFile> UploadMemberImage(IFormFile imageFile, IAmazonS3 s3Client, MemberHandler handler)
        {
            if (imageFile == null)
                return null;

            var key = $"uploads/{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";

            var request = new PutObjectRequest
            {
                BucketName = SMConfigurationManager.S3BucketName,
                Key = key,
                InputStream = imageFile.OpenReadStream(),
                ContentType = imageFile.ContentType
            };

            await s3Client.PutObjectAsync(request);

            var url = $"https://{SMConfigurationManager.S3BucketName}.s3.amazonaws.com/{key}";

            return new AWSImageFile() { Key = key, URL = url };

        }
        public static async Task DeleteFileAsync(string key, IAmazonS3 s3Client)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = SMConfigurationManager.S3BucketName,
                Key = key
            };

            var response = await s3Client.DeleteObjectAsync(deleteObjectRequest);
        }
        public class AWSImageFile
        {
            public string URL { get; set; }
            public string Key { get; set; }
        }
    }
}
