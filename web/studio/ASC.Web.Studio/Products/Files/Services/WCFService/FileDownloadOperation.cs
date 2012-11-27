using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ICSharpCode.SharpZipLib.Zip;

namespace ASC.Web.Files.Services.WCFService
{
    internal class FileDownloadOperation : FileOperation
    {
        protected override FileOperationType OperationType
        {
            get { return FileOperationType.Download; }
        }


        public FileDownloadOperation(Tenant tenant, List<object> folders, List<object> files)
            : base(tenant, folders, files)
        {
            Id = Owner.ToString() + OperationType.ToString(); //one download per user
        }


        protected override void Do()
        {
            var files = GetFiles();
            if (files == null || files.Count == 0)
            {
                throw new Exception(FilesCommonResource.ErrorMassage_FileNotFound);
            }

            ReplaceLongTitles(files);

            using (var stream = CompressToZip(files))
            {
                if (stream != null)
                {
                    stream.Position = 0;
                    var fileName = UrlConstant.DownloadTitle + ".zip";
                    Store.Save(
                        FileConstant.StorageDomainTmp,
                        string.Format(@"{0}\{1}", Owner, fileName),
                        stream,
                        "application/zip",
                        "attachment; filename=\"" + fileName + "\"");
                    Status = string.Format("{0}?{1}=bulk", FileHandler.FileHandlerPath, UrlConstant.Action);
                }
            }
        }

        private NameValueCollection GetFiles()
        {
            var result = new NameValueCollection();
            if (0 < Files.Count)
            {
                var files = FilesSecurity.FilterRead(FileDao.GetFiles(Files.ToArray())).ToList();
                files.ForEach(f => result.Add(f.Title, f.ID.ToString()));
                TagDao.RemoveTags(files.Select(f => Tag.New(SecurityContext.CurrentAccount.ID, f)).ToArray());
            }
            if (0 < Folders.Count)
            {
                Folders.ForEach(f => TagDao.RemoveTags(TagDao.GetTags(f, FileEntryType.Folder, TagType.New)
                                                           .Where(t => t.Owner == SecurityContext.CurrentAccount.ID)
                                                           .ToArray()));

                var filesInFolder = GetFilesInFolders(Folders, string.Empty);
                if (filesInFolder == null) return null;
                result.Add(filesInFolder);
            }
            return result;
        }

        private NameValueCollection GetFilesInFolders(IList<object> folderIds, string path)
        {
            if (Canceled) return null;

            var result = new NameValueCollection();
            foreach (var folderId in folderIds)
            {
                var folder = FolderDao.GetFolder(folderId);
                if (folder == null || !FilesSecurity.CanRead(folder)) continue;
                var folderPath = path + folder.Title + "/";

                var files = FilesSecurity.FilterRead(FolderDao.GetFiles(folder.ID, null, FilterType.None, Guid.Empty, string.Empty)).ToList();
                files.ForEach(f => result.Add(folderPath + f.Title, f.ID.ToString()));
                TagDao.RemoveTags(files.Select(f => Tag.New(SecurityContext.CurrentAccount.ID, f)).ToArray());

                var nestedFolders = FilesSecurity.FilterRead(FolderDao.GetFolders(folder.ID)).ToList();
                if (files.Count == 0 && nestedFolders.Count == 0)
                {
                    result.Add(folderPath, String.Empty);
                }

                var filesInFolder = GetFilesInFolders(nestedFolders.ConvertAll(f => f.ID), folderPath);
                if (filesInFolder == null) return null;
                result.Add(filesInFolder);
            }
            return result;
        }

        private Stream CompressToZip(NameValueCollection entries)
        {
            var stream = TempStream.Create();
            using (var zip = new ZipOutputStream(stream))
            {
                zip.IsStreamOwner = false;
                zip.SetLevel(3);
                zip.UseZip64 = UseZip64.Off;

                foreach (var title in entries.AllKeys)
                {
                    if (Canceled)
                    {
                        zip.Dispose();
                        stream.Dispose();
                        return null;
                    }

                    var counter = 0;
                    foreach (var path in entries[title].Split(','))
                    {
                        var newtitle = title;
                        if (0 < counter)
                        {
                            var suffix = " (" + counter + ")";
                            newtitle = 0 < newtitle.IndexOf('.')
                                           ? newtitle.Insert(newtitle.LastIndexOf('.'), suffix)
                                           : newtitle + suffix;
                        }
                        var zipentry = new ZipEntry(newtitle) {DateTime = DateTime.UtcNow};
                        lock (zip)
                        {
                            ZipConstants.DefaultCodePage = Thread.CurrentThread.CurrentCulture.TextInfo.OEMCodePage;
                            zip.PutNextEntry(zipentry);
                        }
                        if (!string.IsNullOrEmpty(path))
                        {
                            var file = Global.DaoFactory.GetFileDao().GetFile(path);
                            if (file.ConvertedType != null)
                            {
                                //Take from converter
                                try
                                {
                                    using (var readStream = DocumentUtils.GetConvertedFile(file))
                                    {
                                        if (readStream != null)
                                        {
                                            readStream.StreamCopyTo(zip);
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                using (var readStream = Global.DaoFactory.GetFileDao().GetFileStream(file))
                                {
                                    readStream.StreamCopyTo(zip);
                                }
                            }
                        }
                        counter++;
                    }
                    lock (zip)
                    {
                        ZipConstants.DefaultCodePage = Thread.CurrentThread.CurrentCulture.TextInfo.OEMCodePage;
                        zip.CloseEntry();
                    }
                    ProgressStep();
                }
                return stream;
            }
        }

        private void ReplaceLongTitles(NameValueCollection files)
        {
            foreach (var title in new List<string>(files.AllKeys))
            {
                if (200 < title.Length && 0 < title.IndexOf('/'))
                {
                    var path = files[title];
                    files.Remove(title);

                    var newtitle = "LONG_FOLDER_NAME" + title.Substring(title.LastIndexOf('/'));
                    files.Add(newtitle, path);
                }
            }
        }
    }
}