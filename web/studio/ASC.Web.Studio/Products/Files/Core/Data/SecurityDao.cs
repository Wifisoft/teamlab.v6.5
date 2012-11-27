using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Files.Core.Security;

namespace ASC.Files.Core.Data
{
    internal class SecurityDao : AbstractDao, ISecurityDao
    {
        public SecurityDao(int tenant, string key)
            : base(tenant, key)
        {

        }


        public void SetShare(FileShareRecord r)
        {
            using (var DbManager = GetDbManager())
            {
                if (r.Share == FileShare.None)
                {
                    using (var tx = DbManager.BeginTransaction())
                    {
                        var files = new List<object>();

                        if (r.EntryType == FileEntryType.Folder)
                        {
                            var folders =
                                DbManager.ExecuteList(new SqlQuery("files_folder_tree").Select("folder_id").Where("parent_id", r.EntryId))
                                    .ConvertAll(o => (int) o[0]);
                            files.AddRange(DbManager.ExecuteList(Query("files_file").Select("id").Where(Exp.In("folder_id", folders))).
                                               ConvertAll(o => o[0]));

                            var d1 = new SqlDelete("files_security")
                                .Where("tenant_id", r.Tenant)
                                .Where(Exp.In("entry_id", folders))
                                .Where("entry_type", (int) FileEntryType.Folder)
                                .Where("subject", r.Subject.ToString());

                            DbManager.ExecuteNonQuery(d1);
                        }
                        else
                        {
                            files.Add(r.EntryId);
                        }

                        if (0 < files.Count)
                        {
                            var d2 = new SqlDelete("files_security")
                                .Where("tenant_id", r.Tenant)
                                .Where(Exp.In("entry_id", files))
                                .Where("entry_type", (int) FileEntryType.File)
                                .Where("subject", r.Subject.ToString());

                            DbManager.ExecuteNonQuery(d2);
                        }

                        tx.Commit();
                    }
                }
                else
                {
                    var i = new SqlInsert("files_security", true)
                        .InColumnValue("tenant_id", r.Tenant)
                        .InColumnValue("entry_id", r.EntryId)
                        .InColumnValue("entry_type", (int) r.EntryType)
                        .InColumnValue("subject", r.Subject.ToString())
                        .InColumnValue("owner", r.Owner.ToString())
                        .InColumnValue("security", (int) r.Share);

                    DbManager.ExecuteNonQuery(i);
                }
            }
        }

        public IEnumerable<FileShareRecord> GetShares(IEnumerable<Guid> subjects)
        {
            var q = GetQuery(Exp.In("subject", subjects.Select(s => s.ToString()).ToList()));
            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteList(q).ConvertAll(r => ToFileShareRecord(r)).Where(r => r.EntryId is int);
            }
        }

        /// <summary>
        /// Get file share records with hierarchy.
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public IEnumerable<FileShareRecord> GetShares(params FileEntry[] entries)
        {
            if (entries == null) return new List<FileShareRecord>();

            var files = new List<object>();
            var folders = new List<object>();

            foreach (var entry in entries)
            {
                if (entry is File)
                {
                    var fileId = entry.ID;
                    var folderId = ((File) entry).FolderID;
                    if (!files.Contains(fileId)) files.Add(fileId);
                    if (!folders.Contains(folderId)) folders.Add(folderId);
                }
                else
                {
                    if (!folders.Contains(entry.ID)) folders.Add(entry.ID);
                }
            }

            var q = Query("files_security s")
                .Select("s.tenant_id", "cast(t.folder_id as char)", "s.entry_type", "s.subject", "s.owner", "s.security", "t.level")
                .InnerJoin("files_folder_tree t", Exp.EqColumns("s.entry_id", "t.parent_id"))
                .Where(Exp.In("t.folder_id", folders))
                .Where("s.entry_type", (int) FileEntryType.Folder);

            if (0 < files.Count)
            {
                q.Union(GetQuery(Exp.In("s.entry_id", files) & Exp.Eq("s.entry_type", (int) FileEntryType.File)).Select("-1"));
            }

            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(q)
                    .Select(r => ToFileShareRecord(r))
                    .Where(r=>r.EntryId is int)
                    .OrderBy(r => r.Level)
                    .ThenByDescending(r => r.Share)
                    .ToList();
            }
        }

        public void RemoveSubject(Guid subject)
        {
            var batch = new List<ISqlInstruction>
                            {
                                Delete("files_security").Where("subject", subject.ToString()),
                                Delete("files_security").Where("owner", subject.ToString()),
                            };
            using (var DbManager = GetDbManager())
            {
                DbManager.ExecuteBatch(batch);
            }
        }

        private SqlQuery GetQuery(Exp where)
        {
            return Query("files_security s")
                .Select("s.tenant_id", "s.entry_id", "s.entry_type", "s.subject", "s.owner", "s.security")
                .Where(where);
        }
        
        private FileShareRecord ToFileShareRecord(object[] r)
        {
            var result = new FileShareRecord
            {
                Tenant = Convert.ToInt32(r[0]),
                EntryId = r[1],
                EntryType = (FileEntryType)Convert.ToInt32(r[2]),
                Subject = new Guid((string)r[3]),
                Owner = new Guid((string)r[4]),
                Share = (FileShare)Convert.ToInt32(r[5]),
                Level = 6 < r.Length ? Convert.ToInt32(r[6]) : 0,
            };

            int pureID;

            if (int.TryParse(r[1].ToString(), out pureID))
                result.EntryId = pureID;
        
            return result;
        }
    }
}