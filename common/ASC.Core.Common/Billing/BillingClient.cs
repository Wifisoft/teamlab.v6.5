using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using System.Xml.Linq;
using log4net;

namespace ASC.Core.Billing
{
    class BillingClient : ClientBase<IService>, IDisposable
    {
        private readonly static ILog log = LogManager.GetLogger(typeof(TariffService));


        public XElement GetLastPayment(int tenant)
        {
            return Request(string.Format("<GetLatestActiveResource><PortalId>{0}</PortalId></GetLatestActiveResource>", tenant), true);
        }

        public XElement GetPayments(int tenant)
        {
            return Request(string.Format("<GetPayments><PortalId>{0}</PortalId></GetPayments>", tenant), true);
        }

        public XElement GetPaymentUrl(int tenant, string product)
        {
            return Request(string.Format("<GetPaymentSystemUrl><PaymentSystemId>1</PaymentSystemId><ProductId>{0}</ProductId><PortalId>{1}</PortalId></GetPaymentSystemUrl>", product, tenant), false);
        }

        public XElement GetPaymentUpdateUrl(int tenant, string product)
        {
            return Request(string.Format("<GetPaymentSystemUpdateUrl><PaymentSystemId>1</PaymentSystemId><ProductId>{0}</ProductId><PortalId>{1}</PortalId></GetPaymentSystemUpdateUrl>", product, tenant), false);
        }

        private XElement Request(string request, bool xmlContent)
        {
            log.DebugFormat("Billing service request: {0}", request);

            var responce = Channel.Request(new Message { Type = MessageType.Data, Content = request, });

            log.DebugFormat("Billing service responce: {0} {1}", responce.Type, responce.Content);

            if (responce.Type == MessageType.Data)
            {
                if (xmlContent)
                {
                    var xml = responce.Content;
                    var invalidChar = ((char)65279).ToString();
                    if (xml.Contains(invalidChar))
                    {
                        xml = xml.Replace(invalidChar, string.Empty);
                    }
                    if (xml.Contains("&"))
                    {
                        xml = HttpUtility.HtmlDecode(xml);
                    }
                    return XElement.Parse(xml);
                }
                else
                {
                    return new XElement("Url", responce.Content);
                }
            }
            else
            {
                throw new Exception(responce.Content);
            }
        }

        void IDisposable.Dispose()
        {
            try
            {
                Close();
            }
            catch (CommunicationException)
            {
                Abort();
            }
            catch (TimeoutException)
            {
                Abort();
            }
            catch (Exception)
            {
                Abort();
                throw;
            }
        }
    }


    [ServiceContract]
    interface IService
    {
        [OperationContract]
        Message Request(Message message);
    }

    [DataContract(Name = "Message", Namespace = "http://schemas.datacontract.org/2004/07/teamlabservice")]
    [Serializable]
    class Message
    {
        [DataMember]
        public string Content
        {
            get;
            set;
        }

        [DataMember]
        public MessageType Type
        {
            get;
            set;
        }
    }

    [DataContract(Name = "MessageType", Namespace = "http://schemas.datacontract.org/2004/07/teamlabservice")]
    enum MessageType
    {
        [EnumMember]
        Undefined = 0,

        [EnumMember]
        Data = 1,

        [EnumMember]
        Error = 2,
    }
}
