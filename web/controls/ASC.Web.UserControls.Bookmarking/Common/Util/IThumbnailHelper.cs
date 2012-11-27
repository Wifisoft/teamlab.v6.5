using System;
using System.Configuration;
using System.Net;
using System.Web;
using ASC.Web.UserControls.Bookmarking.Common.Util;

namespace ASC.Web.UserControls.Bookmarking.Util
{
    public interface IThumbnailHelper
    {
        void MakeThumbnail(string url, bool async, bool notOverride, HttpContext context, int tenantID);
        string GetThumbnailUrl(string Url, BookmarkingThumbnailSize size);
        string GetThumbnailUrlForUpdate(string Url, BookmarkingThumbnailSize size);
        void DeleteThumbnail(string Url);
    }

    public class ThumbnailHelper
    {
        private static IThumbnailHelper _processHelper = new WebSiteThumbnailHelper();
        private static IThumbnailHelper _serviceHelper = new ServiceThumbnailHelper();

        public static bool HasService
        {
            get { return ConfigurationManager.AppSettings["bookmarking.thumbnail-url"] != null; }
        }

        public static IThumbnailHelper Instance
        {
            get {
                return HasService ? _serviceHelper : _processHelper;
            }
        }
    }

    internal class ServiceThumbnailHelper : IThumbnailHelper
    {
        private string ServiceFormatUrl
        {
            get { return ConfigurationManager.AppSettings["bookmarking.thumbnail-url"]; }
        }

        public void MakeThumbnail(string url, bool async, bool notOverride, HttpContext context, int tenantID)
        {
            
        }

        public string GetThumbnailUrl(string Url, BookmarkingThumbnailSize size)
        {
            var sizeValue = string.Format("{0}x{1}", size.Width, size.Height);
            return string.Format(ServiceFormatUrl, Url, sizeValue,
                                 Url.GetHashCode());
        }

        public string GetThumbnailUrlForUpdate(string Url, BookmarkingThumbnailSize size)
        {
            var url = GetThumbnailUrl(Url, size);
            try
            {
                var req = WebRequest.Create(url);
                var resp = (HttpWebResponse)req.GetResponse();
                if (resp.StatusCode==HttpStatusCode.OK)
                {
                    return url;
                }
            }
            catch (Exception)
            {
                
            }
            return null;
        }

        public void DeleteThumbnail(string Url)
        {
            
        }
    }
}