#region Import

using System;
using System.Runtime.Remoting.Messaging;
using System.Web;
using ASC.Common.Web;
using ASC.Core;
using ASC.Files.Core;
using ASC.Web.Controls;
using ASC.Web.Controls.FileUploader.HttpModule;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

#endregion 

namespace ASC.Web.CRM.Classes
{
    public class FileUploaderHandler : FileUploadHandler
    {
        public override FileUploadResult ProcessUpload(HttpContext context)
        {
            var fileUploadResult = new FileUploadResult();

            if (!ProgressFileUploader.HasFilesToUpload(context)) return fileUploadResult;

            var file = new ProgressFileUploader.FileToUpload(context);

            if (String.IsNullOrEmpty(file.FileName) || file.ContentLength == 0)
                throw new InvalidOperationException("Invalid file.");
            
            if (0 < SetupInfo.MaxUploadSize && SetupInfo.MaxUploadSize < file.ContentLength)
                throw new InvalidOperationException(string.Format("Exceeds the maximum file size ({0}MB).", SetupInfo.MaxUploadSizeToMBFormat));
            
            if (CallContext.GetData("CURRENT_ACCOUNT") == null)
                CallContext.SetData("CURRENT_ACCOUNT", new Guid(context.Request["UserID"]));
            
            var document = new File
                {
                    Title = file.FileName.LastIndexOf('\\') != -1 ? file.FileName.Substring(file.FileName.LastIndexOf('\\') + 1) : file.FileName,
                    FolderID = Global.DaoFactory.GetFileDao().GetRoot()
                };
            
            document.ContentLength = file.ContentLength;
            document.ContentType = MimeMapping.GetMimeMapping(document.Title);

            document = Global.DaoFactory.GetFileDao().SaveFile(document, file.InputStream);

            fileUploadResult.Success = true;
            //fileUploadResult.Data = String.Format("{0}_{1}", document.ID, document.Version);
            fileUploadResult.Data = document.ID;
            fileUploadResult.FileName = document.Title;
            fileUploadResult.FileURL = document.FileUri;

            return fileUploadResult;

        }
    }
}