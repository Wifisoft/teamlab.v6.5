using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

namespace BasecampRestAPI
{
	class ProductionWebRequest : IWebRequest
	{
		private HttpWebRequest _request;
		private XmlDocument _response;

		public static ProductionWebRequest GetInstance(string url)
		{
			return new ProductionWebRequest(url);
		}
		private ProductionWebRequest(string url)
		{
			_request = (HttpWebRequest)WebRequest.Create(url);
			_request.ContentType = "text/xml";
			_request.ServicePoint.Expect100Continue = false;
		}

		#region Implementation of IWebRequest
		public HttpVerb Method
		{
			set { _request.Method = value.ToString().ToUpper(); }
		}

		public string BasicAuthorization
		{
			set { _request.Headers.Add("Authorization", string.Format("Basic {0}", value)); }
		}

		public string RequestText
		{
			set
			{
				using (StreamWriter writer = new StreamWriter(_request.GetRequestStream()))
				{
					writer.WriteLine(value);
				}
			}
		}

		public XmlDocument Response
		{
			get
			{
				if (_response == null)
				{
					_response = new XmlDocument();
					_response.LoadXml(ResponseText);
				}
				return _response;
			}
		}

		public string Location
		{
			get
			{
				if (_location == null)
				{
					GetResponse();
					if (_location == null)
					{
						_location = string.Empty;
					}
				}
				return _location;
			}
		}

		#endregion

		public string ResponseText
		{
			get
			{
				if (_responseText == null)
				{
					GetResponse();
				}
				return _responseText;
			}
		}

        public HttpWebRequest HttpWebRequest
        {
            get
            {
                return _request;
            }
        }

	    private void GetResponse()
	    {
	        GetResponse(0);
	    }

	    private void GetResponse(int numRetries)
		{
		    try
		    {
                WebResponse response = _request.GetResponse();
                _location = response.Headers["Location"];
                if (_location != null)
                {
                    _location = (new System.Uri(_location)).AbsolutePath;
                }
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    _responseText = reader.ReadToEnd();
                }
		    }
		    catch (WebException e)
		    {
                if (numRetries > 5)
                    throw;

		        var httpResponce = e.Response as HttpWebResponse;
                if (httpResponce!=null) 
                {
                    if (httpResponce.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        //Rate limiting
                        if (!string.IsNullOrEmpty(httpResponce.GetResponseHeader("Retry-After")))
                        {
                            int retryAfter;
                            if (int.TryParse(httpResponce.GetResponseHeader("Retry-After"), out retryAfter))
                            {
                                //Sleep for timeout
                                Thread.Sleep(TimeSpan.FromSeconds(retryAfter + 5));
                                //And then retry
                                GetResponse(++numRetries);
                            }
                            else
                            {
                                throw;
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
		    }
		}

		private string _responseText;
		private string _location;
	}
}