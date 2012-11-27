using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Web.Controls;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class ServerError : MainPage
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
                ((StudioTemplate)this.Master).TopNavigationPanel.DisableProductNavigation = true;
                ((StudioTemplate)this.Master).TopNavigationPanel.DisableSearch = true;
            }

            var errorCaption = Resources.Resource.ServerErrorTitle;
            var errorText = Resources.Resource.ServerErrorText;

            var errorCode = Request["code"];
            if (!string.IsNullOrEmpty(errorCode))
            {

                switch(Convert.ToInt32(errorCode ))
                {
                    case 403:
                        errorCaption = Resources.Resource.Error403Title;
                        errorText = Resources.Resource.Error403Text;
                        break;
                    case 404:
                        Response.Redirect(VirtualPathUtility.ToAbsolute("~/error404.aspx").ToLower());
                        return;
                }
            }

            var container = new Container {Body = new PlaceHolder(), Header = new PlaceHolder()};
            master.ContentHolder.Controls.Add(container);
            container.BreadCrumbs.Add(new BreadCrumb {Caption = errorCaption});

            master.DisabledSidePanel = true;

            var sb = new StringBuilder();
            sb.Append(errorText);
            sb.Append("<div style=\"margin-top:20px;\"><a href=\"" + VirtualPathUtility.ToAbsolute("~/") + "\">" + Resources.Resource.BackToHomeLink + "</a></div>");

            container.Body.Controls.Add(new Literal {Text = sb.ToString()});

            Title = HeaderStringHelper.GetPageTitle(errorCaption, container.BreadCrumbs);
        }
    }
}