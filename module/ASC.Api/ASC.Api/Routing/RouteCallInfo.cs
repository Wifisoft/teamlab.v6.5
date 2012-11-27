using System.Collections.Generic;
using System.Linq;

namespace ASC.Api.Routing
{
    public class RouteCallInfo
    {
        public int Tid { get; set; }
        public string Url { get; set; }

        public string Method { get; set; }

        public Dictionary<string, object> Params { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} T:{2},{3}", Method.ToUpper(),Url,Tid,string.Join(",",Params.Select(x=>string.Format("{0}={1}",x.Key,x.Value)).ToArray()));
        }
    }
}