using iig_webapp_test.Entities;
using iig_webapp_test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace iig_webapp_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public UploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAsync()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.FirstOrDefault();
                var type = formCollection["type"].FirstOrDefault();
                var urlpath = _configuration.GetValue<string>("hostUrlPath");

                if (file == null || file.Length == 0)
                    throw new AppException("Unable to upload pictures, please attach files.");

                var extension = Path.GetExtension(file.FileName);
                if (!IsFileExtensionAllowed(extension))
                    throw new AppException("Only .jpg, .jpeg, .png, and .bmp file types are allowed.");

                if (file.Length > 5 * 1024 * 1024)
                    throw new AppException("File size should not exceed 5MB.");

                var folderName = Path.Combine("wwwroot", type);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { status = true, path = dbPath.Replace("wwwroot", urlpath), message = "upload success" });
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }
        }

        private bool IsFileExtensionAllowed(string extension)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
            return allowedExtensions.Contains(extension.ToLower());
        }


    }
}
