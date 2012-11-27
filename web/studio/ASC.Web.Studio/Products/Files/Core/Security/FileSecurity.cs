using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Files.Api;
using ASC.Web.Files.Configuration;

namespace ASC.Files.Core.Security
{
    public class FileSecurity : IFileSecurity
    {
        private readonly IDaoFactory daoFactory;


        public static bool IsAdministrator(Guid userId)
        {
            return CoreContext.UserManager.IsUserInGroup(userId, ASC.Core.Users.Constants.GroupAdmin.ID) ||
                   WebItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, userId);
        }


        public FileShare DefaultMyShare
        {
            get { return FileShare.Restrict; }
        }

        public FileShare DefaultCommonShare
        {
            get { return FileShare.Read; }
        }

        public FileSecurity(IDaoFactory daoFactory)
        {
            this.daoFactory = daoFactory;
        }

        public bool CanRead(FileEntry file, Guid userId)
        {
            return Can(file, userId, FilesSecurityActions.Read);
        }

        public bool CanCreate(FileEntry file, Guid userId)
        {
            return Can(file, userId, FilesSecurityActions.Create);
        }

        public bool CanEdit(FileEntry file, Guid userId)
        {
            return Can(file, userId, FilesSecurityActions.Edit);
        }

        public bool CanDelete(FileEntry file, Guid userId)
        {
            return Can(file, userId, FilesSecurityActions.Delete);
        }

        public bool CanRead(FileEntry file)
        {
            return CanRead(file, SecurityContext.CurrentAccount.ID);
        }

        public bool CanCreate(FileEntry file)
        {
            return CanCreate(file, SecurityContext.CurrentAccount.ID);
        }

        public bool CanEdit(FileEntry file)
        {
            return CanEdit(file, SecurityContext.CurrentAccount.ID);
        }

        public bool CanDelete(FileEntry file)
        {
            return CanDelete(file, SecurityContext.CurrentAccount.ID);
        }

        public IEnumerable<FileEntry> FilterRead(IEnumerable<FileEntry> entries)
        {
            return Filter(entries, FilesSecurityActions.Read, SecurityContext.CurrentAccount.ID);
        }

        public IEnumerable<File> FilterRead(IEnumerable<File> entries)
        {
            return Filter(entries.Cast<FileEntry>(), FilesSecurityActions.Read, SecurityContext.CurrentAccount.ID).Cast<File>();
        }

        public IEnumerable<Folder> FilterRead(IEnumerable<Folder> entries)
        {
            return Filter(entries.Cast<FileEntry>(), FilesSecurityActions.Read, SecurityContext.CurrentAccount.ID).Cast<Folder>();
        }

        private bool Can(FileEntry entry, Guid userId, FilesSecurityActions action)
        {
            return Filter(new[] {entry}, action, userId).Any();
        }

