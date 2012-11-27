using System;
using ASC.Thrdparty.Configuration;

namespace ASC.Web.Studio.Core.SMS
{
    public class SmscSender : SmsSender, ISmsSender
    {

        protected override void BuildRequestString(string message)
        {
            RequestedString = String.Format(KeyStorage.Get("smsOperatorMask_smsc"), KeyStorage.Get("smsOperatorLogin_smsc"), KeyStorage.Get("smsOperatorPass_smsc"), PhoneNumber, message);
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