using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Projects.Core.Domain;
using ASC.Web.Files.Api;
using ASC.Web.Studio.Helpers;
using ASC.Web.Studio.Utility;
using log4net;
using File = ASC.Files.Core.File;
using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Projects.Engine
{
    public class FileEngine
    {
        private readonly IDataStore _projectsStore;

        public FileEngine(IDataStore projectsStore)
        {
            _projectsStore = projectsStore;
        }

        public object GetRoot(int projectId)
        {
            return FilesIntegration.RegisterBunch("projects", "project", projectId.ToString());
        }

        public File GetFile(object id, int version)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                var file = 0 < version ? dao.GetFile(id, version) : dao.GetFile(id);
                return file;
            }
        }

        public File SaveFile(File file, Stream stream)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                return dao.SaveFile(file, stream);
            }
        }

        public void RemoveFile(object id)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                dao.DeleteFolder(id);
                dao.DeleteFile(id);
                try
                {
                    _projectsStore.Delete(GetThumbPath(id));
                }
                catch
                {
                }
            }
        }

        public Folder SaveFolder(Folder folder)
        {
            using (var dao = FilesIntegration.GetFolderDao())
            {
                folder.ID = dao.SaveFolder(folder);
                return folder;
            }
        }

        public void AttachFileToMessage(int messageId, object fileId)
        {
            using (var dao = FilesIntegration.GetTagDao())
            {
                dao.SaveTags(new Tag("Message" + messageId, TagType.System, Guid.Empty) {EntryType = FileEntryType.File, EntryId = fileId});
                GenerateImageThumb(GetFile(fileId, 0));
            }
        }

        public void AttachFileToTask(int taskId, int fileId)
        {
            using (var dao = FilesIntegration.GetTagDao())
            {
                dao.SaveTags(new Tag("Task" + taskId, TagType.System, Guid.Empty) {EntryType = FileEntryType.File, EntryId = fileId});
                GenerateImageThumb(GetFile(fileId, 0));
            }
        }

        public void DetachFileFromTask(int taskId, int fileId)
        {
            using (var dao = FilesIntegration.GetTagDao())
            {
                dao.RemoveTags(new Tag("Task" + taskId, TagType.System, Guid.Empty) {EntryType = FileEntryType.File, EntryId = fileId});
            }
        }

        public List<File> GetTaskFiles(Task task)
        {
            if (task == null) return new List<File>();

            using (var tagdao = FilesIntegration.GetTagDao())
            using (var filedao = FilesIntegration.GetFileDao())
            {
                var ids = tagdao.GetTags("Task" + task.ID, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).ToArray();
                var files = 0 < ids.Length ? filedao.GetFiles(ids) : new List<File>();
                files.ForEach(r => r.Access = GetFileShare(r, task.Project.ID));
                SetThumbUrls(files);
                return files;
            }
        }

        public List<File> GetMessageFiles(Message message)
        {
            if (message == null) return new List<File>();
            using (var tagdao = FilesIntegration.GetTagDao())
            using (var filedao = FilesIntegration.GetFileDao())
            {
                var ids = tagdao.GetTags("Message" + message.ID, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).ToArray();
                var files = 0 < ids.Length ? filedao.GetFiles(ids) : new List<File>();
                files.ForEach(r => r.Access = GetFileShare(r, message.Project.ID));
                SetThumbUrls(files);
                return files;
            }
        }

        private void SetThumbUrls(List<File> files)
        {
            files.ForEach(f =>
                              {
                                  if (f != null && FileUtility.GetFileTypeByFileName(f.Title) == FileType.Image)
                                  {
                                      f.ThumbnailURL = _projectsStore.GetUri(GetThumbPath(f.ID)).ToString();
                                  }
                              });
        }

        private void GenerateImageThumb(File file)
        {
            if (file == null || FileUtility.GetFileTypeByFileName(file.Title) != FileType.Image) return;

            try
            {
                using (var filedao = FilesIntegration.GetFileDao())
                {
                    using (var stream = filedao.GetFileStream(file))
                    {
                        var ii = new ImageInfo();
                        ImageHelper.GenerateThumbnail(stream, GetThumbPath(file.ID), ref ii, 128, 96, _projectsStore);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("ASC.Web.Projects").Error(ex);
            }
        }

        private static string GetThumbPath(object fileId)
        {
            var s = fileId.ToString().PadRight(6, '0');
            return "thumbs/" + s.Substring(0, 2) + "/" + s.Substring(2, 2) + "/" + s.Substring(4) + "/" + fileId.ToString() + ".jpg";
        }

        internal static Hashtable GetFileListInfoHashtable(IEnumerable<File> uploadedFiles)
        {
            var fileListInfoHashtable = new Hashtable();

            if (uploadedFiles != null)
                foreach (var file in uploadedFiles)
                {
                    var fileInfo = String.Format("{0} ({1})", file.Title, Path.GetExtension(file.Title).ToUpper());
                    fileListInfoHashtable.Add(fileInfo, file.ViewUrl);
                }

            return fileListInfoHashtable;
        }

        public List<File> GetEntityFiles(ProjectEntity entity)
        {
            if (entity == null) return new List<File>();

            using (var tagdao = FilesIntegration.GetTagDao())
            using (var filedao = FilesIntegration.GetFileDao())
            {
                var ids = tagdao.GetTags(entity.GetType().Name + entity.ID, TagType.System).Where(t => t.EntryType == FileEntryType.File).Select(t => t.EntryId).ToArray();
                var files = 0 < ids.Length ? filedao.GetFiles(ids) : new List<File>();
                files.ForEach(r => r.Access = GetFileShare(r, entity.Project.ID));
                SetThumbUrls(files);
                return files;
            }
        }

        public void AttachFileToEntity(EntityType entityType, int entityId, object fileId)
        {
            using (var dao = FilesIntegration.GetTagDao())
            {
                dao.SaveTags(new Tag(entityType.ToString() + entityId, TagType.System, Guid.Empty) {EntryType = FileEntryType.File, EntryId = fileId});
                GenerateImageThumb(GetFile(fileId, 0));
            }
        }

        public void DetachFileFromEntity(EntityType entityType, int entityId, object fileId)
        {
            using (var dao = FilesIntegration.GetTagDao())
            {
                dao.RemoveTags(new Tag(entityType.ToString() + entityId, TagType.System, Guid.Empty) {EntryType = FileEntryType.File, EntryId = fileId});
            }
        }

        public bool CanCreate(FileEntry file, int projectId)
        {
            return GetFileSecurity(projectId).CanCreate(file, SecurityContext.CurrentAccount.ID);
        }

        public bool CanDelete(FileEntry file, int projectId)
        {
            return GetFileSecurity(projectId).CanDelete(file, SecurityContext.CurrentAccount.ID);
        }

        public bool CanEdit(FileEntry file, int projectId)
        {
            return GetFileSecurity(projectId).CanEdit(file, SecurityContext.CurrentAccount.ID);
        }

        public bool CanRead(FileEntry file, int projectId)
        {
            return GetFileSecurity(projectId).CanRead(file, SecurityContext.CurrentAccount.ID);
        }


        private IFileSecurity GetFileSecurity(int projectId)
        {
            return SecurityAdapterProvider.GetFileSecurity(projectId);
        }

        private FileShare GetFileShare(FileEntry file, int projectId)
        {
            if (!CanRead(file, projectId)) return FileShare.Restrict;
            if (!CanCreate(file, projectId) || !CanEdit(file, projectId)) return FileShare.Read;
            if (!CanDelete(file, projectId)) return FileShare.ReadWrite;

            return FileShare.None;
        }
    }
}