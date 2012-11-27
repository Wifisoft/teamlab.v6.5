using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Controls.Dashboard;

namespace ASC.Web.Studio.UserControls.Users.Activity
{
    [Serializable]
    [DataContract]
    public class ProductActivityWidgetSettings : ISettings
    {
        [DataMember(Name = "CountActivities")]
        public int CountActivities { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{789E5956-AFC6-4ed7-95C9-8F5673F35F79}"); }
        }

        public ISettings GetDefault()
        {
            return new ProductActivityWidgetSettings() { CountActivities = 5 };
        }

        #endregion
    }

    [WidgetPosition(0, 2)]
    public partial class  ProductActivity : System.Web.UI.UserControl
    {
        public static Guid WidgetID { get { return new Guid("{DA3DA92E-E3A6-49e3-BEEB-6F533BB38845}"); } }

        public static string Location
        {
            get { return "~/UserControls/Users/Activity/ProductActivity.ascx"; }
        }
        
        private Guid productID = Guid.Empty;
        public Guid ProductId
        {
            protected get { return productID; }
            set { productID = value; }
        }

        protected string RenderProfileLink(Guid userId)
        {
            UserInfo userInfo = CoreContext.UserManager.GetUsers(userId);
            return userInfo.RenderProfileLink(ProductId);
        }

        public List<UserContentActivity> Activities { get; set; }       
        protected void Page_Load(object sender, EventArgs e)
        {
            BindData();
        }

        public void BindData()
        {
            rpModuleActivity.DataSource = Activities;
            rpModuleActivity.DataBind();
        }
    }
}