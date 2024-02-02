using CoreSharef.Repository;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using File = CoreSharef.Models.File;

namespace CoreSharef.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly FileRepository _fileRepository;
    private readonly IMinioClient _minioClient;

    public FileController(IConfiguration configuration)
    {
        _configuration = configuration;
        _minioClient = new MinioClient().WithEndpoint(_configuration.GetConnectionString("MinioConnection"))
            .WithCredentials(_configuration.GetConnectionString("MinioAccess"),
                _configuration.GetConnectionString("MinioSecret")).Build();
        _fileRepository = new FileRepository(_configuration.GetConnectionString("MongoDBConnection"), "Core");
    }

    [HttpGet]
    public IActionResult ListFile()
    {
       

       
       
       
       

        var fileList = _fileRepository.GetList();
        
        if (fileList.Count > 0)
        {
            return Ok(new { Data = fileList });
        }
        else
        {
            return NotFound(new {Data = fileList});
        }
    }
    
    [HttpGet("file/{code}")]
    public async Task<IActionResult> GetFile([FromRoute] string code)
    {
        var file = _fileRepository.GetFileByCode(code);
        
        if (file != null)
        {
            try
            {
                _fileRepository.Delete(file.Code);                                                                                                                                                
                var url = _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs().WithBucket("sharef").WithObject(file.Name).WithExpiry(100000)).ConfigureAwait(true);
                return Ok(new {Data = url.GetAwaiter().GetResult()});                                                                                                                             
            }
            catch (Exception e)
            {
                return NotFound(new {Data = e.Message});
            }
        }
        else
        {
            return NotFound(new {Data = file});
        }
    }
    
    [HttpPost("")]
    public async Task<IActionResult> InsertFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Invalid file");
        }

        string code = GenerateRandomCode();

        string fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{Path.GetExtension(file.FileName)}";

        try
        {
            await SaveFileToMinio(file, fileName);

            var newFile = new File
            {
                Name = fileName,
                Code = code
            };

            _fileRepository.Insert(newFile);

            return Ok(new { Data = newFile });
        }
        catch (MinioException ex)
        {
            return BadRequest($"Error saving file to MinIO: {ex.Message}");
        }
    }

    private async Task SaveFileToMinio(IFormFile file, string fileName)
    {
        try
        {
            using (var stream = file.OpenReadStream())
            {
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket("sharef")
                    .WithObject(fileName)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length)
                    .WithContentType(file.ContentType);

                await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                Console.WriteLine("Successfully uploaded " + fileName);
            }
        }
        catch (MinioException ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private string GenerateRandomCode()
    {
        // Generate a random alphanumeric code with 5 characters
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new Random();
        string code = new string(Enumerable.Repeat(chars, 5)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        return code;
    }
}