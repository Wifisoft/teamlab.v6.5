using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.Controls.Dashboard.Widgets
{
    [Serializable]
    [DataContract]
    public class NewEmployeeWidgetSettings : ISettings
    {
        [DataMember(Name = "NewWorkerCount")]
        public int NewWorkerCount { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{51D1335C-C0B2-4d43-851A-863ED6CFAAFD}"); }
        }

        public ISettings GetDefault()
        {
            return new NewEmployeeWidgetSettings() { NewWorkerCount = 5 };
        }

        #endregion
    }

    [WidgetPosition(0, 0)]
    public class NewEmployeeWidget : WebControl, ICheckEmptyContent
    {
        public static Guid WidgetID { get { return new Guid("{117E9CAB-D31C-4495-B276-74A2D3A34433}"); } }

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
                    var now = TenantUtil.DateTimeNow();
                    DateTime xDate = new DateTime(now.Year, x.BirthDate.Value.Month, x.BirthDate.Value.Day);
                    DateTime yDate = new DateTime(now.Year, y.BirthDate.Value.Month, y.BirthDate.Value.Day);

                    TimeSpan ts1 = new TimeSpan((xDate - now).Ticks);
                    TimeSpan ts2 = new TimeSpan((yDate - now).Ticks);
                    return TimeSpan.Compare(ts1, ts2);
                }

            }

            #endregion
        }

        private String RenderContent()
        {
            var newUsers = new List<UserInfo>();
            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<NewEmployeeWidgetSettings>(SecurityContext.CurrentAccount.ID);

            var users = CoreContext.UserManager.GetUsers();
            foreach (var user in users)
            {
                //search new users
                if (user.WorkFromDate.HasValue && (newUsers.Find(u => u.ID.Equals(user.ID)) == null))
                {
                    if (newUsers.Count < widgetSettings.NewWorkerCount)
                        newUsers.Add(user);
                    else
                    {
                        newUsers.Sort(new UserInfoDateComparer(true));
                        for (int i = newUsers.Count - 1; i >= 0; i--)
                        {
                            var newUser = newUsers[i];
                            if (newUser.WorkFromDate.Value.CompareTo(user.WorkFromDate) < 0)
                            {
                                newUsers.Remove(newUser);
                                newUsers.Add(user);
                                break;
                            }
                        }
                    }
                }
            }

            newUsers.Sort(new UserInfoDateComparer(true));

            StringBuilder sb = new StringBuilder();
            if (newUsers.Count > 0)
            {
                foreach (var user in newUsers)
                {
                    sb.Append("<div style='margin-bottom:15px;'>");
                    var imageURL = user.GetSmallPhotoURL();
                    sb.Append("<table cellpadding=\"0\" cellspacing=\"0\"><tr valign='top'>");
                    sb.Append("<td style='width:30px;'><a href=\"" + CommonLinkUtility.GetUserProfile(user.ID, ProductID) + "\"><img class=\"borderBase\" border=0 alt=\"\" src='" + imageURL + "'/></a></td>");

                    sb.Append("<td style='padding-left:10px;'>");
                    sb.Append("<div style='margin-top:-3px;'>");
                    sb.Append("<a class='linkHeaderLightSmall' href=\"" + CommonLinkUtility.GetUserProfile(user.ID, ProductID) + "\">" + user.DisplayUserName() + "</a>");
                    sb.Append("</div>");

                    var dep = user.GetUserDepartment();
                    if (dep != null)
                    {
                        sb.Append("<div style='margin-top:4px;'>");
                        sb.Append("<a class='linkText' href='" + CommonLinkUtility.GetDepartment(ProductID, dep.ID) + "'>" + dep.Name.HtmlEncode() + "</a>");
                        sb.Append("</div>");

                    }

                    sb.Append("</td>");
                    sb.Append("</tr></table>");
                    sb.Append("</div>");
                }

                sb.Append("<div style='margin-top: 10px;'>");
                //all users
                sb.Append("<a href=\"" + CommonLinkUtility.GetEmployees(ProductID) + "&sort=InvAdmissionDate\">" + CustomNamingPeople.Substitute<Resources.Resource>("AllEmployees").HtmlEncode() + "</a>");
                sb.Append("</div>");
            }
            else
            {
                
                //check access rigths
                if (SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser))
                {
                    sb.Append("<div class='empty-widget' style='padding:40px; text-align: center;'>");
                    var url = string.Format("<div style=\"padding-top:3px;\"><a class=\"promoAction\" href=\"javascript:ImportUsersManager.ShowImportControl();\">");
                    sb.Append(string.Format(CustomNamingPeople.Substitute<Resources.Resource>("InviteEmployeesWidgetTitle").HtmlEncode(), url, "</a></div>"));
                    sb.Append("</div>");
                }
                else
                {
                    sb.Append("<div class=\"empty-widget\" style=\"padding:40px; text-align: center;\">" + CustomNamingPeople.Substitute<Resources.Resource>("NoNewEmployees").HtmlEncode() + "</div>");
                }
            }


            return sb.ToString();
        }


        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            writer.Write("<div>");
            writer.Write("<input type='hidden' id='studio_newemp_productID' value='" + this.ProductID + "'/>");
            writer.Write("<div id='studio_newemp_Content'>");
            writer.Write(RenderContent());
            writer.Write("</div>");
            writer.Write("</div>");
        }

        #region ICheckEmptyContent Members

        public CheckEmptyContentResult IsEmpty()
        {
            var newUsers = new List<UserInfo>();
            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<NewEmployeeWidgetSettings>(SecurityContext.CurrentAccount.ID);

            var users = CoreContext.UserManager.GetUsers();
            foreach (var user in users)
            {
                //search new users
                if (user.WorkFromDate.HasValue && (newUsers.Find(u => u.ID.Equals(user.ID)) == null))
                {
                    if (newUsers.Count < widgetSettings.NewWorkerCount)
                        newUsers.Add(user);
                    else
                    {
                        newUsers.Sort(new UserInfoDateComparer(true));
                        for (int i = newUsers.Count - 1; i >= 0; i--)
                        {
                            var newUser = newUsers[i];
                            if (newUser.WorkFromDate.Value.CompareTo(user.WorkFromDate) < 0)
                            {
                                newUsers.Remove(newUser);
                                newUsers.Add(user);
                                break;
                            }
                        }
                    }
                }
            }

            if (newUsers.Count > 0)
                return CheckEmptyContentResult.NotEmpty;            

            return CheckEmptyContentResult.Empty;
        }

        #endregion
    }
}
