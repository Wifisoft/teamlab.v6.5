using System;
using System.Web;
using System.Web.UI;
using AjaxPro;
using ASC.Web.CRM.Controls.Contacts;
using ASC.Web.Studio.Utility;
using ASC.Web.Core.Helpers;

namespace ASC.Web.CRM.Classes
{
    [AjaxNamespace("AjaxPro.AjaxProHelper")]
    public class AjaxProHelper : BasePage
    {

        #region Events

        protected override void PageLoad()
        {
        }

        #endregion


        #region AjaxMethods

        [AjaxMethod]
        public AjaxResponse GetContactInfoCard(int contactID, string popupBoxID)
        {
            var resp = new AjaxResponse {rs1 = popupBoxID};

            var page = new Page();
            var cntrlContactInfoCard = (ContactInfoCard)LoadControl(ContactInfoCard.Location);
            cntrlContactInfoCard.ContactID = contactID;
            page.Controls.Add(cntrlContactInfoCard);
            var writer = new System.IO.StringWriter();
            HttpContext.Current.Server.Execute(page, writer, false);
            var output = writer.ToString();
            writer.Close();

            resp.rs2 = output;

            return resp;
        }

        [AjaxMethod]
        public string ChooseNumeralCase(int count,string nominative,string genitiveSingular,string genitivePlural)
        {
            return GrammaticalHelper.ChooseNumeralCase(count, nominative, genitiveSingular, genitivePlural);
        }
        

        #endregion
    }
}