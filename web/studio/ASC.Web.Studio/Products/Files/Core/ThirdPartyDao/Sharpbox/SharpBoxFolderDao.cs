using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using AppLimit.CloudComputing.SharpBox;

namespace ASC.Files.Core.ThirdPartyDao.Sharpbox
{
    internal class SharpBoxFolderDao : SharpBoxDaoBase, IFolderDao
    {
        public SharpBoxFolderDao(SharpBoxDaoSelector.SharpBoxInfo sharpBoxInfo, SharpBoxDaoSelector sharpBoxDaoSelector)
            : base(sharpBoxInfo, sharpBoxDaoSelector)
        {
        }

        public Folder GetFolder(object folderId)
        {
            return ToFolder(GetFolderById(folderId));
        }

        public Folder GetFolder(string title, object parentId)
        {
            var parentFolder = SharpBoxProviderInfo.Storage.GetFolder(MakePath(parentId));
            return ToFolder(parentFolder.OfType<ICloudDirectoryEntry>().FirstOrDefault(x => x.Name.Equals(title, StringComparison.OrdinalIgnoreCase)));
        }

        public Folder GetRootFolderByFile(object fileId)
        {
            return ToFolder(RootFolder());
        }

        public List<Folder> GetFolders(object parentId)
        {
            var parentFolder = SharpBoxProviderInfo.Storage.GetFolder(MakePath(parentId));
            return parentFolder.OfType<ICloudDirectoryEntry>().Select(x => ToFolder(x)).ToList();
        }

        public List<Folder> GetFolders(object parentId, OrderBy orderBy, FilterType filterType, Guid subjectID, string searchText)
        {
            var folders = GetFolders(parentId).AsEnumerable(); //TODO:!!!
            //Filter
            switch (filterType)
            {
                case FilterType.ByUser:
                    folders = folders.Where(x => x.CreateBy == subjectID);
                    break;

                case FilterType.ByDepartment:
                    folders = folders.Where(x => CoreContext.UserManager.IsUserInGroup(x.CreateBy, subjectID));
                    break;
            }

            if (!string.IsNullOrEmpty(searchText))
                folders = folders.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            switch (orderBy.SortedBy)
            {
                case SortedByType.Author:
                    folders = orderBy.IsAsc ? folders.OrderBy(x => x.CreateBy) : folders.OrderByDescending(x => x.CreateBy);
                    break;
                case SortedByType.AZ:
                    folders = orderBy.IsAsc ? folders.OrderBy(x => x.Title) : folders.OrderByDescending(x => x.Title);
                    break;
                case SortedByType.DateAndTime:
                    folders = orderBy.IsAsc ? folders.OrderBy(x => x.CreateOn) : folders.OrderByDescending(x => x.CreateOn);
                    break;
                default:
                    folders = orderBy.IsAsc ? folders.OrderBy(x => x.Title) : folders.OrderByDescending(x => x.Title);
                    break;
            }

            return folders.ToList();
        }

        public List<Folder> GetFolders(object[] folderIds)
        {
            return folderIds.Select(id => GetFolder(id)).ToList();
        }

        public List<Folder> GetParentFolders(object folderId)
        {
            var path = new List<Folder>();
            var folder = GetFolderById(folderId);
            if (folder != null)
            {
                do
                {
                    path.Add(ToFolder(folder));
                } while ((folder = folder.Parent) != null);
            }
            path.Reverse();
            return path;
        }

        public object SaveFolder(Folder folder)
        {
            if (folder.ID != null)
            {
                //Create with id
                var savedfolder = SharpBoxProviderInfo.Storage.CreateFolder(MakePath(folder.ID));
                return MakeId(savedfolder);
            }
            if (folder.ParentFolderID != null)
            {
                var parentFolder = GetFolderById(folder.ParentFolderID);
                try
                {
                    if (SharpBoxProviderInfo.Storage.GetFileSystemObject(folder.Title, parentFolder) != null)
                        throw new ArgumentException(string.Format(Web.Files.Resources.FilesCommonResource.Error_FolderAlreadyExists, folder.Title));
                }
                catch(ArgumentException)
                {
                    throw;
                }
                catch (Exception)
                {
                    
                }

                var newFolder = SharpBoxProviderInfo.Storage.CreateFolder(folder.Title, parentFolder);
                return MakeId(newFolder);
            }
            return null;
        }

        public void DeleteFolder(object folderId)
        {
            SharpBoxProviderInfo.Storage.DeleteFileSystemEntry(GetFolderById(folderId));
        }

