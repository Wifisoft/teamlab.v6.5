
using System.Text.RegularExpressions;
using ASC.Thrdparty.Configuration;

namespace ASC.Web.Studio.Core.SMS
{
    public sealed class SmsSenderCreator
    {
        public static ISmsSender CreateSender(string phoneNumber)
        {
            // CIS
            if (Regex.IsMatch(phoneNumber, KeyStorage.Get("sms.CISregex")))
                return new SmscSender {PhoneNumber = phoneNumber};
            // US
            else if (Regex.IsMatch(phoneNumber, KeyStorage.Get("sms.USregex")))
                return new USClickatellSender { PhoneNumber = phoneNumber };
            // Other countries
            else
                return new ClickatellSender {PhoneNumber = phoneNumber};
        }

        public static ISmsSender GetDefault()
        {
            return new ClickatellSender();
        }
    }
}