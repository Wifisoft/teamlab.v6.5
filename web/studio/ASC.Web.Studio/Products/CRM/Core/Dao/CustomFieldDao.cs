#region Import

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ASC.Core.Tenants;
using AjaxPro;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.CRM.Core.Entities;
using Newtonsoft.Json;

#endregion

namespace ASC.CRM.Core.Dao
{
    public class CustomFieldDao : AbstractDao
    {
        public CustomFieldDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {

        }

        public void SaveList(List<CustomField> items)
        {
            if (items == null || items.Count == 0) return;

            using (var tx = DbManager.BeginTransaction(true))
            {
                foreach (var customField in items)
                {
                    SetFieldValue(customField.EntityType, customField.EntityID, customField.ID, customField.Value);
                }

                tx.Commit();
            }

        }


        public void SetFieldValue(EntityType entityType, int entityID, int fieldID, String fieldValue)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            fieldValue = fieldValue.Trim();

            if (String.IsNullOrEmpty(fieldValue))
                DbManager.ExecuteNonQuery(Delete("crm_field_value").Where(Exp.Eq("entity_id", entityID) & Exp.Eq("field_id", fieldID)));
            else
                DbManager.ExecuteNonQuery(
                        Insert("crm_field_value")
                        .InColumnValue("entity_id", entityID)
                        .InColumnValue("value", fieldValue)
                        .InColumnValue("field_id", fieldID)
                        .InColumnValue("entity_type", (int)entityType)
                        .InColumnValue("last_modifed_on", TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))
                        .InColumnValue("last_modifed_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                        );
        }

        public int CreateField(EntityType entityType, String label, CustomFieldType customFieldType, String mask)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();


            var sortOrder = DbManager.ExecuteScalar<int>(Query("crm_field_description").SelectMax("sort_order")) + 1;

            return DbManager.ExecuteScalar<int>(
                                              Insert("crm_field_description")
                                              .InColumnValue("id", 0)
                                              .InColumnValue("label", label)
                                              .InColumnValue("type", (int)customFieldType)
                                              .InColumnValue("mask", mask)
                                              .InColumnValue("sort_order", sortOrder)
                                              .InColumnValue("entity_type", (int)entityType)
                                              .Identity(1, 0, true));
        }

        public String GetValue(EntityType entityType, int entityID, int fieldID)
        {
            var sqlQuery = Query("crm_field_value")
                          .Select("value")
                          .Where(Exp.Eq("field_id", fieldID)
                                 & BuildEntityTypeConditions(entityType, "entity_type")
                                 & Exp.Eq("entity_id", entityID));

            return DbManager.ExecuteScalar<String>(sqlQuery);
        }

        public bool IsExist(int id)
        {
            var result = DbManager.ExecuteList(GetFieldDescriptionSqlQuery(Exp.Eq("id", id)));

            if (result.Count == 0) return false;

            return true;
        }

        public void EditItem(CustomField customField)
        {

            if (HaveRelativeLink(customField.ID))
                throw new ArgumentException();

            DbManager.ExecuteNonQuery(
                Update("crm_field_description")
                .Set("label", customField.Label)
                .Set("type", (int)customField.FieldType)
                .Set("mask", customField.Mask)
                .Where(Exp.Eq("id", customField.ID)));
        }

        public void ReorderFields(int[] fieldID)
        {

            for (int index = 0; index < fieldID.Length; index++)
                DbManager.ExecuteNonQuery(Update("crm_field_description")
                                         .Set("sort_order", index)
                                         .Where(Exp.Eq("id", fieldID[index])));

        }

        private bool HaveRelativeLink(int fieldID)
        {

            return
                DbManager.ExecuteScalar<int>(
                    Query("crm_field_value").Where(Exp.Eq("field_id", fieldID)).SelectCount()) > 0;


        }

        public String GetContactLinkCountJSON(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            var sqlQuery = Query("crm_field_description tblFD")
                .Select("count(tblFV.field_id)")
                .LeftOuterJoin("crm_field_value tblFV", Exp.EqColumns("tblFD.id", "tblFV.field_id"))
                .OrderBy("tblFD.sort_order", true)
                .GroupBy("tblFD.id");

            sqlQuery.Where(BuildEntityTypeConditions(entityType, "tblFD.entity_type"));

            var queryResult = DbManager.ExecuteList(sqlQuery);

            return JavaScriptSerializer.Serialize(queryResult.ConvertAll(row => row[0]));

        }

        public List<CustomField> GetEnityFields(EntityType entityType, int entityID, bool includeEmptyFields)
        {
            if (entityID == 0)
                return GetEnityFields(entityType, null, includeEmptyFields);

            return GetEnityFields(entityType, new[] { entityID }, includeEmptyFields);

        }

        public List<CustomField> GetEnityFields(EntityType entityType, int[] entityID)
        {
            return GetEnityFields(entityType, entityID, false);
        }

