using System.Linq;
using ASC.Api.Interfaces;

namespace ASC.Api.Publisher
{
    public class ApiMethodCallData
    {
        public IApiMethodCall Method { get; set; }
        public object Result { get; set; }
    }
}