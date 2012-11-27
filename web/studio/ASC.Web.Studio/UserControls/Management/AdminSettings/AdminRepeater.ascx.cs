using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class AdminRepeater : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/AdminSettings/AdminRepeater.ascx"; } }  

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void BindData(List<object> data)
        {
            _adminsRepeater.DataSource = data;
            _adminsRepeater.DataBind();
        }
    }
}