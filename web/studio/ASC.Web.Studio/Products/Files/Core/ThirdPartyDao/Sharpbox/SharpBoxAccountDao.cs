using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Caching;
using ASC.Collections;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Security.Cryptography;
using ASC.Web.Files.Import;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.GoogleDocs;

namespace ASC.Files.Core.ThirdPartyDao
{
    internal class CachedSharpBoxAccountDao : SharpBoxAccountDao
    {
        private static readonly CachedDictionary<IProviderInfo> ProviderCache =
            new CachedDictionary<IProviderInfo>("shrpbox-providers", Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20), (x) => true);

        private readonly string _rootKey = string.Empty;

        public CachedSharpBoxAccountDao(int tenantID, string storageKey)
            : base(tenantID, storageKey)
        {
            _rootKey = tenantID.ToString(CultureInfo.InvariantCulture);
        }

        public override IProviderInfo GetProviderInfo(int linkId)
        {
            return ProviderCache.Get(_rootKey, linkId.ToString(CultureInfo.InvariantCulture), () => GetProviderInfoBase(linkId));
        }

        private IProviderInfo GetProviderInfoBase(int linkId)
        {
            return base.GetProviderInfo(linkId);
        }

        public override void RemoveProviderInfo(int linkId)
        {
            ProviderCache.Reset(_rootKey, linkId.ToString(CultureInfo.InvariantCulture));
            base.RemoveProviderInfo(linkId);
        }

        public override int UpdateProviderInfo(int linkId, string customerTitle)
        {
            ProviderCache.Reset(_rootKey, linkId.ToString(CultureInfo.InvariantCulture));
            return base.UpdateProviderInfo(linkId, customerTitle);
        }

