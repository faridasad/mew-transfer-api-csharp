using MongoDB.Bson;

namespace mew_transfer_api_csharp.Models
{
    public class Record
    {
        public ObjectId Id { get; set; }
        public string filePath { get; set; } = null!;
        public string originalName { get; set; } = null!;
        public string type { get; set; } = null!;
    }
}
