using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Notify.Model;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;

namespace ASC.PhotoManager.Helpers
{
    public class ImageHTMLHelper
    {
        public static string GetSmallHTMLImage(AlbumItem image, int imgNumber, bool isSelected, int maxSize, ASC.Data.Storage.IDataStore store)
        {
            var sb = new StringBuilder();

            var limit = GetImageSizeLimit(image, maxSize);

            sb.Append("<a href=\"" + PhotoConst.PAGE_PHOTO_DETAILS + "?photo=" + image.Id + "\">");
            sb.Append("<img " + limit + " style=\"border:solid 0px #FFF;\" src=\"" + GetImageUrl(image.ExpandedStoreThumb, store) + "\" title=\"" + HttpUtility.HtmlEncode(image.Name) + "\"/>");
            sb.Append("</a>");

            return "<span style=\"display:inline-block;padding-right:3px;padding-bottom:3px;\"><table border='0' cellpadding='0' cellspacing='0'><tr><td style='width:" + maxSize + "px;height:" + maxSize + "px;' align='center' valign='middle'>" + sb.ToString() + "</td></tr></table></span>";

        }

        public static string GetHTMLThumb(string imgPath, int maxSize, int width, int height)
        {
            var sb = new StringBuilder();
            var limit = string.Empty;

            if (width >= height && width > maxSize)
                limit = " style=\"width:" + maxSize + "px;\"";
            else if (height >= width && height > maxSize)
                limit = " style=\"height:" + maxSize + "px;\"";


            sb.Append("<table border=0 cellpadding=\"0\" cellspacing=\"0\" style='width:" + maxSize + "px;height:" + maxSize + "px;'><tr><td valign='middle' align='center'>");
            sb.Append("<img " + limit + "  src=\"" + imgPath + "\">");
            sb.Append("</td></tr></table>");

            return sb.ToString();
        }

        public static string GetAlbumThumb(AlbumItem image, int maxSize, int pad, string link, ASC.Data.Storage.IDataStore store)
        {
            var sb = new StringBuilder();
            var limit = GetImageSizeLimit(image, maxSize);

            sb.Append("<a style=\"text-align:left;padding:0px;\" href=\"" + link + "\">");
            if (image != null)
                sb.Append("<img " + limit + " class=\"borderBase\" title=\"" + HttpUtility.HtmlEncode(image.Name) + "\" src=\"" + GetImageUrl(image.ExpandedStoreThumb, store) + "\" />");
            sb.Append("</a>");

            var date = image.Album.LastUpdate;

            var caption = (string.IsNullOrEmpty(image.Album.Caption) ? DisplayUserSettings.GetFullUserName(new Guid(image.Album.UserID)) : HttpUtility.HtmlEncode(image.Album.Caption));

            var album_url = PhotoConst.PAGE_PHOTO + "?" + PhotoConst.PARAM_ALBUM + "=" + image.Album.Id;
            return
                "<span>" +
                "<table cellpadding='0' cellspacing='0' border='0' class=\"borderBase\"><tr><td><div  style=\"padding:" + pad + "px;background-color:#fff;\">" + sb.ToString() + "</div></td></tr><tr><td class=\"borderBase\" style='border-width:1px 0px 0px 0px;padding-top:1px;background-color:#fff;'></td></tr><tr><td class=\"borderBase\" style='border-width:1px 0px 0px 0px;padding-top:1px;background-color:#fff;'></td></tr></table>" +
                "<div style='text-align:left;width: 150px;word-wrap: break-word;'><div style=\"padding:10px 5px 6px;width:" + maxSize + "px;\">" +
                "<a href=\"" + album_url + "\" class=\"linkHeader\">" + caption + "</a></div><div style=\"padding:2px 5px 5px;\"><a href=\"" + album_url + "\">" + Grammatical.PhotosCount(image.Album.ImagesCount) + "</a></div>" +
                "<div class=\"textMediumDescribe\" style=\"padding:5px\">" + PhotoManagerResource.LastUpdateTitle + ": " + date.ToShortDateString() + "</div></div></span>";

        }

