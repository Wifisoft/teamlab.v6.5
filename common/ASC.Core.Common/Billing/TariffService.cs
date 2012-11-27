using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Caching;
using ASC.Core.Data;
using ASC.Core.Tenants;
using log4net;

namespace ASC.Core.Billing
{
    class TariffService : DbBaseService, ITariffService
    {
        private const int DEFAULT_TRIAL_PERIOD = 31;
        private const int DEFAULT_GRACE_PERIOD = 7;

        private readonly static ILog log = LogManager.GetLogger(typeof(TariffService));
        private readonly IQuotaService quotaService;
        private readonly ITenantService tenantService;
        private readonly ICache cache;


        public TimeSpan CacheExpiration
        {
            get;
            set;
        }


        public TariffService(ConnectionStringSettings connectionString, IQuotaService quotaService, ITenantService tenantService)
            : base(connectionString, null)
        {
            this.quotaService = quotaService;
            this.tenantService = tenantService;
            this.cache = new AspCache();
            this.CacheExpiration = TimeSpan.FromSeconds(30);
        }


        public Tariff GetTariff(int tenantId)
        {
            var key = "tariff/" + tenantId;
            var tariff = cache.Get(key) as Tariff;
            if (tariff == null)
            {
                tariff = Tariff.CreateDefault();

                try
                {
                    using (var client = new BillingClient())
                    {
                        var xelement = client.GetLastPayment(tenantId);

                        var productid = xelement.Element("product-id").Value;
                        var quota = quotaService.GetTenantQuotas().SingleOrDefault(q => q.AvangateId == productid);
                        tariff.QuotaId = quota.Id;

                        var enddate = xelement.Element("end-date");
                        tariff.DueDate = DateTime.ParseExact(enddate.Value, "yyyy-MM-dd HH:mm:ss", null);

                        SaveBillingInfo(tenantId, Tuple.Create(tariff.QuotaId, tariff.DueDate));
                    }
                }
                catch (Exception error)
                {
                    log.Error(error);

                    var cached = GetBillingInfo(tenantId);
                    if (cached != null)
                    {
                        tariff.QuotaId = cached.Item1;
                        tariff.DueDate = cached.Item2;
                    }
                }

                CalculateState(tenantId, tariff);

                cache.Insert(key, tariff, DateTime.UtcNow.Add(CacheExpiration));
            }

            return tariff;
        }

