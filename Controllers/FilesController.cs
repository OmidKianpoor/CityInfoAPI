using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/Files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        FileExtensionContentTypeProvider FileExtensionContentTypeProvider;
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)

        {
            this.FileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
        }

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            var pathToFile = "Lux Aeterna.mp3";

            if(!System.IO.File.Exists(pathToFile))
            { return NotFound(); }

            var bytes = System.IO.File.ReadAllBytes(pathToFile);

            if(!FileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
            { contentType = "application/octet-stream"; }

            return File(bytes,contentType,Path.GetFileName(pathToFile));
        }
    }
}
