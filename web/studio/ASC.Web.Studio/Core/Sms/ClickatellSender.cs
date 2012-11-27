using System;
using ASC.Thrdparty.Configuration;

namespace ASC.Web.Studio.Core.SMS
{
    public class ClickatellSender : SmsSender, ISmsSender
    {
        public ClickatellSender()
        {
            Method = ProtocolMethod.POST;
        }

        protected override void BuildRequestString(string message)
        {
            RequestedString = String.Format(KeyStorage.Get("smsOperatorMask_clickatel"), KeyStorage.Get("smsOperatorLogin_clickatel"), KeyStorage.Get("smsOperatorPass_clickatel"), KeyStorage.Get("smsOperatorApiid_clickatel"), "+" + PhoneNumber, message);
        }

        public RequestSender Notify(string message)
        {
            if (String.IsNullOrEmpty(RequestedString))
                BuildRequestString(message);

            SendNotifyRequest();

            return new RequestSender();
        }
    }
}