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
using AjaxPro;
using ASC.Web.CRM.Classes;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;
using System.Collections.Specialized;
using System.Text;
using ASC.CRM.Core.Entities;
using System.Collections.Generic;

namespace ASC.Web.CRM.Controls.Common
{
    [AjaxNamespace("AjaxPro.CategorySelector")]
    public partial class CategorySelector : BaseUserControl
    {
        protected string selectorID = Guid.NewGuid().ToString().Replace('-', '_');
        protected string jsObjName;

        #region Properties

        public static string Location
        {
            get
            {
                return PathProvider.GetFileStaticRelativePath("Common/CategorySelector.ascx");
            }
        }

        public ASC.CRM.Core.Entities.ListItem SelectedCategory { get; set; }

        public int MaxWidth { get; set; }

        public List<ASC.CRM.Core.Entities.ListItem> Categories { get; set; }

        protected bool MobileVer = false;

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);
            
            jsObjName = String.IsNullOrEmpty(this.ID) ? "categorySelector_" + selectorID : this.ID;

            itemRepeater.DataSource = Categories;
            itemRepeater.DataBind();

            if (SelectedCategory == null)
            {
                SelectedCategory = Categories.Count > 0 ? Categories[0] : new ASC.CRM.Core.Entities.ListItem();
            }

            if (MaxWidth == 0)
            {
                MaxWidth = 230;
            }
        }

        #endregion

        #region Methods

        #endregion

        #region AjaxMethods

        #endregion
    }
}