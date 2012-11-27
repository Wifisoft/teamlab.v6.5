using System;

namespace ASC.Web.Community.News
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			Response.Redirect(string.Format("?{0}", Request.QueryString));
        }
    }
}
