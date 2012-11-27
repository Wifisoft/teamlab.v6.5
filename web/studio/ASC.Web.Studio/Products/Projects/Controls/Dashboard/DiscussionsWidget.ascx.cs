#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;

#endregion

namespace ASC.Web.Projects.Controls.Dashboard
{
    [Serializable]
    [DataContract]
    public class DiscussionsWidgetSettings : ISettings
    {
        [DataMember(Name = "DiscussionsCount")]
        public int DiscussionsCount { get; set; }

        public Guid ID
        {
            get { return new Guid("{18840320-3F2A-4560-83B6-3043164F3A7D}"); }
        }

        public ISettings GetDefault()
        {
            return new DiscussionsWidgetSettings
            {
                DiscussionsCount = 2
            };
        }
    }

    public partial class DiscussionsWidget : BaseUserControl
    {
        public List<DiscussionWrapper> Discussions { get; set; }

        public static Guid WidgetId { get { return new Guid("{55B83E89-90B2-4C9F-93D8-5653D4778EDA}"); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            DiscussionsRepeater.DataSource = Discussions;
            DiscussionsRepeater.DataBind();
        }
    }

    public class DiscussionWrapper
    {
        public DiscussionWrapper(Message discussion)
        {
            Discussion = discussion;
            CreatedBy = Global.EngineFactory.GetParticipantEngine().GetByID(discussion.CreateBy).UserInfo;
            CommentCount = discussion.CommentsCount;
        }

        public Message Discussion;

        public UserInfo CreatedBy;
        public string CreatedByAvatarUrl { get { return CreatedBy.GetSmallPhotoURL(); } }
        public string CreatedByBigAvatarUrl { get { return CreatedBy.GetBigPhotoURL(); } }
        public string CreatedByName { get { return CreatedBy.DisplayUserName(); } }
        public string CreatedByTitle { get { return CreatedBy.Title; } }
        public string CreatedByProfileLink { get { return CreatedBy.RenderProfileLink(ProductEntryPoint.ID); } }

        public string CreatedDateString { get { return Discussion.CreateOn.ToShortDayMonth(); } }
        public string CreatedTimeString { get { return Discussion.CreateOn.ToString("HH:mm"); } }
        public string CreatedDateTimeString { get { return Discussion.CreateOn.ToString(CultureInfo.CurrentCulture); } }

        public string DiscussionUrl { get { return String.Format("messages.aspx?prjID={0}&id={1}", Discussion.Project.ID, Discussion.ID); } }

        public Project Project { get { return Discussion.Project; } }
        public string ProjectUrl { get { return String.Format("projects.aspx?prjID={0}", Discussion.Project.ID); } }

        public int CommentCount;

        public string CommentsUrl { get { return String.Format("messages.aspx?prjID={0}&id={1}#comments", Discussion.Project.ID, Discussion.ID); } }
        public bool IsReaded;
    }
}
