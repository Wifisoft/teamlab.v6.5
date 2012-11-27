#region Import
using System;
using System.Web;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.CRM.Configuration
{

    public class SearchHandler :  BaseSearchHandlerEx
    {
        public override Guid ProductID
        {
            get { return ProductEntryPoint.ID; }
        }
       
        public override Guid ModuleID
        {
            get { return ProductID; }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "search_in_module.png", PartID = ProductID }; }
        }

        public override string SearchName
        {
            get { return CRMContactResource.Search; }
        }

        public override string AbsoluteSearchURL
        {
            get { return String.Concat(CommonLinkUtility.GetFullAbsolutePath("~/search.aspx"), "?product=crm"); }
        }

        public override IItemControl Control
        {
            get
            {
                return new ResultsView();
            }
        }

        public override string PlaceVirtualPath
        {
            get { return PathProvider.BaseVirtualPath; }
        }

        public override SearchResultItem[] Search(string searchText)
        {
            return Global.DaoFactory.GetSearchDao().Search(searchText);
        }
    }
}