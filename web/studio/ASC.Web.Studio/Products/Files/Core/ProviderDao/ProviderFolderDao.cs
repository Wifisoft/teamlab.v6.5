using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Files.Core.ProviderDao
{
    internal class ProviderFolderDao : ProviderDaoBase, IFolderDao
    {
        public void Dispose()
        {

        }

        public Folder GetFolder(object folderId)
        {
            var selector = GetSelector(folderId);
            return selector.GetFolderDao(folderId).GetFolder(selector.ConvertId(folderId));
        }

        public Folder GetFolder(string title, object parentId)
        {
            var selector = GetSelector(parentId);
            return selector.GetFolderDao(parentId).GetFolder(title, selector.ConvertId(parentId));
        }

        public Folder GetRootFolder(object folderId)
        {
            var selector = GetSelector(folderId);
            return selector.GetFolderDao(folderId).GetRootFolder(selector.ConvertId(folderId));
        }

        public Folder GetRootFolderByFile(object fileId)
        {
            var selector = GetSelector(fileId);
            return selector.GetFolderDao(fileId).GetRootFolderByFile(selector.ConvertId(fileId));
        }

        public List<Folder> GetFolders(object parentId)
        {
            var selector = GetSelector(parentId);
            return selector.GetFolderDao(parentId).GetFolders(selector.ConvertId(parentId));
        }

        public List<Folder> GetFolders(object parentId, OrderBy orderBy, FilterType filterType, Guid subjectID, string searchText)
        {
            var selector = GetSelector(parentId);
            return selector.GetFolderDao(parentId).GetFolders(selector.ConvertId(parentId), orderBy, filterType, subjectID, searchText);
        }

        public List<Folder> GetFolders(object[] folderIds)
        {
            var folders = new List<Folder>();
            foreach (var selector in GetSelectors())
            {
                //Select id's that matches
                var selectorLocal = selector;
                var mathedIds = folderIds.Where(selectorLocal.IsMatch);
                if (mathedIds.Any())
                {
                    var emptyIds = mathedIds.Where(x => selectorLocal.ConvertId(x).Equals(String.Empty));
                    var notEmptyIds = mathedIds.Where(x => !selectorLocal.ConvertId(x).Equals(String.Empty));

                    if (emptyIds.Any())
                        emptyIds.ToList().ForEach(x =>
                                                  folders.Add(selector.GetFolderDao(x).GetFolder(selectorLocal.ConvertId(x))));


                    if (notEmptyIds.Any())
                        folders.AddRange(selector.GetFolderDao(notEmptyIds.First()).GetFolders(notEmptyIds.Select(x => selectorLocal.ConvertId(x)).ToArray()).ToArray());
                }
            }
            return folders;
        }


        public List<Folder> GetParentFolders(object folderId)
        {
            var selector = GetSelector(folderId);
            return selector.GetFolderDao(folderId).GetParentFolders(selector.ConvertId(folderId));
        }

        public object SaveFolder(Folder folder)
        {
            if (folder == null) throw new ArgumentNullException("folder");

            if (folder.ID != null)
            {
                var folderId = folder.ID;
                var selector = GetSelector(folderId);
                folder.ID = selector.ConvertId(folderId);
                var newFolderId = selector.GetFolderDao(folderId).SaveFolder(folder);
                folder.ID = folderId;
                return newFolderId;
            }
            if (folder.ParentFolderID != null)
            {
                var folderId = folder.ParentFolderID;
                var selector = GetSelector(folderId);
                folder.ParentFolderID = selector.ConvertId(folderId);
                var newFolderId = selector.GetFolderDao(folderId).SaveFolder(folder);
                folder.ParentFolderID = folderId;
                return newFolderId;

            }
            throw new ArgumentException("No folder id or parent folder id to determine provider");
        }

        public void DeleteFolder(object folderId)
        {
            var selector = GetSelector(folderId);
            selector.GetFolderDao(folderId).DeleteFolder(selector.ConvertId(folderId));
        }

        public void MoveFolder(object folderId, object toRootFolderId)
        {
            var selector = GetSelector(folderId);
            if (IsCrossDao(folderId, toRootFolderId))
            {
                PerformCrossDaoFolderCopy(folderId, toRootFolderId, true);
            }
            else
            {
                selector.GetFolderDao(folderId).MoveFolder(selector.ConvertId(folderId), selector.ConvertId(toRootFolderId));
            }
        }

        public object CopyFolder(object folderId, object toRootFolderId)
        {
            var selector = GetSelector(folderId);
            if (IsCrossDao(folderId, toRootFolderId))
            {
                return PerformCrossDaoFolderCopy(folderId, toRootFolderId, false);
            }
            else
            {
                return selector.GetFolderDao(folderId).CopyFolder(selector.ConvertId(folderId), selector.ConvertId(toRootFolderId));
            }
        }

        public IDictionary<object, string> CanMoveOrCopy(object[] folderIds, object to)
        {
            if (!folderIds.Any()) return new Dictionary<object, string>();

            var selector = GetSelectors().FirstOrDefault(x => folderIds.All(x.IsMatch));
            return selector.GetFolderDao(folderIds.FirstOrDefault()).CanMoveOrCopy(folderIds, to);
        }

        public object RenameFolder(object folderId, string newTitle)
        {
            var selector = GetSelector(folderId);
            return selector.GetFolderDao(folderId).RenameFolder(selector.ConvertId(folderId), newTitle);
        }

        public List<File> GetFiles(object parentId, OrderBy orderBy, FilterType filterType, Guid subjectID, string searchText)
        {
            var selector = GetSelector(parentId);
            return selector.GetFolderDao(parentId).GetFiles(selector.ConvertId(parentId), orderBy, filterType, subjectID, searchText);
        }

        public List<object> GetFiles(object parentId, bool withSubfolders)
        {
            var selector = GetSelector(parentId);
            return selector.GetFolderDao(parentId).GetFiles(selector.ConvertId(parentId), withSubfolders);
        }

        public int GetItemsCount(object folderId, bool withSubfoldes)
        {
            var selector = GetSelector(folderId);
            return selector.GetFolderDao(folderId).GetItemsCount(selector.ConvertId(folderId), withSubfoldes);
        }

        public bool UseTrashForRemove(Folder folder)
        {
            var selector = GetSelector(folder.ID);
            return selector.GetFolderDao(folder.ID).UseTrashForRemove(folder);
        }

        public bool UseRecursiveOperation(object folderId, object toRootFolderId)
        {
            var selector = GetSelector(folderId);
            var useRecursive = selector.GetFolderDao(folderId).UseRecursiveOperation(folderId, null);
            if (toRootFolderId != null)
            {
                var toFolderSelector = GetSelector(toRootFolderId);
                useRecursive = useRecursive &&
                               toFolderSelector.GetFolderDao(toRootFolderId).
                                   UseRecursiveOperation(folderId, toFolderSelector.ConvertId(toRootFolderId));

            }
            return useRecursive;
        }

        #region Only for TMFolderDao

        public IEnumerable<Folder> Search(string text, FolderType folderType)
        {
            return TryGetFolderDao().Search(text, folderType);
        }

        public List<Folder> GetUpdates(DateTime from, DateTime to)
        {
            return TryGetFolderDao().GetUpdates(from, to);
        }

        public object GetFolderID(string module, string bunch, string data, bool createIfNotExists)
        {
            return (from selector in GetSelectors()
                    let folderId = selector.GetFolderDao(null).GetFolderID(module, bunch, data, createIfNotExists)
                    where folderId != null
                    select folderId).FirstOrDefault();

        }

        public object GetFolderIDCommon(bool createIfNotExists)
        {
            return (from selector in GetSelectors()
                    let folderId = selector.GetFolderDao(null).GetFolderIDCommon(createIfNotExists)
                    where folderId != null
                    select folderId).FirstOrDefault();
        }

        public object GetFolderIDUser(bool createIfNotExists)
        {
            return (from selector in GetSelectors()
                    let folderId = selector.GetFolderDao(null).GetFolderIDUser(createIfNotExists)
                    where folderId != null
                    select folderId).FirstOrDefault();
        }

        public object GetFolderIDShare(bool createIfNotExists)
        {
            return (from selector in GetSelectors()
                    let folderId = selector.GetFolderDao(null).GetFolderIDShare(createIfNotExists)
                    where folderId != null
                    select folderId).FirstOrDefault();
        }

        public object GetFolderIDTrash(bool createIfNotExists)
        {
            return (from selector in GetSelectors()
                    let folderId = selector.GetFolderDao(null).GetFolderIDTrash(createIfNotExists)
                    where folderId != null
                    select folderId).FirstOrDefault();
        }

        #endregion
    }
}