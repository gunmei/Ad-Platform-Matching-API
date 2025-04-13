using Microsoft.AspNetCore.Http;

namespace WebApplication2.Models
{
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
    }
}
