using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.Core;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.PhotoManager.Model;
using ASC.PhotoManager.Resources;
using ASC.Web.Community.PhotoManager.Common;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Core.Mobile;
using ASC.Web.Community.Product;

namespace ASC.Web.Community.PhotoManager.Controls
{
    [AjaxNamespace("ActionContainer")]
    public partial class ActionContainer : System.Web.UI.UserControl
    {
        IImageStorage _storage = null;
        ASC.PhotoManager.Helpers.RequestHelper _requestHelper = null;

        public PlaceHolder ActionsPlaceHolder { get; set; }

        public ActionContainer()
        {
            ActionsPlaceHolder = new PlaceHolder();

        }

        IDirectRecipient IAmAsRecipient
        {
            get
            {
                if (!SecurityContext.IsAuthenticated) return null;
                var service = StorageFactory.GetStorage();
                return (IDirectRecipient)service.NotifySource.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString());
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof(ActionContainer), this.Page);

            _storage = StorageFactory.GetStorage();
            _requestHelper = new RequestHelper(Request, _storage);

            var currentModule = UserOnlineManager.Instance.GetCurrentModule() as Module;
            if (currentModule == null) return;

            var actionsControl = new SideActions();
            if (CommunitySecurity.CheckPermissions(ASC.PhotoManager.PhotoConst.Action_AddPhoto) && !MobileDetector.IsRequestMatchesMobile(Context))
            {
                actionsControl.Controls.Add(new NavigationItem()
                {
                    Name = PhotoManagerResource.UploadPhotosLink,
                    Description = PhotoManagerResource.UploadPhotosLinkDescription,
                    URL = ASC.PhotoManager.PhotoConst.AddPhotoPageUrl
                        + (_requestHelper.EventId != 0 ? "?" + ASC.PhotoManager.PhotoConst.PARAM_EVENT + "=" + _requestHelper.EventId : ""),
                    IsPromo = (SetupInfo.WorkMode == WorkMode.Promo)
                });
            }

            if (IAmAsRecipient != null)
            {
                actionsControl.Controls.Add(new HtmlMenuItem(RenderSubscriptionOnUploadsLink()));
            }
            if (IAmAsRecipient != null && _requestHelper.EventId != 0)
            {
                actionsControl.Controls.Add(new HtmlMenuItem(RenderSubscriptionOnEventLink(_requestHelper.EventId)));
            }
            if (actionsControl.Controls.Count > 0)
            {
                _actionHolder.Controls.Add(actionsControl);
            }
            if (ActionsPlaceHolder.Controls.Count > 0)
            {
                actionsControl.Controls.Add(ActionsPlaceHolder);
            }
            if (currentModule.Navigations.Any())
            {
                var navigationControl = new SideNavigator();
                foreach (var shortcut in currentModule.Navigations)
                {
                    if (shortcut.ID == new Guid("4367C1B3-9F22-41a9-9CF1-DDCC612AFEE0") && !SecurityContext.IsAuthenticated)
                    {
                        // skip My Photos for guest
                        continue;
                    }
                    navigationControl.Controls.Add(new NavigationItem()
                    {
                        Name = shortcut.Name,
                        Description = shortcut.Description,
                        URL = shortcut.StartURL
                    });
                }
                _actionHolder.Controls.Add(navigationControl);
            }
        }

        string RenderSubscriptionOnUploadsLink()
        {

            StringBuilder sb = new StringBuilder();

            bool subscribed = _storage.NotifySource.GetSubscriptionProvider().IsSubscribed(
                ASC.PhotoManager.PhotoConst.NewPhotoUploaded, IAmAsRecipient, null);

            sb.Append("<div id=\"photo_notifies\">");

            sb.Append("<a id=\"notify_photo\" title='" + PhotoManagerResource.NotifyOnUploadsDescription + "' class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" href=\"javascript:void(0);\" />" + (!subscribed ? PhotoManagerResource.NotifyOnUploadsMessage : PhotoManagerResource.UnNotifyOnUploadsMessage) + "</a>");
            sb.Append("<script type=\"text/javascript\">");
            sb.AppendLine("var NotifyNewUploads = " + subscribed.ToString().ToLower() + ";");
            sb.AppendLine("document.getElementById('notify_photo').onclick = function()");
            sb.AppendLine("{AjaxPro.onLoading = function(b){if(b){jq('#photo_notifies').block();}else{jq('#photo_notifies').unblock();}};");
            sb.AppendLine("ActionContainer.SubscribeOnPhoto(NotifyNewUploads, callbackNotifyPhoto);");
            sb.AppendLine("}");
            sb.AppendLine("function callbackNotifyPhoto(result){NotifyNewUploads = result.value;");
            sb.AppendLine("if(!NotifyNewUploads){jq('#notify_photo').html('" + PhotoManagerResource.NotifyOnUploadsMessage.ReplaceSingleQuote() + "');} ");
            sb.AppendLine("else {jq('#notify_photo').html('" + PhotoManagerResource.UnNotifyOnUploadsMessage.ReplaceSingleQuote() + "');} ");
            sb.AppendLine("}");
            sb.Append("</script>");

            sb.Append("</div>");

            return sb.ToString();
        }

