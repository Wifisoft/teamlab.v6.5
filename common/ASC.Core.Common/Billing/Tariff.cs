using System;
using ASC.Core.Tenants;

namespace ASC.Core.Billing
{
    public class Tariff
    {
        public int QuotaId
        {
            get;
            internal set;
        }

        public TariffState State
        {
            get;
            internal set;
        }

        public DateTime DueDate
        {
            get;
            internal set;
        }

        public DateTime FrozenDate
        {
            get;
            internal set;
        }


        public static Tariff CreateDefault()
        {
            return new Tariff
            {
                QuotaId = Tenant.DEFAULT_TENANT,
                State = TariffState.Paid,
                DueDate = DateTime.MaxValue,
                FrozenDate = DateTime.MaxValue,
            };
        }


        public override int GetHashCode()
        {
            return QuotaId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var t = obj as Tariff;
            return t != null && t.QuotaId == QuotaId;
        }
    }
}
