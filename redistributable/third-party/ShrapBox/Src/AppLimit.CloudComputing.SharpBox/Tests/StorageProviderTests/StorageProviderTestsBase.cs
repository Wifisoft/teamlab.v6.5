using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace AppLimit.CloudComputing.SharpBox.Tests.StorageProviderTests
{
    [TestFixture]
    public abstract class StorageProviderTestsBase
    {
        protected abstract CloudStorage CreateStorage();
        protected abstract ICloudStorageAccessToken GetAccessToken();

        private readonly List<ICloudFileSystemEntry> _toDelete = new List<ICloudFileSystemEntry>();
        private ICloudDirectoryEntry _root;
        private ICloudFileSystemEntry _uploadedFile;
        private CloudStorage _storage;

        private const String DownloadPath = @"%USERPROFILE%\Desktop\";

        [TestFixtureTearDown]
        public virtual void TestTearDown()
        {
            foreach (var entry in _toDelete.ToArray())
            {
                GetStorage().DeleteFileSystemEntry(entry);
            }

            if(_uploadedFile != null)
            {
                GetStorage().DeleteFileSystemEntry(_uploadedFile);
            }

            if (File.Exists(GetDownloadedFilePath()))
            {
                File.Delete(GetDownloadedFilePath());   
            }
        }

        [Test]
        public virtual void DeleteResourceTest()
        {
            var folder = GetStorage().CreateFolder("testfolder", GetRoot());
            Assert.That(GetStorage().DeleteFileSystemEntry(folder));
        }

        [Test]
        public virtual void DeleteUploadedFileTest()
        {
            var file = UploadTestFileInternal();
            Assert.That(GetStorage().DeleteFileSystemEntry(file));
            _uploadedFile = null;
        }

        [Test]
        public virtual void UploadFileTest()
        {
            var file = UploadTestFileInternal();
            Assert.AreNotEqual(0, file.Length);
        }

        [Test]
        public virtual void DownloadFileTest()
        {
            var file = UploadTestFileInternal();
            GetStorage().DownloadFile(file.Parent, file.Id, DownloadPath);
            Assert.That(File.Exists(GetDownloadedFilePath()));
        }

        [Test]
        public virtual void GetFileInfoTest()
        {
            var file1 = UploadTestFileInternal();
            var file2 = GetStorage().GetFile('/' + file1.Id, file1.Parent);
            Assert.AreEqual(file1.Id, file2.Id);
        }

        [Test]
        public virtual void GetFolderInfoTest()
        {
            var folder1 = GetStorage().CreateFolder("testfolder", GetRoot());
            var folder2 = GetStorage().GetFolder("/" + folder1.Id);
            Assert.AreEqual(folder1.Id, folder2.Id);
        }

        [Test]
        public virtual void MoveTest()
        {
            var folder1 = GetStorage().CreateFolder("testfolder1", GetRoot());
            var folder2 = GetStorage().CreateFolder("testfolder2", GetRoot());
            _toDelete.Add(folder1);
            _toDelete.Add(folder2);
            Assert.That(GetStorage().MoveFileSystemEntry(folder1, folder2));
            Assert.IsNull(GetRoot().GetChild(folder1.Id, false));
            Assert.IsNotNull(folder2.GetChild(folder1.Id, false));
        }

        [Test]
        public virtual void CreateFolderTest()
        {
            var folder = GetStorage().CreateFolder("testfolder", GetRoot());
            _toDelete.Add(folder);
            Assert.IsNotNull(GetRoot().GetChild(folder.Id));
        }

        [Test]
        public virtual void RenameResourceTest()
        {
            var folder = GetStorage().CreateFolder("testfolder", GetRoot());
            _toDelete.Add(folder);
            Assert.IsTrue(GetStorage().RenameFileSystemEntry(folder, "testfolder_renamed"));
            Assert.AreEqual("testfolder_renamed", folder.Name);
        }

        protected virtual String GetTestFilePath()
        {
            return @"Tests\StorageProviderTests\" + GetTestFileName();
        }

        protected virtual String GetTestFileName()
        {
            return "testtesttesttest.txt";
        }

        protected CloudStorage GetStorage()
        {
            return _storage ?? (_storage = CreateStorage());
        }

        protected ICloudDirectoryEntry GetRoot()
        {
            return _root = GetStorage().GetRoot();
        }

        protected ICloudFileSystemEntry UploadTestFileInternal()
        {
            if (_uploadedFile == null)
            {
                using (var fs = new FileStream(GetTestFilePath(), FileMode.Open, FileAccess.Read))
                {
                    var file = GetStorage().CreateFile(GetRoot(), "testtesttesttest.txt");
                    file.GetDataTransferAccessor().Transfer(fs, nTransferDirection.nUpload);
                    _uploadedFile = file;
                }
            }
            return _uploadedFile;
        }

        private String GetDownloadedFilePath()
        {
            return Environment.ExpandEnvironmentVariables(DownloadPath + GetTestFileName());
        }
    }
}
