using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;

namespace ASC.Core.Data
{
    public class DbTenantService : DbBaseService, ITenantService
    {
        private readonly Regex validDomain = new Regex("^[a-z0-9]([a-z0-9-]){1,98}[a-z0-9]$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private List<string> forbiddenDomains;



        public DbTenantService(ConnectionStringSettings connectionString)
            : base(connectionString, null)
        {
        }


        public void ValidateDomain(string domain)
        {
            ExecAction(db => ValidateDomain(db, domain, Tenant.DEFAULT_TENANT, true));
        }

        public IEnumerable<Tenant> GetTenants(DateTime from)
        {
            return GetTenants(from != default(DateTime) ? Exp.Ge("last_modified", from) : Exp.Empty);
        }

        public IEnumerable<Tenant> GetTenants(string login, string passwordHash)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException("login");

            var q = TenantsQuery(Exp.Empty)
                .InnerJoin("core_user u", Exp.EqColumns("t.id", "u.tenant"))
                .InnerJoin("core_usersecurity s", Exp.EqColumns("u.id", "s.userid"))
                .Where("t.status", (int)TenantStatus.Active)
                .Where(login.Contains('@') ? "u.email" : "u.id", login)
                .Where("u.removed", false);
            if (passwordHash != null)
            {
                q.Where("s.pwdhash", passwordHash);
            }
            return ExecList(q).ConvertAll(r => ToTenant(r));
        }

        public Tenant GetTenant(int id)
        {
            return GetTenants(Exp.Eq("id", id))
                .SingleOrDefault();
        }

        public Tenant GetTenant(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException("domain");

            return GetTenants(Exp.Eq("alias", domain.ToLowerInvariant()) | Exp.Eq("mappeddomain", domain.ToLowerInvariant()))
                .FirstOrDefault();
        }

        public Tenant SaveTenant(Tenant t)
        {
            if (t == null) throw new ArgumentNullException("tenant");

            ExecAction(db =>
            {
                t.LastModified = DateTime.UtcNow;

                if (t.TenantId != 0)
                {
                    // TenantId == 0 in open source version
                    ValidateDomain(db, t.TenantAlias, t.TenantId, true);
                }
                if (!string.IsNullOrEmpty(t.MappedDomain))
                {
                    var baseUrl = ConfigurationManager.AppSettings["core.base-domain"];
                    if (baseUrl != null && t.MappedDomain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ValidateDomain(db, t.MappedDomain.Substring(0, t.MappedDomain.Length - baseUrl.Length - 1), t.TenantId, false);
                    }
                    else
                    {
                        ValidateDomain(db, t.MappedDomain, t.TenantId, false);
                    }
                }

                if (t.TenantId == Tenant.DEFAULT_TENANT)
                {
                    var i = new SqlInsert("tenants_tenants", true)
                        .InColumnValue("id", t.TenantId)
                        .InColumnValue("alias", t.TenantAlias.ToLowerInvariant())
                        .InColumnValue("mappeddomain", !string.IsNullOrEmpty(t.MappedDomain) ? t.MappedDomain.ToLowerInvariant() : null)
                        .InColumnValue("name", t.Name ?? t.TenantAlias)
                        .InColumnValue("language", t.Language)
                        .InColumnValue("timezone", t.TimeZone.Id)
                        .InColumnValue("owner_id", t.OwnerId.ToString())
                        .InColumnValue("trusteddomains", t.GetTrustedDomains())
                        .InColumnValue("trusteddomainsenabled", (int)t.TrustedDomainsType)
                        .InColumnValue("creationdatetime", t.CreatedDateTime)
                        .InColumnValue("status", (int)t.Status)
                        .InColumnValue("statuschanged", t.StatusChangeDate)
                        .InColumnValue("last_modified", t.LastModified)
                        .Identity<int>(0, t.TenantId, true);


                    t.TenantId = db.ExecScalar<int>(i);
                    t.Version = db.ExecScalar<int>(new SqlQuery("tenants_tenants").Select("version").Where("id", t.TenantId));
                }
                else
                {
                    var u = new SqlUpdate("tenants_tenants")
                        .Set("alias", t.TenantAlias.ToLowerInvariant())
                        .Set("mappeddomain", !string.IsNullOrEmpty(t.MappedDomain) ? t.MappedDomain.ToLowerInvariant() : null)
                        .Set("version", t.Version)
                        .Set("name", t.Name ?? t.TenantAlias)
                        .Set("language", t.Language)
                        .Set("timezone", t.TimeZone.Id)
                        .Set("owner_id", t.OwnerId.ToString())
                        .Set("trusteddomains", t.GetTrustedDomains())
                        .Set("trusteddomainsenabled", (int)t.TrustedDomainsType)
                        .Set("creationdatetime", t.CreatedDateTime)
                        .Set("status", (int)t.Status)
                        .Set("statuschanged", t.StatusChangeDate)
                        .Set("last_modified", t.LastModified)
                        .Where("id", t.TenantId);

                    db.ExecNonQuery(u);
                }
            });
            CalculateTenantDomain(t);
            return t;
        }

        public void RemoveTenant(int id)
        {
            var d = new SqlDelete("tenants_tenants").Where("id", id);
            ExecNonQuery(d);
        }

        public IEnumerable<TenantVersion> GetTenantVersions()
        {
            var q = new SqlQuery("tenants_version")
                .Select("id", "version")
                .Where("visible", true);
            return ExecList(q)
                .ConvertAll(r => new TenantVersion(Convert.ToInt32(r[0]), (string)r[1]));
        }


        public byte[] GetTenantSettings(int tenant, string key)
        {
            return ExecScalar<byte[]>(new SqlQuery("core_settings").Select("value").Where("tenant", tenant).Where("id", key));
        }

