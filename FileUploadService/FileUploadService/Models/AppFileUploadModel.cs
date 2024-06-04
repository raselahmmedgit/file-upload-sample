#nullable disable

using System.ComponentModel.DataAnnotations;

namespace FileUploadService.Models
{
    public class AppFileUploadModel
    {
        public AppFileUploadModel()
        {
            AppFileUploadModelDetails = new List<AppFileUploadModelDetail>();
        }
        [Display(Name = "File Size (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long FileSize { get; set; }

        [Display(Name = "Uploaded (UTC)")]
        [DisplayFormat(DataFormatString = "{0:G}")]
        public DateTime UploadDateTime { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        public virtual List<AppFileUploadModelDetail> AppFileUploadModelDetails { get; set; }       
    }

}
