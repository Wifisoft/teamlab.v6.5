using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASC.Web.UserControls.Wiki
{
    public static class WikiStrExtentions
    {
        public static string EscapeString(this string str)
        {
            return str.Replace(@"\", @"\\").Replace(@"/", @"\/").Replace(@"'", @"\'");
        }
    }
}