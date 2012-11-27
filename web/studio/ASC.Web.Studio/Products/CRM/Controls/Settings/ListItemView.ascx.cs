#region Import

using System;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.Studio.Controls.Common;

#endregion

namespace ASC.Web.CRM.Controls.Settings
{
    [AjaxNamespace("AjaxPro.ListItemView")]
    public partial class ListItemView : BaseUserControl
    {
        #region Members

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Settings/ListItemView.ascx"); } }

        public ListType CurrentTypeValue { get; set;}
        
        public string AddButtonText { get; set; }
        public string AddPopupWindowText { get; set; }

        public string EditText { get; set; }
        public string EditPopupWindowText { get; set; }

        public string AjaxProgressText { get; set; }
        public string DeleteText { get; set; }

        public string DescriptionText { get; set; }
        public string DescriptionTextEditDelete { get; set; }


        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(this.GetType());
            _manageItemPopup.Options.IsPopup = true;

            var query = "history/category";
            switch (CurrentTypeValue)
            {
                case ListType.ContactStatus:
                    query = "contact/type";
                    break;
                case ListType.HistoryCategory:
                    query = "history/category";
                    break;
                case ListType.TaskCategory:
                    query = "task/category";
                    break;
            }
            
            var apiServer = new Api.ApiServer();
            
            var data = apiServer.GetApiResponse(String.Format("api/1.0/crm/{0}.json", query), "GET");

            Page.JsonPublisher(data, "itemList");
        }

        #endregion

        #region Ajax Methods

        protected ListType ToListType(int listType)
        {
            var currentVal = ListType.ContactStatus;
            if (listType == Convert.ToInt32(ListType.TaskCategory)) currentVal = ListType.TaskCategory;
            if (listType == Convert.ToInt32(ListType.HistoryCategory)) currentVal = ListType.HistoryCategory;
            
            return currentVal;
        }

        #endregion

        #region Ajax Methods

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void ReorderItems(string[] itemsName, int typeUrl)
        {
            if (!CRMSecurity.IsAdmin)
                Response.Redirect(PathProvider.StartURL());

            var currentVal = ToListType(typeUrl);

            Global.DaoFactory.GetListItemDao().ReorderItems(currentVal, itemsName);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string ChangeColor(int itemID, string color)
        {
            if (!CRMSecurity.IsAdmin)
                Response.Redirect(PathProvider.StartURL());

            var item = Global.DaoFactory.GetListItemDao().GetByID(itemID);
            item.Color = color;
            Global.DaoFactory.GetListItemDao().ChangeColor(itemID, color);
            return color;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void ChangeIcon(int itemID, string icon)
        {
            if (!CRMSecurity.IsAdmin)
                Response.Redirect(PathProvider.StartURL());

            Global.DaoFactory.GetListItemDao().ChangePicture(itemID, icon);
        }

      #endregion

    }
}