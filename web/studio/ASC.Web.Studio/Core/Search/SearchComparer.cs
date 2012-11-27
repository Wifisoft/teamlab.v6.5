using System.Collections.Generic;
using ASC.Web.Core.ModuleManagement.Common;

namespace ASC.Web.Studio.Core.Search
{
    public sealed class SearchComparer : IComparer<SearchResult>
    {
        public int Compare(SearchResult x, SearchResult y)
        {
            if (x.Name == y.Name)
                return 0;
            if(y.Name == Users.CustomNamingPeople.Substitute<Resources.Resource>("Employees"))
                return 1;
            return -1;
        }
    }

    public sealed class DateSearchComparer : IComparer<SearchResultItem>
    {
        public int Compare(SearchResultItem x, SearchResultItem y)
        {
            if (x.Date == y.Date)
                return 0;
            if (x.Date > y.Date)
                return -1;
            return 1;
        }
    }
}
