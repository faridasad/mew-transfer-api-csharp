namespace mew_transfer_api_csharp.Models
{
    public class S3Settings
    {
        public string AccessKeyId { get; set; } = null!;
        public string SecretAccessKey { get; set; } = null!;
        public string BucketName { get; set; } = null!;
        public string Region { get; set; } = null!;
    }
}
