using System;

namespace ASC.Core.Billing
{
    [Serializable]
    public class PaymentInfo
    {
        public string CartId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public Decimal Price
        {
            get;
            set;
        }

        public string Currency
        {
            get;
            set;
        }

        public string Method
        {
            get;
            set;
        }
    }
}
