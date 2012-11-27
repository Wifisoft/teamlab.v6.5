using System;
using AjaxPro;

namespace ASC.Web.CRM.Controls.SocialMedia
{
    [AjaxNamespace("AjaxPro.ContactsSearchView")]
    public partial class ContactsSearchView : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(this.GetType());
            _ctrlContactsSearchContainer.Options.IsPopup = true;
        }

        [AjaxMethod]
        public string FindContactByName(string searchUrl, string contactNamespace)
        {
            var findGET = System.Net.WebRequest.Create(searchUrl);

            var findResp = findGET.GetResponse();
            if (findResp != null)
            {
                var findStream = findResp.GetResponseStream();
                if (findStream != null)
                {
                    var sr = new System.IO.StreamReader(findStream);
                    var s = sr.ReadToEnd();


                    var permalink = Newtonsoft.Json.Linq.JObject.Parse(s)["permalink"].ToString().HtmlEncode();

                    var infoGet = System.Net.WebRequest.Create(@"http://api.crunchbase.com/v/1/" + contactNamespace + "/" + permalink + ".js");
                    var infoResp = infoGet.GetResponse();
                    if (infoResp != null)
                    {
                        var infoStream = infoResp.GetResponseStream();
                        if (infoStream != null)
                        {
                            sr = new System.IO.StreamReader(infoStream);
                            s = sr.ReadToEnd();
                            return s;
                        }
                    }
                    s = sr.ReadToEnd();
                    
                    return s;
                }
            }
            return string.Empty;
        }

    }
}