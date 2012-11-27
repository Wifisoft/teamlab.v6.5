using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.UserControls.Wiki.Handlers;
using ASC.Data.Storage;
using System.IO;
using ASC.Core;
using ASC.Web.UserControls.Wiki.Resources;
using ASC.Core.Tenants;


namespace ASC.Web.UserControls.Wiki.UC
{
    public class FileUploadResult
    {
        public FileUploadResult()
        {
            ErrorText = string.Empty;
            WebPath = string.Empty;
            LocalPath = string.Empty;
        }
        public string WebPath { get; set; }
        public string LocalPath { get; set; }
        public string ErrorText { get; set; }
    }

    

    public partial class FileUploader : System.Web.UI.Page
    {


        public static long MaxUploadSize
        {
            get
            {
                var q = CoreContext.TenantManager.GetTenantQuota(CoreContext.TenantManager.GetCurrentTenant().TenantId);
                return q != null ? q.MaxFileSize : 0;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            FileUploadResult result = new FileUploadResult();
            if(Request.Files.Count > 0 && !string.IsNullOrEmpty(Request["hfUserID"]) )
            {
                try
                {
                    //string uploadedUserName;
                    byte[] content = new byte[Request.Files[0].ContentLength];

                    if (content.Length > MaxUploadSize && MaxUploadSize > 0)
                    {
                        result.ErrorText = WikiUCResource.wikiErrorFileSizeLimitText; 
                    }
                    else
                    {
                        Request.Files[0].InputStream.Read(content, 0, Request.Files[0].ContentLength);
                        string localPath;
                        result.WebPath = TempFileContentSave(content, out localPath);
                        result.LocalPath = localPath;
                    }

                    
                    Response.StatusCode = 200;
                    Response.Write(AjaxPro.JavaScriptSerializer.Serialize(result));
                }
                catch (System.Exception){}
                
                
            }
            Response.End();
        }

        private string TempFileContentSave(byte[] fileContent, out string filaLocation)
        {
            string TenantId = CoreContext.TenantManager.GetCurrentTenant().TenantId.ToString();
            IDataStore storage = StorageFactory.GetStorage(TenantId, WikiSection.Section.DataStorage.ModuleName);
            string result = string.Empty;

            using (MemoryStream ms = new MemoryStream(fileContent))
            {
                result = storage.SaveTemp(WikiSection.Section.DataStorage.TempDomain, out filaLocation, ms).ToString();
            }

            return result;
        }
    }
}
