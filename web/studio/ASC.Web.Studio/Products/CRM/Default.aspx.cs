#region Import

using System;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.CRM.Core.Entities;
using ASC.Web.Controls;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Contacts;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Utility;
using ASC.CRM.Core;
using ASC.Data.Storage;
using ASC.Web.CRM.Controls.Common;

#endregion

namespace ASC.Web.CRM
{
    public partial class Contacts : BasePage
    {
        #region Property

        public static String Location
        {
            get { return "~/products/crm/default.aspx"; }
        }

        #endregion

        #region Events

        protected override void PageLoad()
        {              
            InitControls();
            AjaxPro.Utility.RegisterTypeForAjax(typeof (Tasks));

           // CommonContainerHolder.Controls.Add(Page.LoadControl(TestMailSender.Location));

           // Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "contact_maker_script", WebPath.GetPath("/Products/CRM/js/contacts.js"));
        }

        #endregion

        #region Methods


        protected void InitControls()
        {
            int contactID;

            if (int.TryParse(UrlParameters.ID, out contactID))
            {

                var targetContact = Global.DaoFactory.GetContactDao().GetByID(contactID);

                if (targetContact == null || !CRMSecurity.CanAccessTo(targetContact))
                    Response.Redirect(PathProvider.StartURL());

                if (String.Compare(UrlParameters.Action, "manage", true) == 0)
                    ExecContactActionView(targetContact);
                else
                    ExecContactDetailsView(targetContact);

                _ctrlContactID.Value = targetContact.ID.ToString();
            }
            else
            {
                if (String.Compare(UrlParameters.Action, "manage", true) == 0)
                    ExecContactActionView(null);
                else if (String.Compare(UrlParameters.Action, "import", true) == 0)
                    ExecImportView();
                else
                    ExecListContactView();
            }
        }

        static public string EncodeTo64(string toEncode)
        {

            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(toEncode);

            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }

        static public string DecodeFrom64(string encodedData)

    {

      byte[] encodedDataAsBytes

          = System.Convert.FromBase64String(encodedData);

      string returnValue =

         System.Text.UTF8Encoding.UTF8.GetString(encodedDataAsBytes);

      return returnValue;

    }

        protected void ExecImportView()
        {
            Master.BreadCrumbs.Add(new BreadCrumb
                                       {
                                           Caption = CRMContactResource.ImportContacts
                                       });

            Master.DisabledSidePanel = true;

            var importViewControl = (ImportFromCSVView) LoadControl(ImportFromCSVView.Location);

            importViewControl.EntityType = EntityType.Contact;

            CommonContainerHolder.Controls.Add(importViewControl);

            Title = HeaderStringHelper.GetPageTitle(CRMContactResource.ImportContacts, Master.BreadCrumbs);
        }

        protected void ExecContactDetailsView(Contact targetContact)
        {

            Master.DisabledSidePanel = true;

            var contactDetailsViewControl = (ContactDetailsView) LoadControl(ContactDetailsView.Location);

            contactDetailsViewControl.TargetContact = targetContact;

            var contactTitle = targetContact.GetTitle();

            Master.BreadCrumbs.Add(new BreadCrumb
                                       {
                                           Caption = CRMContactResource.AllContacts,
                                           NavigationUrl = "default.aspx"
                                       });

            Master.BreadCrumbs.Add(new BreadCrumb
                                       {
                                           Caption = contactTitle
                                       });

            var title = contactTitle.HtmlEncode();

            if (CRMSecurity.IsPrivate(targetContact))
            {
                if (targetContact is Company)
                    Master.CommonContainerHeader = Global.RenderPrivateItemHeader(title, EntityType.Company, targetContact.ID);
                else
                    Master.CommonContainerHeader = Global.RenderPrivateItemHeader(title, EntityType.Contact, targetContact.ID);
            }

            CommonContainerHolder.Controls.Add(contactDetailsViewControl);

            Title = HeaderStringHelper.GetPageTitle(contactTitle.HtmlEncode(), Master.BreadCrumbs);
        }

