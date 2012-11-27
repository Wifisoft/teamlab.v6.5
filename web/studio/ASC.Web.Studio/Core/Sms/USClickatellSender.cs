using System;
using ASC.Thrdparty.Configuration;

namespace ASC.Web.Studio.Core.SMS
{
    public class USClickatellSender: ClickatellSender
    {
        protected override void BuildRequestString(string message)
        {
            RequestedString = String.Format(KeyStorage.Get("smsOperatorMask_clickatel"), KeyStorage.Get("smsOperatorLogin_clickatel_us"), KeyStorage.Get("smsOperatorPass_clickatel_us"), KeyStorage.Get("smsOperatorApiid_clickatel_us"), "+" + PhoneNumber, message);
        }
    }
}
