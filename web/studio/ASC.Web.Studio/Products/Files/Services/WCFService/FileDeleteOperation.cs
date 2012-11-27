using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Web.Files.Services.NotifyService;
using ASC.Web.Files.Resources;

namespace ASC.Web.Files.Services.WCFService
{
    internal class FileDeleteOperation : FileOperation
    {
        private object _trashId;


        protected override FileOperationType OperationType
        {
            get { return FileOperationType.Delete; }
        }

        public FileDeleteOperation(Tenant tenant, List<object> folders, List<object> files)
            : base(tenant, folders, files)
        {
        }

        protected override void Do()
        {
            _trashId = FolderDao.GetFolderIDTrash(true);
            Folder root = null;
            if (0 < Folders.Count)
            {
                root = FolderDao.GetRootFolder(Folders[0]);
            }
            else if (0 < Files.Count)
            {
                root = FolderDao.GetRootFolderByFile(Files[0]);
            }
            if (root != null)
            {
                Status += string.Format("folder_{0}{1}", root.ID, splitCharacter);
            }

            DeleteFiles(Files);
            DeleteFolders(Folders);
        }

        private void DeleteFolders(List<object> folderIds)
        {
            if (folderIds.Count == 0) return;

            foreach (var folderId in folderIds)
            {
                if (Canceled) return;

                var folder = FolderDao.GetFolder(folderId);
                if (folder == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FolderNotFound;
                }
                else if (!FilesSecurity.CanDelete(folder))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder;
                }
                else
                {
                    if (FolderDao.UseTrashForRemove(folder))
                    {
                        var files = FolderDao.GetFiles(folder.ID, true);
                        if (files.Exists(fid => (File.GetFileStatus(fid, FileStatus.None) & FileStatus.IsEditing) == FileStatus.IsEditing))
                        {
                            Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteEditingFolder;
                        }
                        else
                        {
                            FolderDao.MoveFolder(folder.ID, _trashId);
                            files.ForEach(fid => NoticeDelete(FileDao.GetFile(fid)));
                            ProcessedFolder(folderId);
                            TagDao.RemoveTags(TagDao.GetTags(folder.ID, FileEntryType.Folder, TagType.New).ToArray());
                        }
                    }
                    else
                    {
                        if (FolderDao.UseRecursiveOperation(folder.ID, null))
                        {
                            DeleteFiles(FolderDao.GetFiles(folder.ID, false));
                            DeleteFolders(FolderDao.GetFolders(folder.ID).Select(f => f.ID).ToList());

                            if (FolderDao.GetItemsCount(folder.ID, true) == 0)
                            {
                                FolderDao.DeleteFolder(folder.ID);
                                ProcessedFolder(folderId);
                            }
                            else
                            {
                                Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteEditingFolder;
                            }
                        }
                        else
                        {
                            FolderDao.DeleteFolder(folder.ID);
                            ProcessedFolder(folderId);
                        }
                    }
                }
                ProgressStep();
            }
        }

        private void DeleteFiles(List<object> fileIds)
        {
            if (fileIds.Count == 0) return;

            foreach (var fileId in fileIds)
            {
                if (Canceled) return;

                var file = FileDao.GetFile(fileId);
                if (file == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FileNotFound;
                }
                else if ((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing)
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteEditingFile;
                }
                else if (!FilesSecurity.CanDelete(file))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFile;
                }
                else
                {
                    if (FileDao.UseTrashForRemove(file))
                    {
                        FileDao.MoveFile(file.ID, _trashId);
                        NoticeDelete(file);
                    }
                    else
                    {
                        FileDao.DeleteFile(file.ID);
                        FileDao.DeleteFolder(file.ID);
                    }
                    ProcessedFile(fileId);
                }
                ProgressStep();
            }
        }

        private void NoticeDelete(File file)
        {
            TagDao.RemoveTags(NotifySource.Instance.GetSubscriptionProvider()
                                  .GetRecipients(NotifyConstants.Event_UpdateDocument, file.UniqID)
                                  .Select(id => Tag.New(new Guid(id.ID), file))
                                  .ToArray());

            NotifySource.Instance.GetSubscriptionProvider().UnSubscribe(NotifyConstants.Event_UpdateDocument, file.UniqID);
        }
    }
}