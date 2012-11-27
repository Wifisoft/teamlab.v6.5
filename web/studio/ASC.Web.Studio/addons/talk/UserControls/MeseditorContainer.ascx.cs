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
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Talk.UserControls
{
  public partial class MeseditorContainer : System.Web.UI.UserControl
  {
    private TalkConfiguration cfg;

    protected void Page_Load(object sender, EventArgs e)
    {
      Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "talk.meseditorcontainer", WebPath.GetPath("addons/talk/js/talk.meseditorcontainer.js"));

      cfg = new TalkConfiguration();

      talkHistoryButton.Visible = cfg.EnabledHistory;
      talkMassendButton.Visible = cfg.EnabledMassend;
      talkConferenceButton.Visible = cfg.EnabledConferences;
    }

    public String GetMeseditorStyle()
    {
        return WebSkin.GetUserSkin().GetAbsoluteWebPath("/addons/talk/css/<theme_folder>/talk.messagearea.css");
    }
  }
}