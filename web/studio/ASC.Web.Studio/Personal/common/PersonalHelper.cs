using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.Personal
{
    public enum PersonalPart
    { 
        Default,
        WebItem,
        Profile,
        Settings,
        Backup
    }

    public class PersonalHelper
    {
        private static List<Guid> _personalItems = new List<Guid>(){
            new Guid("{32D24CB5-7ECE-4606-9C94-19216BA42086}"),//calendar
            new Guid("{E67BE73D-F9AE-4ce1-8FEC-1880CB518CB4}")//docs
            
        };

       
        public static void TransferRequest(MainPage page)
        {
            if (!SetupInfo.IsPersonal)
                return;

            if (page.TemplateSourceDirectory.IndexOf("/personal", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return;

            //undo redirect for personal items
            var curItem = CommonLinkUtility.GetWebItemByUrl(HttpContext.Current.Request.GetUrlRewriter().ToString());
            if (curItem!=null && _personalItems.Contains(curItem.ID))
                return;

            string url = String.Empty;
            if (page is Management)
            {
                var _managementType = ManagementType.General;
                if (!String.IsNullOrEmpty(HttpContext.Current.Request["type"]))
                {
                    try
                    {
                        _managementType = (ManagementType)Convert.ToInt32(HttpContext.Current.Request["type"]);
                    }
                    catch
                    {
                        _managementType = ManagementType.General;
                    }
                }

                if(_managementType ==  ManagementType.Account)
                    url = "~/personal/backup.aspx";
                else
                    url = "~/personal/settings.aspx";
            }

            else if (page is Auth)
                url = "~/personal/auth.aspx";

            else if (page is MyStaff || page is Employee || page is UserProfile)
                url = "~/personal/profile.aspx";

            if (url.Equals(String.Empty) && _personalItems.Count>0)            
                url = WebItemManager.Instance[_personalItems[0]].StartURL;
            
            
            HttpContext.Current.Server.TransferRequest(url);
        }

        
        public static string GetPartUrl(PersonalPart part)
        {
            switch (part)
            { 
                case PersonalPart.Settings:
                    return CommonLinkUtility.GetAdministration(ManagementType.General);

                case PersonalPart.Backup:
                    return CommonLinkUtility.GetAdministration(ManagementType.Account);

                case PersonalPart.Profile:
                    return CommonLinkUtility.GetMyStaff(MyStaffType.General);

            }

            return CommonLinkUtility.GetDefault();
        }

        public static void AdjustTopNavigator(TopNavigationPanel topNavPanel, PersonalPart part)
        {
            AdjustTopNavigator(topNavPanel, part, Guid.Empty);
        }
        public static void AdjustTopNavigator(TopNavigationPanel topNavPanel, PersonalPart part, Guid currentItem)
        {
            topNavPanel.DisableProductNavigation = true;
            topNavPanel.DisableSearch = true;
            topNavPanel.DisableUserInfo = true;

            topNavPanel.DisableSearch = true;
            topNavPanel.CustomTitle = Resources.Resource.Administration;
            topNavPanel.CustomTitleURL = GetPartUrl(PersonalPart.Default);
            topNavPanel.CustomTitleIconURL = WebImageSupplier.GetAbsoluteWebPath("settings.png");

            int i=0;
            //items
            foreach(var itemId in _personalItems)
            {
                var webItem = WebItemManager.Instance[itemId];                 
                topNavPanel.NavigationItems.Add(new NavigationItem()
                {
                    Name = webItem.Name,
                    URL = VirtualPathUtility.ToAbsolute(webItem.StartURL),
                    Selected = currentItem.Equals(webItem.ID) || (part == PersonalPart.Default && i==0)
                });

                i++;
            }            

            //backup
            topNavPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.Backup,
                URL = GetPartUrl(PersonalPart.Backup),
                Selected = (part == PersonalPart.Backup),
                RightAlign = true
            });

            //settings
            topNavPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.Administration,
                URL = GetPartUrl(PersonalPart.Settings),
                Selected = (part == PersonalPart.Settings),
                RightAlign = true
            });

            //profile
            topNavPanel.NavigationItems.Add(new NavigationItem()
            {
                Name = Resources.Resource.Profile,
                URL = GetPartUrl(PersonalPart.Profile),
                Selected = (part == PersonalPart.Profile),
                RightAlign = true
            });
          
        }
    }
}
