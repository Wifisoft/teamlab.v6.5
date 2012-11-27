using System.Collections.Generic;
using ASC.Web.UserControls.Wiki.Data;

namespace ASC.Web.Community.Wiki
{
    public class PageDictionary
    {
        public PageDictionary()
        {
            Pages = new List<Page>();
        }
        public string HeadName { get; set; }
        public List<Page> Pages { get; set; }
    }
}
