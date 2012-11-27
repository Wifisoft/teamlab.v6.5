using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASC.Web.Studio.Core;
using AppLimit.CloudComputing.SharpBox;

namespace ASC.Files.Core.ThirdPartyDao.Sharpbox
{
    internal class SharpBoxFileDao : SharpBoxDaoBase, IFileDao
    {
        public SharpBoxFileDao(SharpBoxDaoSelector.SharpBoxInfo providerInfo, SharpBoxDaoSelector sharpBoxDaoSelector)
            : base(providerInfo, sharpBoxDaoSelector)
        {
        }


        public File GetFile(object fileId)
        {
            return GetFile(fileId, 1);
        }

        public File GetFile(object fileId, int fileVersion)
        {
            return ToFile(GetFileById(fileId));
        }

        public File GetFile(object parentId, string title)
        {
            return ToFile(GetFolderFiles(parentId).FirstOrDefault(x => x.Name.Contains(title)));
        }

        public List<File> GetFileHistory(object fileId)
        {
            return new List<File>();
        }

        public List<File> GetFiles(object[] fileIds)
        {
            var root = RootFolder();
            return fileIds.Select(fileId => ToFile(SharpBoxProviderInfo.Storage.GetFile(MakePath(fileId), root))).ToList();
        }

        public Stream GetFileStream(File file)
        {
            //NOTE: id here is not converted!
            var fileToDownload = GetFileById(file.ID);
            //Check length of the file
            if (fileToDownload == null)
                throw new ArgumentNullException("file", "File is not found");

            if (fileToDownload.Length > SetupInfo.MaxUploadSize)
            {
                throw FileSizeComment.FileSizeException;
            }

            return fileToDownload.GetDataTransferAccessor().GetDownloadStream();
        }

        public File SaveFile(File file, Stream fileStream)
        {
            if (fileStream == null) throw new ArgumentNullException("fileStream");
            ICloudFileSystemEntry entry = null;
            if(file.ID != null)
            {
                entry = SharpBoxProviderInfo.Storage.GetFile(MakePath(file.ID), null);
            }
            else if (file.FolderID != null)
            {
                var folder = GetFolderById(file.FolderID);
                try
                {
                    //Check existense
                    if (SharpBoxProviderInfo.Storage.GetFileSystemObject(file.Title, folder) != null)
                        throw new ArgumentException(string.Format(Web.Files.Resources.FilesCommonResource.Error_FileAlreadyExists, file.Title));
                }
                catch(ArgumentException)
                {
                    throw;
                }
                catch (Exception)
                {
                    
                }

                entry = SharpBoxProviderInfo.Storage.CreateFile(folder, file.Title);
            }
            if (entry != null)
            {
                entry.GetDataTransferAccessor().Transfer(fileStream, nTransferDirection.nUpload);
                return ToFile(entry);
            }
            return null;
        }

        public void DeleteFile(object fileId)
        {
            SharpBoxProviderInfo.Storage.DeleteFileSystemEntry(GetFileById(fileId));
        }

        public bool IsExist(string title, object folderId)
        {
            return GetFolderFiles(folderId).FirstOrDefault(x => x.Name.Contains(title)) != null;
        }

        public void MoveFile(object fileId, object toFolderId)
        {
            SharpBoxProviderInfo.Storage.MoveFileSystemEntry(MakePath(fileId), MakePath(toFolderId));
        }

        public File CopyFile(object fileId, object toFolderId)
        {
            var file = GetFile(fileId);
            SharpBoxProviderInfo.Storage.CopyFileSystemEntry(MakePath(fileId), MakePath(toFolderId));
            return ToFile(GetFolderById(toFolderId).FirstOrDefault(x => x.Name == file.Title));
        }

        public object FileRename(object fileId, string newTitle)
        {
            var file = GetFileById(fileId);
            if (SharpBoxProviderInfo.Storage.RenameFileSystemEntry(file, newTitle))
            {
                //File data must be already updated by provider
                //We can't search google files by title because root can have multiple folders with the same name
                //var newFile = SharpBoxProviderInfo.Storage.GetFileSystemObject(newTitle, file.Parent);
                return MakeId(file);
            }
            return fileId;
        }

        public bool UseTrashForRemove(File file)
        {
            return false;
        }

        #region Only in TMFileDao

        public IEnumerable<File> Search(string text, FolderType folderType)
        {
            return null;
        }

        public List<File> GetUpdates(DateTime from, DateTime to)
        {
            return null;
        }

        public void DeleteFolder(object fileId)
        {
            //Do nothing
        }

        public void DeleteFileStream(object file)
        {
            //Do nothing
        }

        public bool IsExistOnStorage(File file)
        {
            return true;
        }

        #endregion
    }
}