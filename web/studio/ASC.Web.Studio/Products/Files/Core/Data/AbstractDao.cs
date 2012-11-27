using System;
using System.Collections.Generic;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Files.Resources;

namespace ASC.Files.Core.Data
{
    internal class AbstractDao : IDisposable
    {
        private readonly string _dbId;


        protected int TenantID { get; private set; }


        protected AbstractDao(int tenantID, String storageKey)
        {
            TenantID = tenantID;
            _dbId = storageKey;
        }

        protected DbManager GetDbManager()
        {
            return new DbManager(_dbId);
        }

        public void Dispose()
        {
        }


        protected List<object[]> ExecList(ISqlInstruction sql)
        {
            using (var manager = GetDbManager())
            {
                return manager.ExecuteList(sql);
            }
        }

        protected List<object[]> ExecList(string sql)
        {
            using (var manager = GetDbManager())
            {
                return manager.ExecuteList(sql);
            }
        }

        protected T ExecScalar<T>(ISqlInstruction sql)
        {
            using (var manager = GetDbManager())
            {
                return manager.ExecuteScalar<T>(sql);
            }
        }

        protected int ExecNonQuery(ISqlInstruction sql)
        {
            using (var manager = GetDbManager())
            {
                return manager.ExecuteNonQuery(sql);
            }
        }


        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumns(GetTenantColumnName(table)).Values(TenantID);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected string GetTenantColumnName(string table)
        {
            var tenant = "tenant_id";
            if (!table.Contains(" ")) return tenant;
            return table.Substring(table.IndexOf(" ")).Trim() + "." + tenant;
        }


        protected SqlQuery GetFolderQuery(Exp where)
        {
            return Query("files_folder f")
                .Select("f.id")
                .Select("f.parent_id")
                .Select("f.title")
                .Select("f.create_on")
                .Select("f.create_by")
                .Select("f.modified_on")
                .Select("f.modified_by")
                .Select("f.folder_type")
                .Select("f.foldersCount")
                .Select("f.filesCount")
                .Select(GetRootFolderType("parent_id"))
                .Select(GetSharedQuery(FileEntryType.Folder))
                .Where(where);
        }

