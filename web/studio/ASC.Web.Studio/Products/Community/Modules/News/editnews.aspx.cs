using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.News
{
    [AjaxNamespace("EditNews")]
    public partial class EditNews : MainPage
    {
        private RequestInfo info;
        protected bool _mobileVer = false;
        protected string _text = "";

        private RequestInfo Info
        {
            get
            {
                if (info == null) info = new RequestInfo(Request);
                return info;
            }
        }

        private List<BreadCrumb> Breadcrumb
        {
            get { return (Master as NewsMaster).BreadcrumbsControl; }
        }

        public long FeedId
        {
            get { return ViewState["FeedID"] != null ? Convert.ToInt32(ViewState["FeedID"], CultureInfo.CurrentCulture) : 0; }
            set { ViewState["FeedID"] = value; }
        }

        private void BindNewsTypes()
        {
            feedType.DataSource = new[]
                                      {
                                          FeedTypeInfo.FromFeedType(FeedType.News),
                                          FeedTypeInfo.FromFeedType(FeedType.Order),
                                          FeedTypeInfo.FromFeedType(FeedType.Advert)
                                      };
            feedType.DataBind();

            if (!string.IsNullOrEmpty(Request["type"]))
            {
                var requestFeedType = (FeedType) Enum.Parse(typeof (FeedType), Request["type"], true);
                var feedTypeInfo = FeedTypeInfo.FromFeedType(requestFeedType);

                var item = feedType.Items.FindByText(feedTypeInfo.TypeName);

                feedType.SelectedValue = item.Value;
            }
            else
            {
                feedType.SelectedIndex = 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SetupInfo.WorkMode == WorkMode.Promo)
                Response.Redirect(FeedUrls.MainPageUrl, true);

            Utility.RegisterTypeForAjax(this.GetType());

            if (!CommunitySecurity.CheckPermissions(NewsConst.Action_Add))
                Response.Redirect(FeedUrls.MainPageUrl, true);

            _mobileVer = Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            Breadcrumb.Add(new BreadCrumb {NavigationUrl = FeedUrls.MainPageUrl, Caption = NewsResource.NewsBreadCrumbs});
            if (Info.HasUser) Breadcrumb.Add(new BreadCrumb {Caption = Info.User.DisplayUserName(false), NavigationUrl = FeedUrls.GetFeedListUrl(Info.UserId)});

            var storage = FeedStorageFactory.Create();
            Feed feed = null;
            if (!string.IsNullOrEmpty(Request["docID"]))
            {
                long docID;
                if (long.TryParse(Request["docID"], out docID))
                {
                    feed = storage.GetFeed(docID);
                    Breadcrumb.Add(new BreadCrumb {Caption = feed.Caption ?? string.Empty, NavigationUrl = FeedUrls.GetFeedUrl(docID, Info.UserId)});
                    Breadcrumb.Add(new BreadCrumb {Caption = NewsResource.NewsEditBreadCrumbsNews, NavigationUrl = FeedUrls.EditNewsUrl + "?docID=" + docID + Info.UserIdAttribute});
                    _text = (feed != null ? feed.Text : "").HtmlEncode();
                }
            }
            else
            {
                Breadcrumb.Add(new BreadCrumb {Caption = NewsResource.NewsAddBreadCrumbsNews, NavigationUrl = FeedUrls.EditNewsUrl + (string.IsNullOrEmpty(Info.UserIdAttribute) ? string.Empty : "?" + Info.UserIdAttribute.Substring(1))});

            }

            if (_mobileVer && IsPostBack)
                _text = Request["mobiletext"] ?? "";

            if (!IsPostBack)
            {
                //feedNameRequiredFieldValidator.ErrorMessage = NewsResource.RequaredFieldValidatorCaption;
                HTML_FCKEditor.BasePath = VirtualPathUtility.ToAbsolute(CommonControlsConfigurer.FCKEditorBasePath);
                HTML_FCKEditor.ToolbarSet = "NewsToolbar";
                HTML_FCKEditor.EditorAreaCSS = WebSkin.GetUserSkin().BaseCSSFileAbsoluteWebPath;
                HTML_FCKEditor.Visible = !_mobileVer;
                BindNewsTypes();

                if (feed != null)
                {
                    if (!CommunitySecurity.CheckPermissions(feed, NewsConst.Action_Edit))
                    {
                        Response.Redirect(FeedUrls.MainPageName, true);
                    }
                    feedName.Text = feed.Caption;
                    HTML_FCKEditor.Value = feed.Text;
                    FeedId = feed.Id;
                    feedType.SelectedIndex = (int) Math.Log((int) feed.FeedType, 2);
                }
            }
            else
            {
                var control = FindControl(Request.Params["__EVENTTARGET"]);
                if (lbCancel.Equals(control))
                {
                    CancelFeed(sender, e);
                }
                else
                {
                    SaveFeed();
                }
            }

            this.Title = HeaderStringHelper.GetPageTitle(NewsResource.AddonName, Breadcrumb);

            lbCancel.Attributes["name"] = HTML_FCKEditor.ClientID;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public FeedAjaxInfo FeedPreview(string captionFeed, string bodyFeed)
        {
            var feed = new FeedAjaxInfo
                           {
                               FeedCaption = captionFeed,
                               FeedText = HtmlUtility.GetFull(bodyFeed, Product.CommunityProduct.ID),
                               Date = TenantUtil.DateTimeNow().Ago(),
                               UserName = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).RenderProfileLink(Product.CommunityProduct.ID)
                           };
            return feed;
        }

        protected void CancelFeed(object sender, EventArgs e)
        {
            if ((FeedId != 0))
                CommonControlsConfigurer.FCKEditingCancel("news", FeedId.ToString());
            else
                CommonControlsConfigurer.FCKEditingCancel("news");

            Response.Redirect(FeedUrls.MainPageUrl, true);
        }

        protected void SaveFeed()
        {

            if (string.IsNullOrEmpty(feedName.Text))
            {
                ((NewsMaster) this.Master).SetInfoMessage(NewsResource.RequaredFieldValidatorCaption, InfoType.Alert);
                //pnlError.Visible = true;
                return;
            }

            var storage = FeedStorageFactory.Create();
            var isEdit = (FeedId != 0);
            var feed = isEdit ? storage.GetFeed(FeedId) : new FeedNews();
            feed.Caption = feedName.Text;
            feed.Text = _mobileVer ? (Request["mobiletext"] ?? "") : HTML_FCKEditor.Value;
            feed.FeedType = (FeedType) int.Parse(feedType.SelectedValue, CultureInfo.CurrentCulture);
            storage.SaveFeed(feed, isEdit, FeedType.News);

            CommonControlsConfigurer.FCKEditingComplete("news", feed.Id.ToString(), feed.Text, isEdit);

            Response.Redirect(FeedUrls.GetFeedUrl(feed.Id, Info.UserId));
        }

        protected string RedirectUpload()
        {
            return string.Format("{0}://{1}:{2}{3}", Request.GetUrlRewriter().Scheme, Request.GetUrlRewriter().Host, Request.GetUrlRewriter().Port,
                                 VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx?esid=news" + (FeedId != 0 ? "&iid=" + FeedId.ToString() : ""));
        }
    }

    public class FeedAjaxInfo
    {
        public string FeedCaption { get; set; }

        public string FeedText { get; set; }

        public string Date { get; set; }

        public string UserName { get; set; }
    }
}