using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Web.Controls;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class Error404 : MainPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            base.SetProductMasterPage();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var master = this.Master as IStudioMaster;
            if (!(this.Master is IStudioMaster)) return;

            //top panel
            if (this.Master is StudioTemplate)
            {                
                ((StudioTemplate)this.Master).TopNavigationPanel.DisableSearch = true;
            }

            var container = new Container {Body = new PlaceHolder(), Header = new PlaceHolder()};
            master.ContentHolder.Controls.Add(container);
            container.BreadCrumbs.Add(new BreadCrumb {Caption = Resources.Resource.Error404Title});

            master.DisabledSidePanel = true;

            var sb = new StringBuilder();
            sb.Append(Resources.Resource.Error404Text);
            sb.Append("<div style=\"margin-top:20px;\"><a href=\"" + VirtualPathUtility.ToAbsolute("~/") + "\">" + Resources.Resource.BackToHomeLink + "</a></div>");

            container.Body.Controls.Add(new Literal {Text = sb.ToString()});

            Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Error404Title, container.BreadCrumbs);
        }
    }
}