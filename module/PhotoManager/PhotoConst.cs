using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Common.Module;
using ASC.Common.Security.Authorizing;
using ASC.Web.Core.Users.Activity;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.PhotoManager
{
    public sealed class PhotoConst
    {
        public static readonly string AddonPath = "~/products/community/modules/photomanager/";

        public static readonly Guid ModuleId = new Guid("{9D51954F-DB9B-4aed-94E3-ED70B914E101}");

        public static readonly Action Action_AddEvent = new Action(
                                                        new Guid("{08D75C97-CF3F-494b-90D1-751C941FE2DD}"),
                                                        "Add event"
                                                    );

        public static readonly Action Action_EditRemoveEvent = new Action(
                                                        new Guid("{298530EB-435E-4dc6-A776-9ABCD95C70E9}"),
                                                        "Edit event"
                                                    );

        public static readonly Action Action_AddPhoto = new Action(
                                                        new Guid("{40BF31F4-3132-4e76-8D5C-9828A89501A3}"),
                                                        "Load photo"
                                                    );

        public static readonly Action Action_EditRemovePhoto = new Action(
                                                        new Guid("{430EAF70-1886-483c-A746-1A18E3E6BB63}"),
                                                        "Edit photo"
                                                    );

        public static readonly Action Action_AddComment = new Action(
                                                        new Guid("{19F658AE-722B-4cd8-8236-3AD150801D96}"),
                                                        "Add comment"
                                                    );

        public static readonly Action Action_EditRemoveComment = new Action(
                                                        new Guid("{18ECC94D-6AFA-4994-8406-AEE9DFF12CE2}"),
                                                        "Edit comment"
                                                    );

        public static readonly AuthCategory BaseAuthCategory = new AuthCategory(
                                                        "Basic",
                                                        new[]{
                                                            Action_AddEvent,
                                                            Action_EditRemoveEvent,
                                                            Action_AddPhoto,
                                                            Action_EditRemovePhoto,
                                                            Action_AddComment,
                                                            Action_EditRemoveComment,
                                                        }
                                                    );

        public static readonly AuthCategory[] AuthorizingCategories = new[] { BaseAuthCategory };


        public static INotifyAction NewPhotoUploaded = new NotifyAction("new photo uploaded", "new photo uploaded");
        public static INotifyAction NewEventComment = new NotifyAction("new event comment", "new event comment");

        public static ITag TagEventName = new Tag("EventName");
        public static ITag TagEventUrl = new Tag("EventURL");
        public static ITag TagAlbumName = new Tag("AlbumName");
        public static ITag TagAlbumURL = new Tag("AlbumURL");
        public static ITag TagPhotoName = new Tag("PhotoName");
        public static ITag TagUserName = new Tag("UserName");
        public static ITag TagUserURL = new Tag("UserURL");
        public static ITag TagDate = new Tag("Date");
        public static ITag TagCommentBody = new Tag("CommentBody");
        public static ITag TagHostName = new Tag("HostName");
        public static ITag TagURL = new Tag("URL");
        public static ITag TagPhotoCount = new Tag("PhotoCount");
        public static ITag TagCommentCount = new Tag("CommentCount");

        #region Constants

        public static Guid ModuleID = PhotoManagerSettings.ModuleID;

        public const string _NewPhotoSubscribeCategory = "{6F06B039-3F42-47a7-BCF1-37225008DD4A}";

        public const int maxLastCommented = 10;
        public const int MAX_TEXT_LENGTH = 255;

        public const string PAGE_ADD_ALBUM = "addnewalbum.aspx";
        public const string PAGE_ADD_PHOTO = "addphoto.aspx";
        public const string PAGE_DEFAULT = "default.aspx";
        public const string PAGE_LAST_COMMENTED = "lastcommented.aspx";
        public const string PAGE_EVENTS = "events.aspx";
        public const string PAGE_EDIT_PHOTO = "editphoto.aspx";
        public const string PAGE_PHOTO = "photo.aspx";
        public const string PAGE_PHOTO_DETAILS = "photodetails.aspx";
        public const string PAGE_FILE_UPLOADER = "fileuploader.aspx";
        public const string PAGE_SLIDER = "slideshow.aspx";

        public const string PAGE_RATINGS = "ratings.aspx";
        public const string PAGE_NEWS = "news.aspx";

        public const string jpeg_extension = "jpg";

        public const string EMPTY_DATE = "0000:00:00 00:00:00";

        public const string URL_IMAGE_USER_NO_PHOTO = "images/user-small.gif";

        public const string PARAM_PAGE = "page";
        public const string PARAM_ALBUM = "item";
        public const string PARAM_ALBUM_FILTER = "filter";
        public const string PARAM_EVENT = "event";
        public const string PARAM_SELECTED_ITEMS = "selected_items";
        public const string PARAM_VALUE = "value";
        public const string PARAM_PHOTO = "photo";
        public const string PARAM_TAG_NAME = "tagName";
        public const string PARAM_EDIT_TAGS = "tags_";
        public const string PARAM_EDIT_NAME = "name_";
        public const string PARAM_EDIT_ITEMID = "itemID_";
        public const string PARAM_EDIT_DESC = "desc_";
        public const string PARAM_USER = "userID";
        public const string PARAM_RATING_TIME = "time";
        public const string PARAM_RATING_ORDER = "order";
        public const string PARAM_RATING = "rate";

        public const string ALBUM_MODE = "mode";
        public const string ALBUM_MODE_EDIT = "edit";
        public const string ALBUM_MODE_REMOVE = "remove";
        public const string ALBUM_MODE_VIEW = "view";


        public const string RATING_TIME_ALL = "all";
        public const string RATING_TIME_MONTH = "month";
        public const string RATING_TIME_WEEK = "week";

        public const string RATING_ORDER_EVAL = "eval";
        public const string RATING_ORDER_COMMENTS = "comments";
        public const string RATING_ORDER_REVIEWS = "reviews";


        public const string COMMENT_MODE_UPDATE = "update";
        public const string COMMENT_MODE_ADD = "add";

        public const string SESSION_PARAM_DIRECTORY = "directory";
        public const string SESSION_PARAM_REDIRECT_TO = "redirectTo";

        public const string THUMB_SUFFIX = "thumb";
        public const string PREVIEW_SUFFIX = "preview";




        public const double EVALUATION_ONE = 1;
        public const double EVALUATION_TWO = 2;
        public const double EVALUATION_THREE = 3;
        public const double EVALUATION_FOUR = 4;
        public const double EVALUATION_FIVE = 5;

        public const double RATING_ONE = -2.5;
        public const double RATING_TWO = -1;
        public const double RATING_THREE = 0.5;
        public const double RATING_FOUR = 1;
        public const double RATING_FIVE = 2.5;

        public const int countSmallPhoto = 20;
        public const int countMediumPhoto = 12;

        #endregion

        #region UserActivityConstants

        public static int AddPhotoBusinessValue = UserActivityConstants.NormalContent;
        public static int EditPhotoBusinessValue = UserActivityConstants.SmallActivity;
        public static int DeletePhotoBusinessValue = UserActivityConstants.SmallActivity;
        public static int AddCommentBusinessValue = UserActivityConstants.NormalActivity;
        public static int EditCommentBusinessValue = UserActivityConstants.SmallActivity;
        public static int DeleteCommentBusinessValue = UserActivityConstants.SmallActivity;
        public static int EvaluateBusinessValue = UserActivityConstants.SmallActivity;


        #endregion

        #region Virtual Path

        const string BaseVirualPath = "~/products/community/modules/photomanager/";
        public const string ImagesPath = "images/";

        public static string GetModuleAbsolutePath(string virualPath)
        {
            return VirtualPathUtility.ToAbsolute(VirtualPathUtility.Combine(BaseVirualPath, virualPath));
        }

        public static string ViewEventPageUrl { get { return GetModuleAbsolutePath(PAGE_DEFAULT); } }
        public static string ViewAlbumPageUrl { get { return GetModuleAbsolutePath(PAGE_PHOTO); } }
        public static string AddPhotoPageUrl { get { return GetModuleAbsolutePath(PAGE_ADD_PHOTO); } }
        public static string ViewPhotoPageUrl { get { return GetModuleAbsolutePath(PAGE_PHOTO_DETAILS); } }
        public static string UserPhotosPageUrl { get { return GetModuleAbsolutePath(PAGE_PHOTO) + "?" + PARAM_USER + "="; } }
        #endregion
    }
}
