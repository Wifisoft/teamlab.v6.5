using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.CRM.Controls.SocialMedia
{
    public partial class UserSearchView : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            _ctrlUserSearchContainer.Options.IsPopup = true;
        }
    }
}