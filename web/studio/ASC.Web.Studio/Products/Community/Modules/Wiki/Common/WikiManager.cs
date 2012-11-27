using System;
using ASC.Web.Community.Wiki.Resources;

namespace ASC.Web.Community.Wiki.Common
{
    public sealed class WikiManager
    {
        WikiManager()
        {

        }

        public static Guid ModuleId
        {
            get { return new Guid("742CF945-CBBC-4a57-82D6-1600A12CF8CA"); }
        }

        public static int MaxPageSearchResult = 10;
        public static int MaxPageSearchInLinkDlgResult = 3;

        public static string BaseVirtualPath
        {
            get
            {
                return "~/Products/Community/Modules/Wiki".ToLower();
            }
        }

        public static string ViewVirtualPath
        {
            get
            {
                return "~/Products/Community/Modules/Wiki/Default.aspx".ToLower();
            }
        }

        public static string SearchDefaultString
        {
            get
            {
                return WikiResource.SearchDefaultString;
            }
        }

        public static string WikiSectionConfig
        {
            get
            {
                return "/Products/Community/Modules/Wiki";
            }
        }

    }
}
