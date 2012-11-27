#region Import

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Classes;
using ASC.Projects.Core.Domain;
using System.Collections.Generic;
using System.Text;
using ASC.Core.Users;
using AjaxPro;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;
using ASC.Projects.Engine;
using System.Text.RegularExpressions;
using System.Linq;
using ASC.Web.Controls;
#endregion

namespace ASC.Web.Projects.Controls.Common
{
    public partial class SearchView : BaseUserControl
    {

        #region Property

        public SearchGroup SearchGroup { get; set; }
        public bool IsListView { get; set; }
        public String SearchText { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            InitPage();

            List<SearchItem> items = SearchGroup.Items;

            if (IsListView)
                if (items.Count > 5)
                    items = items.GetRange(0, 5);

            SearchResultRepeater.DataSource = items;
            SearchResultRepeater.DataBind();
        }

        #endregion

        #region Methods

        public void InitPage()
        {
        }

        public string GetItemPath(SearchItem item)
        {
            switch (item.EntityType)
            {
                case EntityType.Message:
                    return string.Format("messages.aspx?prjID={0}&ID={1}", SearchGroup.ProjectID, item.ID);
                case EntityType.Milestone:
                    return string.Format("milestones.aspx?prjID={0}&ID={1}", SearchGroup.ProjectID, item.ID);
                case EntityType.Project:
                    return string.Format("projects.aspx?prjID={0}", SearchGroup.ProjectID);
                case EntityType.Task:
                    return string.Format("tasks.aspx?prjID={0}&ID={1}", SearchGroup.ProjectID, item.ID);
                case EntityType.Team:
                    return string.Format("projectteam.aspx?prjID={0}", SearchGroup.ProjectID);
                case EntityType.File:
                    return item.ID;
                default:
                    return string.Empty;
            }
        }

        #endregion
    }
}