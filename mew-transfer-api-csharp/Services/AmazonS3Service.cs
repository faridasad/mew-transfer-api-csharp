using Amazon.S3;
using Amazon.S3.Model;
using mew_transfer_api_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace mew_transfer_api_csharp.Services
{
    public class AmazonS3Service
    {
        private readonly IAmazonS3 _s3Client;

        public AmazonS3Service(IOptions<S3Settings> s3Options)
        {
            _s3Client = new AmazonS3Client(s3Options.Value.AccessKeyId, s3Options.Value.SecretAccessKey, Amazon.RegionEndpoint.EUNorth1);
        }

        public async Task UploadFileAsync(string filePath, string objectId)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = "mew-transfer",
                Key = objectId.ToString(),
                FilePath = filePath,
                
            };

            await _s3Client.PutObjectAsync(putRequest);
        }

        public async Task<Stream> GetFileAsync(string objectId, string type)
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = "mew-transfer",
                Key = objectId.ToString() + type
            };
            var response = await _s3Client.GetObjectAsync(getRequest);
            return response.ResponseStream;
        }
    }
}
