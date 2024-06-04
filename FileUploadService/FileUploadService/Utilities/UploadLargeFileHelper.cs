#nullable disable

using FileUploadService.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

using System.Net;

namespace FileUploadService.Utilities
{
    public class UploadLargeFileHelper :  IUploadLargeFileHelper
    {
        private IHttpContextAccessor _HttpContextAccessor;
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".txt",".png",".jpg", ".pdf", ".zip", ".rar"  };
        private readonly string _targetFilePath;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
       
        public UploadLargeFileHelper(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _HttpContextAccessor = httpContextAccessor;
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");
           
        }

        public async Task<AppFileUploadModel> UploadLargeFileAsync(HttpRequest request) 
        { 
            var appFileModels = new AppFileUploadModel();
            //var request = _HttpContextAccessor.HttpContext.Request;
            // validation of Content-Type
            // 1. first, it must be a form-data request
            // 2. a boundary should be found in the Content-Type
            ModelStateDictionary modelState = new ModelStateDictionary();
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                throw new Exception("UnsupportedMediaTypeResult");
            }
            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);
            //var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary.Value).Value;
            //var reader = new MultipartReader(boundary, request.Body);
            var section = await reader.ReadNextSectionAsync();

            // This sample try to get the first file from request and save it
            // Make changes according to your needs in actual use
            if (section == null)
            {
                throw new Exception("No files data in the request.");
            }
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                    // Otherwise, it is very likely to cause problems such as virus uploading, disk filling, etc
                    // In short, it is necessary to restrict and verify the upload
                    // Here, we just use the temporary folder and a random file name

                    // Get the temporary folder, and combine a random file name with it
          
                   
                    var untrustedFileNameForStorage = contentDisposition.FileName.Value;
                        // Don't trust the file name sent by the client. To display
                        // the file name, HTML-encode the value.
                    var trustedFileNameForDisplay = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                    string fileExt = Path.GetExtension(trustedFileNameForDisplay);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(trustedFileNameForDisplay);
                    var saveToPath = Path.Combine(Path.GetTempPath(), trustedFileNameForDisplay);
                    var streamedFileContent = Array.Empty<byte>();
                    
                    streamedFileContent = 
                            await FileHelpers.ProcessStreamedFile(section, contentDisposition, 
                                modelState, _permittedExtensions, _fileSizeLimit);

                    if (!modelState.IsValid)
                    {
                        throw new Exception("UnsupportedMediaTypeResult");
                        //return new AppFileModel { ModelStateDictionary = modelState};
                    }
                    var appFileModelDetail = new AppFileUploadModelDetail
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FileContent = streamedFileContent,
                        FileName = untrustedFileNameForStorage,
                        FileNameWithoutExtension = fileNameWithoutExtension,
                        FileSize = streamedFileContent.Length,                            
                        MediaType = section.ContentType,
                        FileExtension = fileExt,
                        ModelStateDictionary = modelState
                    };                    
                    appFileModels.UploadDateTime = DateTime.UtcNow;
                    appFileModels.AppFileUploadModelDetails.Add(appFileModelDetail);
                    appFileModels.FileSize =  appFileModels.AppFileUploadModelDetails.Sum(x=> x.FileSize);    
                }
                section = await reader.ReadNextSectionAsync();
            }
            
            //modelState.AddModelError("File", "No files data in the request." );
            //throw new Exception("No files data in the request.");//return new AppFileModel(){ Message = "No files data in the request.", ModelStateDictionary = modelState};
            return appFileModels;
        }
    }
}