        public static string GetUserAlbumThumb(AlbumItem image, int maxSize, int pad, string link, ASC.Data.Storage.IDataStore store)
        {
            var sb = new StringBuilder();
            var limit = GetImageSizeLimit(image, maxSize);

            sb.Append("<a style=\"padding:0px;\" href=\"" + link + "\">");

            if (image != null)
                sb.Append("<img " + limit + " class=\"borderBase\" title=\"" + HttpUtility.HtmlEncode(image.Name) + "\" src=\"" + GetImageUrl(image.ExpandedStoreThumb, store) + "\" />");
            else
                sb.Append("&nbsp;");
            sb.Append("</a>");

            if (image == null)
                return sb.ToString();
            var date = image.Album.LastUpdate;

            var event_url = PhotoConst.PAGE_DEFAULT + "?" + PhotoConst.PARAM_EVENT + "=" + image.Album.Event.Id;
            return "<span ><table cellpadding='0' cellspacing='0' border='0' class=\"borderBase\"><tr><td><div  style=\"padding:" + pad + "px;background-color:#fff;\">" + sb.ToString() + "</div></td></tr><tr><td class=\"borderBase\" style='border-width:1px 0px 0px 0px;padding-top:1px;background-color:#fff;'></td></tr><tr><td class=\"borderBase\" style='border-width:1px 0px 0px 0px;padding-top:1px;background-color:#fff;'></td></tr></table>" +
                   "<div style='text-align:left;width: 150px;word-wrap: break-word;'><div style=\"padding:10px 5px 5px 5px; width:" + maxSize + "px;\">" +
                   "<a href=\"" + event_url + "\" class=\"linkHeader\">" + HttpUtility.HtmlEncode(image.Album.Event.Name) + "</a></div><div style=\"padding:0px 5px;\"><a href=\"" + PhotoConst.PAGE_PHOTO + "?" + PhotoConst.PARAM_ALBUM + "=" + image.Album.Id + "\">" + Grammatical.PhotosCount(image.Album.ImagesCount) + "</a></div>" +
                   "<div class=\"textMediumDescribe\" style=\"padding:8px 5px 5px 5px;\">" + PhotoManagerResource.LastUpdateTitle + ": " + date.ToShortDateString() + "</div></div></span>";

        }