        protected void ExecListContactView()
        {
            CommonContainerHolder.Controls.Add(LoadControl(ListContactView.Location));

            Master.DisabledSidePanel = true;

            Master.BreadCrumbs.Add(new BreadCrumb {Caption = CRMContactResource.AllContacts});

            Title = HeaderStringHelper.GetPageTitle(CRMContactResource.AllContacts, Master.BreadCrumbs);
        }

        protected void ExecContactActionView(Contact targetContact)
        {
            Master.DisabledSidePanel = true;

            var contactActionViewControl = (ContactActionView) LoadControl(ContactActionView.Location);

            contactActionViewControl.TargetContact = targetContact;

            if (targetContact == null)
            {

                if (String.Compare(UrlParameters.Type, "people", true) != 0)
                {
                    contactActionViewControl.TypeAddedContact = "company";
                    contactActionViewControl.SaveContactButtonText = CRMContactResource.AddThisCompanyButton;
                    contactActionViewControl.SaveAndCreateContactButtonText = CRMContactResource.AddThisAndCreateCompanyButton;

                    contactActionViewControl.AjaxProgressText = CRMContactResource.AddingCompany;
                    Master.BreadCrumbs.Add(new BreadCrumb
                                               {
                                                   Caption = CRMContactResource.BreadCrumbsAddCompany
                                               });

                    Title = HeaderStringHelper.GetPageTitle(CRMContactResource.BreadCrumbsAddCompany, Master.BreadCrumbs);
                }
                else
                {
                    contactActionViewControl.TypeAddedContact = "people";
                    contactActionViewControl.SaveContactButtonText = CRMContactResource.AddThisPersonButton;
                    contactActionViewControl.SaveAndCreateContactButtonText = CRMContactResource.AddThisAndCreatePeopleButton;

                    contactActionViewControl.AjaxProgressText = CRMContactResource.AddingPersonProgress;
                    Master.BreadCrumbs.Add(new BreadCrumb
                                               {
                                                   Caption = CRMContactResource.BreadCrumbsAddPerson
                                               });
                    Title = HeaderStringHelper.GetPageTitle(CRMContactResource.BreadCrumbsAddPerson, Master.BreadCrumbs);
                }
            }
            else
            {
                var contactTitle = targetContact.GetTitle();

                contactActionViewControl.SaveAndCreateContactButtonText = String.Compare(UrlParameters.Type, "people", true) != 0 ? CRMContactResource.SaveThisAndCreateCompanyButton : CRMContactResource.SaveThisAndCreatePeopleButton;

                contactActionViewControl.SaveContactButtonText = CRMContactResource.SaveChanges;
                contactActionViewControl.AjaxProgressText = CRMContactResource.SavingChangesProgress;

                if (targetContact is Company)
                {
                    contactActionViewControl.TypeAddedContact = "company";
                    Title = HeaderStringHelper.GetPageTitle(String.Format(CRMContactResource.EditCompany, contactTitle.HtmlEncode()), Master.BreadCrumbs);
                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = String.Format(CRMContactResource.EditCompany, contactTitle)
                    });
                }
                else
                {
                    contactActionViewControl.TypeAddedContact = "people";
                    Title = HeaderStringHelper.GetPageTitle(String.Format(CRMContactResource.EditPerson, contactTitle.HtmlEncode()), Master.BreadCrumbs);
                    Master.BreadCrumbs.Add(new BreadCrumb
                    {
                        Caption = String.Format(CRMContactResource.EditPerson, contactTitle)
                    });
                }
            }

            CommonContainerHolder.Controls.Add(contactActionViewControl);
            SideNavigatorPanel.Visible = false;

            /*if (targetContact != null)
                SideActionsPanel.Controls.Add(new NavigationItem
                {
                    Name = DealResource.DeleteDeal,
                    URL = "settings.aspx?type=custom_field"
                });
            */
        }

        #endregion
    }
}