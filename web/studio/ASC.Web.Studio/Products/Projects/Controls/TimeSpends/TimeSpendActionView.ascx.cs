using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Projects.Controls.TimeSpends
{
    public partial class TimeSpendActionView : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            _timetrackingContainer.Options.IsPopup = true;
        }
    }
}