        public static string GetHTMLSmallThumb(AlbumItem image, int maxSize, string link, bool selected, bool isVisible, ASC.Data.Storage.IDataStore store)
        {
            var sb = new StringBuilder();
            var limit = GetImageSizeLimit(image, maxSize);

            if (image.Id == 0)
            {
                sb.Append("<td style=\"padding-right:1px;width:" + maxSize + "px;table-layout:fixed;" + (isVisible ? "display:;" : "display:none;") + "\" >&nbsp;</td>");
            }
            else
            {
                sb.Append("<td style=\"padding-right:1px;table-layout:fixed;" + (isVisible ? "display:;" : "display:none;") + "\" >");
                if (selected)
                {
                    sb.Append("<div class='PhotoManager_CurrentPhoto' style=\"background-imagealbumUrll(" + WebImageSupplier.GetAbsoluteWebPath("current.png", PhotoConst.ModuleID) + ");\"></div>");
                }
                sb.Append("<table border='0' cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"border:0 solid #000000;table-layout:fixed;text-align:center;vertical-align:middle;\">");
                sb.Append("<div style=\"padding:0px;\"><img " + link + " " + limit + " style=\"cursor:pointer; border: solid 0px #FFF;\" title=\"" + HttpUtility.HtmlEncode(image.Name) + "\" src=\"" + GetImageUrl(image.ExpandedStoreThumb, store) + "\" />");

                sb.Append("</div></td></tr></table>");

                sb.Append("</td>");


            }
            return sb.ToString();
        }

        public static string GetHTMLImgUserAvatar(Guid userID)
        {
            var imgPath = UserPhotoManager.GetSmallPhotoURL(userID);
            if (imgPath != null)
                return "<img class=\"userMiniPhoto\" src=\"" + imgPath + "\"/>";

            return "";
        }

        public static string GetImageSizeLimit(AlbumItem image, int maxSize)
        {
            if (image == null)
                return "";

            if (image.ThumbnailSize.Width >= image.ThumbnailSize.Height && image.ThumbnailSize.Width > maxSize)
                return " width=\"" + maxSize + "\"";
            else if (image.ThumbnailSize.Height >= image.ThumbnailSize.Width && image.ThumbnailSize.Height > maxSize)
                return " height=\"" + maxSize + "\"";
            else
                return string.Empty;
        }

        public static string GetImagePreviewSizeLimit(AlbumItem image, int maxSize)
        {
            if (image == null)
                return "";

            if (image.PreviewSize.Width >= image.PreviewSize.Height && image.PreviewSize.Width > maxSize)
                return " width=\"" + maxSize + "\"";
            else if (image.PreviewSize.Height >= image.PreviewSize.Width && image.PreviewSize.Height > maxSize)
                return " height=\"" + maxSize + "\"";
            else
                return string.Empty;
        }

        public static string GetImagePreviewSizeLimit(AlbumItem image, int maxXSize, int maxYSize)
        {
            if (image == null)
                return "";

            if (image.PreviewSize.Width > maxXSize && image.PreviewSize.Height > maxYSize)
            {
                if ((double) image.PreviewSize.Width/(double) maxXSize > (double) image.PreviewSize.Height/(double) maxYSize)
                    return " width=\"" + maxXSize + "\"";
                else
                    return " height=\"" + maxYSize + "\"";
            }
            else if (image.PreviewSize.Width > maxXSize)
            {
                return " width=\"" + maxXSize + "\"";
            }
            else if (image.PreviewSize.Height > maxYSize)
            {
                return " height=\"" + maxYSize + "\"";
            }
            else
            {
                return string.Empty;
            }
        }

        public static string DrawEvents(IList<Event> events, ASC.Data.Storage.IDataStore store)
        {
            var sb = new StringBuilder();

            foreach (var Event in events)
            {
                sb.Append("<div class=\"borderBase tintMedium clearFix\" style=\"border-left:none;border-right:none;margin-bottom:8px;width: 722px;overflow: hidden;\">");

                sb.Append(DrawEvent(Event, true, store));

                sb.Append("</div>");
            }

            return sb.ToString();
        }

        public static string DrawEvents(IList<Event> events, string tagText, string userID, string page, bool drawAddnew, ASC.Data.Storage.IDataStore store)
        {
            var sb = new StringBuilder();

            foreach (var Event in events)
            {
                var storage = StorageFactory.GetStorage();

                IList<Album> albums;

                if (string.IsNullOrEmpty(tagText))
                {
                    albums = storage.GetAlbums(Event.Id, userID);
                }
                else
                {
                    throw new NotImplementedException();
                }


                sb.Append("<table cellpadding='0' cellspacing='0' border='0' ><tr valign='top'>");

                var i = 0;
                foreach (var album in albums)
                {
                    if (album.ImagesCount == 0) continue;

                    if (i%3 == 0 && i != 0)
                        sb.Append("</tr><tr valign='top'>");

                    if (i%3 == 0)
                        sb.Append("<td align='left' style='width:220px;padding-bottom:15px;'>");

                    if (i%3 == 1)
                        sb.Append("<td align='center' style='width:220px;padding-bottom:15px;'>");

                    if (i%3 == 2)
                        sb.Append("<td align='right' style='width:220px;padding-bottom:15px;'>");

                    sb.Append(GetAlbumThumb(album.FaceItem, 135, 6, PhotoConst.PAGE_PHOTO + "?" + PhotoConst.PARAM_ALBUM + "=" + album.Id, store));

                    sb.Append("</td>");

                    i++;
                }

                sb.Append("</tr></table>");


                var isSubscribed = StorageFactory.GetStorage().NotifySource.GetSubscriptionProvider().IsSubscribed(
                    PhotoConst.NewEventComment,
                    new Notify.Recipients.DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), ""),
                    Event.Id.ToString()
                    );

                sb.AppendFormat("<div id=\"photo_comment_notifies\" style=\"padding: 5px; float: left;\">");
                sb.AppendFormat("<a id=\"sub_on_event_{2}\" title=\"{1}\" href=\"javascript:;\" >{0}</a>",
                                (isSubscribed ? PhotoManagerResource.UnNotifyOnNewEventCommentsMessage : PhotoManagerResource.NotifyOnNewEventCommentsMessage),
                                PhotoManagerResource.NotifyOnNewEventCommentsDescription,
                                Event.Id);
                sb.AppendFormat("<script>");
                sb.AppendFormat(@"
var subscribed_at_event_{0} = {1}
jq('#sub_on_event_{0}').click(function(){{
AjaxPro.onLoading = function(b){{if(b){{jq('#photo_comment_notifies').block();}}else{{jq('#photo_comment_notifies').unblock();}}}};
ASC.Web.Community.PhotoManager.Photo.SubscribeOnComments('{0}',subscribed_at_event_{0},function(result){{
        if(result.value != null){{
            subscribed_at_event_{0} = (result.value.rs1=='true')?true:false;
            jq('#sub_on_event_{0}').html(result.value.rs2);
        }}
    }});    
}})
", Event.Id, isSubscribed.ToString().ToLower());
                sb.AppendFormat("</script>");
                sb.AppendFormat("</div>");


                sb.Append("</div>");

            }

            return sb.ToString();
        }