        public IEnumerable<PaymentInfo> GetPayments(int tenantId)
        {
            var key = "billing/payments/" + tenantId;
            var payments = cache.Get(key) as List<PaymentInfo>;
            if (payments == null)
            {
                payments = new List<PaymentInfo>();
                try
                {
                    using (var client = new BillingClient())
                    {
                        var xelement = client.GetPayments(tenantId);
                        foreach (var x in xelement.Elements("payment"))
                        {
                            var pi = new PaymentInfo();

                            var name = string.Empty;
                            var fname = x.Element("fname");
                            if (fname != null)
                            {
                                name += fname.Value;
                            }
                            var lname = x.Element("lname");
                            if (lname != null)
                            {
                                name += " " + lname.Value;
                            }
                            pi.Name = name.Trim();

                            var email = x.Element("email");
                            if (email != null)
                            {
                                pi.Email = email.Value;
                            }

                            var paymentdate = x.Element("payment-date");
                            if (paymentdate != null)
                            {
                                pi.Date = DateTime.ParseExact(paymentdate.Value, "yyyy-MM-dd HH:mm:ss", null);
                            }

                            var price = x.Element("price");
                            if (price != null)
                            {
                                var separator = CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator;
                                pi.Price = Decimal.Parse(price.Value.Replace(".", separator).Replace(",", separator), NumberStyles.Currency, CultureInfo.InvariantCulture);
                            }

                            var currency = x.Element("payment-currency");
                            if (currency != null)
                            {
                                pi.Currency = currency.Value;
                            }

                            var method = x.Element("payment-method");
                            if (method != null)
                            {
                                pi.Method = method.Value;
                            }

                            var cartid = x.Element("cart-id");
                            if (cartid != null)
                            {
                                pi.CartId = cartid.Value;
                            }

                            payments.Add(pi);
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error(error);
                }

                cache.Insert(key, payments, DateTime.UtcNow.Add(CacheExpiration));
            }

            return payments;
        }

        public Uri GetShoppingUri(int tenant, int plan)
        {
            var key = "billing/urls/" + tenant;
            var urls = cache.Get(key) as IDictionary<int, Uri>;
            if (urls == null)
            {
                urls = new Dictionary<int, Uri>();
            }
            if (!urls.ContainsKey(plan))
            {
                try
                {
                    using (var client = new BillingClient())
                    {
                        var q = quotaService.GetTenantQuota(plan);
                        var avangateId = q.AvangateId;
                        if (!string.IsNullOrEmpty(avangateId))
                        {
                            try
                            {
                                var cached = GetBillingInfo(tenant);
                                if (cached == null || cached.Item1 == plan)
                                {
                                    urls[plan] = new Uri(client.GetPaymentUrl(tenant, avangateId).Value);
                                }
                                else
                                {
                                    urls[plan] = new Uri(client.GetPaymentUpdateUrl(tenant, avangateId).Value);
                                }
                            }
                            catch
                            {
                                // new tariff
                                urls[plan] = new Uri(client.GetPaymentUrl(tenant, avangateId).Value);
                            }
                        }
                        else
                        {
                            urls[plan] = null;
                        }
                    }
                }
                catch (Exception error)
                {
                    log.Error(error);
                    urls[plan] = null;
                }
                cache.Insert(key, urls, DateTime.UtcNow.Add(CacheExpiration));
            }
            return urls[plan];
        }

        public void SetTariffCoupon(int tenant, string coupon)
        {
            var bi = GetBillingInfo(tenant);
            if (bi != null && DateTime.UtcNow <= bi.Item2)
            {
                throw new CouponNotApplicableExcepton();
            }

            var q = new SqlQuery("tenants_tariffcoupon")
                .Select("tariff", "tariff_period", "valid_from", "valid_to")
                .Where("coupon", coupon);

            var couponInfo = ExecList(q)
                .ConvertAll(r => Tuple.Create(
                    Convert.ToInt32(r[0]),
                    Convert.ToInt32(r[1]),
                    Convert.ToDateTime(r[2]),
                    r[3] != null ? Convert.ToDateTime(r[3]) : DateTime.MaxValue))
                .SingleOrDefault();

            if (couponInfo == null)
            {
                throw new CouponNotFoundExcepton();
            }
            if (DateTime.UtcNow < couponInfo.Item3 || couponInfo.Item4 < DateTime.UtcNow)
            {
                throw new CouponExpiredExcepton(couponInfo.Item3, couponInfo.Item4);
            }

            SaveBillingInfo(tenant, Tuple.Create(couponInfo.Item1, DateTime.UtcNow.AddMonths(couponInfo.Item2)));
        }


        private Tuple<int, DateTime> GetBillingInfo(int tenant)
        {
            var q = new SqlQuery("tenants_tariff")
                .Select("tariff", "stamp")
                .Where(Exp.Eq("tenant", tenant) | Exp.Eq("tenant", Tenant.DEFAULT_TENANT))
                .OrderBy("id", false)
                .SetMaxResults(1);

            return ExecList(q)
                .ConvertAll(r => Tuple.Create(Convert.ToInt32(r[0]), (DateTime)r[1]))
                .SingleOrDefault();
        }

        private void SaveBillingInfo(int tenant, Tuple<int, DateTime> bi)
        {
            var oldbi = GetBillingInfo(tenant);
            if (oldbi == null || !Equals(oldbi, bi))
            {
                var i = new SqlInsert("tenants_tariff")
                    .InColumnValue("tenant", tenant)
                    .InColumnValue("tariff", bi.Item1)
                    .InColumnValue("stamp", bi.Item2);
                ExecNonQuery(i);
                cache.Remove("tariff/" + tenant);
            }
        }


        private void CalculateState(int tenantId, Tariff tariff)
        {
            var today = DateTime.UtcNow.Date;
            var tenant = tenantService.GetTenant(tenantId);
            var gracePeriod = GetPeriod("GracePeriod", DEFAULT_GRACE_PERIOD);
            var q = quotaService.GetTenantQuota(tariff.QuotaId);

            if (q.Trial)
            {
                tariff.State = TariffState.Trial;
                tariff.DueDate = tenant.CreatedDateTime.Date.AddDays(GetPeriod("TrialPeriod", DEFAULT_TRIAL_PERIOD));
            }
            if (tariff.DueDate != DateTime.MaxValue)
            {
                tariff.FrozenDate = tariff.DueDate.AddDays(gracePeriod);
            }
            if (tariff.DueDate < today && today <= tariff.FrozenDate)
            {
                tariff.State = TariffState.NotPaid;
            }
            else if (tariff.FrozenDate < today)
            {
                tariff.State = TariffState.Frozen;
            }
        }

        private int GetPeriod(string key, int defaultValue)
        {
            var period = Convert.ToInt32((tenantService.GetTenantSettings(Tenant.DEFAULT_TENANT, key) ?? new byte[0]).FirstOrDefault());
            return period != 0 ? period : defaultValue;
        }
    }
}
