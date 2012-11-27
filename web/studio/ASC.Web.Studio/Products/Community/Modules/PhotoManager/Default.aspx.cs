using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.PhotoManager;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.PhotoManager.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.PhotoManager
{
    [AjaxNamespace("Default")]
    public partial class Default : BasePage
    {
        #region Members

        private int _selectedPage;
        private int _CountEventPerPage = 3;

        #endregion

        #region Methods

        protected override void PageLoad()
        {
            //this.Title = Resources.PhotoManagerResource.PageTitleDefault;
            LoadData();
            formContainer.Options.IsPopup = true;
            Utility.RegisterTypeForAjax(typeof (Default), Page);
            this.Title = HeaderStringHelper.GetPageTitle(PhotoManagerResource.AddonName, mainContainer.BreadCrumbs);

            sideRecentActivity.TenantId = TenantProvider.CurrentTenantID;
            sideRecentActivity.ProductId = Product.CommunityProduct.ID;
            sideRecentActivity.ModuleId = PhotoConst.ModuleID;
        }

        private void LoadData()
        {
            InitPageParams();

            if (!IsPostBack)
            {
                var storage = StorageFactory.GetStorage();
                var store = Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");
                var sb = new StringBuilder();

                #region all events

                if (String.IsNullOrEmpty(Request.QueryString[PhotoConst.PARAM_EVENT]))
                {
                    var events = storage.GetEvents((_selectedPage - 1)*_CountEventPerPage, _CountEventPerPage);

                    mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = PhotoManagerResource.PhotoTitle, NavigationUrl = PhotoConst.PAGE_DEFAULT});

                    var count = storage.GetEventsCount();

                    var pageNavigator = new Web.Controls.PageNavigator
                                            {
                                                PageUrl = PhotoConst.PAGE_DEFAULT + "?t=",
                                                CurrentPageNumber = _selectedPage,
                                                EntryCountOnPage = _CountEventPerPage,
                                                VisiblePageCount = 5,
                                                ParamName = "page",
                                                EntryCount = (int) count
                                            };

                    pageNavigatorHolder.Controls.Add(pageNavigator);

                    sb.Append(ImageHTMLHelper.DrawEvents(events, store));
                }
                    #endregion

                    #region selected event

                else
                {
                    var Event = storage.GetEvent(Convert.ToInt64(Request.QueryString[PhotoConst.PARAM_EVENT]));

                    mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = PhotoManagerResource.PhotoTitle, NavigationUrl = PhotoConst.PAGE_DEFAULT});

                    if (Event != null)
                    {
                        mainContainer.BreadCrumbs.Add(new Web.Controls.BreadCrumb {Caption = Event != null ? Event.Name : Request.QueryString[PhotoConst.PARAM_EVENT]});

                        if (storage.GetAlbumsCount(Event.Id, null) == 0)
                        {
                            sb.Append("<center><div style='margin: 40px 0px 80px 0px;' class=\"headerPanel\">" + PhotoManagerResource.EventHaveNoAlbumsMsg.Replace(":UPLOAD_LINK", "<a class=\"linkHeaderLight\" style=\"text-decoration: underline;\" href=\"" + ASC.PhotoManager.PhotoConst.PAGE_ADD_PHOTO + "?" + "event=" + Event.Id + "\">" + PhotoManagerResource.UploadPhotosLink + "</a>").Replace(":REMOVE_LINK", "<a class=\"linkHeaderLight\" style=\"text-decoration: underline;\" href=\"javascript:EventsManager.RemoveEvent(" + Event.Id + ");\">" + PhotoManagerResource.RemoveButton + "</a>") + "</div><center>");
                        }
                        sb.Append(ImageHTMLHelper.DrawEvent(Event, store));
                    }
                    else
                        sb.AppendFormat("<div class=\"noContentBlock\">{0}</div>", PhotoManagerResource.NoFoundMessage);

                }

                #endregion

                if (String.IsNullOrEmpty(sb.ToString()))
                {
                    var emptyScreenControl = new EmptyScreenControl
                                                 {
                                                     ImgSrc = WebImageSupplier.GetAbsoluteWebPath("150x_photo_icon.png", PhotoConst.ModuleID),
                                                     Header = PhotoManagerResource.EmptyScreenCaption,
                                                     Describe = PhotoManagerResource.EmptyScreenText,
                                                     ButtonHTML = CommunitySecurity.CheckPermissions(PhotoConst.Action_AddPhoto) ? String.Format("<a class='linkAddMediumText' href='" + PhotoConst.PAGE_ADD_PHOTO + "'>{0}</a>", PhotoManagerResource.EmptyScreenLink) : String.Empty
                                                 };
                    _contentHolder.Controls.Add(emptyScreenControl);
                }
                else
                {
                    _contentHolder.Controls.Add(new Literal {Text = sb.ToString()});
                }
            }
        }

        private void InitPageParams()
        {
            if (Request.QueryString[PhotoConst.PARAM_PAGE] == null || Request.QueryString[PhotoConst.PARAM_PAGE] == string.Empty)
                _selectedPage = 1;
            else
            {
                try
                {
                    _selectedPage = Convert.ToInt32(Request.QueryString[PhotoConst.PARAM_PAGE]);
                }
                catch
                {
                    _selectedPage = 1;
                }
            }

            if (_selectedPage <= 0)
                _selectedPage = 1;
        }

        #endregion

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string SaveEvent(long id, string name, string description, string dateTime)
        {
            var storage = StorageFactory.GetStorage();
            var Event = storage.GetEvent(id);

            CommunitySecurity.DemandPermissions(Event, PhotoConst.Action_EditRemoveEvent);

            DateTime date;
            DateTime.TryParse(dateTime, out date);

            Event.Name = GetLimitedText(name);
            Event.Description = GetLimitedText(description);
            Event.Timestamp = date;

            storage.SaveEvent(Event);

            return id.ToString();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string RemoveEvent(long id)
        {
            var storage = StorageFactory.GetStorage();
            var Event = storage.GetEvent(id);

            CommunitySecurity.DemandPermissions(Event, PhotoConst.Action_EditRemoveEvent);
            var store = Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");

            foreach (var album in storage.GetAlbums(id, null))
            {
                var pathAlbum = PhotoConst.ImagesPath + album.UserID + "/" + album.Id + "/";

                store.DeleteFiles(pathAlbum, "*", false);
            }

            var _storage = StorageFactory.GetStorage();
            var subscriptionProvider = _storage.NotifySource.GetSubscriptionProvider();

            subscriptionProvider.UnSubscribe(PhotoConst.NewEventComment, id.ToString());

            storage.RemoveEvent(Event.Id);

            return id.ToString(); //RenderEvents();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse GetInfoEvent(long id)
        {

            var storage = StorageFactory.GetStorage();
            var Event = storage.GetEvent(id);

            var rs = new AjaxResponse
                         {
                             rs1 = Event.Name,
                             rs2 = Event.Description,
                             rs3 = Event.Timestamp.ToShortDateString()
                         };

            return rs; //RenderEvents();
        }
    }
}