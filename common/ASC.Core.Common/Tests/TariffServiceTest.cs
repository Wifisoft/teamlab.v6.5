#if DEBUG
namespace ASC.Core.Common.Tests
{
    using System;
    using System.Configuration;
    using ASC.Core.Billing;
    using ASC.Core.Data;
    using NUnit.Framework;

    [TestFixture]
    public class TariffServiceTest
    {
        private readonly ITariffService tariffService;


        public TariffServiceTest()
        {
            var cs = ConfigurationManager.ConnectionStrings["core"];
            tariffService = new TariffService(cs, new DbQuotaService(cs), new DbTenantService(cs));
        }


        [Test]
        public void TestShoppingUri()
        {
            var uri = tariffService.GetShoppingUri(10, -10);
            Assert.AreEqual(uri, new Uri("https://secure.avangate.com/order/checkout.php?PRODS=4542487&QTY=1&CART=2&REF=10&ORDERSTYLE=nLW04pa5iH4="));

            uri = tariffService.GetShoppingUri(10, -11);
            Assert.AreEqual(uri, new Uri("https://secure.avangate.com/order/checkout.php?PRODS=4549384&QTY=1&CART=2&REF=10&ORDERSTYLE=nLW04pa5iH4="));
        }

        [Test]
        public void TestPaymentInfo()
        {
            //var payments = tariffService.GetPayments(1234567);
        }

        [Test]
        public void TestTariff()
        {
            //var tariff = tariffService.GetTariff(1234567);
        }

        [Test]
        public void TestSetTariffCoupon()
        {
            tariffService.SetTariffCoupon(1024, "coupon1");
        }
    }
}
#endif
