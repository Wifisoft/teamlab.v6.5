#region Import

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using AjaxPro;

#endregion

namespace ASC.CRM.Core.Dao
{
    public class TagDao : AbstractDao
    {
        #region Constructor

        public TagDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {


        }

        #endregion

        #region Methods

        public bool IsExist(EntityType entityType, String tagName)
        {
            if (String.IsNullOrEmpty(tagName))
                throw new ArgumentNullException(tagName);

            var findedTags = DbManager.ExecuteList(Query("crm_tag")
                                                   .Select("title")
                                                   .Where(Exp.Eq("entity_type", (int) entityType) &
                                                          Exp.Eq("title", tagName)));
            if (findedTags.Count == 0) return false;

            return true;

        }

        public String[] GetAllTags(EntityType entityType)
        {
            return DbManager.ExecuteList(
                Query("crm_tag")
                .Select("title")
                .Where(Exp.Eq("entity_type", (int)entityType))
                .OrderBy("title", true)).ConvertAll(row => row[0].ToString()).ToArray();
        }

        public String GetTagsLinkCountJSON(EntityType entityType)
        {
            var sqlQuery = new SqlQuery("crm_tag tbl_tag")
                .SelectCount("tag_id")
                .LeftOuterJoin("crm_entity_tag", Exp.EqColumns("id", "tag_id"))
                .Where(Exp.Eq("tbl_tag.entity_type", (int)entityType) & Exp.Eq("tbl_tag.tenant_id", TenantID))
                .OrderBy("title", true)
                .GroupBy("tbl_tag.id");

            var queryResult = DbManager.ExecuteList(sqlQuery);

            return JavaScriptSerializer.Serialize(queryResult.ConvertAll(row => row[0]));
        }


        public Dictionary<int, List<String>> GetEntitiesTags(EntityType entityType)
        {

            var result = new Dictionary<int, List<String>>();

            var sqlQuery =
                 new SqlQuery("crm_entity_tag")
                .Select("entity_id", "title")
                .LeftOuterJoin("crm_tag", Exp.EqColumns("id", "tag_id"))
                .Where(Exp.Eq("crm_tag.entity_type", (int)entityType) & Exp.Eq("crm_tag.tenant_id", TenantID))
                .OrderBy("entity_id", true)
                .OrderBy("title", true);

            DbManager.ExecuteList(sqlQuery).ForEach(row =>
                                                        {
                                                            var entityID = Convert.ToInt32(row[0]);
                                                            var tagTitle = Convert.ToString(row[1]);

                                                            if (!result.ContainsKey(entityID))
                                                                result.Add(entityID, new List<String>
                                                                                         {
                                                                                            tagTitle
                                                                                         });
                                                            else
                                                                result[entityID].Add(tagTitle);

                                                        });

            return result;
        }

        public String[] GetEntityTags(EntityType entityType, int entityID)
        {

            SqlQuery sqlQuery = Query("crm_tag")
                .Select("title")
                .LeftOuterJoin("crm_entity_tag", Exp.EqColumns("id", "tag_id"))
                .Where(Exp.Eq("entity_id", entityID) & Exp.Eq("crm_tag.entity_type", (int)entityType));

            return DbManager.ExecuteList(sqlQuery).ConvertAll(row => Convert.ToString(row[0])).ToArray();

        }



        public String[] GetUnusedTags(EntityType entityType)
        {

            return DbManager.ExecuteList(Query("crm_tag")
                                  .Select("title")
                                  .LeftOuterJoin("crm_entity_tag", Exp.EqColumns("tag_id", "id"))
                                  .Where(Exp.Eq("tag_id", Exp.Empty) & Exp.Eq("crm_tag.entity_type", (int)entityType))).ConvertAll(row => Convert.ToString(row[0])).ToArray();

        }

        public void DeleteTag(EntityType entityType, String tagName)
        {

            DeleteTagFromEntity(entityType, 0, tagName);
        }

        public void DeleteTagFromEntity(EntityType entityType, int entityID, String tagName)
        {

            var tagID = DbManager.ExecuteScalar<int>(Query("crm_tag").Select("id")
                                 .Where(Exp.Eq("lower(title)", tagName.ToLower()) & Exp.Eq("entity_type", (int)entityType)));

            if (tagID == 0) return;

            var sqlQuery = new SqlDelete("crm_entity_tag")
                .Where(Exp.Eq("entity_type", (int)entityType) & Exp.Eq("tag_id", tagID));
            
            if (entityID > 0)
                sqlQuery.Where(Exp.Eq("entity_id", entityID));

            DbManager.ExecuteNonQuery(sqlQuery);

            if (entityID == 0)
                DbManager.ExecuteNonQuery(Delete("crm_tag")
                    .Where(Exp.Eq("id", tagID) & Exp.Eq("entity_type", (int)entityType)));

        }

        public void DeleteUnusedTags(EntityType entityType)
        {

            if (!_supportedEntityType.Contains(entityType))
                throw new ArgumentException();

            using (var tx = DbManager.BeginTransaction())
            {

                var sqlSubQuery = Query("crm_tag")
                    .Select("id")
                    .LeftOuterJoin("crm_entity_tag", Exp.EqColumns("tag_id", "id"))
                    .Where(Exp.Eq("crm_tag.entity_type", (int) entityType) & Exp.Eq("tag_id", Exp.Empty));

                var tagIDs = DbManager.ExecuteList(sqlSubQuery).ConvertAll(row => Convert.ToInt32(row[0]));
                               
                if (tagIDs.Count > 0)
                    DbManager.ExecuteNonQuery(Delete("crm_tag").Where(Exp.In("id", tagIDs) & Exp.Eq("entity_type", (int)entityType)));
                

                tx.Commit();
            }

        }

        public int AddTag(EntityType entityType, String tagName)
        {
            return DbManager.ExecuteScalar<int>(
                                 Insert("crm_tag")
                                 .InColumnValue("id", 0)
                                 .InColumnValue("title", tagName)
                                 .InColumnValue("entity_type", (int)entityType)
                                 .Identity(1, 0, true));

        }


     
        public int AddTagToEntity(EntityType entityType, int entityID, String tagName)
        {
            if (String.IsNullOrEmpty(tagName) || entityID == 0)
                throw new ArgumentException();

            var tagID = DbManager.ExecuteScalar<int>(Query("crm_tag").Select("id")
                                   .Where(Exp.Eq("lower(title)", tagName.ToLower()) & Exp.Eq("entity_type", (int)entityType)));

            if (tagID == 0)
                tagID = AddTag(entityType, tagName);

            DbManager.ExecuteNonQuery(new SqlInsert("crm_entity_tag", true)
                                       .InColumnValue("entity_id", entityID)
                                       .InColumnValue("entity_type", (int)entityType)
                                       .InColumnValue("tag_id", tagID));

            return tagID;
        }

        public void SetTagToEntity(EntityType entityType, int entityID, String[] tags)
        {
            using (var tx = DbManager.BeginTransaction())
            {

                DbManager.ExecuteNonQuery(new SqlDelete("crm_entity_tag")
                                                      .Where(Exp.Eq("entity_id", entityID) & Exp.Eq("entity_type", (int)entityType)));


                foreach (var tagName in tags)
                    AddTagToEntity(entityType, entityID, tagName);

                tx.Commit();
            }
        }

        #endregion
    }
}
