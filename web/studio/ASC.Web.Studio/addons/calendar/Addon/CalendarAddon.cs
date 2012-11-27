using System;
using ASC.Web.Calendar.UserControls;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;
using ASC.Web.Calendar.Notification;
using ASC.Web.Core.Calendars;
using ASC.Api.Calendar.BusinessObjects;
using System.Text;
using System.IO;
using System.Web.UI;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using System.Web;

namespace ASC.Web.Calendar
{
    [WebZoneAttribute(WebZoneType.CustomProductList)]
    public class CalendarAddon : IAddon, IRenderCustomNavigation
    {
        public static Guid AddonID { get { return new Guid("{32D24CB5-7ECE-4606-9C94-19216BA42086}"); } }

        public static string BaseVirtualPath { get { return "~/addons/calendar/"; } }

        private AddonContext _context;
        public AddonContext Context
        {
            get { return _context; }
        }

        WebItemContext IWebItem.Context { get { return _context; } }

        public string Description
        {
            get { return Resources.CalendarAddonResource.AddonDescription; }
        }

        public Guid ID
        {
            get { return AddonID; }
        }

        public void Init(AddonContext context)
        {
            _context = context;
            _context.ImageFolder = "images";
            _context.ThemesFolderVirtualPath = "~/addons/calendar/app_themes/";
            _context.DefaultSortOrder = 80;
            _context.DisabledIconFileName = "disabledlogo.png";
            _context.IconFileName = "logo.png";            
            _context.LargeIconFileName = "biglogo.png";            
            _context.SubscriptionManager = new SubscriptionManager();
        }

        public string Name
        {
            get { return Resources.CalendarAddonResource.AddonName; }
        }

        public void Shutdown()
        {

        }

        public string StartURL
        {
            get { return "~/addons/calendar/"; }
        }

        #region IRenderCustomNavigation Members

        public Control LoadCustomNavigationControl(Page page)
        {
            return null;
        }

        public string RenderCustomNavigation(Page page)
        {
            var sb = new StringBuilder();
//            sb.AppendFormat(@"<style type=""text/css"">
//                            .studioTopNavigationPanel .systemSection .calendar a{{background:url(""{0}"") left 1px no-repeat;}}
//                            </style>", WebImageSupplier.GetAbsoluteWebPath("minilogo.png", AddonID));

//            sb.AppendFormat(@"<li class=""itemBox calendar"" style=""float: right;"">
//                    <a href=""{0}""><span>{1}</span>
//                    </a></li>", VirtualPathUtility.ToAbsolute(this.StartURL), this.Name);
            sb.AppendFormat(@"<li class=""top-item-box calendar"">
                                  <a class=""inner-text"" href=""{0}"" title=""{1}"">
                                      <span class=""inner-label""></span>
                                  </a>
                              </li>", VirtualPathUtility.ToAbsolute(StartURL), Name);
            return sb.ToString();
        }

        #endregion
    }
}
