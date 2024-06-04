#nullable disable

using FileUploadService.Utilities;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

using System.Text;

namespace LargeFileUpload.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly long _fileSizeLimit;
        private readonly ILogger<FileUploadController> _logger;
        private readonly string[] _permittedExtensions = { ".txt",".png",".jpg", ".pdf", ".zip", ".rar"  };
        private readonly string _targetFilePath;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        private readonly IUploadLargeFileHelper _uploadLargeFileHelper;

        public FileUploadController(ILogger<FileUploadController> logger, IConfiguration config, IUploadLargeFileHelper uploadLargeFileHelper)
        {
            _logger = logger;
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");
            _uploadLargeFileHelper = uploadLargeFileHelper;
        }
        /// <summary>
        /// Action for upload large file
        /// </summary>
        /// <remarks>
        /// Request to this action will not trigger any model binding or model validation,
        /// because this is a no-argument action
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadLargeFile(string note, IFormFile file)
        { 
            //await Task.Yield();
            var request = HttpContext.Request;
            // validation of Content-Type
            // 1. first, it must be a form-data request
            // 2. a boundary should be found in the Content-Type
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                  return new UnsupportedMediaTypeResult();
            }
            var dd = await _uploadLargeFileHelper.UploadLargeFileAsync(request);
            // If the code runs to this location, it means that no files have been saved
            //if (!dd.ModelStateDictionary.IsValid)
            //{
            //     return BadRequest(dd.Message);
            //}
            //return SweetAlert("Upload Success", $"File:  Size:  MB", "success");
           
           return SweetAlert("Upload Success", $"File Size: {dd.FileSize/1024/1024} MB", "success");
        }
        private IActionResult SweetAlert(string title, string message, string type)
        {
            return Content($"swal ('{title}',  '{message}',  '{type}')", "text/javascript");
        }
        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = 
                MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
