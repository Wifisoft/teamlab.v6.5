using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.Studio.UserControls.Company
{
    public partial class CompanyNavigation : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Company/CompanyNavigation.ascx"; } }

        private Guid _productID;

        protected void Page_Load(object sender, EventArgs e)
        {
            EmployeeSiteNavigator.HeaderCSSClass = "studioSideBoxActionsHeader";
            EmployeeSiteNavigator.BodyCSSClass = "studioSideBoxNavigationBody";
            EmployeeSiteNavigator.ImageURL = WebImageSupplier.GetAbsoluteWebPath("home.png");
            EmployeeSiteNavigator.Title = CustomNamingPeople.Substitute<Resources.Resource>("Departments");

            _productID = CommonLinkUtility.GetProductID();
            if (!Page.IsPostBack)
            {
                categoryTree.ShowExpandCollapse = false;
                FillTree();
            }
        }

        private string GetLinkByGuid(Guid id)
        {
            string pageRelUrl = CommonLinkUtility.GetEmployees(_productID) + "&";

            string pageUrl = string.Empty;
            if (Request["deplist"] == null)
            {
                pageUrl = string.Format("{0}depID={1}&search=&page=1&sort={2}{3}", pageRelUrl,
                    id == Guid.Empty ? string.Empty : id.ToString(),
                    Request["sort"], Request["list"] == null ? string.Empty : "&list=" + Request["list"]);
            }
            else
            {
                if (id == Guid.Empty)
                {
                    pageUrl = string.Format("{0}deplist={1}", pageRelUrl, Request["list"] == null ? string.Empty : "&list=" + Request["list"]);
                }
                else
                {
                    pageUrl = string.Format("{0}deplist={1}&search=&page=1&sort={2}{3}", pageRelUrl, id.ToString(),
                        Request["sort"], Request["list"] == null ? string.Empty : "&list=" + Request["list"]);
                }

            }

            return ResolveUrl(UrlQueryManager.AddDefaultParameters(Request, pageUrl));
        }

        private void FillTree()
        {
            categoryTree.Nodes.Clear();
            var groups = CoreContext.GroupManager.GetGroups();
            if (groups.Length == 0)
            {
                EmployeeSiteNavigator.Visible = false;
                return;
            }

            foreach (var group in groups.OrderBy(g => g.Name))
            {
                categoryTree.Nodes.Add(AddNode(group));
            }

            categoryTree.Nodes.AddAt(0, new TreeNode(CustomNamingPeople.Substitute<Resources.Resource>("EmployeeAllDepartments").HtmlEncode()) { NavigateUrl = GetLinkByGuid(Guid.Empty) });
        }

        private TreeNode AddNode(GroupInfo group)
        {
            TreeNode node = new TreeNode(group.Name.HtmlEncode());

            node.ToolTip = group.Description.HtmlEncode();
            node.Expanded = true;
            node.NavigateUrl = GetLinkByGuid(group.ID);
            foreach (var _group in group.Descendants.OrderBy(g => g.Name))
            {
                node.ChildNodes.Add(AddNode(_group));
            }

            return node;
        }
    }
}