        public void MoveFolder(object folderId, object toRootFolderId)
        {
            SharpBoxProviderInfo.Storage.MoveFileSystemEntry(MakePath(folderId), MakePath(toRootFolderId));
        }

        public object CopyFolder(object folderId, object toRootFolderId)
        {
            var folder = GetFolderById(folderId);
            SharpBoxProviderInfo.Storage.CopyFileSystemEntry(MakePath(folderId), MakePath(toRootFolderId));
            return GetFolderById(toRootFolderId).FirstOrDefault(x => x.Name == folder.Name);
        }

        public IDictionary<object, string> CanMoveOrCopy(object[] folderIds, object to)
        {
            return new Dictionary<object, string>();
        }

        public object RenameFolder(object folderId, string newTitle)
        {
            if ("/".Equals(MakePath(folderId)))
            {
                //It's root folder
                SharpBoxDaoSelector.RenameProvider(SharpBoxProviderInfo, newTitle);
                //rename provider customer title
                var folder = GetFolderById(folderId);
                return MakeId(folder);
            }
            else
            {
                //rename folder
                var folder = GetFolderById(folderId);
                if (SharpBoxProviderInfo.Storage.RenameFileSystemEntry(folder, newTitle))
                {
                    //Folder data must be already updated by provider
                    //We can't search google folders by title because root can have multiple folders with the same name
                    //var newFolder = SharpBoxProviderInfo.Storage.GetFileSystemObject(newTitle, folder.Parent);
                    return MakeId(folder);
                }
            }
            return folderId;
        }

        public List<File> GetFiles(object parentId, OrderBy orderBy, FilterType filterType, Guid subjectID, string searchText)
        {
            //Get only files
            var files = GetFolderById(parentId).Where(x => !(x is ICloudDirectoryEntry)).Select(x => ToFile(x));
            //Filter
            switch (filterType)
            {
                case FilterType.ByUser:
                    files = files.Where(x => x.CreateBy == subjectID);
                    break;

                case FilterType.ByDepartment:
                    files = files.Where(x => CoreContext.UserManager.IsUserInGroup(x.CreateBy, subjectID));
                    break;
            }

            if (!string.IsNullOrEmpty(searchText))
                files = files.Where(x => x.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1);

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            switch (orderBy.SortedBy)
            {
                case SortedByType.Author:
                    files = orderBy.IsAsc ? files.OrderBy(x => x.CreateBy) : files.OrderByDescending(x => x.CreateBy);
                    break;
                case SortedByType.AZ:
                    files = orderBy.IsAsc ? files.OrderBy(x => x.Title) : files.OrderByDescending(x => x.Title);
                    break;
                case SortedByType.DateAndTime:
                    files = orderBy.IsAsc ? files.OrderBy(x => x.CreateOn) : files.OrderByDescending(x => x.CreateOn);
                    break;
                default:
                    files = orderBy.IsAsc ? files.OrderBy(x => x.Title) : files.OrderByDescending(x => x.Title);
                    break;
            }


            return files.ToList();
        }

        public List<object> GetFiles(object parentId, bool withSubfolders)
        {
            var folder = GetFolderById(parentId).AsEnumerable();
            if (!withSubfolders)
            {
                folder = folder.Where(x => !(x is ICloudDirectoryEntry));
            }
            return folder.Select(x => (object) MakeId(x)).ToList();
        }

        public int GetItemsCount(object folderId, bool withSubfoldes)
        {
            var folder = GetFolderById(folderId);
            return folder.Count;
        }

        public bool UseTrashForRemove(Folder folder)
        {
            return false;
        }

        public bool UseRecursiveOperation(object folderId, object toRootFolderId)
        {
            return false;
        }

        #region Only for TMFolderDao

        public IEnumerable<Folder> Search(string text, FolderType folderType)
        {
            return null;
        }

        public List<Folder> GetUpdates(DateTime from, DateTime to)
        {
            return null;
        }

        public object GetFolderID(string module, string bunch, string data, bool createIfNotExists)
        {
            return null;
        }

        public object GetFolderIDCommon(bool createIfNotExists)
        {
            return null;
        }

        public object GetFolderIDUser(bool createIfNotExists)
        {
            return null;
        }

        public object GetFolderIDShare(bool createIfNotExists)
        {
            return null;
        }

        public object GetFolderIDTrash(bool createIfNotExists)
        {
            return null;
        }

        #endregion
    }
}