using System.Collections.Generic;

namespace ASC.Api.Web.Help.Helpers
{
    public class HttpMethodOrderComarer: IComparer<string>
    {
        private readonly List<string> _methods = new List<string> {"get", "post", "put", "delete"};

        public int Compare(string x, string y)
        {
            return _methods.IndexOf(x.ToLowerInvariant()).CompareTo(_methods.IndexOf(y.ToLowerInvariant()));
        }
    }
}