using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ASC.Data.Storage;

namespace ASC.Web.Talk.UserControls
{
  public partial class RoomsContainer : System.Web.UI.UserControl
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.roomscontainer", WebPath.GetPath("addons/talk/js/talk.roomscontainer.js"));
    }

    public String GetOverdueInterval()
    {
      return new TalkConfiguration().OverdueInterval;
    }
  }
}