        public static string DrawAlbumsAlone(IList<Album> albums, ASC.Data.Storage.IDataStore store)
        {
            return DrawInBox(DrawAlbums(albums, false, store));
        }

        public static string DrawAlbums(IList<Album> albums, bool inEvent, ASC.Data.Storage.IDataStore store)
        {
            var sb = new StringBuilder();

            sb.Append("<table cellpadding='0' cellspacing='0' border='0' ><tr valign='top'>");

            var i = 0;
            foreach (var album in albums)
            {
                if (album.ImagesCount == 0) continue;

                if (i%3 == 0 && i != 0)
                    sb.Append("</tr><tr valign='top'>");

                if (i%3 == 0)
                    sb.Append("<td align='left' style='width:220px;padding-bottom:15px;'>");
                if (i%3 == 1)
                    sb.Append("<td align='center' style='width:220px;padding-bottom:15px;'>");
                if (i%3 == 2)
                    sb.Append("<td align='right' style='width:220px;padding-bottom:15px;'>");

                if (inEvent)
                    sb.Append(GetAlbumThumb(album.FaceItem, 135, 6, PhotoConst.PAGE_PHOTO + "?" + PhotoConst.PARAM_ALBUM + "=" + album.Id, store));
                else
                    sb.Append(GetUserAlbumThumb(album.FaceItem, 135, 6, PhotoConst.PAGE_PHOTO + "?" + PhotoConst.PARAM_ALBUM + "=" + album.Id, store));

                sb.Append("</td>");

                i++;
            }
            sb.Append("</tr></table>");


            return sb.ToString();
        }

        public static string DrawAlbums(IList<Album> albums, ASC.Data.Storage.IDataStore store)
        {
            return DrawAlbums(albums, true, store);
        }

        public static string DrawEvent(Event Event, ASC.Data.Storage.IDataStore store)
        {
            return DrawEvent(Event, false, store);
        }

        public static string DrawEvent(Event Event, bool showCaption, ASC.Data.Storage.IDataStore store)
        {
            var sb = new StringBuilder();

            var storage = StorageFactory.GetStorage();

            var albums = storage.GetAlbums(Event.Id, null);
            if (showCaption)
            {
                sb.Append("<div style=\"padding:10px 0px 20px 0px;\"><a href=\"" + PhotoConst.PAGE_DEFAULT + "?" + PhotoConst.PARAM_EVENT + "=" + Event.Id + "\" class=\"linkHeaderLightBig\">" + HttpUtility.HtmlEncode(Event.Name) + "</a><div class=\"textMediumDescribe\" style=\"margin-top:5px;\">" + Event.Timestamp.ToShortDateString() + "<span class='splitter'>|</span><a href=\"" + PhotoConst.PAGE_DEFAULT + "?" + PhotoConst.PARAM_EVENT + "=" + Event.Id + "\">" + Grammatical.AlbumsCount(albums.Count) + "</a></div>");
                sb.Append("</div>");
            }

            sb.AppendFormat("<div id='event_{0}'>", Event.Id);
            sb.Append(DrawAlbums(albums, store));

            if (!showCaption && CommunitySecurity.CheckPermissions(PhotoConst.Action_EditRemoveEvent))
            {
                sb.Append("<div style=\"float:right;\"><a href='javascript:EventsManager.EditEvent(" + Event.Id + ");' class=\"linkAction" + "\">" + PhotoManagerResource.EditButton + "</a><span class='splitter'>|</span><a href='javascript:EventsManager.RemoveEvent(" + Event.Id + ");' class=\"linkAction" + "" + "\">" + PhotoManagerResource.RemoveButton + "</a></div>");
            }
            sb.Append("</div>");


            return DrawInBox(sb.ToString());
        }

        public static string DrawInBox(string html)
        {
            return String.Format("<div class='clearFix' style=\"padding:5px 30px;\">{0}</div>", html);
        }

        public static string GetImageUrl(string name, ASC.Data.Storage.IDataStore store)
        {
            return store.GetUri("", PhotoConst.ImagesPath + name).OriginalString;
        }

    }
}