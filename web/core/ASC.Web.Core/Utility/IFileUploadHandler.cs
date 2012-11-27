using System.Web;

namespace ASC.Web.Core.Utility
{
    public class FileUploadResult
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }

    public interface IFileUploadHandler
    {
        FileUploadResult ProcessUpload(HttpContext context);
    }
}
