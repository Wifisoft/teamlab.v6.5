using System;
using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core.Billing
{
    public interface ITariffService
    {
        Tariff GetTariff(int tenantId);

        IEnumerable<PaymentInfo> GetPayments(int tenantId);

        Uri GetShoppingUri(int tenant, int plan);

        void SetTariffCoupon(int tenant, string group);
    }
}