        private IEnumerable<FileEntry> Filter(IEnumerable<FileEntry> entries, FilesSecurityActions action, Guid userId)
        {
            if (entries == null || !entries.Any()) return Enumerable.Empty<FileEntry>();

            entries = entries.Where(f => f != null);
            var result = new List<FileEntry>(entries.Count());

            // save entries order
            var order = entries.Select((f, i) => new {Id = f.UniqID, Pos = i}).ToDictionary(e => e.Id, e => e.Pos);

            // common or my files
            Func<FileEntry, bool> filter =
                f => f.RootFolderType == FolderType.COMMON ||
                     f.RootFolderType == FolderType.USER ||
                     f.RootFolderType == FolderType.SHARE;
            if (entries.Any(filter))
            {
                var subjects = GetUserSubjects(userId);
                List<FileShareRecord> shares = null;
                foreach (var e in entries.Where(filter))
                {
                    if (!CoreContext.Authentication.GetAccountByID(userId).IsAuthenticated && userId != FileConstant.ShareLinkId)
                    {
                        continue;
                    }

                    if (action != FilesSecurityActions.Read && e is Folder && ((Folder) e).FolderType == FolderType.SHARE)
                    {
                        // Root Share folder read-only
                        continue;
                    }

                    if (e.RootFolderType == FolderType.USER && e.RootFolderCreator == userId)
                    {
                        // user has all right in his folder
                        result.Add(e);
                        continue;
                    }

                    if (DefaultCommonShare == FileShare.Read && action == FilesSecurityActions.Read && e is Folder &&
                        ((Folder) e).FolderType == FolderType.COMMON)
                    {
                        // all can read Common folder
                        result.Add(e);
                        continue;
                    }

                    if (action == FilesSecurityActions.Read && e is Folder &&
                        ((Folder) e).FolderType == FolderType.SHARE)
                    {
                        // all can read Share folder
                        result.Add(e);
                        continue;
                    }

                    if (e.RootFolderType == FolderType.COMMON && IsAdministrator(userId))
                    {
                        // administrator in Common has all right
                        result.Add(e);
                        continue;
                    }

                    if (shares == null)
                    {
                        shares = GetShares(entries.ToArray()).Join(subjects, r => r.Subject, s => s, (r, s) => r).ToList();
                        // shares ordered by level
                    }

                    FileShareRecord ace;
                    if (e is File)
                    {
                        ace = shares
                            .OrderBy(r => subjects.IndexOf(r.Subject))
                            .FirstOrDefault(r => Equals(r.EntryId, e.ID) && r.EntryType == FileEntryType.File);
                        if (ace == null)
                        {
                            // share on parent folders
                            ace = shares.Where(r => Equals(r.EntryId, ((File) e).FolderID) && r.EntryType == FileEntryType.Folder)
                                .OrderBy(r => subjects.IndexOf(r.Subject))
                                .ThenBy(r => r.Level)
                                .ThenByDescending(r => r.Share)
                                .FirstOrDefault();
                        }
                    }
                    else
                    {
                        ace = shares.Where(r => Equals(r.EntryId, e.ID) && r.EntryType == FileEntryType.Folder)
                            .OrderBy(r => subjects.IndexOf(r.Subject))
                            .ThenBy(r => r.Level)
                            .ThenByDescending(r => r.Share)
                            .FirstOrDefault();
                    }
                    var defaultShare = e.RootFolderType == FolderType.USER ? DefaultMyShare : DefaultCommonShare;
                    e.Access = ace != null ? ace.Share : defaultShare;

                    if (action == FilesSecurityActions.Read && e.Access <= FileShare.Read) result.Add(e);
                    else if (action == FilesSecurityActions.Edit && e.Access <= FileShare.ReadWrite) result.Add(e);
                    else if (action == FilesSecurityActions.Create && e.Access <= FileShare.ReadWrite) result.Add(e);
                        // can't delete in My other people's files
                    else if (action == FilesSecurityActions.Delete && e.Access <= FileShare.ReadWrite && e.RootFolderType == FolderType.COMMON) result.Add(e);
                    else if (e.Access <= FileShare.Read && e.CreateBy == userId) result.Add(e);

                    if (e.CreateBy == userId) e.Access = FileShare.None; //HACK: for client
                }
            }

            // files in bunch
            filter = f => f.RootFolderType == FolderType.BUNCH;
            if (entries.Any(filter))
            {
                using (var dao = daoFactory.GetFolderDao())
                {
                    var findedAdapters = new Dictionary<object, IFileSecurity>();
                    foreach (var e in entries.Where(filter))
                    {
                        IFileSecurity adapter = null;

                        if (!findedAdapters.ContainsKey(e.RootFolderId))
                        {
                            var root = dao.GetFolder(e.RootFolderId);
                            if (root != null)
                            {
                                adapter = FilesIntegration.GetFileSecurity(root.Title);
                            }
                            findedAdapters[e.RootFolderId] = adapter;
                        }

                        adapter = findedAdapters[e.RootFolderId];
                        if (adapter != null)
                        {
                            if (action == FilesSecurityActions.Create && adapter.CanCreate(e, userId)) result.Add(e);
                            if (action == FilesSecurityActions.Delete && adapter.CanDelete(e, userId)) result.Add(e);
                            if (action == FilesSecurityActions.Read && adapter.CanRead(e, userId)) result.Add(e);
                            if (action == FilesSecurityActions.Edit && adapter.CanEdit(e, userId)) result.Add(e);
                        }
                    }
                }
            }

            // files in trash
            filter = f => f.RootFolderType == FolderType.TRASH;
            if (entries.Any(filter))
            {
                using (var dao = daoFactory.GetFolderDao())
                {
                    var mytrashId = dao.GetFolderID(FileConstant.ModuleId, "trash", userId.ToString(), false);
                    foreach (var e in entries.Where(filter))
                    {
                        // only in my trash
                        if (Equals(e.RootFolderId, mytrashId)) result.Add(e);
                    }
                }
            }

            if (IsAdministrator(userId))
            {
                // administrator can work with crashed entries (crash in files_folder_tree)
                filter = f => f.RootFolderType == FolderType.DEFAULT;
                result.AddRange(entries.Where(filter));
            }

            // restore entries order
            result.Sort((x, y) => order[x.UniqID].CompareTo(order[y.UniqID]));
            return result;
        }


