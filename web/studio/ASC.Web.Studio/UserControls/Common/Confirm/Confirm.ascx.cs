using System;
using System.Web;
using System.Web.UI;
using ASC.Data.Storage;

namespace ASC.Web.Studio.UserControls.Common
{
    public partial class Confirm : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/usercontrols/common/confirm/confirm.ascx"; } }

        public Confirm()
        {
            AdditionalID = "";
        }

        public string Title { get; set; }
        public string Value { get; set; }
        public string SelectTitle { get; set; }
        public string AdditionalID { get; set; }
        public string SelectJSCallback { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _studioConfirm.Options.IsPopup = true;
            _confirmEnterCode.Value = String.Format("StudioConfirm.Select('{0}',{1});", AdditionalID, SelectJSCallback);

            Page.ClientScript.RegisterClientScriptInclude(GetType(), "confirm_script", WebPath.GetPath("usercontrols/common/confirm/js/confirm.js"));
        }

    }
}