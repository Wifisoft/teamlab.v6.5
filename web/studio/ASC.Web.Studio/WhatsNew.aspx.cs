using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Web.Controls;
using ASC.Web.Core;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class WhatsNew : MainPage
    {
        public static string PageUrl { get { return "~/whatsnew.aspx"; } }

        protected Guid ProductID { get; set; }
        protected List<Guid> ModuleIDs { get; set; }


        public static string GetUrlForModule(Guid productId, Guid? moduleId)
        {
            var url = string.Format("{0}?{1}", VirtualPathUtility.ToAbsolute(PageUrl), CommonLinkUtility.GetProductParamsPair(productId));
            if (moduleId.HasValue) url = string.Format("{0}#mid{1}mid", url, moduleId.Value);
            return url;
        }


        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            base.SetProductMasterPage();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ProductID = GetProductID();
            
            if (ProductID.Equals(Guid.Empty))
            {
                Response.Redirect(CommonLinkUtility.GetDefault());
            }
            
            var master = Master as IStudioMaster;
            if (master == null) return;

            var container = new Container() { Body = new PlaceHolder(), Header = new PlaceHolder() };
            master.ContentHolder.Controls.Add(container);

            container.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.MainTitle, NavigationUrl = VirtualPathUtility.ToAbsolute(ProductManager.Instance[ProductID].StartURL) });
            container.BreadCrumbs.Add(new BreadCrumb() { Caption = Resources.Resource.RecentActivity });

            Title = HeaderStringHelper.GetPageTitle(Resources.Resource.RecentActivity, container.BreadCrumbs);

            InitBody(container.Body);

            var navigate = new SideNavigator();
            navigate.Controls.Add(new NavigationItem(Resources.Resource.MainTitle, VirtualPathUtility.ToAbsolute(ProductManager.Instance[ProductID].StartURL)));
            master.SideHolder.Controls.Add(navigate);
        }

        private void InitBody(PlaceHolder bodyHolder)
        {
            var whatsNewBody = ((WhatsNewBody)LoadControl(WhatsNewBody.Location));
            whatsNewBody.ProductId = ProductID;
            bodyHolder.Controls.Add(whatsNewBody);
        }
    }
}