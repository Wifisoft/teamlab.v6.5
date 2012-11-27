using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Controls.Common
{
    public class BottomNavigator : Control
    {
        public List<NavigationItem> NavigationItems { get; set; }

        public BottomNavigator()
        {
            this.NavigationItems = new List<NavigationItem>();


        }

        private void AddStudioItems()
        {
            if (SecurityContext.IsAuthenticated && !(Page is Wizard) && !(Page is confirm))
            {
                //all products
                this.NavigationItems.Add(new NavigationItem()
                {
                    Name = Resources.UserControlsCommonResource.AllProductsTitle,
                    URL = CommonLinkUtility.GetDefault(),
                    //Selected = Page is ASC.Web.Studio._Default

                });

                //settings
                if (SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser,
                                                    ASC.Core.Users.Constants.Action_EditUser,
                                                    ASC.Core.Users.Constants.Action_EditGroups))
                {
                    this.NavigationItems.Add(new NavigationItem()
                    {
                        //Selected = UserOnlineManager.Instance.IsAdministrationPage(),
                        URL = CommonLinkUtility.GetAdministration(ManagementType.General),
                        Name = Resources.Resource.Administration
                    });
                }

            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            AddStudioItems();

            foreach (var item in this.NavigationItems)
            {
                if (item.Selected)                
                    output.Write(@"<span class=""footerMenu"" title=""{0}"" >{1}</span>", item.Description.HtmlEncode(), item.Name.HtmlEncode());                
                else                
                    output.Write(@"<a class=""footerMenu"" title=""{0}"" href=""{1}"">{2}</a>", item.Description.HtmlEncode(), ResolveUrl(item.URL), item.Name.HtmlEncode());                
            }

            string lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower();
            if (!String.IsNullOrEmpty(SetupInfo.MobileRedirect))
            {
                output.Write(string.Format(@"<a class=""footerMenu"" href=""{0}"">{1}</a>",
                                           VirtualPathUtility.ToAbsolute(SetupInfo.MobileRedirect), Resources.Resource.MobileVersion));
            }

            output.Write(@"<a class=""footerMenu"" href=""mailto:support@teamlab.com"">" + Resources.Resource.ContactUs + "</a>");
            output.Write(@"<a class=""footerMenu"" target=""_blank"" href=""http://www.teamlab.com/" + lang + @"/"">" + Resources.Resource.TeamLabSiteTitle+ "</a>");
            output.Write(@"<a class=""footerMenu"" target=""_blank"" href=""http://www.teamlab.com/" + lang + @"/help/helpcenter.aspx"">" + Resources.Resource.Help + "</a>");
            if (DebugInfo.ShowDebugInfo)
            {
                output.Write("<a class='footerMenu' href=\"javascript:alert('" + DebugInfo.DebugString.HtmlEncode().ReplaceSingleQuote() + "')\">Debug Info</a>");
            }

            var attr = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).FirstOrDefault();
            if (attr != null)
            {
                output.Write(@"<span class=""footerCopyright"">&copy; " + ((AssemblyCopyrightAttribute)attr).Copyright + "</span>");
            }
        }
    }
}
