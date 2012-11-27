using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.PhotoManager;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.PhotoManager.Resources;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Controls.Dashboard;
using ASC.Web.Studio.Utility;
using ASC.Web.Community.Product;

namespace ASC.Web.Community.PhotoManager
{
    [Serializable]
    [DataContract]
    public class PhotoManagerWidgetSettings : ISettings
    {
        [DataMember(Name = "MaxCountLastAlbums")]
        public int MaxCountLastAlbums { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{CAB3155F-F112-4e21-A27E-EA3A91AEAEC8}"); }
        }

        public ISettings GetDefault()
        {
            return new PhotoManagerWidgetSettings {MaxCountLastAlbums = 5};
        }

        #endregion
    }

    [AjaxNamespace("PhotoManagerLastPhoto")]
    [WidgetPosition(2, 2)]
    public class PhotoManagerLastPhoto : WebControl, ICheckEmptyContent
    {
        #region Members

        #endregion

        #region Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Utility.RegisterTypeForAjax(typeof (PhotoManagerLastPhoto));
        }

        #endregion

        #region Methods

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string UpdateContent()
        {
            return RenderLastUpdateContent();
        }

        public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderBeginTag(writer);
            writer.Write("<div id=\"PhotoManager_LastPhotoContent\" >");
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            writer.Write(RenderLastUpdateContent());
        }

        public override void RenderEndTag(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("</div>");
            base.RenderEndTag(writer);
        }

        private string RenderLastUpdateContent()
        {
            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<PhotoManagerWidgetSettings>(SecurityContext.CurrentAccount.ID);
            var sb = new StringBuilder();

            var storage = StorageFactory.GetStorage();
            IList<Album> list = storage.GetLastAlbums(widgetSettings.MaxCountLastAlbums);

            sb.Append("<div class='clearFix'>");

            if (list.Count > 0)
            {

                foreach (var item in list)
                {
                    sb.Append(GetAlbumInfo(item));
                }

                sb.Append("<div style=\"padding-top:10px;\">");
                sb.Append("<a href=\"" + VirtualPathUtility.ToAbsolute(PhotoConst.AddonPath + PhotoConst.PAGE_DEFAULT) + "\">" + PhotoManagerResource.SeeAllAlbumsLink + "</a>");
                sb.Append("</div>");
            }
            else
            {
                if (CommunitySecurity.CheckPermissions(PhotoConst.Action_AddPhoto))
                {
                    sb.Append("<div class=\"empty-widget\" style=\"padding:40px; text-align: center;\">" +
                              String.Format(PhotoManagerResource.EmptyWidgetMessage,
                                            string.Format("<div style=\"padding-top:3px;\"><a class=\"promoAction\" href=\"{0}\">", VirtualPathUtility.ToAbsolute(PhotoConst.AddonPath + PhotoConst.PAGE_ADD_PHOTO)),
                                            "</a></div>") + "</div>");
                }
                else
                    sb.Append("<div class=\"empty-widget\" style=\"padding:40px; text-align: center;\">" + PhotoManagerResource.YouHaveNoPhotosTitle + "</div>");
            }


            sb.Append("</div>");


            return sb.ToString();
        }

        private string GetAlbumInfo(Album item)
        {
            var sb = new StringBuilder();

            var face = item.FaceItem;

            var date = item.LastUpdate;
            var store = Data.Storage.StorageFactory.GetStorage("~/Products/Community/Modules/PhotoManager/web.config", TenantProvider.CurrentTenantID.ToString(), "photo", HttpContext.Current);

            var albumURL = VirtualPathUtility.ToAbsolute(PhotoConst.AddonPath + PhotoConst.PAGE_PHOTO) + "?" + PhotoConst.PARAM_ALBUM + "=" + item.Id;

            sb.Append("<div style=\"margin-bottom:10px;\">");
            sb.Append("<table cellpadding='0' cellspacing='0' border='0'><tr valign=\"top\"><td style=\"width:60px;\">");
            sb.Append(GetHTMLSmallThumb(face, 54, albumURL, store));

            sb.Append("</td><td>");
            sb.Append("<div style='padding-left:10px;'>");

            sb.Append("<div style=\"margin-top:2px;\">");
            sb.Append("<a class='linkHeaderLightMedium' href='" + VirtualPathUtility.ToAbsolute(PhotoConst.AddonPath + PhotoConst.PAGE_DEFAULT) + "?" + PhotoConst.PARAM_EVENT + "=" + item.Event.Id + "'>" + HttpUtility.HtmlEncode(item.Event.Name) + "</a>");
            sb.Append("</div>");

            sb.Append("<div style=\"margin-top: 6px;\">");
            sb.Append("<a class='linkHeaderSmall' href='" + albumURL + "'>" + (face != null ? DisplayUserSettings.GetFullUserName(new Guid(face.UserID)) : "") + "</a>");
            sb.Append("</div>");

            sb.Append("<div style=\"margin-top: 5px;\">");
            sb.Append("<a href='" + albumURL + "'>" + Grammatical.PhotosCount("{0}&nbsp;{1}", item.ImagesCount) + "</a>");

            sb.Append("<span class='textMediumDescribe' style='margin-left:10px;'>" + date.ToShortDateString() + "</span>");
            sb.Append("</div>");

            sb.Append("</div>");
            sb.Append("</td></tr></table>");

            sb.Append("</div>");

            return sb.ToString();
        }

        private string GetHTMLSmallThumb(AlbumItem image, int maxSize, string link, Data.Storage.IDataStore store)
        {
            var sb = new StringBuilder();

            var limit = ImageHTMLHelper.GetImageSizeLimit(image, maxSize);

            sb.Append("<span style=\"padding: 3px;	display: inline-block;	position: relative;	text-align: center;	vertical-align: top;\" >");
            sb.Append("<table border=0 cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"text-align:center;vertical-align:middle;border:solid 0px #cfcfcf;height:" + maxSize + "px;width:" + maxSize + "px;table-layout:fixed;\">");
            sb.Append("<a href=\"" + link + "\">");
            sb.Append("<img " + limit + " title=\"" + (image != null ? HttpUtility.HtmlEncode(image.Name) : "") + "\" src=\"" + (image != null ? ImageHTMLHelper.GetImageUrl(image.ExpandedStoreThumb, store) : "") + "\" class=\"borderBase\">");

            sb.Append("</a></td></tr></table></span>");

            return sb.ToString();
        }

        #endregion

        #region ICheckEmptyContent Members

        public CheckEmptyContentResult IsEmpty()
        {
            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<PhotoManagerWidgetSettings>(SecurityContext.CurrentAccount.ID);
            var storage = StorageFactory.GetStorage();
            IList<Album> list = storage.GetLastAlbums(widgetSettings.MaxCountLastAlbums);

            if (list.Count > 0)
                return CheckEmptyContentResult.NotEmpty;

            return CheckEmptyContentResult.Empty;
        }

        #endregion
    }
}