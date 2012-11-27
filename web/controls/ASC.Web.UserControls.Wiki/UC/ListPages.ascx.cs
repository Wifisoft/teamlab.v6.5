using System;
using System.Web.UI;

namespace ASC.Web.UserControls.Wiki.UC
{
    public partial class ListPages : BaseUserControl
    {
        public string mainPath
        {
            get
            {
                if(ViewState["mainPath"] == null)
                    return string.Empty;
                else
                    return ViewState["mainPath"].ToString();
            }
            set { ViewState["mainPath"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                BindListPages();
            }           
        }
     
        protected string GetPageLink(string PageName)
        {
            return ActionHelper.GetViewPagePath(mainPath, PageName);
        }


        private void BindListPages()
        {
            rptListPages.DataSource = Wiki.GetPages();
            rptListPages.DataBind();
        }
    }
}
