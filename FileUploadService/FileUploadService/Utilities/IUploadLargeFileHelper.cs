using FileUploadService.Models;

using Microsoft.AspNetCore.Http;

namespace FileUploadService.Utilities
{
    public interface IUploadLargeFileHelper
    {
       Task<AppFileUploadModel> UploadLargeFileAsync(HttpRequest request);
    }
}
