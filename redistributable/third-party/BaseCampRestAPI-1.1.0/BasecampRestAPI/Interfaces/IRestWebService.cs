using System.Xml;
using System.Net;

namespace BasecampRestAPI
{
	public interface IRestWebService
	{
		string Url { get; }
		string UserName { get; }

		XmlElement GetRequestResponseElement(string requestPath);
		XmlElement GetRequestResponseElement(string requestPath, string data);
		XmlElement GetRequestResponseElement(string requestPath, string data, HttpVerb verb);
		string PostRequestGetLocation(string requestPath, string data);
		string PutRequest(string requestPath, string data);
		string DeleteRequest(string requestPath);

        HttpWebRequest GetRequest(string url);
	}
}