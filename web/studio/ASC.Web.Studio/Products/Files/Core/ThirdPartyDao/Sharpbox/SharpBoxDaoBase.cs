using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASC.Files.Core.Security;
using AppLimit.CloudComputing.SharpBox;

namespace ASC.Files.Core.ThirdPartyDao.Sharpbox
{
    internal abstract class SharpBoxDaoBase : IDisposable
    {
        private class ErrorDirectoryEntry : ICloudDirectoryEntry
        {
            public ErrorDirectoryEntry(Exception e)
            {
                if (e != null)
                    Error = e.Message;
            }

            public string Error { get; set; }

            public string Name
            {
                get { return "/"; }
            }

            public string Id
            {
                get { return "/"; }
            }

            public long Length
            {
                get { return 0; }
            }

            public DateTime Modified
            {
                get { return DateTime.UtcNow; }
            }

            public string ParentID 
            {
                get { return ""; }
                set { }
            }

            public ICloudDirectoryEntry Parent
            {
                get { return null; }
                set { }
            }

            public ICloudFileDataTransfer GetDataTransferAccessor()
            {
                return null;
            }

            public string GetPropertyValue(string key)
            {
                return null;
            }

            private readonly List<ICloudFileSystemEntry> _entries = new List<ICloudFileSystemEntry>(0);

            public IEnumerator<ICloudFileSystemEntry> GetEnumerator()
            {
                return _entries.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public ICloudFileSystemEntry GetChild(string name)
            {
                return null;
            }

            public ICloudFileSystemEntry GetChild(string name, bool bThrowException)
            {
                if (bThrowException)
                    throw new ArgumentNullException(name);
                return null;
            }

            public ICloudFileSystemEntry GetChild(string idOrName, bool bThrowException, bool firstByNameIfNotFound)
            {
                if (bThrowException)
                    throw new ArgumentNullException(idOrName);
                return null;
            }

            public ICloudFileSystemEntry GetChild(int idx)
            {
                return null;
            }

            public int Count
            {
                get { return 0; }
            }

            public nChildState HasChildrens
            {
                get { return nChildState.HasNoChilds; }
            }
        }


        protected SharpBoxDaoBase(SharpBoxDaoSelector.SharpBoxInfo sharpBoxInfo, SharpBoxDaoSelector sharpBoxDaoSelector)
        {
            SharpBoxProviderInfo = sharpBoxInfo.SharpBoxProviderInfo;
            PathPrefix = sharpBoxInfo.PathPrefix;
            SharpBoxDaoSelector = sharpBoxDaoSelector;
        }

        protected readonly SharpBoxDaoSelector SharpBoxDaoSelector;
        public SharpBoxProviderInfo SharpBoxProviderInfo { get; private set; }
        public string PathPrefix { get; private set; }

        protected string MakePath(object entryId)
        {
            return string.Format("/{0}", Convert.ToString(entryId, CultureInfo.InvariantCulture).TrimStart('/').TrimEnd('/'));
        }

        protected string MakeId(ICloudFileSystemEntry entry)
        {
            var path = string.Empty;
            if (entry != null && !(entry is ErrorDirectoryEntry))
            {
                path = SharpBoxProviderInfo.Storage.GetFileSystemObjectPath(entry);
            }
            return string.Format("{0}{1}", PathPrefix, string.IsNullOrEmpty(path) || path == "/" ? "" : ("-" + path.Replace('/', '|')));
        }

        protected String MakeTitle(ICloudFileSystemEntry fsEntry)
        {
            if (fsEntry is ICloudDirectoryEntry && IsRoot(fsEntry as ICloudDirectoryEntry))
            {
                return SharpBoxProviderInfo.CustomerTitle;
            }

            return Web.Files.Classes.Global.ReplaceInvalidCharsAndTruncate(fsEntry.Name);
        }

        protected string PathParent(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var index = path.TrimEnd('/').LastIndexOf('/');
                if (index != -1)
                {
                    //Cut to it
                    return path.Substring(0, index);
                }
            }
            return path;
        }

        public void Dispose()
        {
            if (SharpBoxProviderInfo.Storage.IsOpened)
            {
                SharpBoxProviderInfo.Storage.Close();
            }
        }

