using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdPlatformsController : ControllerBase
    {
        private readonly AdPlatformService _service;

        public AdPlatformsController(AdPlatformService service)
        {
            _service = service;
        }

        // POST: /api/adplatforms/upload
        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // Указываем, что метод принимает форму с файлом
        public async Task<IActionResult> Upload([FromForm] FileUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest("Файл не загружен.");

            using var stream = model.File.OpenReadStream();
            try
            {
                await _service.LoadFromFileAsync(stream);
                return Ok("Файл успешно загружен.");
            }
            catch
            {
                return BadRequest("Ошибка обработки файла.");
            }
        }

        // GET: /api/adplatforms/search?location=/ru/svrd/revda
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Локация не указана.");

            var results = _service.FindMatchingPlatforms(location);
            return Ok(results);
        }
    }
}
