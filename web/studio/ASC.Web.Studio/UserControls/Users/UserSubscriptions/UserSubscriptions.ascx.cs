﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.Web.Core;
using ASC.Web.Core.Subscriptions;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Users
{

    [AjaxNamespace("SubscriptionManager")]
    public partial class UserSubscriptions : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/UserSubscriptions/UserSubscriptions.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
        }

        protected bool IsAdmin()
        {
            return CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID);
        }

        #region Init Notify by comboboxes

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            try
            {
                Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "subscription_manager", ASC.Data.Storage.WebPath.GetPath("usercontrols/users/usersubscriptions/js/subscription_manager.js"));

                const string notifyByComboboxesScriptKey = "QuickLinksOnLoadJavaScriptBlock";
                if (!Page.ClientScript.IsClientScriptBlockRegistered(notifyByComboboxesScriptKey))
                {
                    var notifyByComboboxesScript = "jq(function() { CommonSubscriptionManager.InitNotifyByComboboxes(); } ); ";
                    notifyByComboboxesScript += " CommonSubscriptionManager.ConfirmMessage=\"" + Resources.Resource.ConfirmMessage + "\"; ";
                    Page.ClientScript.RegisterClientScriptBlock(typeof (string), notifyByComboboxesScriptKey, notifyByComboboxesScript, true);
                }

                //styles
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "studio_usermaker_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebSkin.GetUserSkin().GetAbsoluteWebPath("usercontrols/users/usermaker/css/<theme_folder>/usermaker.css") + "\">", false);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "studio_usermaker_textoverflow", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + VirtualPathUtility.ToAbsolute("~/usercontrols/users/usermaker/css/" + WebSkin.DefaultSkin.FolderName + "/usermaker.text-overflow.css") + "\" />", false);
            }
            catch
            {
            }
        }

        #endregion

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object GetAllSubscriptions()
        {
            var result = new {Items = new List<object>()};
            var isFirst = true;
            foreach (var item in WebItemManager.Instance.GetItems(ASC.Web.Core.WebZones.WebZoneType.All)
                .FindAll(i => i.Context != null && i.Context.SubscriptionManager is IProductSubscriptionManager))
            {
                try
                {
                    result.Items.Add(GetItemSubscriptions(item, isFirst));
                    isFirst = false;
                }
                catch
                {
                }
            }
            return result;
        }

        private object GetItemSubscriptions(IWebItem webItem, bool isOpen)
        {
            var isEmpty = true;
            var canUnsubscribe = false;

            var groups = new List<object>();
            var types = new List<object>();
            var itemType = 1;

            var productSubscriptionManager = webItem.Context.SubscriptionManager as IProductSubscriptionManager;
            if (productSubscriptionManager.GroupByType == GroupByType.Modules)
            {
                foreach (var subItem in WebItemManager.Instance.GetSubItems(webItem.ID))
                {
                    if (subItem.Context == null || subItem.Context.SubscriptionManager == null)
                        continue;

                    var subscriptionTypes = subItem.Context.SubscriptionManager.GetSubscriptionTypes();
                    if (subscriptionTypes == null || subscriptionTypes.Count == 0)
                        continue;
                    else
                        subscriptionTypes = subscriptionTypes.FindAll(type => (type.IsEmptySubscriptionType != null && !type.IsEmptySubscriptionType(webItem.ID, subItem.ID, type.ID)));

                    if (subscriptionTypes == null || subscriptionTypes.Count == 0)
                        continue;

                    var group = new
                                    {
                                        Id = subItem.ID,
                                        ImageUrl = subItem.GetIconAbsoluteURL(),
                                        Name = subItem.Name.HtmlEncode(),
                                        Types = new List<object>()
                                    };

                    foreach (var type in subscriptionTypes)
                    {
                        var t = new
                                    {
                                        Id = type.ID,
                                        Name = type.Name.HtmlEncode(),
                                        Single = type.Single,
                                        IsSubscribed = type.CanSubscribe ? subItem.Context.SubscriptionManager.SubscriptionProvider.IsSubscribed(type.NotifyAction, GetCurrentRecipient(), null) : true
                                    };
                        if (t.IsSubscribed)
                            canUnsubscribe = true;

                        group.Types.Add(t);
                    }

                    groups.Add(group);
                    isEmpty = false;
                }

            }
            else if (productSubscriptionManager.GroupByType == GroupByType.Groups)
            {
                var subscriptionTypes = productSubscriptionManager.GetSubscriptionTypes();
                var subscriptionGroups = productSubscriptionManager.GetSubscriptionGroups();
                if (subscriptionTypes != null && subscriptionGroups != null)
                {
                    foreach (var gr in subscriptionGroups)
                    {
                        var sTypes = subscriptionTypes.FindAll(type => (type.IsEmptySubscriptionType != null && !type.IsEmptySubscriptionType(webItem.ID, gr.ID, type.ID)));
                        if (sTypes == null || sTypes.Count == 0)
                            continue;

                        var group = new
                                        {
                                            Id = gr.ID,
                                            ImageUrl = "",
                                            Name = gr.Name.HtmlEncode(),
                                            Types = new List<object>()
                                        };

                        foreach (var type in sTypes)
                        {
                            var t = new
                                        {
                                            Id = type.ID,
                                            Name = type.Name.HtmlEncode(),
                                            Single = type.Single,
                                            IsSubscribed = type.CanSubscribe ? productSubscriptionManager.SubscriptionProvider.IsSubscribed(type.NotifyAction, GetCurrentRecipient(), null) : true
                                        };

                            if (t.IsSubscribed)
                                canUnsubscribe = true;

                            group.Types.Add(t);
                        }

                        groups.Add(group);
                        isEmpty = false;
                    }
                }

            }
            else if (productSubscriptionManager.GroupByType == GroupByType.Simple)
            {
                itemType = 0;
                var subscriptionTypes = productSubscriptionManager.GetSubscriptionTypes();
                if (subscriptionTypes != null)
                {
                    foreach (var type in subscriptionTypes)
                    {
                        if (type.IsEmptySubscriptionType != null && type.IsEmptySubscriptionType(webItem.ID, webItem.ID, type.ID))
                            continue;

                        var t = new
                                    {
                                        Id = type.ID,
                                        Name = type.Name.HtmlEncode(),
                                        Single = type.Single,
                                        IsSubscribed = type.CanSubscribe ? productSubscriptionManager.SubscriptionProvider.IsSubscribed(type.NotifyAction, GetCurrentRecipient(), null) : true
                                    };
                        if (t.IsSubscribed)
                            canUnsubscribe = true;

                        types.Add(t);
                        isEmpty = false;
                    }
                }
            }

            return new
                       {
                           Id = webItem.ID,
                           LogoUrl = webItem.GetIconAbsoluteURL(),
                           Name = HttpUtility.HtmlEncode(webItem.Name),
                           IsEmpty = isEmpty,
                           IsOpen = isOpen,
                           CanUnSubscribe = canUnsubscribe,
                           NotifyType = GetNotifyByMethod(webItem.ID),
                           Groups = groups,
                           Types = types,
                           Type = itemType
                       };
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object RenderGroupItemSubscriptions(Guid itemId, Guid subItemId, Guid subscriptionTypeId)
        {
            try
            {
                SubscriptionType type = null;

                var productSubscriptionManager = WebItemManager.Instance[itemId].Context.SubscriptionManager as IProductSubscriptionManager;

                ISubscriptionManager subscriptionManager = productSubscriptionManager;

                if (productSubscriptionManager.GroupByType == GroupByType.Modules)
                    subscriptionManager = WebItemManager.Instance[subItemId].Context.SubscriptionManager;

                var subscriptionTypes = subscriptionManager.GetSubscriptionTypes();
                if (subscriptionTypes != null && subscriptionTypes.Count != 0)
                {
                    type = (from s in subscriptionTypes
                            where s.ID.Equals(subscriptionTypeId)
                            select s).Single<SubscriptionType>();
                }

                var result = new {Status = 1, ItemId = itemId, SubItemId = subItemId, TypeId = subscriptionTypeId, Objects = new List<object>()};

                if (type.Single || type.CanSubscribe)
                    return result;

                if (type.IsEmptySubscriptionType != null && type.IsEmptySubscriptionType(itemId, subItemId, subscriptionTypeId))
                    return result;

                if (type.GetSubscriptionObjects == null)
                    return result;

                var typeObjects = type.GetSubscriptionObjects(itemId, subItemId, subscriptionTypeId);
                if (typeObjects == null || typeObjects.Count == 0)
                    return result;

                typeObjects.Sort((s1, s2) => String.Compare(s1.Name, s2.Name, true));

                foreach (var subscription in typeObjects)
                    result.Objects.Add(new {Id = subscription.ID, Name = HttpUtility.HtmlEncode(subscription.Name), Url = String.IsNullOrEmpty(subscription.URL) ? "" : subscription.URL});

                return result;
            }
            catch (Exception e)
            {
                return new {Status = 0, Message = e.Message.HtmlEncode()};
            }

        }

        #region what's new

        protected string RenderWhatsNewSubscriptionState()
        {
            return RenderWhatsNewSubscriptionState(StudioNotifyService.Instance.IsSubscribeToWhatsNew(SecurityContext.CurrentAccount.ID));
        }

        protected string RenderWhatsNewSubscriptionState(bool isSubscribe)
        {
            if (isSubscribe)
                return "<a class='baseLinkAction " + (SetupInfo.WorkMode == WorkMode.Promo ? "promoAction" : "") + "' href=\"javascript:CommonSubscriptionManager.SubscribeToWhatsNew();\">" + Resources.Resource.UnsubscribeButton + "</a>";
            else
                return "<a class='baseLinkAction " + (SetupInfo.WorkMode == WorkMode.Promo ? "promoAction" : "") + "' href=\"javascript:CommonSubscriptionManager.SubscribeToWhatsNew();\">" + Resources.Resource.SubscribeButton + "</a>";

        }

        protected string RenderWhatsNewNotifyByCombobox()
        {
            var subscriptionManager = StudioSubscriptionManager.Instance;

            var notifyBy = ConvertToNotifyByValue(subscriptionManager, Constants.ActionSendWhatsNew);

            return string.Format(@"
<select id='NotifyByCombobox_WhatsNew' class='comboBox notify-by-combobox' style='display: none;' onchange='CommonSubscriptionManager.SetWhatsNewNotifyByMethod(jq(this).val());'>
	<option class='optionItem' value='0'{3}>{0}</option>
	<option class='optionItem' value='1'{4}>{1}</option>
	<option class='optionItem' value='2'{5}>{2}</option>
</select>",
                                 Resources.Resource.NotifyByEmail, Resources.Resource.NotifyByTMTalk, Resources.Resource.NotifyByEmailAndTMTalk,
                                 0 == notifyBy ? " selected='selected'" : string.Empty,
                                 1 == notifyBy ? " selected='selected'" : string.Empty,
                                 2 == notifyBy ? " selected='selected'" : string.Empty);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SetWhatsNewNotifyByMethod(int notifyBy)
        {
            try
            {
                var resp = new AjaxResponse();
                var notifyByList = ConvertToNotifyByList(notifyBy);
                SetNotifyBySubsriptionTypes(notifyByList, StudioSubscriptionManager.Instance, Constants.ActionSendWhatsNew);
                return resp;
            }
            catch
            {
                return null;
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SubscribeToWhatsNew()
        {
            var resp = new AjaxResponse {rs1 = "0"};
            try
            {
                var isSubscribe = StudioNotifyService.Instance.IsSubscribeToWhatsNew(SecurityContext.CurrentAccount.ID);

                StudioNotifyService.Instance.SubscribeToWhatsNew(SecurityContext.CurrentAccount.ID, !isSubscribe);
                resp.rs2 = RenderWhatsNewSubscriptionState(!isSubscribe);

                resp.rs1 = "1";
            }
            catch (Exception e)
            {
                resp.rs2 = e.Message.HtmlEncode();
            }

            return resp;

        }

        #endregion

        #region admin notifies

        protected string RenderAdminNotifySubscriptionState()
        {
            return RenderAdminNotifySubscriptionState(StudioNotifyService.Instance.IsSubscribeToAdminNotify(SecurityContext.CurrentAccount.ID));
        }

        protected string RenderAdminNotifySubscriptionState(bool isSubscribe)
        {
            if (isSubscribe)
                return "<a class='baseLinkAction " + (SetupInfo.WorkMode == WorkMode.Promo ? "promoAction" : "") + "' href=\"javascript:CommonSubscriptionManager.SubscribeToAdminNotify();\">" + Resources.Resource.UnsubscribeButton + "</a>";
            else
                return "<a class='baseLinkAction " + (SetupInfo.WorkMode == WorkMode.Promo ? "promoAction" : "") + "' href=\"javascript:CommonSubscriptionManager.SubscribeToAdminNotify();\">" + Resources.Resource.SubscribeButton + "</a>";

        }

        protected string RenderAdminNotifyNotifyByCombobox()
        {
            var subscriptionManager = StudioSubscriptionManager.Instance;

            var notifyBy = ConvertToNotifyByValue(subscriptionManager, Constants.ActionAdminNotify);

            return string.Format(@"
<select id='NotifyByCombobox_AdminNotify' class='comboBox notify-by-combobox' style='display: none;' onchange='CommonSubscriptionManager.SetAdminNotifyNotifyByMethod(jq(this).val());'>
	<option class='optionItem' value='0'{3}>{0}</option>
	<option class='optionItem' value='1'{4}>{1}</option>
	<option class='optionItem' value='2'{5}>{2}</option>
</select>",
                                 Resources.Resource.NotifyByEmail, Resources.Resource.NotifyByTMTalk, Resources.Resource.NotifyByEmailAndTMTalk,
                                 0 == notifyBy ? " selected='selected'" : string.Empty,
                                 1 == notifyBy ? " selected='selected'" : string.Empty,
                                 2 == notifyBy ? " selected='selected'" : string.Empty);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SetAdminNotifyNotifyByMethod(int notifyBy)
        {
            try
            {
                var resp = new AjaxResponse();
                var notifyByList = ConvertToNotifyByList(notifyBy);
                SetNotifyBySubsriptionTypes(notifyByList, StudioSubscriptionManager.Instance, Constants.ActionAdminNotify);
                return resp;
            }
            catch
            {
                return null;
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SubscribeToAdminNotify()
        {
            var resp = new AjaxResponse {rs1 = "0"};
            try
            {
                var isSubscribe = StudioNotifyService.Instance.IsSubscribeToAdminNotify(SecurityContext.CurrentAccount.ID);

                StudioNotifyService.Instance.SubscribeToAdminNotify(SecurityContext.CurrentAccount.ID, !isSubscribe);
                resp.rs2 = RenderAdminNotifySubscriptionState(!isSubscribe);

                resp.rs1 = "1";
            }
            catch (Exception e)
            {
                resp.rs2 = e.Message.HtmlEncode();
            }

            return resp;

        }

        #endregion


        private IRecipient GetCurrentRecipient()
        {
            return new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), SecurityContext.CurrentAccount.Name);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse UnsubscribeObjects(Guid itemId, Guid subItemID, Guid subscriptionTypeID, List<string> objIDs)
        {
            var resp = new AjaxResponse {rs2 = itemId.ToString(), rs3 = subItemID.ToString(), rs4 = subscriptionTypeID.ToString()};

            try
            {
                ISubscriptionManager subscriptionManager = null;
                var item = WebItemManager.Instance[itemId];
                var productSubscriptionManager = item.Context.SubscriptionManager as IProductSubscriptionManager;

                if (productSubscriptionManager.GroupByType == GroupByType.Groups || productSubscriptionManager.GroupByType == GroupByType.Simple)
                    subscriptionManager = productSubscriptionManager;

                else if (productSubscriptionManager.GroupByType == GroupByType.Modules)
                    subscriptionManager = WebItemManager.Instance[subItemID].Context.SubscriptionManager;

                if (subscriptionManager != null)
                {
                    var types = subscriptionManager.GetSubscriptionTypes();
                    var type = types.Find(t => t.ID.Equals(subscriptionTypeID));

                    foreach (var objID in objIDs)
                    {
                        resp.rs5 += objID + ",";
                        subscriptionManager.SubscriptionProvider.UnSubscribe(type.NotifyAction, objID, GetCurrentRecipient());
                    }

                    resp.rs5 = resp.rs5.TrimEnd(',');
                }
                resp.rs1 = "1";
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs6 = "<div class='errorBox'>" + HttpUtility.HtmlEncode(e.Message) + "</div>";
            }
            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object UnsubscribeType(Guid itemId, Guid subItemId, Guid subscriptionTypeID)
        {
            try
            {
                var item = WebItemManager.Instance[itemId];
                var productSubscriptionManager = item.Context.SubscriptionManager as IProductSubscriptionManager;

                if (productSubscriptionManager.GroupByType == GroupByType.Groups)
                {
                    var type = productSubscriptionManager.GetSubscriptionTypes().Find(t => t.ID.Equals(subscriptionTypeID));
                    if (type != null && type.CanSubscribe)
                        productSubscriptionManager.SubscriptionProvider.UnSubscribe(type.NotifyAction, null, GetCurrentRecipient());
                    else
                    {
                        var objs = productSubscriptionManager.GetSubscriptionObjects();
                        objs.RemoveAll(o => !o.SubscriptionGroup.ID.Equals(subItemId) || !o.SubscriptionType.ID.Equals(subscriptionTypeID));

                        foreach (var o in objs)
                            productSubscriptionManager.SubscriptionProvider.UnSubscribe(o.SubscriptionType.NotifyAction, o.ID, GetCurrentRecipient());
                    }
                }

                else if (productSubscriptionManager.GroupByType == GroupByType.Modules
                         || productSubscriptionManager.GroupByType == GroupByType.Simple)
                {
                    var subItem = WebItemManager.Instance[subItemId];
                    if (subItem != null && subItem.Context != null && subItem.Context.SubscriptionManager != null)
                    {
                        var subscriptionType = subItem.Context.SubscriptionManager.GetSubscriptionTypes().Find(st => st.ID.Equals(subscriptionTypeID));
                        if (subscriptionType.CanSubscribe)
                            subItem.Context.SubscriptionManager.SubscriptionProvider.UnSubscribe(subscriptionType.NotifyAction, null, GetCurrentRecipient());
                        else
                            subItem.Context.SubscriptionManager.SubscriptionProvider.UnSubscribe(subscriptionType.NotifyAction, GetCurrentRecipient());
                    }
                }

                var data = GetItemSubscriptions(item, true);
                return new {Status = 1, Message = "", Data = data};
            }
            catch (Exception e)
            {
                return new {Status = 0, Message = HttpUtility.HtmlEncode(e.Message)};
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SubscribeType(Guid itemId, Guid subItemId, Guid subscriptionTypeID)
        {
            try
            {
                var item = WebItemManager.Instance[itemId];

                ISubscriptionManager subscriptionManager = null;
                var productSubscriptionManager = item.Context.SubscriptionManager as IProductSubscriptionManager;

                if (productSubscriptionManager.GroupByType == GroupByType.Modules
                    || productSubscriptionManager.GroupByType == GroupByType.Simple)
                {
                    var subItem = WebItemManager.Instance[subItemId];
                    subscriptionManager = subItem.Context.SubscriptionManager;
                }
                else
                    subscriptionManager = productSubscriptionManager;

                var types = subscriptionManager.GetSubscriptionTypes();
                if (types != null)
                {
                    var type = types.Find(t => t.ID.Equals(subscriptionTypeID) && t.CanSubscribe);
                    if (type != null)
                        subscriptionManager.SubscriptionProvider.Subscribe(type.NotifyAction, null, GetCurrentRecipient());
                }

                var data = GetItemSubscriptions(item, true);
                return new {Status = 1, Message = "", Data = data};
            }
            catch (Exception e)
            {
                return new {Status = 0, Message = HttpUtility.HtmlEncode(e.Message)};
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object UnsubscribeProduct(Guid itemId)
        {
            try
            {
                var item = WebItemManager.Instance[itemId];
                var productSubscriptionManager = item.Context.SubscriptionManager as IProductSubscriptionManager;
                if (productSubscriptionManager.GroupByType == GroupByType.Modules)
                {
                    foreach (var subItem in WebItemManager.Instance.GetSubItems(item.ID))
                    {
                        if (subItem.Context != null && subItem.Context.SubscriptionManager != null)
                        {
                            foreach (var subscriptionType in subItem.Context.SubscriptionManager.GetSubscriptionTypes())
                            {
                                if (subscriptionType.CanSubscribe)
                                    subItem.Context.SubscriptionManager.SubscriptionProvider.UnSubscribe(subscriptionType.NotifyAction, null, GetCurrentRecipient());
                                else
                                    subItem.Context.SubscriptionManager.SubscriptionProvider.UnSubscribe(subscriptionType.NotifyAction, GetCurrentRecipient());
                            }
                        }
                    }
                }
                else if (productSubscriptionManager.GroupByType == GroupByType.Groups
                         || productSubscriptionManager.GroupByType == GroupByType.Simple)
                {
                    foreach (var subscriptionType in productSubscriptionManager.GetSubscriptionTypes())
                    {
                        if (subscriptionType.CanSubscribe)
                            productSubscriptionManager.SubscriptionProvider.UnSubscribe(subscriptionType.NotifyAction, null, GetCurrentRecipient());
                        else
                            productSubscriptionManager.SubscriptionProvider.UnSubscribe(subscriptionType.NotifyAction, GetCurrentRecipient());
                    }
                }

                var data = GetItemSubscriptions(item, true);
                return new {Status = 1, Message = "", Data = data};
            }
            catch (Exception e)
            {
                return new {Status = 0, Message = HttpUtility.HtmlEncode(e.Message)};
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SetNotifyByMethod(Guid productID, int notifyBy)
        {
            var resp = new AjaxResponse {rs2 = productID.ToString()};
            try
            {
                var notifyByList = ConvertToNotifyByList(notifyBy);

                var productSubscriptionManager = WebItemManager.Instance[productID].Context.SubscriptionManager as IProductSubscriptionManager;
                if (productSubscriptionManager.GroupByType == GroupByType.Modules)
                {
                    foreach (var item in WebItemManager.Instance.GetSubItems(productID))
                    {
                        if (item.Context != null && item.Context.SubscriptionManager != null)
                            SetNotifyBySubsriptionTypes(notifyByList, item.Context.SubscriptionManager);
                    }
                }
                else if (productSubscriptionManager.GroupByType == GroupByType.Groups
                         || productSubscriptionManager.GroupByType == GroupByType.Simple)
                {
                    SetNotifyBySubsriptionTypes(notifyByList, productSubscriptionManager);
                }
                resp.rs1 = "1";
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs6 = "<div class='errorBox'>" + HttpUtility.HtmlEncode(e.Message) + "</div>";
            }

            return null;
        }

        private static IList<string> ConvertToNotifyByList(int notifyBy)
        {
            IList<string> notifyByList = new List<string>();
            switch (notifyBy)
            {
                case 0:
                    notifyByList.Add(ASC.Core.Configuration.Constants.NotifyEMailSenderSysName);
                    break;
                case 1:
                    notifyByList.Add(ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName);
                    break;
                case 2:
                    notifyByList.Add(ASC.Core.Configuration.Constants.NotifyEMailSenderSysName);
                    notifyByList.Add(ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName);
                    break;
            }
            return notifyByList;
        }

        private void SetNotifyBySubsriptionTypes(IList<string> notifyByList, ISubscriptionManager subscriptionManager)
        {
            var subscriptionTypes = subscriptionManager.GetSubscriptionTypes();
            if (subscriptionTypes != null)
            {
                foreach (var type in subscriptionTypes)
                    SetNotifyBySubsriptionTypes(notifyByList, subscriptionManager, type.NotifyAction);
            }
        }

        private void SetNotifyBySubsriptionTypes(IList<string> notifyByList, ISubscriptionManager subscriptionManager, INotifyAction action)
        {
            subscriptionManager
                .SubscriptionProvider
                .UpdateSubscriptionMethod(
                    action,
                    GetCurrentRecipient(),
                    notifyByList.ToArray());
        }

        private int GetNotifyByMethod(Guid itemID)
        {
            var productSubscriptionManager = WebItemManager.Instance[itemID].Context.SubscriptionManager as IProductSubscriptionManager;
            if (productSubscriptionManager == null)
                return 0;

            if (productSubscriptionManager.GroupByType == GroupByType.Modules)
            {
                foreach (var item in WebItemManager.Instance.GetSubItems(itemID))
                {
                    if (item.Context == null || item.Context.SubscriptionManager == null)
                        continue;

                    foreach (var s in item.Context.SubscriptionManager.GetSubscriptionTypes())
                        return ConvertToNotifyByValue(item.Context.SubscriptionManager, s);
                }
            }
            else if (productSubscriptionManager.GroupByType == GroupByType.Groups
                     || productSubscriptionManager.GroupByType == GroupByType.Simple)
            {
                foreach (var s in productSubscriptionManager.GetSubscriptionTypes())
                    return ConvertToNotifyByValue(productSubscriptionManager, s);
            }
            return 0;
        }

        private int ConvertToNotifyByValue(ISubscriptionManager subscriptionManager, SubscriptionType s)
        {
            return ConvertToNotifyByValue(subscriptionManager, s.NotifyAction);
        }

        private int ConvertToNotifyByValue(ISubscriptionManager subscriptionManager, INotifyAction action)
        {
            var notifyByArray = subscriptionManager.SubscriptionProvider.GetSubscriptionMethod(action, GetCurrentRecipient());
            if (notifyByArray.Length == 1)
            {
                if (notifyByArray.Contains(ASC.Core.Configuration.Constants.NotifyEMailSenderSysName))
                {
                    return 0;
                }
                if (notifyByArray.Contains(ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName))
                {
                    return 1;
                }
            }
            if (notifyByArray.Length == 2)
            {
                if (notifyByArray.Contains(ASC.Core.Configuration.Constants.NotifyEMailSenderSysName) && notifyByArray.Contains(ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName))
                {
                    return 2;
                }
            }
            return 0;
        }

    }
}