using System;
using System.IO;
using System.Net;
using log4net;

namespace ASC.Web.Studio.Core.SMS
{
    public class SmsSender
    {
        private readonly static ILog log = LogManager.GetLogger("ASC.Web.Studio.Core.SMS");

        #region - Accessors -
        public string PhoneNumber { get; set; }

        protected string RequestedString { get; set; }

        protected ProtocolMethod Method { get; set; }

        protected enum ProtocolMethod { GET, POST }
        #endregion

        protected virtual void BuildRequestString(string messge)
        {
            throw new Exception("Non implemented");
        }

        protected string SendNotifyRequest()
        {
            try
            {
                var smsSendReq = (HttpWebRequest)WebRequest.Create(new Uri(RequestedString));
                smsSendReq.Method = Method.ToString();
                smsSendReq.ContentType = "application/x-www-form-urlencoded";

                using (var response = smsSendReq.GetResponse())
                using (var dataStream = response.GetResponseStream())
                using (var reader = new StreamReader(dataStream))
                {
                    var r = reader.ReadToEnd();
                    log.InfoFormat("SMS was sent to {0} \nService returned: {1}", PhoneNumber, r);
                    return r;
                }
            }
            catch (Exception ex)
            {
                log.Fatal("Failed to send sms message", ex);
                return String.Empty;
            }
        }
    }
}