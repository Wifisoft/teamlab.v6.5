using System;
using System.Web;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.Core.SearchHandlers
{
    public class StudioSearchHandler : BaseSearchHandlerEx
    {
        public override string AbsoluteSearchURL
        {
            get { return VirtualPathUtility.ToAbsolute("~/search.aspx"); }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "common_search_icon.png", PartID = Guid.Empty }; }
        }

        public override string PlaceVirtualPath
        {
            get { return "~/"; }
        }

        public override string SearchName
        {
            get { return Resources.Resource.Search; }
        }

        public override SearchResultItem[] Search(string text)
        {
            return new SearchResultItem[0];
        }
    }
}