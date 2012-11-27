using System.Net;
using System.Xml.Linq;

namespace ASC.Xmpp.Common
{
    public interface IHttpResponder
    {
        string Path { get; }

        XDocument Process(HttpListenerRequest request);
    }
}