        string RenderSubscriptionOnEventLink(long eventID)
        {
            var sb = new StringBuilder();

            bool isSubscribed = _storage.NotifySource.GetSubscriptionProvider().IsSubscribed(
                    ASC.PhotoManager.PhotoConst.NewEventComment,
                    IAmAsRecipient,
                    eventID.ToString()
                );

            sb.AppendFormat("<div id=\"photo_comment_notifies\">");
            sb.AppendFormat("<a class=\"linkAction" + (SetupInfo.WorkMode == WorkMode.Promo ? " promoAction" : "") + "\" id=\"sub_on_event_{2}\" title=\"{1}\" href=\"javascript:;\" >{0}</a>",
                (isSubscribed ? PhotoManagerResource.UnNotifyOnNewEventCommentsMessage : PhotoManagerResource.NotifyOnNewEventCommentsMessage),
                PhotoManagerResource.NotifyOnNewEventCommentsDescription,
                eventID);
            sb.AppendFormat("<script>");
            sb.AppendFormat(@"
var subscribed_at_event_{0} = {1}
jq('#sub_on_event_{0}').click(function(){{
AjaxPro.onLoading = function(b){{if(b){{jq('#photo_comment_notifies').block();}}else{{jq('#photo_comment_notifies').unblock();}}}};
ActionContainer.SubscribeOnComments('{0}',subscribed_at_event_{0},function(result){{
        if(result.value != null){{
            subscribed_at_event_{0} = (result.value.rs1=='true')?true:false;
            jq('#sub_on_event_{0}').html(result.value.rs2);
        }}
    }});    
}})
", eventID, isSubscribed.ToString().ToLower());
            sb.AppendFormat("</script>");
            sb.AppendFormat("</div>");

            return sb.ToString();
        }

        [AjaxPro.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public bool SubscribeOnPhoto(bool isSubscribe)
        {
            IImageStorage _storage = StorageFactory.GetStorage();
            ISubscriptionProvider subscriptionProvider = _storage.NotifySource.GetSubscriptionProvider();
            if (IAmAsRecipient == null)
            {
                return false;
            }
            if (!isSubscribe)
            {
                subscriptionProvider.Subscribe(
                         ASC.PhotoManager.PhotoConst.NewPhotoUploaded,
                         null,
                         IAmAsRecipient
                    );
                return true;
            }
            else
            {
                subscriptionProvider.UnSubscribe(
                         ASC.PhotoManager.PhotoConst.NewPhotoUploaded,
                         null,
                         IAmAsRecipient
                    );
                return false;
            }

        }

        [AjaxPro.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SubscribeOnComments(string eventID, bool isSubscribe)
        {
            var service = StorageFactory.GetStorage();
            AjaxResponse responce = new AjaxResponse();
            responce.rs1 = isSubscribe.ToString().ToLower();

            ISubscriptionProvider subscriptionProvider = service.NotifySource.GetSubscriptionProvider();
            if (IAmAsRecipient != null)
            {
                if (!isSubscribe)
                {
                    subscriptionProvider.Subscribe(
                             ASC.PhotoManager.PhotoConst.NewEventComment,
                             eventID,
                             IAmAsRecipient
                        );
                    responce.rs1 = "true";
                    responce.rs2 = PhotoManagerResource.UnNotifyOnNewEventCommentsMessage;
                }
                else
                {
                    subscriptionProvider.UnSubscribe(
                             ASC.PhotoManager.PhotoConst.NewEventComment,
                             eventID,
                             IAmAsRecipient
                        );
                    responce.rs1 = "false";
                    responce.rs2 = PhotoManagerResource.NotifyOnNewEventCommentsMessage;
                }
            }

            return responce;
        }
    }
}