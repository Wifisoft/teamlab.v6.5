using System;
using ASC.Core.Tenants;

namespace ASC.Web.UserControls.Forum.Common
{
    
    public class DateTimeService
    {
        public static DateTime CurrentDate()
        {
            return TenantUtil.DateTimeNow();
        }
        public static string DateTime2String(DateTime dateTime, string format)
        {
            DateTime now = CurrentDate();
            TimeSpan difference = (now - dateTime);

            if ((now.DayOfYear - dateTime.DayOfYear) < 1 &&
                now.Year == dateTime.Year &&
                now.Month == dateTime.Month
                )
            {
                return Resources.ForumUCResource.Today + ", " + dateTime.ToShortTimeString();
            }
            if ((now.DayOfYear - dateTime.DayOfYear) < 2 &&
                now.Year == dateTime.Year &&
                now.Month == dateTime.Month
                )
            {
                return Resources.ForumUCResource.Yesterday + ", " + dateTime.ToShortTimeString();
            }
            return dateTime.ToString(format);
        }
        public static string DateTime2StringTopicStyle(DateTime dateTime)
        {
            DateTime now = CurrentDate();
            TimeSpan difference = (now - dateTime);

            if ((now.DayOfYear - dateTime.DayOfYear) < 1 &&
                now.Year == dateTime.Year &&
                now.Month == dateTime.Month
                )
            {
                return "<span class='textSmallDescribe'>" + Resources.ForumUCResource.Today + " " + dateTime.ToShortTimeString() + "</span>";
            }
            if ((now.DayOfYear - dateTime.DayOfYear) < 2 &&
                now.Year == dateTime.Year &&
                now.Month == dateTime.Month
                )
            {
                return "<span class='textSmallDescribe'>" + Resources.ForumUCResource.Yesterday + " " + dateTime.ToShortTimeString() + "</span>";
            }
            return "<span class='textSmallDescribe'>" + dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString() + "</span>";
        }

        public static string DateTime2StringWidgetStyle(DateTime dateTime)
        {
            return "<span class='textMediumDescribe'>" + dateTime.ToShortDayMonth() + "<br/>" + dateTime.ToShortTimeString() + "</span>";
        }

        public static string DateTime2StringPostStyle(DateTime dateTime)
        {
            DateTime now = CurrentDate();
            TimeSpan difference = (now - dateTime);

            if ((now.DayOfYear - dateTime.DayOfYear) < 1 &&
                now.Year == dateTime.Year &&
                now.Month == dateTime.Month
                )
            {
                return "<span class='textMediumDescribe'>" + dateTime.ToShortTimeString() + "  " + Resources.ForumUCResource.Today + "</span>";
            }
            if ((now.DayOfYear - dateTime.DayOfYear) < 2 &&
                now.Year == dateTime.Year &&
                now.Month == dateTime.Month
                )
            {
                return "<span class='textMediumDescribe'>" + dateTime.ToShortTimeString() + "  " + Resources.ForumUCResource.Yesterday + "</span>";
            }
            return "<span class='textMediumDescribe'>" + dateTime.ToShortTimeString() + "  " + dateTime.ToShortDateString() + "</span>";
        }
    }    
}