        public void Share(object entryId, FileEntryType entryType, Guid @for, FileShare share)
        {
            using (var dao = daoFactory.GetSecurityDao())
            {
                var r = new FileShareRecord
                            {
                                Tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId,
                                EntryId = entryId,
                                EntryType = entryType,
                                Subject = @for,
                                Owner = SecurityContext.CurrentAccount.ID,
                                Share = share,
                            };
                dao.SetShare(r);
            }
        }

        public IEnumerable<FileShareRecord> GetShares(params FileEntry[] entries)
        {
            using (var dao = daoFactory.GetSecurityDao())
            {
                return dao.GetShares(entries);
            }
        }

        public List<FileEntry> GetSharesForMe()
        {
            using (var folderDao = daoFactory.GetFolderDao())
            using (var fileDao = daoFactory.GetFileDao())
            using (var securityDao = daoFactory.GetSecurityDao())
            {
                var subjects = GetUserSubjects(SecurityContext.CurrentAccount.ID);

                var records = securityDao.GetShares(subjects);

                var fileIds = new Dictionary<object, FileShare>();
                var folderIds = new Dictionary<object, FileShare>();

                foreach (var record in records)
                {
                    var firstRecord = records.Where(x => Equals(record.EntryId, x.EntryId) && record.EntryType == x.EntryType)
                         .OrderBy(r => subjects.IndexOf(r.Subject))
                         .First();

                    if (firstRecord.Share == FileShare.Restrict) continue;

                    if (firstRecord.EntryType == FileEntryType.Folder)
                    {
                        if (!folderIds.ContainsKey(firstRecord.EntryId))
                            folderIds.Add(firstRecord.EntryId, firstRecord.Share);
                    }
                    else
                    {
                        if (!fileIds.ContainsKey(firstRecord.EntryId))
                            fileIds.Add(firstRecord.EntryId, firstRecord.Share);
                    }

                }

                var files = fileDao.GetFiles(fileIds.Keys.ToArray());

                files.ForEach(x => x.Access = fileIds[x.ID]);

                var folders = folderDao.GetFolders(folderIds.Keys.ToArray());

                folders.ForEach(x => x.Access = folderIds[x.ID]);

                return files.Cast<FileEntry>()
                    .Concat(folders.Cast<FileEntry>()).ToList();

            }
        }

        public void RemoveSubject(Guid subject)
        {
            using (var dao = daoFactory.GetSecurityDao())
            {
                dao.RemoveSubject(subject);
            }
        }

        public List<Guid> GetUserSubjects(Guid userId)
        {
            // priority order
            return new[] {userId}
                .Union(CoreContext.UserManager.GetUserGroups(userId, ASC.Core.Users.IncludeType.Distinct).Select(g => g.ID))
                .ToList();
        }

        private enum FilesSecurityActions
        {
            Read,
            Create,
            Edit,
            Delete,
        }
    }
}