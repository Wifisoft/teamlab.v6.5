using System;
using System.Collections.Generic;
using ASC.Web.Core.ModuleManagement.Common;

namespace ASC.Web.Studio.Core.Search
{
    public class SearchResult
    {
        public string LogoURL { get; set; }
        public string Name { get; set; }
        public Guid ModuleID { get; set; }
        public Guid ProductID { get; set; }
        public List<SearchResultItem> Items { get; set; }
        public string MoreURL { get; set; }
        public bool Adv { get; set; }
        public bool ShowAll { get; set; }

        public ItemSearchControl PresentationControl { get; set; }

        public SearchResult()
        {
            Items = new List<SearchResultItem>();
        }
    }
}