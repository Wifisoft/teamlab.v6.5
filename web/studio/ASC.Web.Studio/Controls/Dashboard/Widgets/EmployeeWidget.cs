using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Settings;


namespace ASC.Web.Studio.Controls.Dashboard.Widgets
{
    [Serializable]
    [DataContract]
    public class BirthdayReminderWidgetSettings : ISettings
    {
        [DataMember(Name = "DaysBeforeBirthday")]
        public int DaysBeforeBirthday { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{8D98DDE7-43F3-4af8-9D1A-12094D32ECC0}"); }
        }

        public ISettings GetDefault()
        {
            return new BirthdayReminderWidgetSettings() { DaysBeforeBirthday = 3 };
        }

        #endregion
    }

    [WidgetPosition(0, 1)]
    public class BirthdayReminderWidget : WebControl, ICheckEmptyContent
    {
        public static Guid WidgetID { get { return new Guid("{18598566-FF7F-417f-B9BF-887E46F7ABDF}"); } }

        public Guid ProductID { get; set; }


        private class UserInfoDateComparer : IComparer<UserInfo>
        {
            public bool SortByWorkDateOrBirthDate = false;
            public UserInfoDateComparer(bool sortByWorkDateOrBirthDate)
            {
                SortByWorkDateOrBirthDate = sortByWorkDateOrBirthDate;
            }

            #region IComparer<UserInfo> Members

            public int Compare(UserInfo x, UserInfo y)
            {
                if (SortByWorkDateOrBirthDate)
                    return DateTime.Compare(y.WorkFromDate.Value, x.WorkFromDate.Value);
                else
                {
                    var d1 = DaysToBithdate(x.BirthDate.Value);
                    var d2 = DaysToBithdate(y.BirthDate.Value);
                    return d1 - d2;
                }
            }

            #endregion
        }
        const int daysAfterBirthday = 3;

        private String RenderContent(Guid productID)
        {
            var happyUsers = new List<UserInfo>();
            var futureHappyUsers = new List<UserInfo>();
            var pastHappyUsers = new List<UserInfo>();

            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<BirthdayReminderWidgetSettings>(SecurityContext.CurrentAccount.ID);

            var users = CoreContext.UserManager.GetUsers();
            foreach (var user in users)
            {
                //search user bithday
                if (user.BirthDate.HasValue)
                {
                    int days = DaysToBithdate(user.BirthDate.Value);

                    if (days == 0)
                    {
                        //bithday today
                        happyUsers.Add(user);
                    }
                    else if (days > 0 && days <= widgetSettings.DaysBeforeBirthday)
                    {
                        //bithday will be soon
                        futureHappyUsers.Add(user);
                    }
                    else if (days < 0 && days <= daysAfterBirthday)
                    {
                        pastHappyUsers.Add(user);
                    }
                }
            }

            happyUsers.SortByUserName();
            futureHappyUsers.Sort(new UserInfoDateComparer(false));
            pastHappyUsers.Sort(new UserInfoDateComparer(false));

            StringBuilder sb = new StringBuilder();
            if (happyUsers.Count > 0)
            {
                foreach (var user in happyUsers)
                    sb.Append(RenderUserItem(user, productID));

            }
            if (futureHappyUsers.Count > 0)
            {
                foreach (var user in futureHappyUsers)
                    sb.Append(RenderUserItem(user, productID));

            }

            if (happyUsers.Count <= 0 && futureHappyUsers.Count <= 0)
                sb.Append("<div class=\"empty-widget\" style=\"padding:40px; text-align: center;\">" + Resources.Resource.NoSoonBirthday + "</div>");

            return sb.ToString();
        }

        public static int DaysToBithdate(DateTime bd)
        {
            bd = bd.Date;
            var now = TenantUtil.DateTimeNow().Date;
            var days = (CultureInfo.CurrentCulture.Calendar.AddYears(bd, now.Year - bd.Year) - now).Days;
            if (days < -1 * daysAfterBirthday)
            {
                days = (CultureInfo.CurrentCulture.Calendar.AddYears(bd, now.Year - bd.Year + 1) - now).Days;
            }
            return days;
        }

        private string RenderUserItem(UserInfo user, Guid productID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='margin-bottom:10px;'>");
            sb.Append("<table cellpadding='0' cellspacing=0><tr>");

            var imageURL = user.GetSmallPhotoURL();
            sb.Append("<td style='width:30px;'>");
            sb.Append("<a href=\"" + user.GetUserProfilePageURL(productID) + "\"><img class=\"userMiniPhoto\" alt=\"\" src='" + imageURL + "'/></a>");
            sb.Append("</td>");

            sb.Append("<td style='padding-left:10px;'>");
            sb.Append("<div style='margin-top:-3px;'><a class='linkHeaderLightSmall' href=\"" + user.GetUserProfilePageURL(productID) + "\">" + user.DisplayUserName() + "</a></div>");
            sb.Append("<div class='headerBaseSmall' style='margin-top:4px;'>" + (user.BirthDate.HasValue ? DateTimeExtension.Yet(DaysToBithdate(user.BirthDate.Value)) : "") + "</div>");
            sb.Append("</td>");

            sb.Append("</tr></table>");
            sb.Append("</div>");
            return sb.ToString();
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("<input type='hidden' id='studio_birthdayReminderProductID' value='" + this.ProductID + "'/>");
            writer.Write("<div id='studio_birthdayReminderContent'>");
            writer.Write(RenderContent(this.ProductID));
            writer.Write("</div>");
        }

        #region ICheckEmptyContent Members

        public CheckEmptyContentResult IsEmpty()
        {
            var happyUsers = new List<UserInfo>();
            var futureHappyUsers = new List<UserInfo>();
            var pastHappyUsers = new List<UserInfo>();

            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<BirthdayReminderWidgetSettings>(SecurityContext.CurrentAccount.ID);

            var users = CoreContext.UserManager.GetUsers();
            foreach (var user in users)
            {
                //search user bithday
                if (user.BirthDate.HasValue)
                {
                    int days = DaysToBithdate(user.BirthDate.Value);

                    if (days == 0)
                    {
                        //bithday today
                        happyUsers.Add(user);
                    }
                    else if (days > 0 && days <= widgetSettings.DaysBeforeBirthday)
                    {
                        //bithday will be soon
                        futureHappyUsers.Add(user);
                    }
                    else if (days < 0 && days <= daysAfterBirthday)
                    {
                        pastHappyUsers.Add(user);
                    }
                }
            }

            if (happyUsers.Count <= 0 && futureHappyUsers.Count <= 0)
                return CheckEmptyContentResult.Empty;

            return CheckEmptyContentResult.NotEmpty;
        }

        #endregion
    }
}
