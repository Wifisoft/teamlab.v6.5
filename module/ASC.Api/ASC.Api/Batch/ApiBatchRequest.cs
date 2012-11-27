using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;

namespace ASC.Api.Batch
{
    public class ApiBatchRequest
    {
        public ApiBatchRequest()
        {
            //Defaults
            BodyContentType = new ContentType("application/x-www-form-urlencoded"){CharSet = Encoding.UTF8.WebName}.ToString();
            Method = "GET";
        }

        public int Order { get; set; }

        public string[] After { get; set; }
        public string[] Before { get; set; }

        public string RelativeUrl { get; set; }

        public string Name { get; set; }

        private string _method;
        public string Method
        {
            get { return string.IsNullOrEmpty(_method)?"GET":_method; }
            set { _method = value; }
        }

        public string Body { get; set; }

        public string BodyContentType { get; set; }
    }
}