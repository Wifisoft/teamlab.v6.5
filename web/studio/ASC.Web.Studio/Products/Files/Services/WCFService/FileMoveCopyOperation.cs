using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Services.NotifyService;

namespace ASC.Web.Files.Services.WCFService
{
    internal class FileMoveCopyOperation : FileOperation
    {
        private readonly object _toFolder;
        private readonly bool _copy;
        private readonly FileConflictResolveType _resolveType;


        protected override FileOperationType OperationType
        {
            get { return _copy ? FileOperationType.Copy : FileOperationType.Move; }
        }


        public FileMoveCopyOperation(Tenant tenant, List<object> folders, List<object> files, object toFolder, bool copy, FileConflictResolveType resolveType)
            : base(tenant, folders, files)
        {
            this._toFolder = toFolder;
            this._copy = copy;
            this._resolveType = resolveType;
        }


        protected override void Do()
        {
            Status += string.Format("folder_{0}{1}", _toFolder, splitCharacter);

            //TODO: check on each iteration?
            var to = FolderDao.GetFolder(_toFolder);
            if (to == null) return;
            if (!FilesSecurity.CanCreate(to)) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);

            if (_copy)
            {
                Folder rootFrom = null;
                if (0 < Folders.Count) rootFrom = FolderDao.GetRootFolder(Folders[0]);
                if (0 < Files.Count) rootFrom = FolderDao.GetRootFolderByFile(Files[0]);
                if (rootFrom != null && rootFrom.FolderType == FolderType.TRASH) throw new InvalidOperationException("Can not copy from Trash.");
                if (to.RootFolderType == FolderType.TRASH) throw new InvalidOperationException("Can not copy to Trash.");
            }

            MoveOrCopyFolders(Folders, _toFolder, _copy);
            MoveOrCopyFiles(Files, _toFolder, _copy, _resolveType);
        }

        private void MoveOrCopyFolders(List<object> folderIds, object to, bool copy)
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
                else if (!FilesSecurity.CanRead(folder))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFolder;
                }
                else if (!Equals((folder.ParentFolderID ?? string.Empty).ToString(), to))
                {
                    //if destination folder contains folder with same name then merge folders
                    var conflictFolder = FolderDao.GetFolder(folder.Title, to);

                    if (copy || conflictFolder != null)
                    {
                        object newFolder;
                        if (conflictFolder != null)
                        {
                            newFolder = conflictFolder.ID;
                        }
                        else
                        {
                            newFolder = FolderDao.CopyFolder(folder.ID, to);
                            ProcessedFolder(folderId);
                        }

                        if (FolderDao.UseRecursiveOperation(folder,to))
                        {
                            MoveOrCopyFiles(FolderDao.GetFiles(folder.ID, false), newFolder, copy, _resolveType);
                            MoveOrCopyFolders(FolderDao.GetFolders(folder.ID).Select(f => f.ID).ToList(), newFolder, copy);

                            if (!copy && conflictFolder != null)
                            {
                                if (FolderDao.GetItemsCount(folder.ID, true) == 0 && FilesSecurity.CanDelete(folder))
                                {
                                    FolderDao.DeleteFolder(folder.ID);
                                    ProcessedFolder(folderId);
                                }
                            }
                        }
                        else
                        {
                            if (conflictFolder != null)
                            {
                                if (copy)
                                {
                                    FolderDao.CopyFolder(folder.ID, to);
                                }
                                else
                                {
                                    FolderDao.MoveFolder(folder.ID, to);
                                }
                                ProcessedFolder(folderId);
                            }
                        }

                    }
                    else
                    {
                        if (FilesSecurity.CanDelete(folder))
                        {
                            FolderDao.MoveFolder(folder.ID, to);
                            ProcessedFolder(folderId);
                        }
                        else
                        {
                            Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder;
                        }
                    }
                }
                ProgressStep();
            }
        }

        private void MoveOrCopyFiles(List<object> fileIds, object to, bool copy, FileConflictResolveType resolveType)
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
                else if (!FilesSecurity.CanRead(file))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFile;
                }
                else if (!Equals(file.FolderID.ToString(), to))
                {
                    var conflict = FileDao.GetFile(to, file.Title);
                    if (conflict != null && !FilesSecurity.CanEdit(conflict))
                    {
                        Error = FilesCommonResource.ErrorMassage_SecurityException;
                    }
                    else if (conflict == null)
                    {
                        if (copy)
                        {
                            File newFile = null;
                            try
                            {
                                newFile = FileDao.CopyFile(file.ID, to); //Stream copy will occur inside dao
                                ProcessedFile(fileId);
                            }
                            catch
                            {
                                if (newFile != null) FileDao.DeleteFile(newFile.ID);
                                throw;
                            }
                        }
                        else
                        {
                            if ((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing)
                            {
                                Error = FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile;
                            }
                            else if (FilesSecurity.CanDelete(file))
                            {
                                FileDao.MoveFile(file.ID, to);
                                ProcessedFile(fileId);
                            }
                            else
                            {
                                Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFile;
                            }
                        }
                    }
                    else
                    {
                        if (resolveType == FileConflictResolveType.Overwrite)
                        {
                            conflict.Version++;
                            using (var stream = FileDao.GetFileStream(file))
                            {
                                conflict = FileDao.SaveFile(conflict, stream);
                            }

                            Global.PublishUpdateDocument(conflict.ID);

                            if (copy)
                            {
                                ProcessedFile(fileId);
                            }
                            else
                            {
                                if ((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing)
                                {
                                    Error = FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile;
                                }
                                else if (FilesSecurity.CanDelete(file))
                                {
                                    NotifySource.Instance.GetSubscriptionProvider()
                                        .GetRecipients(NotifyConstants.Event_UpdateDocument, file.UniqID)
                                        .ToList()
                                        .ForEach(r => NotifySource.Instance.GetSubscriptionProvider().Subscribe(NotifyConstants.Event_UpdateDocument, conflict.UniqID, r));

                                    NotifySource.Instance.GetSubscriptionProvider().UnSubscribe(NotifyConstants.Event_UpdateDocument, file.UniqID);

                                    FileDao.DeleteFile(file.ID);
                                    FileDao.DeleteFolder(file.ID);
                                    ProcessedFile(fileId);
                                }
                                else
                                {
                                    Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFile;
                                }
                            }
                        }
                        else if (resolveType == FileConflictResolveType.Skip)
                        {
                            //nothing
                        }
                    }
                }
                ProgressStep();
            }
        }
    }
}