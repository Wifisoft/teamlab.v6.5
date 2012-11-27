using System;
using System.Collections.Generic;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Files.Core.Data
{
    internal class TagDao : AbstractDao, ITagDao
    {
        public TagDao(int tenant, string dbid)
            : base(tenant, dbid)
        {
        }

        public IEnumerable<Tag> GetTags(object entryID, FileEntryType entryType, TagType tagType)
        {
            var q = Query("files_tag t")
                .InnerJoin("files_tag_link l", Exp.EqColumns("l.tag_id", "t.id"))
                .Select("t.name", "t.flag", "t.owner", "entry_id", "entry_type", "t.id")
                .Where("l.tenant_id", TenantID)
                .Where("l.entry_type", (int)entryType)
                .Where(Exp.Eq("l.entry_id", entryID))
                .Where("t.flag", (int)tagType);

            return SelectTagByQuery(q);
        }

        
        public IEnumerable<Tag> GetTags(string[] names, TagType tagType)
        {
            if (names == null) throw new ArgumentNullException("names");

            var q = Query("files_tag t")
                .InnerJoin("files_tag_link l", Exp.EqColumns("l.tag_id", "t.id"))
                .Select("t.name", "t.flag", "t.owner", "entry_id", "entry_type", "t.id")
                .Where("l.tenant_id", TenantID)
                .Where("t.owner", Guid.Empty.ToString())
                .Where(Exp.In("t.name", names))
                .Where("t.flag", (int)tagType);

            return SelectTagByQuery(q);
        }

        private IEnumerable<Tag> SelectTagByQuery(SqlQuery q)
        {
            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteList(q).ConvertAll(r => ToTag(r));
            }
        }

        public IEnumerable<Tag> GetTags(string name, TagType tagType)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            return GetTags(new[] { name }, tagType);
        }

        public IEnumerable<Tag> GetTags(Guid owner, TagType tagType)
        {
            var q = Query("files_tag t")
                .InnerJoin("files_tag_link l", Exp.EqColumns("l.tag_id", "t.id"))
                .Select("t.name", "t.flag", "t.owner", "entry_id", "entry_type", "t.id")
                .Where("l.tenant_id", TenantID)
                .Where("t.owner", owner.ToString())
                .Where("t.flag", (int)tagType);

            return SelectTagByQuery(q);

        }

        public IEnumerable<Tag> SaveTags(params Tag[] tags)
        {
            var result = new List<Tag>();
            if (tags == null || tags.Length == 0) return result;

            using (var DbManager = GetDbManager())
            {
                using (var tx = DbManager.BeginTransaction())
                {
                    foreach (var t in tags)
                    {
                        var id = DbManager.ExecuteScalar<int>(Query("files_tag")
                                                                  .Select("id")
                                                                  .Where("name", t.TagName)
                                                                  .Where("owner", t.Owner.ToString())
                                                                  .Where("flag", (int) t.TagType));
                        if (id == 0)
                        {
                            var i1 = Insert("files_tag")
                                .InColumnValue("id", 0)
                                .InColumnValue("name", t.TagName)
                                .InColumnValue("owner", t.Owner.ToString())
                                .InColumnValue("flag", (int) t.TagType)
                                .Identity(1, 0, true);
                            id = DbManager.ExecuteScalar<int>(i1);
                        }
                        t.Id = id;
                        result.Add(t);

                        var i2 = Insert("files_tag_link")
                            .InColumnValue("tag_id", id)
                            .InColumnValue("entry_id", t.EntryId)
                            .InColumnValue("entry_type", (int) t.EntryType);
                        DbManager.ExecuteNonQuery(i2);
                    }
                    tx.Commit();
                }
            }
            return result;
        }

        public void RemoveTags(params Tag[] tags)
        {
            if (tags == null || tags.Length == 0) return;

            using (var DbManager = GetDbManager())
            {
                using (var tx = DbManager.BeginTransaction())
                {
                    foreach (var t in tags)
                    {
                        var id = DbManager.ExecuteScalar<int>(Query("files_tag")
                                                                  .Select("id")
                                                                  .Where("name", t.TagName)
                                                                  .Where("owner", t.Owner.ToString())
                                                                  .Where("flag", (int) t.TagType));
                        if (id != 0)
                        {
                            var d = Delete("files_tag_link")
                                .Where("tag_id", id)
                                .Where("entry_id", t.EntryId)
                                .Where("entry_type", (int) t.EntryType);
                            DbManager.ExecuteNonQuery(d);

                            var count =
                                DbManager.ExecuteScalar<int>(Query("files_tag_link").SelectCount().Where("tag_id", id));
                            if (count == 0)
                            {
                                d = Delete("files_tag").Where("id", id);
                                DbManager.ExecuteNonQuery(d);
                            }
                        }
                    }
                    tx.Commit();
                }
            }
        }

        public void RemoveTags(params int[] ids)
        {
            if (ids == null || ids.Length == 0) return;
            using (var DbManager = GetDbManager())
            {
                using (var tx = DbManager.BeginTransaction())
                {
                    DbManager.ExecuteNonQuery(Delete("files_tag_link").Where(Exp.In("tag_id", ids)));
                    DbManager.ExecuteNonQuery(Delete("files_tag").Where(Exp.In("id", ids)));
                    tx.Commit();
                }
            }
        }


        private Tag ToTag(object[] r)
        {
            var result = new Tag((string)r[0], (TagType)Convert.ToInt32(r[1]), new Guid((string)r[2]))
                       {
                           EntryId = r[3],
                           EntryType = (FileEntryType)Convert.ToInt32(r[4]),
                           Id = Convert.ToInt32(r[5]),
                       };

            int tagId;

            if (int.TryParse(result.EntryId.ToString(), out tagId))
                result.EntryId = tagId;

            return result;
        }
    }
}