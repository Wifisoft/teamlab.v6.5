using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASC.Files.Core.ProviderDao
{
    internal class ProviderFileDao : ProviderDaoBase, IFileDao
    {
        public void Dispose()
        {
        }

        public File GetFile(object fileId)
        {
            var selector = GetSelector(fileId);
            return selector.GetFileDao(fileId).GetFile(selector.ConvertId(fileId));
        }

        public File GetFile(object fileId, int fileVersion)
        {
            var selector = GetSelector(fileId);
            return selector.GetFileDao(fileId).GetFile(selector.ConvertId(fileId), fileVersion);
        }

        public File GetFile(object parentId, string title)
        {
            var selector = GetSelector(parentId);
            return selector.GetFileDao(parentId).GetFile(selector.ConvertId(parentId), title);
        }

        public List<File> GetFileHistory(object fileId)
        {
            var selector = GetSelector(fileId);
            return selector.GetFileDao(fileId).GetFileHistory(selector.ConvertId(fileId));
        }

        public List<File> GetFiles(object[] fileIds)
        {
            var selector = GetSelector(fileIds.FirstOrDefault());
            return selector.GetFileDao(fileIds.FirstOrDefault()).GetFiles(fileIds.Select(x => selector.ConvertId(x)).ToArray());
        }

        public Stream GetFileStream(File file)
        {
            if (file == null) throw new ArgumentNullException("file");
            var fileId = file.ID;
            var selector = GetSelector(fileId);
            file.ID = selector.ConvertId(fileId);
            var stream = selector.GetFileDao(fileId).GetFileStream(file);
            file.ID = fileId; //Restore id
            return stream;
        }

        public File SaveFile(File file, Stream fileStream)
        {
            if (file == null) throw new ArgumentNullException("file");

            var fileId = file.ID;
            var folderId = file.FolderID;

            IDaoSelector selector;
            File fileSaved = null;
            //Convert
            if (fileId != null)
            {
                selector = GetSelector(fileId);
                file.ID = selector.ConvertId(fileId);
                if (folderId != null)
                    file.FolderID = selector.ConvertId(folderId);
                fileSaved = selector.GetFileDao(fileId).SaveFile(file, fileStream);
            }
            else if (folderId != null)
            {
                selector = GetSelector(folderId);
                file.FolderID = selector.ConvertId(folderId);
                fileSaved = selector.GetFileDao(folderId).SaveFile(file, fileStream);
            }

            if (fileSaved != null)
            {
                return fileSaved;
            }
            throw new ArgumentException("No file id or folder id toFolderId determine provider");
        }

        public void DeleteFile(object fileId)
        {
            var selector = GetSelector(fileId);
            selector.GetFileDao(fileId).DeleteFile(selector.ConvertId(fileId));
        }

        public bool IsExist(string title, object folderId)
        {
            var selector = GetSelector(folderId);
            return selector.GetFileDao(folderId).IsExist(title, selector.ConvertId(folderId));
        }

        public void MoveFile(object fileId, object toFolderId)
        {
            var selector = GetSelector(fileId);
            if (IsCrossDao(fileId, toFolderId))
            {
                PerformCrossDaoFileCopy(fileId, toFolderId, true);
                return;
            }

            selector.GetFileDao(fileId).MoveFile(selector.ConvertId(fileId), selector.ConvertId(toFolderId));
        }

        public File CopyFile(object fileId, object toFolderId)
        {
            var selector = GetSelector(fileId);
            if (IsCrossDao(fileId, toFolderId))
            {
                return PerformCrossDaoFileCopy(fileId, toFolderId, false);
            }

            return selector.GetFileDao(fileId).CopyFile(selector.ConvertId(fileId), selector.ConvertId(toFolderId));
        }

        public object FileRename(object fileId, string newTitle)
        {
            var selector = GetSelector(fileId);
            return selector.GetFileDao(fileId).FileRename(selector.ConvertId(fileId), newTitle);
        }

        public bool UseTrashForRemove(File file)
        {
            var selector = GetSelector(file.ID);
            return selector.GetFileDao(file.ID).UseTrashForRemove(file);
        }

        #region Only in TMFileDao

        public IEnumerable<File> Search(string text, FolderType folderType)
        {
            return TryGetFileDao().Search(text, folderType);
        }

        public List<File> GetUpdates(DateTime from, DateTime to)
        {
            return TryGetFileDao().GetUpdates(from, to);
        }

        public void DeleteFileStream(object fileId)
        {
            var selector = GetSelector(fileId);
            selector.GetFileDao(fileId).DeleteFileStream(selector.ConvertId(fileId));
        }

        public void DeleteFolder(object fileId)
        {
            var selector = GetSelector(fileId);
            selector.GetFileDao(fileId).DeleteFolder(selector.ConvertId(fileId));
        }

        public bool IsExistOnStorage(File file)
        {
            var fileId = file.ID;
            var selector = GetSelector(fileId);
            file.ID = selector.ConvertId(fileId);
            var exist = selector.GetFileDao(fileId).IsExistOnStorage(file);
            file.ID = fileId; //Restore
            return exist;
        }

        #endregion
    }
}