using mew_transfer_api_csharp.Models;
using mew_transfer_api_csharp.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;

namespace mew_transfer_api_csharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordController
    {
        
        private readonly MongoDBService _mongoDBService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly AmazonS3Service _amazonS3Service;

        public RecordController(MongoDBService mongoDBService, ICloudinaryService cloudinaryService, AmazonS3Service amazonS3Service)
        {
            _mongoDBService = mongoDBService;
            _cloudinaryService = cloudinaryService;
            _amazonS3Service = amazonS3Service;
        }

        [HttpGet]
        [Route("single/{id}")]
        public async Task<Record> Get(string id)
        {
            return await _mongoDBService.GetAsync(id);
        }



        [HttpGet]
        [Route("all")]
        public async Task<List<Record>> Get()
        {
            return await _mongoDBService.GetAllAsync();
        }

        [HttpGet]
        [Route("download/{id}")]
        public async Task<IActionResult> Download(string id)
        {
            var response = await _mongoDBService.GetAsync(id);
            Console.WriteLine(response.type);
            try
            {
                var fileStream = await _amazonS3Service.GetFileAsync(id, response.type);
                return new FileStreamResult(fileStream, GetContentType(response.type))
                {
                    FileDownloadName = response.originalName
                };
            }
            catch (Exception e)
            {
                return new BadRequestResult();
            }
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Add(List<IFormFile> files)
        {
            files.ForEach(file => Console.WriteLine(file.FileName));
            try
            {
                if (files == null || files.Count == 0)
                {
                    Console.WriteLine("yo");
                    return new BadRequestResult();
                }


                if(files.Count == 1)
                {
                    string fileName = files[0].FileName;
                    string uniqueFileName = ChangeFileName() + "_" + fileName;

                    string filePath = Path.Combine(Path.GetTempPath(), uniqueFileName);

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        files[0].CopyTo(fs);
                    }

                    var ext = Path.GetExtension(files[0].FileName);

                    var objectId = await _mongoDBService.UploadFileAsync(files[0].FileName, files[0].OpenReadStream());

                    await _amazonS3Service.UploadFileAsync(filePath, objectId + ext);

                    await _mongoDBService.Add(new Record { Id = objectId, filePath = filePath, originalName = files[0].FileName, type = ext});

                    return new OkObjectResult(new { id = objectId.ToString() });


                }
                else if(files.Count > 1)
                {

                    string zipFilePath = Path.GetTempFileName() + ".zip";

                    using (ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                    {
                        foreach (var file in files)
                        {
                            string entryFileName = ChangeFileName() + "_" + file.FileName;
                            ZipArchiveEntry entry = archive.CreateEntry(entryFileName);

                            using (Stream entryStream = entry.Open())
                            {
                                file.CopyTo(entryStream);
                            }
                        }
                    }


                    var objectId = await _mongoDBService.UploadFileAsync(zipFilePath, files[0].OpenReadStream());

                    await _amazonS3Service.UploadFileAsync(zipFilePath, objectId + ".zip");

                    await _mongoDBService.Add(new Record { Id = objectId, filePath = zipFilePath, originalName = files[0].FileName + ".zip", type = ".zip" });

                    return new OkObjectResult(new { id = objectId.ToString() });

                }

                return new BadRequestResult();
                
            }
            catch
            {
                return new ObjectResult(new { error = "Something went wrong" }) { StatusCode = 500 };  
            }
        }

        

        private string ChangeFileName()
        {
            return Guid.NewGuid().ToString();
        }

        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case "zip":
                    return "application/zip";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
