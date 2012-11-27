using System;
using System.Reflection;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Users.Activity
{
    [Serializable]
	public class UserContentActivity : ASC.Web.Core.Users.Activity.UserActivity
    {
        public UserContentActivity()
        {
        }

        public UserContentActivity(ASC.Web.Core.Users.Activity.UserActivity userActivity)
        {
            ActionText = userActivity.ActionText;
            ActionType = userActivity.ActionType;
            BusinessValue = userActivity.BusinessValue;
            ContentID = userActivity.ContentID;
            Date = userActivity.Date;
            ModuleID = userActivity.ModuleID;
            ProductID = userActivity.ProductID;
            Title = userActivity.Title;
            URL = userActivity.URL;
            UserID = userActivity.UserID;
            ContainerID = userActivity.ContainerID;
            AdditionalData = userActivity.AdditionalData;
            ImageOptions = userActivity.ImageOptions;
        }
       

        public virtual string Ago
        {
            get { return Date.Ago(); }
        }

        public virtual string DateFormated
        {
            get { return Date.ToShortDayMonth()+"<br/>"+Date.ToShortTimeString(); }
        }

        public virtual string AgoSentence
        {
            get { return Date.AgoSentence(); }
        }

        public virtual string UserName
        {
            get { return DisplayUserSettings.GetFullUserName(UserID); }
        }

        public virtual string ActivityAbsoluteURL
        {
            get { return CommonLinkUtility.GetFullAbsolutePath(this.URL); }
        }


        public virtual string UserAbsoluteURL
        {
            get { return CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(UserID, CommonLinkUtility.GetProductID()));}
        }


        public virtual string ModuleName
        {
            get
            {
                IModule module = ProductManager.Instance.GetModuleByID(ModuleID);
                return module == null ? "Unknown module" : module.Name;
            }
        }

        public virtual string ModuleIconUrl
        {
            get
            {
                if(this.ImageOptions != null && !string.IsNullOrEmpty(this.ImageOptions.ImageFileName))
                {
                    return WebImageSupplier.GetAbsoluteWebPath(this.ImageOptions.ImageFileName, ModuleID);
                }
                IModule module = ProductManager.Instance.GetModuleByID(ModuleID);
                return module == null ? "Unknown module" : module.GetIconAbsoluteURL();
            }
        }
    }
}