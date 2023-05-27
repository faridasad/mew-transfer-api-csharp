using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using mew_transfer_api_csharp.Models;
using Microsoft.Extensions.Options;

namespace mew_transfer_api_csharp.Services
{
    

    public interface ICloudinaryService
    {
        Task<string> UploadFile(IFormFile file);
        // Other methods for image transformations, deletions, etc.
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> cloudinaryConfig)
        {
            var account = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(file.Name, stream),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            Console.WriteLine(uploadResult.Url);

            return uploadResult.Url.ToString();
        }

        // Implement other methods for image transformations, deletions, etc.
    }

}
