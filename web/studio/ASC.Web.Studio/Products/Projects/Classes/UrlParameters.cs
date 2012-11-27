using System;
using System.Web;

namespace ASC.Web.Projects.Classes
{

    public static class UrlParameters
    {

        public static String ProjectsFilter
        {
            get
            {
                return HttpContext.Current.Request[UrlConstant.ProjectsFilter] ?? string.Empty;
            }
        }
        public static String ProjectsTag
        {
            get
            {
                return HttpContext.Current.Request[UrlConstant.ProjectsTag] ?? string.Empty;
            }
        }

        public static String ActionType
        {
            get
            {
                return HttpContext.Current.Request[UrlConstant.Action] ?? string.Empty;
            }
        }

        public static String Search
        {
            get
            {
                return HttpContext.Current.Request[UrlConstant.Search] ?? string.Empty;
            }
        }

        public static String EntityID
        {
            get
            {
                return HttpContext.Current.Request[UrlConstant.EntityID] ?? string.Empty;
            }
        }

        public static String ProjectID
        {
            get
            {
                return HttpContext.Current.Request[UrlConstant.ProjectID] ?? string.Empty;
            }
        }

        public static Int32 PageNumber
        {
            get
            {
                int result;
                return int.TryParse(HttpContext.Current.Request[UrlConstant.PageNumber], out result) ? result : 1;
            }
        }

        public static Guid UserID
        {
            get
            {
                var result = HttpContext.Current.Request[UrlConstant.UserID];
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        return new Guid(result);
                    }
                    catch (OverflowException) { }
                    catch (FormatException) { }
                }
                return Guid.Empty;
            }
        }

        public static String ReportType
        {
            get
            {
                return HttpContext.Current.Request[UrlConstant.ReportType] ?? string.Empty;
            }
        }
    }
}
