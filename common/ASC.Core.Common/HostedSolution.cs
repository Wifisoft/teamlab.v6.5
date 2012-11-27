using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using ASC.Core.Data;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;

namespace ASC.Core
{
    public class HostedSolution
    {
        private readonly ITenantService tenantService;
        private readonly IUserService userService;


        public HostedSolution(ConnectionStringSettings connectionString)
        {
            tenantService = new DbTenantService(connectionString);
            userService = new DbUserService(connectionString);
        }

        public List<Tenant> FindTenants(string login)
        {
            return FindTenants(login, null);
        }

        public List<Tenant> FindTenants(string login, string password)
        {
            var hash = !string.IsNullOrEmpty(password) ? Hasher.Base64Hash(password, HashAlg.SHA256) : null;
            if (hash != null && userService.GetUser(Tenant.DEFAULT_TENANT, login, hash) == null)
            {
                throw new SecurityException("Invalid login or password.");
            }
            return tenantService.GetTenants(login, hash).ToList();
        }

        public void CheckTenantAddress(string address)
        {
            tenantService.ValidateDomain(address);
        }

        public string RegisterTenant(TenantRegistrationInfo ri, out Tenant tenant)
        {
            tenant = null;

            if (ri == null) throw new ArgumentNullException("registrationInfo");
            if (string.IsNullOrEmpty(ri.Name)) throw new Exception("Community name can not be empty");
            if (string.IsNullOrEmpty(ri.Address)) throw new Exception("Community address can not be empty");

            if (string.IsNullOrEmpty(ri.Email)) throw new Exception("Account email can not be empty");
            if (string.IsNullOrEmpty(ri.FirstName)) throw new Exception("Account firstname can not be empty");
            if (string.IsNullOrEmpty(ri.LastName)) throw new Exception("Account lastname can not be empty");
            if (string.IsNullOrEmpty(ri.Password)) ri.Password = Crypto.GeneratePassword(6);

            // create tenant
            tenant = new Tenant(ri.Address.ToLowerInvariant())
            {
                Name = ri.Name,
                Language = ri.Culture.Name,
                TimeZone = ri.TimeZoneInfo,
            };
            tenant = tenantService.SaveTenant(tenant);

            // create user
            var user = new UserInfo()
            {
                UserName = ri.Email.Substring(0, ri.Email.IndexOf('@')),
                LastName = ri.LastName,
                FirstName = ri.FirstName,
                Email = ri.Email,
            };
            user = userService.SaveUser(tenant.TenantId, user);
            userService.SetUserPassword(tenant.TenantId, user.ID, ri.Password);
            userService.SaveUserGroupRef(tenant.TenantId, new UserGroupRef(user.ID, Constants.GroupAdmin.ID, UserGroupRefType.Contains));

            // save tenant owner
            tenant.OwnerId = user.ID;
            tenant = tenantService.SaveTenant(tenant);

            return CreateAuthenticationCookie(tenant.TenantId, user.ID);
        }

        public string CreateAuthenticationCookie(int tenantId, string login, string password)
        {
            var u = userService.GetUser(tenantId, login, password);
            return CreateAuthenticationCookie(tenantId, u.ID);
        }

        public string CreateAuthenticationCookie(int tenantId, Guid userId)
        {
            return CookieStorage.EncryptCookie(tenantId, userId, null, null);
        }
    }
}