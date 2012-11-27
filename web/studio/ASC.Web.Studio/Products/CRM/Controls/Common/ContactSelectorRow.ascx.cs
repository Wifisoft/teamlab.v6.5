using System;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Resources;

namespace ASC.Web.CRM.Controls.Common
{
    public partial class ContactSelectorRow : System.Web.UI.UserControl
    {
        public string ObjName { get; set; }
        
        public string ObjID { get; set; }

        public static string Location
        {
            get
            {
                return PathProvider.GetFileStaticRelativePath("Common/ContactSelectorRow.ascx");
            }
        }

        public Contact SelectedContact { get; set; }

        public bool ShowContactImg { get; set; }

        public bool ShowChangeButton { get; set; }

        public bool ShowDeleteButton { get; set; }

        public bool ShowAddButton { get; set; }

        public bool ShowOnlySelectorContent { get; set; }

        public bool ShowNewCompanyContent { get; set; }

        public bool ShowNewContactContent { get; set; }

        public string DescriptionText { get; set; }

        public string DeleteContactText { get; set; }

        public string AddButtonText { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DeleteContactText == string.Empty)
                DeleteContactText = CRMContactResource.DeleteContact;
        }

        protected int GetContactID()
        {
            return SelectedContact != null ? SelectedContact.ID : 0;
        }
        
        protected string GetContactImgSrc()
        {
            return SelectedContact!=null ? ContactPhotoManager.GetSmallSizePhoto(SelectedContact.ID, SelectedContact is Company) : WebImageSupplier.GetAbsoluteWebPath("blank.gif");
        }

        protected string GetContactTitle()
        {
            return SelectedContact!=null ? SelectedContact.GetTitle().HtmlEncode() : string.Empty;
        }

        protected string GetCssSelectorStyle()
        {
            if (ShowOnlySelectorContent)
                return string.Empty;
            return SelectedContact == null ? string.Empty : "display: none;";
        }
    }
}