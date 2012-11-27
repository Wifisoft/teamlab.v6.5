using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace ASC.Web.Controls.FileUploader.HttpModule
{
    public class UploadProgressModule : IHttpModule
    {
        private readonly static FieldInfo _requestWorkerField = typeof(HttpRequest).GetField("_wr", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly Regex _isUrlWithExtension = new Regex(@"[^\.]+\.a[^x]+x", RegexOptions.Compiled);
        

        public void Init(HttpApplication application)
        {
            application.BeginRequest += OnBeginRequest;
            application.EndRequest += OnEndRequest;
        }

        public void Dispose()
        {
        }


        private void OnBeginRequest(object sender, EventArgs e)
        {
            var request = ((HttpApplication)sender).Context.Request;
            var origWr = (HttpWorkerRequest)_requestWorkerField.GetValue(request);
            
            if (UploadProgressUtils.IsUpload(origWr))
            {
                var s = request.RawUrl;

                if (string.IsNullOrEmpty(s))
                    return;

                if (!_isUrlWithExtension.IsMatch(s))
                    return;

                var newWr = new HttpUploadWorkerRequest(origWr);                
                _requestWorkerField.SetValue(request, newWr);
            }
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var origWr = _requestWorkerField.GetValue(((HttpApplication)sender).Context.Request) as HttpUploadWorkerRequest;
            if (origWr != null)
            {
                origWr.EndOfUploadRequest();
            }
        }
    }        
}
