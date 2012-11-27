#region Import

using System;
using System.Collections.Generic;
using ASC.Collections;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.CRM.Core.Entities;
using ASC.Core.Tenants;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace ASC.CRM.Core.Dao
{
    public class CachedContactInfo : ContactInfoDao
    {
        private readonly HttpRequestDictionary<ContactInfo> _contactInfoCache = new HttpRequestDictionary<ContactInfo>("crm_contact_info");

        public CachedContactInfo(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {

        }

        public override ContactInfo GetByID(int id)
        {
            return _contactInfoCache.Get(id.ToString(), () => GetByIDBase(id));
        }

        public override void Delete(int id)
        {

            ResetCache(id);

            base.Delete(id);
        }

        private ContactInfo GetByIDBase(int id)
        {
            return base.GetByID(id);
        }

        private void ResetCache(int id)
        {
            _contactInfoCache.Reset(id.ToString());
        }

        public override void DeleteByContact(int contactID)
        {
            _contactInfoCache.Clear();
            
            base.DeleteByContact(contactID);
        }

        public override int Update(ContactInfo contactInfo)
        {
            ResetCache(contactInfo.ID);

           return  base.Update(contactInfo);
        }

    }

    public class ContactInfoDao : AbstractDao
    {
        #region Constructor

        public ContactInfoDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {
        }

        #endregion

        public virtual ContactInfo GetByID(int id)
        {
            var sqlResult = DbManager.ExecuteList(GetSqlQuery(Exp.Eq("id", id))).ConvertAll(row => ToContactInfo(row));

            if (sqlResult.Count == 0) return null;

            return sqlResult[0];
        }

        public virtual void Delete(int id)
        {
            DbManager.ExecuteNonQuery(Delete("crm_contact_info").Where(Exp.Eq("id", id)));

        }

        public virtual void DeleteByContact(int contactID)
        {
            if (contactID <= 0) return;

            DbManager.ExecuteNonQuery(Delete("crm_contact_info").Where(Exp.Eq("contact_id", contactID)));

        }

        public virtual int Update(ContactInfo contactInfo)
        {

            if (contactInfo == null || contactInfo.ID == 0 || contactInfo.ContactID == 0)
                throw new ArgumentException();

            DbManager.ExecuteNonQuery(Update("crm_contact_info")
                                               .Where("id", contactInfo.ID)
                                               .Set("data", contactInfo.Data)
                                               .Set("category", contactInfo.Category)
                                               .Set("is_primary", contactInfo.IsPrimary)
                                               .Set("contact_id", contactInfo.ContactID)
                                               .Set("type", (int)contactInfo.InfoType)
                                               .Set("last_modifed_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                                               .Set("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                                                );

            return contactInfo.ID;
        }
      

        public int Save(ContactInfo contactInfo)
        {

            return DbManager.ExecuteScalar<int>(Insert("crm_contact_info")
                                               .InColumnValue("id", 0)
                                               .InColumnValue("data", contactInfo.Data)
                                               .InColumnValue("category", contactInfo.Category)
                                               .InColumnValue("is_primary", contactInfo.IsPrimary)
                                               .InColumnValue("contact_id", contactInfo.ContactID)
                                               .InColumnValue("type", (int)contactInfo.InfoType)
                                               .InColumnValue("last_modifed_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                                               .InColumnValue("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                                               .Identity(1, 0, true));
        }

        public List<String> GetListData(int contactID, ContactInfoType infoType)
        {
            return GetList(contactID, infoType, null, null).ConvertAll(item => item.Data);

        }

        public List<ContactInfo> GetAll()
        {
            return GetList(0, null, null, null);
        }

        public List<ContactInfo> GetAll(int[] contactID)
        {

            if (contactID == null || contactID.Length == 0) return null;

            SqlQuery sqlQuery = GetSqlQuery(null);

            sqlQuery.Where(Exp.In("contact_id", contactID));

            return DbManager.ExecuteList(sqlQuery).ConvertAll(row => ToContactInfo(row));
        }

        public virtual List<ContactInfo> GetList(int contactID, ContactInfoType? infoType, int? categoryID, bool? isPrimary)
        {
            SqlQuery sqlQuery = GetSqlQuery(null);

            if (contactID > 0)
                sqlQuery.Where(Exp.Eq("contact_id", contactID));

            if (infoType.HasValue)
                sqlQuery.Where(Exp.Eq("type", infoType.Value));

            if (categoryID.HasValue)
                sqlQuery.Where(Exp.Eq("category", categoryID.Value));

            if (isPrimary.HasValue)
                sqlQuery.Where(Exp.Eq("is_primary", isPrimary.Value));

            sqlQuery.OrderBy("type", true);
           // sqlQuery.OrderBy("category", true);
          //  sqlQuery.OrderBy("is_primary", true);


            return DbManager.ExecuteList(sqlQuery).ConvertAll(row => ToContactInfo(row));
        }
        

        public int[] UpdateList(List<ContactInfo> items)
        {

            if (items == null || items.Count == 0) return null;

            var result = new List<int>();

            using (var tx = DbManager.BeginTransaction(true))
            {
                foreach (var contactInfo in items)
                    result.Add(Update(contactInfo));


                tx.Commit();
            }

            return result.ToArray();
        }




        public int[] SaveList(List<ContactInfo> items)
        {
            if (items == null || items.Count == 0) return null;

            var result = new List<int>();

            using (var tx = DbManager.BeginTransaction(true))
            {
               foreach (var contactInfo in items)
                 result.Add(Save(contactInfo));
                

                tx.Commit();
            }

            return result.ToArray();
        }

        protected static ContactInfo ToContactInfo(object[] row)
        {
            return new ContactInfo
                       {
                           ID = Convert.ToInt32(row[0]),
                           Category = Convert.ToInt32(row[1]),
                           Data = row[2].ToString(),
                           InfoType = (ContactInfoType)Convert.ToInt32(row[3]),
                           IsPrimary = Convert.ToBoolean(row[4]),
                           ContactID = Convert.ToInt32(row[5])
                       };
        }

        private SqlQuery GetSqlQuery(Exp where)
        {
            var sqlQuery = Query("crm_contact_info")
                .Select("id",
                        "category",
                        "data",
                        "type",
                        "is_primary",
                        "contact_id");

            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;

        }
    }
}