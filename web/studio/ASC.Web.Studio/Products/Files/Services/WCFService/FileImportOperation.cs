using System;
using System.Collections.Generic;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Import;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Core;

namespace ASC.Web.Files.Services.WCFService
{
    internal class FileImportOperation : FileOperation
    {
        private readonly IDocumentProvider _docProvider;
        private readonly List<DataToImport> _files;
        private readonly object _parentId;
        private readonly bool _overwrite;
        private readonly string _folderName;


        protected override FileOperationType OperationType
        {
            get { return FileOperationType.Import; }
        }


        public FileImportOperation(Tenant tenant, IDocumentProvider docProvider, List<DataToImport> files, object parentId, bool overwrite, string folderName)
            : base(tenant, null, null)
        {
            Id = Owner.ToString() + OperationType.ToString();
            Source = docProvider.Name;
            this._docProvider = docProvider;
            this._files = files ?? new List<DataToImport>();
            this._parentId = parentId;
            this._overwrite = overwrite;
            this._folderName = folderName;
        }


        protected override double InitProgressStep()
        {
            return _files.Count == 0 ? 100d : 100d/_files.Count;
        }

        protected override void Do()
        {
            if (_files.Count == 0) return;

            var parent = FolderDao.GetFolder(_parentId);
            if (parent == null) throw new Exception(FilesCommonResource.ErrorMassage_FolderNotFound);
            if (!FilesSecurity.CanCreate(parent)) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
            if (parent.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ImportToTrash);
            if (!string.IsNullOrEmpty(parent.ProviderName)) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);

            var to = FolderDao.GetFolder(_folderName, _parentId);
            if (to == null)
            {
                to = new Folder
                         {
                             FolderType = FolderType.DEFAULT,
                             ParentFolderID = _parentId,
                             Title = _folderName
                         };
                to.ID = FolderDao.SaveFolder(to);
            }

            foreach (var f in _files)
            {
                if (Canceled) return;
                try
                {
                    long size;
                    using (var stream = _docProvider.GetDocumentStream(f.ContentLink, out size))
                    {
                        if (stream == null)
                            throw new Exception("Can not import document " + f.ContentLink + ". Empty stream.");

                        if (SetupInfo.MaxUploadSize < size)
                        {
                            throw FileSizeComment.FileSizeException;
                        }

                        var folderId = to.ID;
                        var pos = f.Title.LastIndexOf('/');
                        if (0 < pos)
                        {
                            folderId = GetOrCreateHierarchy(f.Title.Substring(0, pos), to);
                            f.Title = f.Title.Substring(pos + 1);
                        }

                        f.Title = Global.ReplaceInvalidCharsAndTruncate(f.Title);
                        var file = new File
                                       {
                                           Title = f.Title,
                                           FolderID = folderId,
                                           ContentLength = size,
                                           ContentType = "application/octet-stream",
                                       };

                        var conflict = FileDao.GetFile(file.FolderID, file.Title);
                        if (conflict != null)
                        {
                            if (_overwrite)
                            {
                                file.ID = conflict.ID;
                                file.Version = conflict.Version + 1;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (size <= 0L)
                        {
                            using (var buffered = stream.GetBuffered())
                            {
                                size = buffered.Length;

                                if (SetupInfo.MaxUploadSize < size)
                                {
                                    throw FileSizeComment.FileSizeException;
                                }

                                file.ContentLength = size;
                                try
                                {
                                    file = FileDao.SaveFile(file, buffered);
                                }
                                catch (Exception error)
                                {
                                    FileDao.DeleteFile(file.ID);
                                    throw error;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                file = FileDao.SaveFile(file, stream);
                            }
                            catch (Exception error)
                            {
                                FileDao.DeleteFile(file.ID);
                                throw error;
                            }
                        }

                        if (conflict != null)
                        {
                            Global.PublishUpdateDocument(file.ID);
                        }
                        else
                        {
                            FilesActivityPublisher.UploadFile(FileDao.GetFile(file.ID));
                        }
                    }
                }
                catch (Exception error)
                {
                    Error = error;
                }
                finally
                {
                    ProgressStep();
                }
            }
        }

        private object GetOrCreateHierarchy(string path, Folder root)
        {
            path = path != null ? path.Trim('/') : null;
            if (string.IsNullOrEmpty(path)) return root.ID;

            var pos = path.IndexOf("/");
            var title = 0 < pos ? path.Substring(0, pos) : path;
            path = 0 < pos ? path.Substring(pos + 1) : null;

            title = Global.ReplaceInvalidCharsAndTruncate(title);

            var folder = FolderDao.GetFolder(title, root.ID);
            if (folder == null)
            {
                folder = new Folder
                             {
                                 ParentFolderID = root.ID,
                                 Title = title,
                                 FolderType = FolderType.DEFAULT
                             };
                folder.ID = FolderDao.SaveFolder(folder);
            }
            return GetOrCreateHierarchy(path, folder);
        }
    }
}