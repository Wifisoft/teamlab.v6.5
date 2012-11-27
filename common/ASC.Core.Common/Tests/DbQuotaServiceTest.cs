#if DEBUG
namespace ASC.Core.Common.Tests
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using ASC.Common.Data.Sql;
    using ASC.Core.Data;
    using ASC.Core.Tenants;
    using NUnit.Framework;

    [TestFixture]
    public class DbQuotaServiceTest : DbBaseTest<DbQuotaService>
    {
        [SetUp]
        public void ClearData()
        {
            Service.RemoveTenantQuota(Tenant);
            foreach (var row in Service.FindTenantQuotaRows(new TenantQuotaRowQuery(Tenant) { Path = "path" }))
            {
                DeleteQuotaRow(row);
            }
        }

        [Test]
        public void QuotaMethod()
        {
            var quota1 = new TenantQuota(Tenant)
            {
                MaxFileSize = 3,
                MaxTotalSize = 4,
                ActiveUsers = 30,
            };
            Service.SaveTenantQuota(quota1);
            CompareQuotas(quota1, Service.GetTenantQuota(quota1.Id));

            Service.RemoveTenantQuota(Tenant);
            Assert.IsNull(Service.GetTenantQuota(quota1.Id));

            var row = new TenantQuotaRow { Tenant = this.Tenant, Path = "path", Counter = 1000, Tag = "tag" };
            Service.SetTenantQuotaRow(row, false);

            var rows = Service.FindTenantQuotaRows(new TenantQuotaRowQuery(Tenant).WithPath("path")).ToList();
            CompareQuotaRows(row, rows.Find(r => r.Tenant == row.Tenant && r.Tag == row.Tag));

            Service.SetTenantQuotaRow(row, true);
            row.Counter += 1000;
            rows = Service.FindTenantQuotaRows(new TenantQuotaRowQuery(Tenant).WithPath("path")).ToList();
            CompareQuotaRows(row, rows.Find(r => r.Tenant == row.Tenant && r.Tag == row.Tag));

            DeleteQuotaRow(row);
        }

        private void CompareQuotas(TenantQuota q1, TenantQuota q2)
        {
            Assert.AreEqual(q1.Id, q2.Id);
            Assert.AreEqual(q1.Name, q2.Name);
            Assert.AreEqual(q1.Desc, q2.Desc);
            Assert.AreEqual(q1.MaxFileSize, q2.MaxFileSize);
            Assert.AreEqual(q1.MaxTotalSize, q2.MaxTotalSize);
            Assert.AreEqual(q1.ActiveUsers, q2.ActiveUsers);
            Assert.AreEqual(q1.Features, q2.Features);
            Assert.AreEqual(q1.Price, q2.Price);
            Assert.AreEqual(q1.AvangateId, q2.AvangateId);
        }

        private void CompareQuotaRows(TenantQuotaRow r1, TenantQuotaRow r2)
        {
            Assert.AreEqual(r1.Path, r2.Path);
            Assert.AreEqual(r1.Tag, r2.Tag);
            Assert.AreEqual(r1.Tenant, r2.Tenant);
            Assert.AreEqual(r1.Counter, r2.Counter);
        }

        private void DeleteQuotaRow(TenantQuotaRow row)
        {
            var d = new SqlDelete(DbQuotaService.tenants_quotarow).Where("tenant", row.Tenant).Where("path", row.Path);
            new DbExecuter(ConfigurationManager.ConnectionStrings["core"]).ExecNonQuery(d);
        }
    }
}
#endif