        public override int UpdateProviderInfo(int linkId, string customerTitle, AuthData authData, FolderType folderType)
        {
            ProviderCache.Reset(_rootKey, linkId.ToString(CultureInfo.InvariantCulture));
            return base.UpdateProviderInfo(linkId, customerTitle, authData, folderType);
        }
    }

    internal class SharpBoxAccountDao : IProviderDao
    {
        private const string TableTitle = "files_thirdparty_account";

        protected DbManager DbManager { get; private set; }

        protected int TenantID { get; private set; }

        public SharpBoxAccountDao(int tenantID, String storageKey)
        {
            TenantID = tenantID;
            DbManager = new DbManager(storageKey);
        }

        public virtual IProviderInfo GetProviderInfo(int linkId)
        {
            return GetProviderInfoInernal(linkId);
        }

        private IProviderInfo GetProviderInfoInernal(int linkId)
        {
            var querySelect = new SqlQuery(TableTitle)
                        .Select("id", "provider", "customer_title", "user_name", "password", "token", "user_id", "folder_type",
                                "create_on")
                        .Where("id", linkId)
                        .Where("tenant_id", TenantID);

            //     .Where(Exp.Eq("user_id", SecurityContext.CurrentAccount.ID.ToString()) | Exp.Eq("folder_type", (int) FolderType.COMMON));

            return DbManager.ExecuteList(querySelect).ConvertAll<IProviderInfo>(ToProviderInfo).Single();
        }

        public virtual List<IProviderInfo> GetProvidersInfo(FolderType folderType)
        {
            return GetProvidersInfoInternal(folderType);
        }

        private List<IProviderInfo> GetProvidersInfoInternal(FolderType folderType)
        {
            var querySelect = new SqlQuery(TableTitle)
                .Select("id", "provider", "customer_title", "user_name", "password", "token", "user_id", "folder_type", "create_on")
                .Where("tenant_id", TenantID)
                .Where("folder_type", (int) folderType);

            if (folderType == FolderType.USER)
                querySelect = querySelect.Where(Exp.Eq("user_id", SecurityContext.CurrentAccount.ID.ToString()));

            return DbManager.ExecuteList(querySelect).ConvertAll<IProviderInfo>(ToProviderInfo);
        }

        public virtual int SaveProviderInfo(string providerName, string customerTitle, AuthData authData, FolderType folderType)
        {
            var prName = (nSupportedCloudConfigurations) Enum.Parse(typeof (nSupportedCloudConfigurations), providerName, true);

            //for google docs
            authData = GetEncodedAccesToken(authData, providerName);

            if (!CheckProviderInfo(ToProviderInfo(0, providerName, customerTitle, authData, SecurityContext.CurrentAccount.ID.ToString(), folderType, TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))))
                throw new UnauthorizedAccessException("Can't authorize at " + providerName + " provider with given credentials");

            var queryInsert = new SqlInsert(TableTitle, true)
                .InColumnValue("id", 0)
                .InColumnValue("tenant_id", TenantID)
                .InColumnValue("provider", prName.ToString())
                .InColumnValue("customer_title", customerTitle)
                .InColumnValue("user_name", authData.Login)
                .InColumnValue("password", EncryptPassword(authData.Password))
                .InColumnValue("folder_type", (int) folderType)
                .InColumnValue("create_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                .InColumnValue("user_id", SecurityContext.CurrentAccount.ID.ToString())
                .InColumnValue("token", authData.Token)
                .Identity(0, 0, true);

            return Int32.Parse(DbManager.ExecuteScalar<string>(queryInsert));
        }

        public bool CheckProviderInfo(IProviderInfo providerInfo)
        {
            return providerInfo.CheckAccess();
        }

        public virtual int UpdateProviderInfo(int linkId, string customerTitle, AuthData authData, FolderType folderType)
        {
            var oldprovider = GetProviderInfoInernal(linkId);

            //for google docs
            authData = GetEncodedAccesToken(authData, oldprovider.ProviderName);

            if (!CheckProviderInfo(ToProviderInfo(0, oldprovider.ProviderName, customerTitle, authData, SecurityContext.CurrentAccount.ID.ToString(), folderType, TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))))
                throw new UnauthorizedAccessException("Can't authorize at " + oldprovider.ProviderName + " provider with given credentials");

            var queryUpdate = new SqlUpdate(TableTitle)
                .Set("customer_title", customerTitle)
                .Set("user_name", authData.Login)
                .Set("password", EncryptPassword(authData.Password))
                .Set("folder_type", (int) folderType)
                .Set("create_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                .Set("user_id", SecurityContext.CurrentAccount.ID.ToString())
                .Set("token", authData.Token)
                .Where("id", linkId)
                .Where("tenant_id", TenantID);

            return DbManager.ExecuteNonQuery(queryUpdate) == 1 ? linkId : default(int);
        }

        public virtual int UpdateProviderInfo(int linkId, string customerTitle)
        {
            var queryUpdate = new SqlUpdate(TableTitle)
                .Set("customer_title", customerTitle)
                .Where("id", linkId)
                .Where("tenant_id", TenantID);

            return DbManager.ExecuteNonQuery(queryUpdate) == 1 ? linkId : default(int);
        }

        public virtual void RemoveProviderInfo(int linkId)
        {
            var queryDelete = new SqlDelete(TableTitle)
                .Where("id", linkId)
                .Where("tenant_id", TenantID);

            DbManager.ExecuteNonQuery(queryDelete);
        }

        private static IProviderInfo ToProviderInfo(int id, string providerName, string customerTitle, AuthData authData, string owner, FolderType type, DateTime createOn)
        {
            return ToProviderInfo(new object[] {id, providerName, customerTitle, authData.Login, EncryptPassword(authData.Password), authData.Token, owner, (int) type, createOn});
        }

        private static IProviderInfo ToProviderInfo(object[] input)
        {
            return new SharpBoxProviderInfo(
                Convert.ToInt32(input[0]),
                input[1] as string,
                input[2] as string,
                new AuthData(input[3] as string, DecryptPassword(input[4] as string), input[5] as string),
                input[6] == null ? Guid.Empty : new Guid(input[6] as string),
                (FolderType) Convert.ToInt32(input[7]),
                TenantUtil.DateTimeFromUtc(Convert.ToDateTime(input[8])));
        }

        public void Dispose()
        {
            DbManager.Dispose();
        }

        private static AuthData GetEncodedAccesToken(AuthData authData, string providerName)
        {
            var prName = (nSupportedCloudConfigurations) Enum.Parse(typeof (nSupportedCloudConfigurations), providerName, true);
            if (prName != nSupportedCloudConfigurations.Google)
                return authData;

            var tokenSecret = ImportConfiguration.GoogleTokenManager.GetTokenSecret(authData.Token);
            var consumerKey = ImportConfiguration.GoogleTokenManager.ConsumerKey;
            var consumerSecret = ImportConfiguration.GoogleTokenManager.ConsumerSecret;

            var accessToken = GoogleDocsAuthorizationHelper.BuildToken(authData.Token, tokenSecret, consumerKey, consumerSecret);
            var storage = new CloudStorage();

            authData.Token = storage.SerializeSecurityTokenToBase64Ex(accessToken, typeof (GoogleDocsConfiguration), null);
            return authData;
        }

        private static string EncryptPassword(string password)
        {
            return string.IsNullOrEmpty(password) ? string.Empty : InstanceCrypto.Encrypt(password);
        }

        private static string DecryptPassword(string password)
        {
            return string.IsNullOrEmpty(password) ? string.Empty : InstanceCrypto.Decrypt(password);
        }
    }
}