        protected Folder ToFolder(object[] r)
        {
            var f = new Folder
                        {
                            ID = Convert.ToInt32(r[0]),
                            ParentFolderID = Convert.ToInt32(r[1]),
                            Title = Convert.ToString(r[2]),
                            CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[3])),
                            CreateBy = new Guid(r[4].ToString()),
                            ModifiedOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[5])),
                            ModifiedBy = new Guid(r[6].ToString()),
                            FolderType = (FolderType) Convert.ToInt32(r[7]),
                            TotalSubFolders = Convert.ToInt32(r[8]),
                            TotalFiles = Convert.ToInt32(r[9]),
                            RootFolderType = ParseRootFolderType(r[10]),
                            RootFolderCreator = ParseRootFolderCreator(r[10]),
                            RootFolderId = ParseRootFolderId(r[10]),
                            SharedByMe = Convert.ToBoolean(r[11]),
                        };
            switch (f.FolderType)
            {
                case FolderType.COMMON:
                    f.Title = FilesUCResource.CorporateFiles;
                    break;
                case FolderType.USER:
                    f.Title = FilesUCResource.MyFiles;
                    break;
                case FolderType.SHARE:
                    f.Title = FilesUCResource.SharedForMe;
                    break;
                case FolderType.TRASH:
                    f.Title = FilesUCResource.Trash;
                    break;
            }
            if (f.FolderType != FolderType.DEFAULT && 0.Equals(f.ParentFolderID)) f.RootFolderType = f.FolderType;
            if (f.FolderType != FolderType.DEFAULT && f.RootFolderCreator == default(Guid)) f.RootFolderCreator = f.CreateBy;
            if (f.FolderType != FolderType.DEFAULT && 0.Equals(f.RootFolderId)) f.RootFolderId = f.ID;
            return f;
        }

        protected SqlQuery GetFileQuery(Exp where)
        {
            return Query("files_file f")
                .Select("f.id")
                .Select("f.title")
                .Select("f.folder_id")
                .Select("f.create_on")
                .Select("f.create_by")
                .Select("f.version")
                .Select("f.content_type")
                .Select("f.content_length")
                .Select("f.modified_on")
                .Select("f.modified_by")
                .Select("f.file_status")
                .Select(GetRootFolderType("folder_id"))
                .Select(GetSharedQuery(FileEntryType.File))
                .Select("converted_type")
                .Where(where);
        }

        protected File ToFile(object[] r)
        {
            return new File
                       {
                           ID = Convert.ToInt32(r[0]),
                           Title = (String) r[1],
                           FolderID = Convert.ToInt32(r[2]),
                           CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[3])),
                           CreateBy = new Guid((string) r[4]),
                           Version = Convert.ToInt32(r[5]),
                           ContentType = Convert.ToString(r[6]),
                           ContentLength = Convert.ToInt64(r[7]),
                           ModifiedOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[8])),
                           ModifiedBy = new Guid((string) r[9]),
                           RootFolderType = ParseRootFolderType(r[11]),
                           RootFolderCreator = ParseRootFolderCreator(r[11]),
                           RootFolderId = ParseRootFolderId(r[11]),
                           SharedByMe = Convert.ToBoolean(r[12]),
                           ConvertedType = (string) r[13],
                       };
        }


        private SqlQuery GetRootFolderType(string parentFolderColumnName)
        {
            return Query("files_folder d")
                .From("files_folder_tree t")
                .Select("concat(cast(d.folder_type as char),d.create_by,cast(d.id as char))")
                .Where(Exp.EqColumns("d.id", "t.parent_id") &
                       Exp.EqColumns("t.folder_id", "f." + parentFolderColumnName))
                .OrderBy("level", false)
                .SetMaxResults(1);
        }

        private FolderType ParseRootFolderType(object v)
        {
            return v != null
                       ? (FolderType) Enum.Parse(typeof (FolderType), v.ToString().Substring(0, 1))
                       : default(FolderType);
        }

        private Guid ParseRootFolderCreator(object v)
        {
            return v != null ? new Guid(v.ToString().Substring(1, 36)) : default(Guid);
        }

        private int ParseRootFolderId(object v)
        {
            return v != null ? int.Parse(v.ToString().Substring(1 + 36)) : default(int);
        }

        private SqlQuery GetSharedQuery(FileEntryType type)
        {
            return Query("files_security s")
                .SelectCount()
                .Where(Exp.EqColumns("s.entry_id", "f.id"))
                .Where("s.entry_type", (int) type)
                .Where("owner", SecurityContext.CurrentAccount.ID.ToString());
        }


        protected SqlUpdate GetRecalculateFilesCountUpdate(object folderId)
        {
            if (DbRegistry.GetSqlDialect(_dbId).SupportMultiTableUpdate)
            {
                return new SqlUpdate("files_folder d, files_folder_tree t")
                    .Set(
                        "d.filesCount = (select count(distinct f.id) from files_file f, files_folder_tree t2 where f.tenant_id = d.tenant_id and f.folder_id = t2.folder_id and t2.parent_id = d.id)")
                    .Where(Exp.EqColumns("d.id", "t.parent_id") & Exp.Eq("t.folder_id", folderId) &
                           Exp.Eq("d.tenant_id", TenantID));
            }
            else
            {
                return new SqlUpdate("files_folder")
                    .Set(
                        "filesCount = (select count(distinct f.id) from files_file f, files_folder_tree t2 where f.tenant_id = files_folder.tenant_id and f.folder_id = t2.folder_id and t2.parent_id = files_folder.id)")
                    .Where(Exp.Eq("files_folder.tenant_id", TenantID) &
                           Exp.In("files_folder.id",
                                  new SqlQuery("files_folder_tree t").Select("t.parent_id").Where("t.folder_id",
                                                                                                  folderId)));
            }
        }
    }
}