using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.FullTextIndex;

namespace ASC.Files.Core.Data
{
    internal class FolderDao : AbstractDao, IFolderDao
    {
        private const string my = "my";
        private const string common = "common";
        private const string share = "share";
        private const string trash = "trash";

        public FolderDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {
        }


        public Folder GetFolder(object folderId)
        {
            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(GetFolderQuery(Exp.Eq("id", folderId)))
                    .ConvertAll(r => ToFolder(r))
                    .SingleOrDefault();
            }
        }

        public Folder GetFolder(String title, object parentId)
        {
            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(
                        GetFolderQuery(Exp.Eq("title", title) & Exp.Eq("parent_id", parentId)).OrderBy("create_on", true))
                    .ConvertAll(r => ToFolder(r))
                    .FirstOrDefault();
            }
        }

        public Folder GetRootFolder(object folderId)
        {
            var q = new SqlQuery("files_folder_tree")
                .Select("parent_id")
                .Where("folder_id", folderId)
                .SetMaxResults(1)
                .OrderBy("level", false);

            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(GetFolderQuery(Exp.EqColumns("id", q)))
                    .ConvertAll(r => ToFolder(r))
                    .SingleOrDefault();
            }
        }

        public Folder GetRootFolderByFile(object fileId)
        {
            var subq = Query("files_file")
                .Select("folder_id")
                .Where("id", fileId)
                .Distinct();

            var q = new SqlQuery("files_folder_tree")
                .Select("parent_id")
                .Where(Exp.EqColumns("folder_id", subq))
                .SetMaxResults(1)
                .OrderBy("level", false);

            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(GetFolderQuery(Exp.EqColumns("id", q)))
                    .ConvertAll(r => ToFolder(r))
                    .SingleOrDefault();
            }
        }

        public List<Folder> GetFolders(object parentId)
        {
            return GetFolders(parentId, default(OrderBy), default(FilterType), default(Guid), string.Empty);
        }

        public List<Folder> GetFolders(object parentId, OrderBy orderBy, FilterType filterType, Guid subjectID, string searchText)
        {
            if (filterType == FilterType.FilesOnly) return new List<Folder>();

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            var q = GetFolderQuery(Exp.Eq("parent_id", parentId));
            switch (orderBy.SortedBy)
            {
                case SortedByType.Author:
                    q.OrderBy("create_by", orderBy.IsAsc);
                    break;
                case SortedByType.AZ:
                    q.OrderBy("title", orderBy.IsAsc);
                    break;
                case SortedByType.DateAndTime:
                    q.OrderBy("create_on", orderBy.IsAsc);
                    break;
                default:
                    q.OrderBy("title", true);
                    break;
            }

            if (!string.IsNullOrEmpty(searchText))
                q.Where(Exp.Like("lower(title)", searchText.ToLower().Trim()));

            if (filterType == FilterType.ByDepartment || filterType == FilterType.ByUser ||
                filterType == FilterType.DocumentsOnly || filterType == FilterType.ImagesOnly ||
                filterType == FilterType.PresentationsOnly || filterType == FilterType.SpreadsheetsOnly)
            {
                var existsQuery = Query("files_file file")
                    .From("files_folder_tree tree")
                    .Select("file.id")
                    .Where(Exp.EqColumns("file.folder_id", "tree.folder_id"))
                    .Where(Exp.EqColumns("tree.parent_id", "f.id"));
                switch (filterType)
                {
                    case FilterType.DocumentsOnly:
                    case FilterType.ImagesOnly:
                    case FilterType.PresentationsOnly:
                    case FilterType.SpreadsheetsOnly:
                        existsQuery.Where("file.category", (int) filterType);
                        break;
                    case FilterType.ByUser:
                        existsQuery.Where("file.create_by", subjectID.ToString());
                        break;
                    case FilterType.ByDepartment:
                        var users = CoreContext.UserManager.GetUsersByGroup(subjectID).Select(u => u.ID.ToString()).ToArray();
                        existsQuery.Where(Exp.In("file.create_by", users));
                        break;
                }
                q.Where(Exp.Exists(existsQuery));
            }

            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(q)
                    .ConvertAll(r => ToFolder(r));
            }
        }

        public List<Folder> GetFolders(object[] folderIds)
        {
            var q = GetFolderQuery(Exp.In("id", folderIds));
            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(q)
                    .ConvertAll(r => ToFolder(r));
            }
        }

        public List<Folder> GetParentFolders(object folderId)
        {
            var q = GetFolderQuery(Exp.Empty)
                .InnerJoin("files_folder_tree t", Exp.EqColumns("id", "t.parent_id"))
                .Where("t.folder_id", folderId)
                .OrderBy("t.level", false);

            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(q)
                    .ConvertAll(r => ToFolder(r));
            }
        }

        public object SaveFolder(Folder folder)
        {
            return SaveFolder(folder, null);
        }

        public object SaveFolder(Folder folder, DbManager dbManager)
        {
            bool ownsManager = false;
            if (folder == null) throw new ArgumentNullException("folder");

            folder.ModifiedOn = TenantUtil.DateTimeNow();
            folder.ModifiedBy = SecurityContext.CurrentAccount.ID;

            if (folder.CreateOn == default(DateTime)) folder.CreateOn = TenantUtil.DateTimeNow();
            if (folder.CreateBy == default(Guid)) folder.CreateBy = SecurityContext.CurrentAccount.ID;
            try
            {
                if (dbManager == null)
                {
                    ownsManager = true;
                    dbManager = GetDbManager();
                }

                using (var tx = dbManager.BeginTransaction(true))
                {
                    if (folder.ID != null && IsExist(folder.ID))
                    {
                        dbManager.ExecuteNonQuery(
                            Update("files_folder")
                                .Set("title", folder.Title)
                                .Set("modified_on", TenantUtil.DateTimeToUtc(folder.ModifiedOn))
                                .Set("modified_by", folder.ModifiedBy.ToString())
                                .Where("id", folder.ID));
                    }
                    else
                    {
                        folder.ID = dbManager.ExecuteScalar<int>(
                            Insert("files_folder")
                                .InColumnValue("id", 0)
                                .InColumnValue("parent_id", folder.ParentFolderID)
                                .InColumnValue("title", folder.Title)
                                .InColumnValue("create_on", TenantUtil.DateTimeToUtc(folder.CreateOn))
                                .InColumnValue("create_by", folder.CreateBy.ToString())
                                .InColumnValue("modified_on", TenantUtil.DateTimeToUtc(folder.ModifiedOn))
                                .InColumnValue("modified_by", folder.ModifiedBy.ToString())
                                .InColumnValue("folder_type", (int) folder.FolderType)
                                .Identity(1, 0, true));

                        //itself link
                        dbManager.ExecuteNonQuery(
                            new SqlInsert("files_folder_tree")
                                .InColumns("folder_id", "parent_id", "level")
                                .Values(folder.ID, folder.ID, 0));

                        //full path to root
                        dbManager.ExecuteNonQuery(
                            new SqlInsert("files_folder_tree")
                                .InColumns("folder_id", "parent_id", "level")
                                .Values(
                                    new SqlQuery("files_folder_tree t")
                                        .Select(folder.ID.ToString(), "t.parent_id", "t.level + 1")
                                        .Where("t.folder_id", folder.ParentFolderID)));
                    }

                    tx.Commit();
                }

                if (!dbManager.InTransaction)
                {
                    RecalculateFoldersCount(dbManager, folder.ID);
                }
            }
            finally
            {
                if (dbManager != null && ownsManager && !dbManager.Disposed)
                {
                    //If it's our manager - dispose
                    dbManager.Dispose();
                }
            }


            return folder.ID;
        }

        private bool IsExist(object folderId)
        {
            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteScalar<int>(Query("files_folder").SelectCount().Where(Exp.Eq("id", folderId))) > 0;
            }
        }

        public void DeleteFolder(object folderId)
        {
            if (folderId == null) throw new ArgumentNullException("folderId");

            var id = int.Parse(Convert.ToString(folderId));

            if (id == 0) return;

            using (var DbManager = GetDbManager())
            {
                using (var tx = DbManager.BeginTransaction())
                {
                    var subfolders = DbManager
                        .ExecuteList(new SqlQuery("files_folder_tree").Select("folder_id").Where("parent_id", id))
                        .ConvertAll(r => Convert.ToInt32(r[0]));
                    if (!subfolders.Contains(id)) subfolders.Add(id); // chashed folder_tree

                    var parent = DbManager.ExecuteScalar<int>(Query("files_folder").Select("parent_id").Where("id", id));

                    DbManager.ExecuteNonQuery(Delete("files_folder").Where(Exp.In("id", subfolders)));
                    DbManager.ExecuteNonQuery(new SqlDelete("files_folder_tree").Where(Exp.In("folder_id", subfolders)));
                    DbManager.ExecuteNonQuery(Delete("files_tag_link").Where(Exp.In("entry_id", subfolders)).Where("entry_type", (int)FileEntryType.Folder));
                    DbManager.ExecuteNonQuery(Delete("files_tag").Where(Exp.EqColumns("0", Query("files_tag_link l").SelectCount().Where(Exp.EqColumns("tag_id", "id")))));
                    DbManager.ExecuteNonQuery(Delete("files_security").Where(Exp.In("entry_id", subfolders)).Where("entry_type", (int)FileEntryType.Folder));

                    tx.Commit();

                    RecalculateFoldersCount(DbManager, parent);
                }
            }
        }

        public void MoveFolder(object id, object toRootFolderId)
        {
            using (var DbManager = GetDbManager())
            {
                using (var tx = DbManager.BeginTransaction())
                {
                    var recalcFolders = new List<object> {toRootFolderId};
                    var parent = DbManager.ExecuteScalar<int>(Query("files_folder").Select("parent_id").Where("id", id));
                    if (parent != 0 && !recalcFolders.Contains(parent)) recalcFolders.Add(parent);

                    DbManager.ExecuteNonQuery(
                        Update("files_folder")
                            .Set("parent_id", toRootFolderId)
                            .Set("modified_on", DateTime.UtcNow)
                            .Set("modified_by", SecurityContext.CurrentAccount.ID.ToString())
                            .Where("id", id));

                    var subfolders = DbManager
                        .ExecuteList(new SqlQuery("files_folder_tree").Select("folder_id", "level").Where("parent_id",
                                                                                                          id))
                        .ToDictionary(r => Convert.ToInt32(r[0]), r => Convert.ToInt32(r[1]));

                    DbManager.ExecuteNonQuery(
                        new SqlDelete("files_folder_tree").Where(Exp.In("folder_id", subfolders.Keys) &
                                                                 !Exp.In("parent_id", subfolders.Keys)));

                    foreach (var subfolder in subfolders)
                    {
                        DbManager.ExecuteNonQuery(new SqlInsert("files_folder_tree", true)
                                                      .InColumns("folder_id", "parent_id", "level")
                                                      .Values(new SqlQuery("files_folder_tree")
                                                                  .Select(
                                                                      subfolder.Key.ToString(
                                                                          CultureInfo.InvariantCulture), "parent_id",
                                                                      "level + 1 + " +
                                                                      subfolder.Value.ToString(
                                                                          CultureInfo.InvariantCulture))
                                                                  .Where("folder_id", toRootFolderId)));
                    }

                    tx.Commit();

                    recalcFolders.ForEach(id1 => RecalculateFoldersCount(DbManager, id1));
                    recalcFolders.ForEach(fid => DbManager.ExecuteNonQuery(GetRecalculateFilesCountUpdate(fid)));
                }
            }
        }

        public object CopyFolder(object id, object toRootFolderId)
        {
            var folder = GetFolder(id);
            var copy = new Folder
                           {
                               ParentFolderID = toRootFolderId,
                               Title = folder.Title,
                               FolderType = folder.FolderType,
                           };

            return SaveFolder(copy);
        }

        public IDictionary<object, string> CanMoveOrCopy(object[] folderIds, object to)
        {
            var result = new Dictionary<object, string>();

            using (var DbManager = GetDbManager())
            {
                foreach (var folderId in folderIds)
                {
                    var count = DbManager.ExecuteScalar<int>(new SqlQuery("files_folder_tree").SelectCount().Where("parent_id", folderId).Where("folder_id", to));
                    if (0 < count)
                    {
                        throw new InvalidOperationException(
                            Web.Files.Resources.FilesCommonResource.ErrorMassage_FolderCopyError);
                    }

                    var title = DbManager.ExecuteScalar<string>(Query("files_folder").Select("lower(title)").Where("id", folderId));
                    var conflict = DbManager.ExecuteScalar<int>(Query("files_folder").Select("id").Where("lower(title)", title).Where("parent_id", to));
                    if (conflict != 0)
                    {
                        DbManager.ExecuteList(new SqlQuery("files_file f1")
                                                  .InnerJoin("files_file f2", Exp.EqColumns("lower(f1.title)", "lower(f2.title)"))
                                                  .Select("f1.id", "f1.title")
                                                  .Where(Exp.Eq("f1.tenant_id", TenantID) & Exp.Eq("f1.current_version", true) &
                                                         Exp.Eq("f1.folder_id", folderId))
                                                  .Where(Exp.Eq("f2.tenant_id", TenantID) & Exp.Eq("f2.current_version", true) &
                                                         Exp.Eq("f2.folder_id", conflict)))
                            .ForEach(r => result[Convert.ToInt32(r[0])] = (string) r[1]);

                        var childs = DbManager.ExecuteList(Query("files_folder").Select("id").Where("parent_id", folderId)).ConvertAll(r => r[0]);
                        foreach (var pair in CanMoveOrCopy(childs.ToArray(), conflict))
                        {
                            result.Add(pair.Key, pair.Value);
                        }
                    }
                }
            }

            return result;
        }

        public object RenameFolder(object folderId, string title)
        {
            using (var DbManager = GetDbManager())
            {
                DbManager.ExecuteNonQuery(
                    Update("files_folder")
                        .Set("title", title)
                        .Set("modified_on", DateTime.UtcNow)
                        .Set("modified_by", SecurityContext.CurrentAccount.ID.ToString())
                        .Where("id", folderId));
            }
            return folderId;
        }

        public List<object> GetFiles(object parentId, bool withSubfolders)
        {
            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteList(
                    Query("files_file")
                        .Select("id")
                        .Where("folder_id", parentId))
                    .ConvertAll(r => r[0]);
            }
        }

        public List<File> GetFiles(object parentId, OrderBy orderBy, FilterType filter, Guid subjectId, string searchText)
        {
            if (filter == FilterType.FoldersOnly) return new List<File>();

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            var q = GetFileQuery(Exp.Eq("current_version", true) & Exp.Eq("folder_id", parentId));

            switch (orderBy.SortedBy)
            {
                case SortedByType.Author:
                    q.OrderBy("create_by", orderBy.IsAsc);
                    break;
                case SortedByType.Size:
                    q.OrderBy("content_length", orderBy.IsAsc);
                    break;
                case SortedByType.AZ:
                    q.OrderBy("title", orderBy.IsAsc);
                    break;
                case SortedByType.DateAndTime:
                    q.OrderBy("create_on", orderBy.IsAsc);
                    break;
                default:
                    q.OrderBy("title", true);
                    break;
            }

            if (!string.IsNullOrEmpty(searchText))
                q.Where(Exp.Like("lower(title)", searchText.ToLower().Trim()));

            switch (filter)
            {
                case FilterType.DocumentsOnly:
                case FilterType.ImagesOnly:
                case FilterType.PresentationsOnly:
                case FilterType.SpreadsheetsOnly:
                    q.Where("category", (int) filter);
                    break;
                case FilterType.ByUser:
                    q.Where("create_by", subjectId.ToString());
                    break;
                case FilterType.ByDepartment:
                    var users = CoreContext.UserManager.GetUsersByGroup(subjectId).Select(u => u.ID.ToString()).ToArray();
                    q.Where(Exp.In("create_by", users));
                    break;
            }

            using (var DbManager = GetDbManager())
            {
                return DbManager
                    .ExecuteList(q)
                    .ConvertAll(r => ToFile(r));
            }
        }

        public int GetItemsCount(object folderId, bool withSubfoldes)
        {
            return GetFoldersCount(folderId, withSubfoldes) +
                   GetFilesCount(folderId, withSubfoldes);
        }

        private int GetFoldersCount(object parentId, bool withSubfoldes)
        {
            var q = new SqlQuery("files_folder_tree").SelectCount().Where("parent_id", parentId);
            if (withSubfoldes)
            {
                q.Where(Exp.Gt("level", 0));
            }
            else
            {
                q.Where("level", 1);
            }
            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteScalar<int>(q);
            }
        }

        private int GetFilesCount(object folderId, bool withSubfoldes)
        {
            var q = Query("files_file").SelectCount("distinct id");

            if (withSubfoldes)
            {
                q.Where(Exp.In("folder_id", new SqlQuery("files_folder_tree").Select("folder_id").Where("parent_id", folderId)));
            }
            else
            {
                q.Where("folder_id", folderId);
            }

            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteScalar<int>(q);
            }
        }

        public List<object> GetFiles(int folderId, bool withSubfolders)
        {
            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteList(
                    Query("files_file")
                        .Select("id")
                        .Where("folder_id", folderId))
                    .ConvertAll(r => r[0]);
            }
        }

        public bool UseTrashForRemove(Folder folder)
        {
            return folder.RootFolderType != FolderType.TRASH && folder.RootFolderType != FolderType.BUNCH;
        }

        public bool UseRecursiveOperation(object folderId, object toRootFolderId)
        {
            return true;
        }

        private void RecalculateFoldersCount(DbManager dbManager, object id)
        {
            dbManager.ExecuteNonQuery(
                Update("files_folder")
                    .Set("foldersCount = (select count(*) - 1 from files_folder_tree where parent_id = id)")
                    .Where(Exp.In("id", new SqlQuery("files_folder_tree").Select("parent_id").Where("folder_id", id))));
        }

        #region Only for TMFolderDao

        public IEnumerable<Folder> Search(string text, FolderType folderType)
        {
            if (string.IsNullOrEmpty(text)) return new List<Folder>();

            if (FullTextSearch.SupportModule(FullTextSearch.FileModule))
            {
                var ids = FullTextSearch.Search(text, FullTextSearch.FileModule)
                    .GetIdentifiers()
                    .Where(id => !string.IsNullOrEmpty(id) && id[0] == 'd')
                    .Select(id => int.Parse(id.Substring(1)))
                    .ToList();
                using (var DbManager = GetDbManager())
                {
                    return DbManager
                        .ExecuteList(GetFolderQuery(Exp.In("id", ids)))
                        .ConvertAll(r => ToFolder(r))
                        .Where(
                            f =>
                            folderType == FolderType.BUNCH
                                ? f.RootFolderType == FolderType.BUNCH
                                : f.RootFolderType == FolderType.USER | f.RootFolderType == FolderType.COMMON);
                }
            }
            else
            {
                var keywords = text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(k => 3 <= k.Trim().Length)
                    .ToList();
                if (keywords.Count == 0) return new List<Folder>();

                var where = Exp.Empty;
                keywords.ForEach(k => where &= Exp.Like("title", k));
                using (var DbManager = GetDbManager())
                {
                    return DbManager
                        .ExecuteList(GetFolderQuery(where))
                        .ConvertAll(r => ToFolder(r))
                        .Where(
                            f =>
                            folderType == FolderType.BUNCH
                                ? f.RootFolderType == FolderType.BUNCH
                                : f.RootFolderType == FolderType.USER | f.RootFolderType == FolderType.COMMON);
                }
            }
        }

        public List<Folder> GetUpdates(DateTime from, DateTime to)
        {
            var query = GetFolderQuery(Exp.Between("create_on", from, to)
                                       | Exp.Between("modified_on", from, to)
                                       | (Exp.Between("s.timestamp", from, to)
                                          & Exp.Eq("s.subject", SecurityContext.CurrentAccount.ID.ToString())))
                .Select("s.timestamp", "s.owner")
                .LeftOuterJoin("files_security s", Exp.EqColumns("s.entry_id", "f.id") & Exp.Eq("s.entry_type", (int) FileEntryType.Folder));

            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteList(query).ConvertAll(r =>
                                                                   {
                                                                       var len = r.Length;
                                                                       var folder = ToFolder(r);
                                                                       folder.SharedToMeOn = Convert.ToDateTime(r[len - 2]);
                                                                       folder.SharedToMeBy = Convert.ToString(r[len - 1]);
                                                                       return folder;
                                                                   }).ToList();
            }
        }

        public object GetFolderID(string module, string bunch, string data, bool createIfNotExists)
        {
            if (string.IsNullOrEmpty(module)) throw new ArgumentNullException("module");
            if (string.IsNullOrEmpty(bunch)) throw new ArgumentNullException("bunch");

            using (var DbManager = GetDbManager())
            {

                var key = string.Format("{0}/{1}/{2}", module, bunch, data);
                var folderId =
                    DbManager.ExecuteScalar<object>(
                        Query("files_bunch_objects").Select("left_node").Where("right_node", key));
                if (createIfNotExists && folderId == null)
                {
                    var folder = new Folder {ParentFolderID = 0};
                    switch (bunch)
                    {
                        case my:
                            folder.FolderType = FolderType.USER;
                            folder.Title = my;
                            break;
                        case common:
                            folder.FolderType = FolderType.COMMON;
                            folder.Title = common;
                            break;
                        case trash:
                            folder.FolderType = FolderType.TRASH;
                            folder.Title = trash;
                            break;
                        case share:
                            folder.FolderType = FolderType.SHARE;
                            folder.Title = share;
                            break;
                        default:
                            folder.FolderType = FolderType.BUNCH;
                            folder.Title = key;
                            break;
                    }
                    using (var tx = DbManager.BeginTransaction()) //NOTE: Maybe we shouldn't start transaction here at all
                    {
                        folderId = SaveFolder(folder, DbManager); //Save using our db manager

                        DbManager.ExecuteNonQuery(
                            Insert("files_bunch_objects")
                                .InColumnValue("left_node", folderId)
                                .InColumnValue("right_node", key));

                        tx.Commit(); //Commit changes
                    }
                }
                return Convert.ToInt32(folderId);
            }
        }

        public object GetFolderIDTrash(bool createIfNotExists)
        {
            return GetFolderID(FileConstant.ModuleId, trash, SecurityContext.CurrentAccount.ID.ToString(), createIfNotExists);
        }

        public object GetFolderIDCommon(bool createIfNotExists)
        {
            return GetFolderID(FileConstant.ModuleId, common, null, createIfNotExists);
        }

        public object GetFolderIDUser(bool createIfNotExists)
        {
            return GetFolderID(FileConstant.ModuleId, my, SecurityContext.CurrentAccount.ID.ToString(), createIfNotExists);
        }

        public object GetFolderIDShare(bool createIfNotExists)
        {
            return GetFolderID(FileConstant.ModuleId, share, null, createIfNotExists);
        }

        #endregion
    }
}