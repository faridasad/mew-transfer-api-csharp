using mew_transfer_api_csharp.Models;
using mew_transfer_api_csharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace mew_transfer_api_csharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordController
    {
        
        private readonly MongoDBService _mongoDBService;

        public RecordController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<List<Record>> Get()
        {
            return await _mongoDBService.GetAsync();
        }
    }
}
