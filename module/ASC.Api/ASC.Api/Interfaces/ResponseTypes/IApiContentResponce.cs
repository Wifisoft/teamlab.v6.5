using System.IO;
using System.Net.Mime;
using System.Text;

namespace ASC.Api.Interfaces.ResponseTypes
{
    public interface IApiContentResponce
    {
        Stream ContentStream { get; }
        ContentType ContentType { get; }
        Encoding ContentEncoding { get; }
        ContentDisposition ContentDisposition { get; }
    }
}