        public void SetTenantSettings(int tenant, string key, byte[] data)
        {
            var i = data == null || data.Length == 0 ?
                (ISqlInstruction)new SqlDelete("core_settings").Where("tenant", tenant).Where("id", key) :
                (ISqlInstruction)new SqlInsert("core_settings", true).InColumns("tenant", "id", "value").Values(tenant, key, data);
            ExecNonQuery(i);
        }


        private IEnumerable<Tenant> GetTenants(Exp where)
        {
            var q = TenantsQuery(where);
            return ExecList(q).ConvertAll(r => ToTenant(r));
        }

        private SqlQuery TenantsQuery(Exp where)
        {
            return new SqlQuery("tenants_tenants t")
                .Select("t.id", "t.alias", "t.mappeddomain", "t.version", "t.name", "t.language", "t.timezone", "t.owner_id", "t.trusteddomains", "t.trusteddomainsenabled")
                .Select("t.creationdatetime", "t.status", "t.statuschanged", "t.last_modified")
                .Where(where);
        }

        private Tenant ToTenant(object[] r)
        {
            var tenant = new Tenant(Convert.ToInt32(r[0]), (string)r[1])
            {
                MappedDomain = (string)r[2],
                Version = Convert.ToInt32(r[3]),
                Name = (string)r[4],
                Language = (string)r[5],
                TimeZone = GetTimeZone((string)r[6]),
                OwnerId = r[7] != null ? new Guid((string)r[7]) : Guid.Empty,
                TrustedDomainsType = (TenantTrustedDomainsType)Convert.ToInt32(r[9]),
                CreatedDateTime = (DateTime)r[10],
                Status = (TenantStatus)Convert.ToInt32(r[11]),
                StatusChangeDate = r[12] != null ? (DateTime)r[12] : default(DateTime),
                LastModified = (DateTime)r[13],
            };
            tenant.SetTrustedDomains((string)r[8]);
            CalculateTenantDomain(tenant);

            return tenant;
        }

        private void CalculateTenantDomain(Tenant tenant)
        {
            var baseHost = ConfigurationManager.AppSettings["core.base-domain"];
            if (baseHost == "localhost" || tenant.TenantAlias == "localhost")
            {
                //single tenant on local host
                tenant.TenantAlias = "localhost";
                tenant.TenantDomain = Dns.GetHostName().ToLowerInvariant();
            }
            else
            {
                tenant.TenantDomain = string.Format("{0}.{1}", tenant.TenantAlias, baseHost).TrimEnd('.').ToLowerInvariant();
            }
            if (!string.IsNullOrEmpty(tenant.MappedDomain))
            {
                if (tenant.MappedDomain.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
                {
                    tenant.MappedDomain = tenant.MappedDomain.Substring(7);
                }
                if (tenant.MappedDomain.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                {
                    tenant.MappedDomain = tenant.MappedDomain.Substring(8);
                }
                tenant.TenantDomain = tenant.MappedDomain.ToLowerInvariant();
            }
        }

        private TimeZoneInfo GetTimeZone(string zoneId)
        {
            if (!string.IsNullOrEmpty(zoneId))
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                }
                catch (TimeZoneNotFoundException) { }
            }
            return TimeZoneInfo.Utc;
        }

        private void ValidateDomain(IDbExecuter db, string domain, int tenantId, bool validateCharacters)
        {
            // size
            if (string.IsNullOrEmpty(domain))
            {
                throw new TenantTooShortException("Tenant domain can not be empty.");
            }
            if (domain.Length < 6 || 50 <= domain.Length)
            {
                throw new TenantTooShortException("Length of domain must be greater than or equal to 6 and less than or equal to 50.");
            }

            // characters
            if (validateCharacters && !validDomain.IsMatch(domain))
            {
                throw new TenantIncorrectCharsException("Domain contains invalid characters.");
            }

            // forbidden or exists
            var exists = false;
            domain = domain.ToLowerInvariant();
            if (!exists)
            {
                if (forbiddenDomains == null)
                {
                    forbiddenDomains = ExecList(new SqlQuery("tenants_forbiden").Select("address")).Select(r => (string)r[0]).ToList();
                }
                exists = tenantId != 0 && forbiddenDomains.Contains(domain);
            }
            if (!exists)
            {
                exists = 0 < db.ExecScalar<int>(new SqlQuery("tenants_tenants").SelectCount().Where(Exp.Eq("alias", domain) & !Exp.Eq("id", tenantId)));
            }
            if (!exists)
            {
                exists = 0 < db.ExecScalar<int>(new SqlQuery("tenants_tenants").SelectCount().Where(Exp.Eq("mappeddomain", domain) & !Exp.Eq("id", tenantId)));
            }
            if (exists)
            {
                // cut number suffix
                while (true)
                {
                    if (6 < domain.Length && char.IsNumber(domain, domain.Length - 1))
                    {
                        domain = domain.Substring(0, domain.Length - 1);
                    }
                    else
                    {
                        break;
                    }
                }

                var q = new SqlQuery("tenants_forbiden").Select("address").Where(Exp.Like("address", domain, SqlLike.StartWith))
                    .Union(new SqlQuery("tenants_tenants").Select("alias").Where(Exp.Like("alias", domain, SqlLike.StartWith) & !Exp.Eq("id", tenantId)))
                    .Union(new SqlQuery("tenants_tenants").Select("mappeddomain").Where(Exp.Like("mappeddomain", domain, SqlLike.StartWith) & !Exp.Eq("id", tenantId)));

                var existsTenants = db.ExecList(q).ConvertAll(r => (string)r[0]);
                throw new TenantAlreadyExistsException("Address busy.", existsTenants);
            }
        }
    }
}
