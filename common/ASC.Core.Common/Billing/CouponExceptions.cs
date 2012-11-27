using System;
using System.Runtime.Serialization;

namespace ASC.Core.Billing
{
    [Serializable]
    public class CouponExcepton : Exception
    {
        public CouponExcepton()
            : base()
        {
        }

        public CouponExcepton(string message)
            : base(message)
        {
        }

        protected CouponExcepton(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class CouponNotApplicableExcepton : CouponExcepton
    {
        public CouponNotApplicableExcepton()
            : base("Coupon not applicable.")
        {
        }

        protected CouponNotApplicableExcepton(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class CouponNotFoundExcepton : CouponExcepton
    {
        public CouponNotFoundExcepton()
            : base("Coupon not found.")
        {
        }

        protected CouponNotFoundExcepton(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class CouponExpiredExcepton : CouponExcepton
    {
        public DateTime ValidFromDate
        {
            get;
            private set;
        }

        public DateTime ValidToDate
        {
            get;
            private set;
        }

        public CouponExpiredExcepton(DateTime validFrom, DateTime validTo)
            : base("Coupon expired.")
        {
            ValidFromDate = validFrom;
            ValidToDate = validTo;
        }

        protected CouponExpiredExcepton(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