        protected Folder ToFolder(ICloudDirectoryEntry fsEntry)
        {
            if (fsEntry == null) return null;
            if (fsEntry is ErrorDirectoryEntry)
            {
                //Return error entry
                return ToErrorFolder(fsEntry as ErrorDirectoryEntry);
            }

            //var childFoldersCount = fsEntry.OfType<ICloudDirectoryEntry>().Count();//NOTE: Removed due to performance isssues
            var isRoot = IsRoot(fsEntry);

            return new Folder
                       {
                           ID = MakeId(fsEntry),
                           ParentFolderID = isRoot ? null : MakeId(fsEntry.Parent),
                           CreateBy = SharpBoxProviderInfo.Owner,
                           CreateOn = isRoot ? SharpBoxProviderInfo.CreateOn : fsEntry.Modified,
                           FolderType = FolderType.DEFAULT,
                           ModifiedBy = SharpBoxProviderInfo.Owner,
                           ModifiedOn = isRoot ? SharpBoxProviderInfo.CreateOn : fsEntry.Modified,
                           ProviderId = SharpBoxProviderInfo.ID,
                           ProviderName = SharpBoxProviderInfo.ProviderName,
                           ProviderUserName = SharpBoxProviderInfo.UserName,
                           RootFolderCreator = SharpBoxProviderInfo.Owner,
                           RootFolderId = MakeId(RootFolder()),
                           RootFolderType = SharpBoxProviderInfo.RootFolderType,
                           Shareable = false,
                           Title = MakeTitle(fsEntry),
                           TotalFiles = 0, /*fsEntry.Count - childFoldersCount NOTE: Removed due to performance isssues*/
                           TotalSubFolders = 0, /*childFoldersCount NOTE: Removed due to performance isssues*/
                       };
        }

        private bool IsRoot(ICloudDirectoryEntry entry)
        {
            if (entry != null && entry.Name != null)
                return string.IsNullOrEmpty(entry.Name.Trim('/'));
            return false;
        }

        private Folder ToErrorFolder(ErrorDirectoryEntry fsEntry)
        {
            return new Folder
                       {
                           ID = MakeId(fsEntry),
                           ParentFolderID = null,
                           CreateBy = SharpBoxProviderInfo.Owner,
                           CreateOn = fsEntry.Modified,
                           FolderType = FolderType.DEFAULT,
                           ModifiedBy = SharpBoxProviderInfo.Owner,
                           ModifiedOn = fsEntry.Modified,
                           ProviderId = SharpBoxProviderInfo.ID,
                           ProviderName = SharpBoxProviderInfo.ProviderName,
                           ProviderUserName = SharpBoxProviderInfo.UserName,
                           RootFolderCreator = SharpBoxProviderInfo.Owner,
                           RootFolderId = MakeId(fsEntry),
                           RootFolderType = SharpBoxProviderInfo.RootFolderType,
                           Shareable = false,
                           Title = MakeTitle(fsEntry),
                           TotalFiles = fsEntry.Count - 0,
                           TotalSubFolders = 0,
                           Error = (fsEntry).Error
                       };
        }

        protected File ToFile(ICloudFileSystemEntry fsEntry)
        {
            return fsEntry == null ? null
                       : new File
                             {
                                 ID = MakeId(fsEntry),
                                 Access = FileShare.None,
                                 ContentLength = fsEntry.Length,
                                 CreateBy = SharpBoxProviderInfo.Owner,
                                 CreateOn = fsEntry.Modified,
                                 FileStatus = FileStatus.None,
                                 FolderID = MakeId(fsEntry.Parent),
                                 ModifiedBy = SharpBoxProviderInfo.Owner,
                                 ModifiedOn = fsEntry.Modified,
                                 NativeAccessor = fsEntry,
                                 ProviderId = SharpBoxProviderInfo.ID,
                                 ProviderName = SharpBoxProviderInfo.ProviderName,
                                 Title = MakeTitle(fsEntry),
                                 RootFolderId = MakeId(RootFolder()),
                                 RootFolderType = SharpBoxProviderInfo.RootFolderType,
                                 RootFolderCreator = SharpBoxProviderInfo.Owner,
                                 Version = 1
                             };
        }

        public Folder GetRootFolder(object folderId)
        {
            return ToFolder(RootFolder());
        }

        protected ICloudDirectoryEntry RootFolder()
        {
            return SharpBoxProviderInfo.Storage.GetRoot();
        }

        protected ICloudDirectoryEntry GetFolderById(object folderId)
        {
            ICloudDirectoryEntry entry = null;
            Exception e = null;
            try
            {
                entry = SharpBoxProviderInfo.Storage.GetFolder(MakePath(folderId));
            }
            catch (Exception ex)
            {
                e = ex;
            }
            if (entry == null)
            {
                //Create error entry
                entry = new ErrorDirectoryEntry(e);
            }
            return entry;
        }

        protected ICloudFileSystemEntry GetFileById(object fileId)
        {
            return SharpBoxProviderInfo.Storage.GetFile(MakePath(fileId), RootFolder());
        }

        protected IEnumerable<ICloudFileSystemEntry> GetFolderFiles(object folderId)
        {
            return GetFolderFiles(SharpBoxProviderInfo.Storage.GetFolder(MakePath(folderId)));
        }

        protected IEnumerable<ICloudFileSystemEntry> GetFolderSubfolders(object folderId)
        {
            return GetFolderSubfolders(SharpBoxProviderInfo.Storage.GetFolder(MakePath(folderId)));
        }

        protected IEnumerable<ICloudFileSystemEntry> GetFolderFiles(ICloudDirectoryEntry folder)
        {
            return folder.Where(x => !(x is ICloudDirectoryEntry));
        }

        protected IEnumerable<ICloudFileSystemEntry> GetFolderSubfolders(ICloudDirectoryEntry folder)
        {
            return folder.Where(x => (x is ICloudDirectoryEntry));
        }
    }
}