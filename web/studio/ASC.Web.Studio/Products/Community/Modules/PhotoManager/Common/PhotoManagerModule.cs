using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Web.Core;
using ASC.Web.Community.PhotoManager.Common;
using ASC.Web.Core.ModuleManagement;
using ASC.PhotoManager.Resources;
using ASC.Web.Core.Users.Activity;
using ASC.PhotoManager.Helpers;
using ASC.Core;
using ASC.PhotoManager;
using ASC.Web.Community.Product;

[assembly: Module(typeof(PhotoManagerModule))]

namespace ASC.Web.Community.PhotoManager.Common
{
    public class PhotoManagerModule : Module
    {
        public override Guid ID
        {
            get { return PhotoConst.ModuleId; }
        }

        public override string Name
        {
            get { return PhotoManagerResource.AddonName; }
        }

        public override string Description
        {
            get { return PhotoManagerResource.AddonDescriptionResourceKey; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/photomanager/"; }
        }

        public override IEnumerable<Widget> Widgets
        {
            get
            {
                var w = new Widget
                {
                    ID = new Guid("A5C1E3B9-D639-4fd1-A29A-2B97CB4E58E4"),
                    Name = PhotoManagerResource.AddonName,
                    Description = PhotoManagerResource.WidgetDescriptionResourceKey,
                    StartURL = "~/products/community/modules/photomanager/default.aspx",
                    SettingsType = typeof(PhotoManagerWidgetSettingsProvider),
                    WidgetType = typeof(PhotoManagerLastPhoto),
                    Context = new WebItemContext { IconFileName = "photo_icon.png" }
                };
                return new[] { w };
            }
        }

        public override IEnumerable<IWebItem> Actions
        {
            get
            {
                if (CanAddPhoto())
                {
                    var a1 = new NavigationWebItem
                    {
                        ID = new Guid("CDE7CBAF-98A6-4228-902F-A690DA89B763"),
                        Name = PhotoManagerResource.UploadPhotoTitle,
                        StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/photomanager/addphoto.aspx"),
                    };
                    return new[] { a1, };
                }
                return Enumerable.Empty<IWebItem>();
            }
        }

        public override IEnumerable<IWebItem> Navigations
        {
            get
            {
                var n1 = new NavigationWebItem
                {
                    ID = new Guid("1E16B64A-DBAF-40e8-86AB-A116AE12E949"),
                    Name = PhotoManagerResource.MainPageMenuTitle,
                    StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/photomanager/"),
                    Context = new WebItemContext { IconFileName = "home.png" },
                };
                var n2 = new NavigationWebItem
                {
                    ID = new Guid("4367C1B3-9F22-41a9-9CF1-DDCC612AFEE0"),
                    Name = PhotoManagerResource.MyPhotoTitle,
                    StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/photomanager/photo.aspx"),
                };
                var n3 = new NavigationWebItem
                {
                    ID = new Guid("66308582-DAEC-4112-8CF1-F0610C44447F"),
                    Name = PhotoManagerResource.LastCommentedTitle,
                    StartURL = VirtualPathUtility.ToAbsolute("~/products/community/modules/photomanager/lastcommented.aspx"),
                };
                return new []{n1,n2,n3,};
            }
        }


        public PhotoManagerModule()
        {
            Context = new ModuleContext
            {
                DefaultSortOrder = 3,
                ThemesFolderVirtualPath = "~/products/community/modules/photomanager/app_themes",
                ImageFolder = "photomanagerimages",
                SmallIconFileName = "photo_small.png",
                IconFileName = "photo_icon.png",
                SubscriptionManager = new PhotoManagerSubscriptionManager(),
                StatisticProvider = new PhotoStatisticProvider(),
                UserActivityPublishers = new List<IUserActivityPublisher>() { new PhotoUserActivityPublisher() },
                GetCreateContentPageAbsoluteUrl = () => CanAddPhoto() ? VirtualPathUtility.ToAbsolute(PhotoConst.AddonPath + PhotoConst.PAGE_ADD_PHOTO) : null,
            };
        }

        private static bool CanAddPhoto()
        {
            return CommunitySecurity.CheckPermissions(PhotoConst.Action_AddPhoto);
        }
    }
}
