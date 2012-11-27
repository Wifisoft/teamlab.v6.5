using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Core;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Files.Configuration;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.UserControls.Users.Activity;

namespace ASC.Web.Files.Classes
{
    internal class UserActivityControlLoader : IUserActivityControlLoader
    {
        #region IUserActivityControlLoader Members

        public Control LoadControl(Guid userID)
        {
            return new UserActivityViewer(userID);
        }

        #endregion

        private class UserActivityViewer : WebControl
        {
            private readonly Guid _userID;

            public UserActivityViewer(Guid userID)
            {
                _userID = userID;
            }

            protected override void OnLoad(EventArgs e)
            {
                Controls.Add(new UserStatistics(_userID));

                Controls.Add(new Literal {Text = "<div style=height:20px;>&nbsp;</div>"});

                var recentActivity = (RecentUserActivity) Page.LoadControl(RecentUserActivity.Location);
                recentActivity.UserID = _userID;
                recentActivity.ProductID = ProductEntryPoint.ID;
                Controls.Add(recentActivity);
                base.OnLoad(e);
            }
        }

        private class UserStatistics : Control
        {
            private readonly Guid _userID;

            public UserStatistics(Guid userID)
            {
                _userID = userID;
            }

            protected override void Render(HtmlTextWriter output)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("<div class='headerBase borderBase' style='padding: 0px 0px 5px 15px; border-top:none; border-right:none; border-left:none;'>{0}</div>",
                                FilesUCResource.Statistic);
                sb.Append("<div class='clearFix' style='padding:15px 15px 0 15px;'>");
                sb.Append(RenderUserStatistic());
                sb.Append("</div>");

                output.Write(sb.ToString());
            }

            private string RenderUserStatistic()
            {
                const float width = 46;
                var fullCount = .0;
                double k = 0;
                double max = 0;

                var product = ProductManager.Instance[ProductEntryPoint.ID];
                if (product == null) return "";

                var module = WebItemManager.Instance.GetSubItems(product.ID).FirstOrDefault();
                if (module is IModule == false) return "";

                var statProvider = (module as IModule).Context.StatisticProvider;
                if (statProvider == null) return "";

                var statistic = statProvider.GetAllStatistic(_userID);
                foreach (var stat in statistic)
                {
                    fullCount += stat.Count;
                    if (max < stat.Count) max = stat.Count;
                }

                if (max > 0) k = width/max;

                var sb = new StringBuilder();
                foreach (var stat in statistic)
                {

                    sb.AppendFormat(@"<div class=""{1}"">{0}</div>", stat.Name, "VariantTextCSSClass");
                    sb.Append("<div class='clearFix' style='padding-bottom:15px;'>");
                    sb.AppendFormat(@"<div class=""{1}"" style=""width:{0}%;"">&nbsp;</div>", Math.Round(k*stat.Count), max == stat.Count ? "statisticBar liaderBar" : "statisticBar");
                    sb.AppendFormat(@"<div class=""{2}"" style=""width:20%; float:left;""><span id=""strong"">{0}</span><span> ({1}%)</span></div>", stat.Count, fullCount != 0 ? Math.Round((stat.Count*100)/fullCount) : 0, "pollSmallDescribe");
                    sb.AppendFormat(
                        "<div style='text-align:right; float:right; width:30%;'><a href=\"{0}\">{1}:&nbsp;{2}</a></div>",
                        stat.URL,
                        stat.Name,
                        stat.Count);
                    sb.Append("</div>");
                }

                return sb.ToString();
            }
        }
    }
}