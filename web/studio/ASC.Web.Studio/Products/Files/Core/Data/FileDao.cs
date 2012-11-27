using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.FullTextIndex;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Utility;

namespace ASC.Files.Core.Data
{
    internal class FileDao : AbstractDao, IFileDao
    {
        private readonly object syncRoot = new object();

        public FileDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {

        }

        private Exp BuildLike(string[] columns, string[] keywords)
        {
            var like = Exp.Empty;
            foreach (var keyword in keywords)
            {
                var keywordLike = Exp.Empty;
                foreach (var column in columns)
                {
                    keywordLike |= Exp.Like(column, keyword, SqlLike.StartWith) | Exp.Like(column, ' ' + keyword);
                }
                like &= keywordLike;
            }
            return like;
        }


        public File GetFile(object fileId)
        {
            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(GetFileQuery(Exp.Eq("id", fileId) & Exp.Eq("current_version", true)))
                    .ConvertAll(r => ToFile(r))
                    .SingleOrDefault();
            }
        }

        public File GetFile(object fileId, int fileVersion)
        {
            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(GetFileQuery(Exp.Eq("id", fileId) & Exp.Eq("version", fileVersion)))
                    .ConvertAll(r => ToFile(r))
                    .SingleOrDefault();
            }
        }

        public File GetFile(object parentId, String title)
        {
            if (String.IsNullOrEmpty(title)) throw new ArgumentNullException(title);
            using (var DbManager = GetDbManager())
            {
                var sqlQueryResult = DbManager
                    .ExecuteList(GetFileQuery(Exp.Eq("title", title) & Exp.Eq("current_version", true) & Exp.Eq("folder_id", parentId)))
                    .ConvertAll(r => ToFile(r));

                return sqlQueryResult.Count > 0 ? sqlQueryResult[0] : null;
            }
        }

        public List<File> GetFileHistory(object fileId)
        {
            using (var DbManager = GetDbManager())
            {
                var files = DbManager
                    .ExecuteList(GetFileQuery(Exp.Eq("id", fileId)).OrderBy("version", false))
                    .ConvertAll(r => ToFile(r));

                if (files.Count > 0) files.RemoveAt(0);

                return files;
            }
        }

        public List<File> GetFiles(object[] fileIds)
        {
            if (fileIds == null || fileIds.Length == 0) return new List<File>();

            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(GetFileQuery(Exp.In("id", fileIds) & Exp.Eq("current_version", true)))
                    .ConvertAll(r => ToFile(r));
            }
        }

        public Stream GetFileStream(File file)
        {
            return Global.GetStore().IronReadStream(string.Empty, GetUniqFilePath(file), 10);
        }

        public File SaveFile(File file, Stream fileStream)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            lock (syncRoot)
            {
                using (var DbManager = GetDbManager())
                {
                    using (var tx = DbManager.BeginTransaction())
                    {
                        if (file.ID == null)
                        {
                            file.ID = DbManager.ExecuteScalar<int>(new SqlQuery("files_file").SelectMax("id")) + 1;
                            file.Version = 1;
                        }

                        file.ModifiedBy = SecurityContext.CurrentAccount.ID;
                        file.ModifiedOn = TenantUtil.DateTimeNow();
                        if (file.CreateBy == default(Guid)) file.CreateBy = SecurityContext.CurrentAccount.ID;
                        if (file.CreateOn == default(DateTime)) file.CreateOn = TenantUtil.DateTimeNow();

                        DbManager.ExecuteNonQuery(
                            Update("files_file")
                                .Set("current_version", false)
                                .Where("id", file.ID)
                                .Where("current_version", true));

                        DbManager.ExecuteNonQuery(
                            Insert("files_file")
                                .InColumnValue("id", file.ID)
                                .InColumnValue("version", file.Version)
                                .InColumnValue("title", file.Title)
                                .InColumnValue("folder_id", file.FolderID)
                                .InColumnValue("create_on", TenantUtil.DateTimeToUtc(file.CreateOn))
                                .InColumnValue("create_by", file.CreateBy.ToString())
                                .InColumnValue("content_type", file.ContentType)
                                .InColumnValue("content_length", file.ContentLength)
                                .InColumnValue("modified_on", TenantUtil.DateTimeToUtc(file.ModifiedOn))
                                .InColumnValue("modified_by", file.ModifiedBy.ToString())
                                .InColumnValue("category", (int) file.FilterType)
                                .InColumnValue("current_version", true)
                                .InColumnValue("file_status", (int) FileStatus.None)
                                .InColumnValue("converted_type", file.ConvertedType));

                        tx.Commit();
                    }

                    RecalculateFilesCount(DbManager, file.FolderID);
                }
            }
            if (fileStream != null)
            {
                try
                {
                    SaveFileStream(file, fileStream);
                }
                catch (Exception)
                {
                    DeleteFile(file.ID);
                    throw;
                }
            }
            return file;
        }

        private void SaveFileStream(File file, Stream stream)
        {
            Global.GetStore().Save(string.Empty, GetUniqFilePath(file), stream, file.Title);
        }

        public void DeleteFile(object fileId)
        {
            if (fileId == null) return;
            using (var DbManager = GetDbManager())
            {
                using (var tx = DbManager.BeginTransaction())
                {
                    var fromFolders = DbManager
                        .ExecuteList(Query("files_file").Select("folder_id").Where("id", fileId).GroupBy("id"))
                        .ConvertAll(r => r[0]);

                    DbManager.ExecuteNonQuery(Delete("files_file").Where("id", fileId));
                    DbManager.ExecuteNonQuery(Delete("files_tag_link").Where("entry_id", fileId).Where("entry_type", (int)FileEntryType.File));
                    DbManager.ExecuteNonQuery(Delete("files_tag").Where(Exp.EqColumns("0", Query("files_tag_link l").SelectCount().Where(Exp.EqColumns("tag_id", "id")))));
                    DbManager.ExecuteNonQuery(Delete("files_security").Where("entry_id", fileId).Where("entry_type", (int)FileEntryType.File));

                    tx.Commit();

                    fromFolders.ForEach(folderId => RecalculateFilesCount(DbManager, folderId));
                }
            }
        }

        public bool IsExist(String title, object folderId)
        {
            using (var DbManager = GetDbManager())
            {
                var fileCount = DbManager.ExecuteScalar<int>(
                    Query("files_file")
                        .SelectCount()
                        .Where("title", title)
                        .Where("folder_id", folderId));

                return fileCount != 0;
            }
        }

        public void MoveFile(object id, object toFolderId)
        {
            if (id == null) return;
            using (var DbManager = GetDbManager())
            {
                using (var tx = DbManager.BeginTransaction())
                {
                    var fromFolders = DbManager
                        .ExecuteList(Query("files_file").Select("folder_id").Where("id", id).GroupBy("id"))
                        .ConvertAll(r => r[0]);

                    DbManager.ExecuteNonQuery(
                        Update("files_file")
                            .Set("folder_id", toFolderId)
                            .Where("id", id));

                    //DbManager.ExecuteNonQuery(
                    //    Update("files_file")
                    //        .Set("modified_by", SecurityContext.CurrentAccount.ID.ToString())
                    //        .Set("modified_on", DateTime.UtcNow)
                    //        .Where("id", id)
                    //        .Where("current_version", true));

                    tx.Commit();

                    fromFolders.ForEach(folderId => RecalculateFilesCount(DbManager, folderId));
                    RecalculateFilesCount(DbManager, toFolderId);
                }
            }
        }

        public File CopyFile(object id, object toFolderId)
        {
            // copy only current version
            var file = GetFiles(new[] {id}).SingleOrDefault();
            if (file != null)
            {
                var copy = new File
                               {
                                   ContentLength = file.ContentLength,
                                   ContentType = file.ContentType,
                                   FileStatus = file.FileStatus,
                                   FolderID = toFolderId,
                                   Title = file.Title,
                                   Version = file.Version,
                                   ConvertedType = file.ConvertedType,
                               };

                copy = SaveFile(copy, null);

                //Copy streams
                using (var stream = GetFileStream(file))
                {
                    SaveFileStream(copy, stream);
                }
                return copy;
            }
            return null;
        }

        public object FileRename(object fileId, String newTitle)
        {
            using (var DbManager = GetDbManager())
            {
                DbManager.ExecuteNonQuery(
                    Update("files_file")
                        .Set("title", newTitle)
                        .Set("modified_on", DateTime.UtcNow)
                        .Set("modified_by", SecurityContext.CurrentAccount.ID.ToString())
                        .Where("id", fileId));
            }
            return fileId;
        }

        public bool UseTrashForRemove(File file)
        {
            return file.RootFolderType != FolderType.TRASH && file.RootFolderType != FolderType.BUNCH;
        }

        public bool IsExist(object fileId)
        {
            using (var DbManager = GetDbManager())
            {
                var count = DbManager.ExecuteScalar<int>(Query("files_file").SelectCount().Where("id", fileId));
                return count != 0;
            }
        }

        public String GetUniqFileDirectory(object fileIdObject)
        {
            if (fileIdObject == null) throw new ArgumentNullException("fileIdObject");
            var fileIdInt = Convert.ToInt32(Convert.ToString(fileIdObject));
            return string.Format("folder_{0}/file_{1}", (fileIdInt/1000 + 1)*1000, fileIdInt);
        }

        public String GetUniqFilePath(File file)
        {
            return file != null
                       ? string.Format("{0}/v{1}/content{2}", GetUniqFileDirectory(file.ID), file.Version, FileUtility.GetFileExtension(file.Title))
                       : null;
        }

        private void RecalculateFilesCount(DbManager dbManager, object folderId)
        {
            dbManager.ExecuteNonQuery(GetRecalculateFilesCountUpdate(folderId));
        }

        #region Only in TMFileDao

        public IEnumerable<File> Search(String searchText, FolderType folderType)
        {
            if (FullTextSearch.SupportModule(FullTextSearch.FileModule))
            {
                var indexResult = FullTextSearch.Search(searchText, FullTextSearch.FileModule);
                var ids = indexResult.GetIdentifiers()
                    .Where(id => !string.IsNullOrEmpty(id) && id[0] != 'd')
                    .ToArray();

                using (var DbManager = GetDbManager())
                {
                    return DbManager
                        .ExecuteList(GetFileQuery(Exp.In("id", ids) & Exp.Eq("current_version", true)))
                        .ConvertAll(r => ToFile(r))
                        .Where(
                            f =>
                            folderType == FolderType.BUNCH
                                ? f.RootFolderType == FolderType.BUNCH
                                : f.RootFolderType == FolderType.USER | f.RootFolderType == FolderType.COMMON)
                        .ToList();
                }
            }
            else
            {
                var keywords = searchText
                    .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(k => 3 <= k.Trim().Length)
                    .ToArray();

                if (keywords.Length == 0) return Enumerable.Empty<File>();

                var q = GetFileQuery(Exp.Eq("f.current_version", true) & BuildLike(new[] {"f.title"}, keywords));
                using (var DbManager = GetDbManager())
                {
                    return DbManager
                        .ExecuteList(q)
                        .ConvertAll(r => ToFile(r))
                        .Where(f =>
                               folderType == FolderType.BUNCH
                                   ? f.RootFolderType == FolderType.BUNCH
                                   : f.RootFolderType == FolderType.USER | f.RootFolderType == FolderType.COMMON)
                        .ToList();
                }
            }
        }

        public List<File> GetUpdates(DateTime from, DateTime to)
        {
            var query = GetFileQuery(Exp.Between("create_on", from, to)
                                     | Exp.Between("modified_on", from, to)
                                     | (Exp.Between("s.timestamp", from, to)))
                .Select("s.timestamp", "s.owner")
                .LeftOuterJoin("files_security s", Exp.EqColumns("s.entry_id", "f.id") & Exp.Eq("s.entry_type", (int) FileEntryType.File) & Exp.Eq("s.subject", SecurityContext.CurrentAccount.ID.ToString()));
            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteList(query).ConvertAll(r =>
                                                                   {
                                                                       var len = r.Length;
                                                                       var file = ToFile(r);
                                                                       file.SharedToMeOn = Convert.ToDateTime(r[len - 2]);
                                                                       file.SharedToMeBy = Convert.ToString(r[len - 1]);
                                                                       return file;
                                                                   }).ToList();
            }
        }

        public void DeleteFileStream(object fileId)
        {
            Global.GetStore().Delete(GetUniqFilePath(GetFile(fileId)));
        }

        public void DeleteFolder(object fileId)
        {
            Global.GetStore().DeleteDirectory(GetUniqFileDirectory(fileId));
        }

        public bool IsExistOnStorage(File file)
        {
            return Global.GetStore().IsFile(GetUniqFilePath(file));
        }

        #endregion
    }
}