        private List<CustomField> GetEnityFields(EntityType entityType, int[] entityID, bool includeEmptyFields)
        {
            // TODO: Refactoring Query!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            SqlQuery sqlQuery = Query("crm_field_description tbl_field")
                .Select("tbl_field.id",
                        "tbl_field_value.entity_id",
                        "tbl_field.label",
                        "tbl_field_value.value",
                        "tbl_field.type",
                        "tbl_field.sort_order",
                        "tbl_field.mask");

            sqlQuery.Where(BuildEntityTypeConditions(entityType, "tbl_field.entity_type"));

            if (entityID != null && entityID.Length > 0)
                sqlQuery.LeftOuterJoin("crm_field_value tbl_field_value",
                                   Exp.EqColumns("tbl_field_value.field_id", "tbl_field.id") &
                                   Exp.In("tbl_field_value.entity_id", entityID))
                .OrderBy("tbl_field.sort_order", true);
            else
                sqlQuery.LeftOuterJoin("crm_field_value tbl_field_value",
                                      Exp.EqColumns("tbl_field_value.field_id", "tbl_field.id"))
               .Where(Exp.Eq("tbl_field_value.tenant_id", TenantID))
               .OrderBy("tbl_field_value.entity_id", true)
               .OrderBy("tbl_field.sort_order", true);

            if (!includeEmptyFields)
                return DbManager.ExecuteList(sqlQuery)
                        .ConvertAll(row => ToCustomField(row)).FindAll(item =>
                        {
                            if (item.FieldType == CustomFieldType.Heading)
                                return true;

                            return !String.IsNullOrEmpty(item.Value.Trim());

                        }).ToList();

            return DbManager.ExecuteList(sqlQuery)
                   .ConvertAll(row => ToCustomField(row));

        }

        public CustomField GetFieldDescription(int fieldID)
        {

            var sqlQuery = GetFieldDescriptionSqlQuery(null);

            sqlQuery.Where(Exp.Eq("id", fieldID));

            var fields = DbManager.ExecuteList(sqlQuery)
                        .ConvertAll(row => ToCustomField(row));

            return fields.Count == 0 ? null : fields[0];
        }

        public List<CustomField> GetFieldsDescription(EntityType entityType)
        {
            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            SqlQuery sqlQuery = GetFieldDescriptionSqlQuery(null);

            sqlQuery.Where(BuildEntityTypeConditions(entityType, "entity_type"));

            return DbManager.ExecuteList(sqlQuery)
                  .ConvertAll(row => ToCustomField(row));
        }

        private SqlQuery GetFieldDescriptionSqlQuery(Exp where)
        {
            var sqlQuery =  Query("crm_field_description")
                .Select("id",
                        "-1",
                        "label",
                        "\" \"",
                        "type",
                        "sort_order",
                        "mask")
                .OrderBy("sort_order", true);

            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;
        }

        private Exp BuildEntityTypeConditions(EntityType entityType, String dbFieldName)
        {
            switch (entityType)
            {
                case EntityType.Company:
                case EntityType.Person:
                    return Exp.In(dbFieldName, new[] { (int)entityType, (int)EntityType.Contact });

                default:
                    return Exp.Eq(dbFieldName, (int)entityType);

            }

        }

        public void DeleteField(int fieldID)
        {
            if (HaveRelativeLink(fieldID))
                throw new ArgumentException();

            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(Delete("crm_field_description").Where(Exp.Eq("id", fieldID)));
                DbManager.ExecuteNonQuery(Delete("crm_field_value").Where(Exp.Eq("field_id", fieldID)));

                tx.Commit();
            }
        }

        public static CustomField ToCustomField(object[] row)
        {
            return new CustomField
            {
                ID = Convert.ToInt32(row[0]),
                EntityID = Convert.ToInt32(row[1]),
                Label = Convert.ToString(row[2]),
                Value = Convert.ToString(row[3]),
                FieldType = (CustomFieldType)Convert.ToInt32(row[4]),
                Position = Convert.ToInt32(row[5]),
                Mask = Convert.ToString(row[6])
            };
        }
    }

}

//sqlQuery.Where(BuildEntityTypeConditions(entityType, "tbl_field.entity_type"));

//if (entityID != null && entityID.Length > 0)
//{
//    sqlQuery.LeftOuterJoin("crm_field_value tbl_field_value",
//                       Exp.EqColumns("tbl_field_value.field_id", "tbl_field.id"))

//    .OrderBy("tbl_field.sort_order", true);

//    //  if (!includeEmptyFields)
//    //      sqlQuery.Where(Exp.Eq("tbl_field_value.tenant_id", TenantID) &
//    //                    Exp.In("tbl_field_value.entity_id", entityID));
//    //  else
//    sqlQuery.Where(Exp.Or(Exp.Eq("tbl_field_value.tenant_id", TenantID), Exp.Eq("tbl_field_value.tenant_id", Exp.Empty)) &
//                   Exp.Or(Exp.In("tbl_field_value.entity_id", entityID), Exp.Eq("tbl_field_value.entity_id", Exp.Empty)));


//}
//else
//{
//    sqlQuery.LeftOuterJoin("crm_field_value tbl_field_value",
//                          Exp.EqColumns("tbl_field_value.field_id", "tbl_field.id"))
//   .Where(Exp.Eq("tbl_field_value.tenant_id", TenantID))
//   .OrderBy("tbl_field_value.entity_id", true)
//   .OrderBy("tbl_field.sort_order", true);


//    // if (!includeEmptyFields)
//    //       sqlQuery.Where(Exp.Eq("tbl_field_value.tenant_id", TenantID));
//    //  else
//    sqlQuery.Where(Exp.Or(Exp.Eq("tbl_field_value.tenant_id", TenantID),
//                          Exp.Eq("tbl_field_value.tenant_id", Exp.Empty)));

//}

//if (!includeEmptyFields)
//    return DbManager.ExecuteList(sqlQuery)
//            .ConvertAll(row => ToCustomField(row)).FindAll(item =>
//                                                              {
//                                                                  if (item.FieldType == CustomFieldType.Heading)
//                                                                      return true;

//                                                                  return !String.IsNullOrEmpty(item.Value.Trim());

//                                                              }).ToList();

//return DbManager.ExecuteList(sqlQuery).ConvertAll(row => ToCustomField(row));
