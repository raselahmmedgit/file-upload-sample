#nullable disable

using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.ComponentModel.DataAnnotations;

namespace FileUploadService.Models
{
    public class AppFileUploadModelDetail
    {
        public string FileId { get; set; }

        public byte[] FileContent { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Note")]
        public string Note { get; set; }

        [Display(Name = "File Size (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long FileSize { get; set; }

        public string MediaType { get; set; }

        public string FileExtension { get; set; }

        public string FileNameWithoutExtension { get; set; }

        public string Message {  get; set; }

        public  ModelStateDictionary ModelStateDictionary { get; set; }
    }
}
