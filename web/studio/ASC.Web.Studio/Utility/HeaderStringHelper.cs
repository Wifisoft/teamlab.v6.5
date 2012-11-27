using System;
using System.Collections.Generic;
using ASC.Web.Controls;
using ASC.Web.Core;

namespace ASC.Web.Studio.Utility
{
    public static class HeaderStringHelper
    {
        public static string GetHTMLSearchHeader(string searchString)
        {
            return String.Format("{0}: \"{1}\"", Resources.Resource.SearchResult, searchString);
        }

        public static string GetPageTitle(string moduleName, string pageHeader, params string[] path)
        {
            var productName = "";
            var product = ProductManager.Instance[CommonLinkUtility.GetProductID()];
            if (product != null)
                productName = product.Name;

            return String.Format(
                "{0} - {1}",
                (path == null || path.Length == 0) ? moduleName : pageHeader,
                String.IsNullOrEmpty(productName) ? Resources.Resource.WebStudioName : productName
                );
        }

        public static string GetPageTitle(string moduleName, IEnumerable<BreadCrumb> breadCrumbs)
        {
            var list = new List<BreadCrumb>(breadCrumbs);
            var path = new List<string>();
            if (list.Count > 0)
                path.AddRange(list.GetRange(0, list.Count - 1).ConvertAll(bc => bc.Caption));

            return GetPageTitle(
                moduleName,
                list.Count > 0 ? list[list.Count - 1].Caption : null,
                list.Count > 1 ? path.ToArray() : null
                );
        }
    }
}