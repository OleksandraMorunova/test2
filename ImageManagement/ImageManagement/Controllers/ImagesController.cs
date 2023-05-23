using DocumentFormat.OpenXml.Office2010.Excel;
using ImageManagement.Model;
using ImageManagement.NewFolder;
using ImageManagement.Service;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace ImageManagement.Controllers
{
    [Route("api/images")]
    public class ImagesController : Controller
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImagesController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ImagesController(IImageService imageService, ILogger<ImagesController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _imageService = imageService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        const int maxFileSize = 5 * 1024 * 1024;

        [HttpPost("/upload-by-url")]
        public async Task<IActionResult> uploadImage([FromBody]UploadImageUrl uri)
        {
            if (!AuditUrl.IsValidUri(uri.Url))
            {
                return BadRequest("It isn't url.");
            }
            string fileName = Path.GetFileName(uri.Url);
            var file = DownloadImage(uri.Url).Result;

            if (uri == null)
            {
                return BadRequest("This file is empty. Please, choose another image.");
            }

            if (!AuditUrl.isImage(file))
            {
                return BadRequest("This file isn't image. Please, choose another image.");
            }

            if (file.Length > maxFileSize)
            {
                return BadRequest("This file is more than 5MB. Please, choose another image.");
            }
            MemoryStream stream = new MemoryStream(file);
            Bitmap bitmap = new Bitmap(stream);
            if (bitmap.Height < 300 && bitmap.Width < 300)
            {
                return BadRequest($"Dimension of your image is {bitmap.Width}X{bitmap.Height}. Please, upload dimension more than 300 or 300");
            }
            var result = await _imageService.uploadIamge(file, fileName, uri.Url);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpGet("/get-url/{id}/size/{size}")]
        public async Task<IActionResult> getPreView([FromRoute] string id, [FromRoute] string size)
        {
            if (!int.TryParse(id, out var intId))
            {
                return BadRequest($"The value isn't integer: {id}. Please, enter integer for id.");
            }
            if (!size.Equals("100") && !size.Equals("300"))
            {
                return BadRequest($"Don't exist preview with this size: {size}");
            }
            var result = await _imageService.getPreView(id, size);
            if (result != null)
            {
                MemoryStream stream = new MemoryStream(result.Slice);
                Bitmap bitmap = new Bitmap(stream); //This code targets Windows.
                _logger.LogInformation($"Dimension Of Image: width={bitmap.Width}  height={bitmap.Height}");

                return Ok(getUrlServer() + bitmap.Width + "/" + result.FileName);
            }
             return BadRequest($"Don't exist preview with this id: {id}");
        }

        [HttpGet("/get-url/{id}")]
        public async Task<IActionResult> getPreViewByIdImage([FromRoute] string id)
        {
            if (!int.TryParse(id, out var intId))
            {
                return BadRequest($"The value isn't integer: {id}. Please, enter integer for id.");
            }
            var pathSearch = await _imageService.getPathImageById(id);
            if(pathSearch != null)
            {
                return Ok(pathSearch);
            }
            return BadRequest($"Don't exist preview with this id: {id}");
        }

        [HttpDelete("get-url/remove/{id}")]
        public async Task<IActionResult> removeImageById([FromRoute] string id)
        {
            if (!int.TryParse(id, out var intId))
            {
                return BadRequest($"The value isn't integer: {id}. Please, enter integer for id.");
            }
            var checkRemoveImage = await _imageService.removeImageById(id);
            if (!checkRemoveImage)
            {
                return BadRequest($"Don't exist preview with this id: {id}");
            }
            if (checkRemoveImage)
            {
                return Ok(checkRemoveImage);
            }
            return BadRequest();
        }

        private async Task<byte[]> DownloadImage(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        private string getUrlServer()
        {
            return $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://{_httpContextAccessor?.HttpContext?.Request.Host}/";
        }
    }
}