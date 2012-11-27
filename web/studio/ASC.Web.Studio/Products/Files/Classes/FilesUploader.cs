using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using ASC.Web.Controls;
using ASC.Web.Controls.FileUploader.HttpModule;
using ASC.Web.Files.Resources;
using File = ASC.Files.Core.File;

namespace ASC.Web.Files.Classes
{
    public class FilesUploader : FileUploadHandler
    {
        public override FileUploadResult ProcessUpload(HttpContext context)
        {
            try
            {
                if (!ProgressFileUploader.HasFilesToUpload(context)) throw new InvalidOperationException(FilesCommonResource.ErrorMassage_EmptyFile);

                var postedFile = new ProgressFileUploader.FileToUpload(context);
                var contentLength = postedFile.ContentLength;

                var fileName = postedFile.FileName;
                if (fileName.LastIndexOf('\\') != 0)
                    fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                fileName = Global.ReplaceInvalidCharsAndTruncate(fileName);

                var inputStream = postedFile.InputStream;
                var contentType = postedFile.FileContentType;

                var folderID = context.Request[UrlConstant.FolderId];

                var file = DocumentUtils.UploadFile(folderID, fileName, contentLength, contentType, inputStream);

                using (var ms = new MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof (File));
                    serializer.WriteObject(ms, file);
                    ms.Seek(0, SeekOrigin.Begin);

                    return new FileUploadResult
                               {
                                   Success = true,
                                   Data = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int) ms.Length)
                               };
                }
            }
            catch (Exception ex)
            {
                return new FileUploadResult
                           {
                               Success = false,
                               Message = ex.Message,
                           };
            }
        }
    }
}