using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Common.Web;
using ASC.Data.Storage;

namespace ASC.Web.Controls
{
    [Themeable(true)]
    [ToolboxData("<{0}:ProgressFileUploader runat=server></{0}:ProgressFileUploader>")]
    public class ProgressFileUploader : WebControl
    {
		public bool EnableHtml5 { get; set; }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            var  ajaxuploadScriptLocation = Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.FileUploader.js.ajaxupload.js");
            Page.ClientScript.RegisterClientScriptInclude("ajaxupload_script", ajaxuploadScriptLocation);

            var fileUploaderScriptLocation = Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.FileUploader.js.fileuploader.js");
            Page.ClientScript.RegisterClientScriptInclude("fileuploader_script", fileUploaderScriptLocation);

            var swfUploadJSLocation = Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.FileUploader.js.swfupload.js");
            Page.ClientScript.RegisterClientScriptInclude("fileuploader_swf_script", swfUploadJSLocation);

			var fileHtml5UploaderJSLocation = Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.FileUploader.js.fileHtml5Uploader.js");
			Page.ClientScript.RegisterClientScriptInclude("fileHtml5Uploader_script", fileHtml5UploaderJSLocation);

            Page.ClientScript.RegisterClientScriptBlock(typeof(string), "fileHtml5Uploader_Enable", string.Format("FileHtml5Uploader.EnableHtml5 = typeof window.FileReader != 'undefined' && typeof (new XMLHttpRequest()).upload != 'undefined' && {0};", EnableHtml5.ToString().ToLower()), true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Page.ClientScript.RegisterClientScriptBlock(typeof(string), "fileuploader_swf_init", string.Format(" ASC.Controls.FileUploaderSWFLocation = '{0}'; ", WebPath.GetPath("js/swfupload.swf")), true);
        }
    
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write("<div id=\"asc_fileuploaderSWFContainer\" style='position:absolute;'><span id=\"asc_fileuploaderSWFObj\"></span></div>");
        }

		private static bool IsHtml5Upload(HttpContext context)
		{
			return "html5".Equals(context.Request["type"]);
		}

		private static string GetFileName(HttpContext context)
		{
			return context.Request["fileName"];
		}

		private static string GetFileContentType(HttpContext context)
		{
			return context.Request["fileContentType"];
		}

		public static bool HasFilesToUpload(HttpContext context)
		{
            return 0 < context.Request.Files.Count || (IsHtml5Upload(context) && context.Request.InputStream != null);
		}

		public class FileToUpload
		{
			public string FileName {get; private set;}
			public Stream InputStream {get; private set;}
			public string FileContentType {get; private set;}
			public long ContentLength { get; private set; }

			public FileToUpload(HttpContext context)
			{
				if (IsHtml5Upload(context))
				{
					FileName = GetFileName(context);
					InputStream = context.Request.InputStream;
					FileContentType = GetFileContentType(context);
					ContentLength = (int)context.Request.InputStream.Length;
				}
				else
				{
					var file = context.Request.Files[0];
					FileName = file.FileName;
					InputStream = file.InputStream;
					FileContentType = file.ContentType;
					ContentLength = file.ContentLength;
				}
				if (string.IsNullOrEmpty(FileContentType))
				{
					FileContentType = MimeMapping.GetMimeMapping(FileName) ?? string.Empty;
				}
				FileName = FileName.Replace("'", "_").Replace("\"", "_");
			}
		}        
    }
}
