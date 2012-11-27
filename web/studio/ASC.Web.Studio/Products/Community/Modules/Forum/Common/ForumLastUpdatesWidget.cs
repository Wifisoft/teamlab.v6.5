using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.Forum;
using ASC.Web.Community.Product;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Controls.Dashboard;
using ASC.Web.Studio.Controls.Dashboard.Settings;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{
    public class LastUpdateSettingsProvider : IWidgetSettingsProvider
    {
        #region IWidgetSettingsProvider Members

        public bool Check(List<WidgetSettings> settings, Guid widgetID, Guid userID, out string errorMessage)
        {
            errorMessage = "";

            if (settings == null && settings.Count != 2)
                return false;

            foreach (var set in settings)
            {
                if (set.ID.Equals(new Guid("{5F3D3A47-CB41-4c9b-A081-82ADE80BC9B2}")))
                {
                    var data = set.ConvertToNumber();
                    if (data.Value > 0 && data.Value <= 30)
                        return true;

                    errorMessage = Resources.ForumResource.ErrorNotCorrectMaxTopicCountSettings;
                    return false;

                }
                else if (set.ID.Equals(new Guid("{5A698AF1-A4A0-4de8-86BC-03D20E6C85A6}")))
                {

                }
            }

            return false;
        }

        public List<WidgetSettings> Load(Guid widgetID, Guid userID)
        {
            List<WidgetSettings> settings = new List<WidgetSettings>();
            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<ForumLastUpdatesWidgetSettings>(userID);
            settings.Add(new NumberWidgetSettings()
            {
                ID = new Guid("{5F3D3A47-CB41-4c9b-A081-82ADE80BC9B2}"),
                Title = Resources.ForumResource.MaxLastTopicCountSettingsTitle,
                Value = widgetSettings.MaxTopicCount,
                Description = ""
            });

            settings.Add(new BoolWidgetSettings()
            {
                ID = new Guid("{5A698AF1-A4A0-4de8-86BC-03D20E6C85A6}"),
                Title = Resources.ForumResource.CutLongMassageSettingsTitle,
                Value = widgetSettings.IsCutLongMessage,
                Description = ""
            });

            return settings;
        }

        public void Save(List<WidgetSettings> settings, Guid widgetID, Guid userID)
        {
            if (settings == null && settings.Count != 2)
                return;

            var widgetSettings = SettingsManager.Instance.LoadSettingsFor<ForumLastUpdatesWidgetSettings>(userID);
            foreach (var set in settings)
            {
                if (set.ID.Equals(new Guid("{5F3D3A47-CB41-4c9b-A081-82ADE80BC9B2}")))
                {
                    var data = set.ConvertToNumber();
                    widgetSettings.MaxTopicCount = data.Value;

                }
                else if (set.ID.Equals(new Guid("{5A698AF1-A4A0-4de8-86BC-03D20E6C85A6}")))
                {
                    var data = set.ConvertToBool();
                    widgetSettings.IsCutLongMessage = data.Value;
                }
            }

            SettingsManager.Instance.SaveSettingsFor<ForumLastUpdatesWidgetSettings>(widgetSettings, userID);
        }

        #endregion
    }


    [Serializable]
    [DataContract]
    public class ForumLastUpdatesWidgetSettings : ISettings
    {
        [DataMember(Name = "IsCutLongMessage")]
        public bool IsCutLongMessage { get; set; }

        [DataMember(Name = "MaxTopicCount")]
        public int MaxTopicCount { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{096A4E5A-E648-42f0-BFC5-4D58AD88C8CF}"); }
        }

        public ISettings GetDefault()
        {
            return new ForumLastUpdatesWidgetSettings() { IsCutLongMessage = true, MaxTopicCount = 5 };
        }

        #endregion
    }

    [WidgetPosition(1, 1)]
    public class ForumLastUpdatesWidget : WebControl, ICheckEmptyContent
    {

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            ForumLastUpdatesWidgetSettings widgetSettings = SettingsManager.Instance.LoadSettingsFor<ForumLastUpdatesWidgetSettings>(SecurityContext.CurrentAccount.ID);

            var topicList = ForumDataProvider.GetLastUpdateTopics(TenantProvider.CurrentTenantID, widgetSettings.MaxTopicCount);
            StringBuilder sb = new StringBuilder();

            foreach (Topic topic in topicList)
            {
                if (topic.RecentPostID != 0)
                {
                    sb.Append("<div class='clearFix' style='margin-bottom:20px;'>");
                    sb.Append("<table cellspacing='0' cellpadding='0' border='0'><tr valign='top'>");

                    //date
                    sb.Append("<td style='width:30px; text-align:left;'>");
                    sb.Append(DateTimeService.DateTime2StringWidgetStyle(topic.RecentPostCreateDate));
                    sb.Append("</td>");

                    //message
                    sb.Append("<td style='padding-left:10px;'>");
                    string recentPostURL = VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/posts.aspx") + "?t=" + topic.ID;
                    int pageNum = Convert.ToInt32(Math.Ceiling(topic.PostCount / (ForumManager.Settings.PostCountOnPage * 1.0)));
                    if (pageNum <= 0)
                        pageNum = 1;

                    if (topic.RecentPostID != 0)
                        recentPostURL = VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/posts.aspx") + "?t=" + topic.ID.ToString() + "&p=" + pageNum.ToString() + "#" + topic.RecentPostID.ToString();

                    string message = topic.RecentPostText;
                    if (topic.RecentPostFormatter == PostTextFormatter.BBCode)
                        message = new ASC.Web.Controls.BBCodeParser.Parser(CommonControlsConfigurer.TextConfig).Parse(topic.RecentPostText);

                    if (widgetSettings.IsCutLongMessage)
                        message = HtmlUtility.GetText(message, 120, true);
                    else
                        message = HtmlUtility.GetText(message, true);

                    //topic
                    sb.Append("<div>");
                    sb.Append("<a href='" + recentPostURL + "'>" + HttpUtility.HtmlEncode(topic.Title) + "</a>");
                    if (topic.IsNew())
                        sb.Append("<span style='margin-left:7px;' class='errorText'>(" + topic.PostCount + ")</span>");
                    else
                        sb.Append("<span style='margin-left:7px;' class='describeText'>(" + topic.PostCount + ")</span>");
                    sb.Append("</div>");

                    //thread

                    //post
                    sb.Append("<div style='margin-top:5px;'>");
                    sb.Append(message);
                    sb.Append("</div>");

                    //who
                    sb.Append("<div style='margin-top:5px;'>");
                    sb.Append("<span class='textBigDescribe' style='margin-right:5px;'>" + Resources.ForumResource.PostedBy + ":</span>");
                    sb.Append(topic.RecentPostAuthor.RenderProfileLink(CommunityProduct.ID));

                    sb.Append("</div>");

                    sb.Append("</td>");
                    sb.Append("</tr></table>");
                    sb.Append("</div>");
                }

            }


            if (topicList.Count > 0)
            {
                sb.Append("<div style='margin-top:10px;'>");
                sb.Append("<a href=\"" + VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/default.aspx") + "\">" + Resources.ForumResource.ForumsBreadCrumbs + "</a>");
                sb.Append("</div>");
            }
            else
            {

                sb.Append("<div class=\"empty-widget\" style='text-align:center; padding:40px 0px;'>");

                if (ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessForumEditor, null))
                    sb.AppendFormat(
                         Resources.ForumResource.ForumNotCreatedWidgetMessageForAdmin,
                          string.Format("<div style=\"padding-top:3px;\"><a href=\"{0}\">", VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/managementcenter.aspx")),
                          "</a></div>");
                else

                    sb.Append(Resources.ForumResource.ForumNotCreatedWidgetMessage);
                sb.Append("</div>");
            }

            writer.Write(sb.ToString());
        }


        #region ICheckEmptyContent Members

        public CheckEmptyContentResult IsEmpty()
        {
            ForumLastUpdatesWidgetSettings widgetSettings = SettingsManager.Instance.LoadSettingsFor<ForumLastUpdatesWidgetSettings>(SecurityContext.CurrentAccount.ID);
            var topicList = ForumDataProvider.GetLastUpdateTopics(TenantProvider.CurrentTenantID, widgetSettings.MaxTopicCount);
            if (topicList.Count > 0)
                return CheckEmptyContentResult.NotEmpty;

            return CheckEmptyContentResult.Empty;
        }

        #endregion
    }
}
