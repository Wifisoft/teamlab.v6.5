using System;
using ASC.Web.Community.Wiki.Common;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Data;
using System.Linq;
using System.Text;
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.Community.Wiki
{
    public partial class Diff : WikiBasePage
    {


        protected int OldVer
        {
            get
            {
                int result;
                if (Request["ov"] == null || !int.TryParse(Request["ov"], out result))
                    return 0;

                return result;
            }
        }

        protected int NewVer
        {
            get
            {
                int result;
                if (Request["nv"] == null || !int.TryParse(Request["nv"], out result))
                    return 0;

                return result;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            (Master as WikiMaster).GetNavigateActionsVisible += new WikiMaster.GetNavigateActionsVisibleHandle(Diff_GetNavigateActionsVisible);

            UpdateBreadCrumb();

            if (!IsPostBack)
            {
                FindDiff();
            }
        }

        WikiNavigationActionVisible Diff_GetNavigateActionsVisible()
        {
            return WikiNavigationActionVisible.AddNewPage;
        }

        private void FindDiff()
        {
            string pageName = PageNameUtil.Decode(WikiPage);

            //Page oldPage = PagesProvider.PagesHistGetByNameVersion(pageName, OldVer, TenantId);
            //Page newPage = PagesProvider.PagesHistGetByNameVersion(pageName, NewVer, TenantId);
            var oldPage = Wiki.GetPage(pageName, OldVer);
            var newPage = Wiki.GetPage(pageName, NewVer);
            
            string oldVersion, newVersion;

            if (oldPage == null)
            {
                oldVersion = string.Empty;
            }
            else
            {
                oldVersion = oldPage.Body;
            }

            if (newPage == null)
            {
                newVersion = string.Empty;
            }
            else
            {
                newVersion = newPage.Body;
            }


            DiffHelper.Item[] f = DiffHelper.DiffText(oldVersion, newVersion, true, true, false);
            string[] aLines = oldVersion.Split('\n');
            string[] bLines = newVersion.Split('\n');

            int n = 0;
            StringBuilder sb = new StringBuilder();
            foreach (DiffHelper.Item aItem in f)
            {

                // write unchanged lines
                while ((n < aItem.StartB) && (n < bLines.Length))
                {
                    WriteLine(n, null, bLines[n], sb);
                    n++;
                } // while

                // write deleted lines
                for (int m = 0; m < aItem.deletedA; m++)
                {
                    WriteLine(-1, "d", aLines[aItem.StartA + m], sb);
                } // for

                // write inserted lines
                while (n < aItem.StartB + aItem.insertedB)
                {
                    WriteLine(n, "i", bLines[n], sb);
                    n++;
                } // while
            } // while

            if (f.Length > 0 || (from bline in bLines where !bline.Trim().Equals(string.Empty) select bline).Count() > 0)
            {
                // write rest of unchanged lines
                while (n < bLines.Length)
                {
                    WriteLine(n, null, bLines[n], sb);
                    n++;
                } // while
            }
            litDiff.Text = sb.ToString();
        }

        

        private void UpdateBreadCrumb()
        {
            if (OldVer == 0 || NewVer == 0 || OldVer == NewVer)
            {
                Response.RedirectLC(ActionHelper.GetViewPagePath(this.ResolveUrlLC("PageHistoryList.aspx"), PageNameUtil.Decode(WikiPage)), this);
            }

            string pageName = PageNameUtil.Decode(WikiPage);
            if (string.IsNullOrEmpty(pageName))
            {
                pageName = WikiResource.MainWikiCaption;
            }

            BreadCrumb.Clear();
            BreadCrumb.Add(new ASC.Web.Controls.BreadCrumb() { Caption = pageName, NavigationUrl = ActionHelper.GetViewPagePath(this.ResolveUrlLC("Default.aspx"), PageNameUtil.Decode(WikiPage)) });
            BreadCrumb.Add(new ASC.Web.Controls.BreadCrumb() { Caption = WikiResource.wikiHistoryCaption, NavigationUrl = ActionHelper.GetViewPagePath(this.ResolveUrlLC("PageHistoryList.aspx"), PageNameUtil.Decode(WikiPage)) });
            BreadCrumb.Add(new ASC.Web.Controls.BreadCrumb() { Caption = string.Format(WikiResource.wikiDiffDescriptionFormat, OldVer, NewVer) });
        }

        private void WriteLine(int nr, string typ, string aText, StringBuilder sb)
        {
            if (nr >= 0)
            {
                sb.Append("<li>");
            }
            else
            {
                sb.Append("<br/>");
            }

            sb.Append("<span style='width:100%'");
            if (typ != null)
            {
                sb.Append(" class=\"" + typ + "\"");
            }
            sb.AppendFormat(@">{0}</span>", Server.HtmlEncode(aText).Replace("\r", "").Replace(" ", "&nbsp;"));
        }
    }
}
