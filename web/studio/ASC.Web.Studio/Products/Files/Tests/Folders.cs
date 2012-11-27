#if DEBUG
namespace ASC.Web.Files.Tests
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using ASC.Core;
    using ASC.Files.Core;
    using NUnit.Framework;

    [TestFixture]
    public class FoldersTest : BaseTest
    {

        [Test]
        public void CopyFoldersTest()
        {
            FolderDAO.CopyFolder(160, 180);
        }


        [Test]
        public void MoveFoldersCheck()
        {
            FolderDAO.CanMoveOrCopy(new object[] { 19 }, 15).ToList().ForEach(item => Console.WriteLine(item));
        }

        [Test]
        public void GetFavoritesTest()
        {
            var xElement = XElement.Load(@"c:\Works\Teamlab.com\web\studio\ASC.Web.Studio\Products\Files\tips.xml");
            var rnd = new Random();
            var query = (from c in xElement.Descendants("tip") select c).ToList();
            var temp = query[0];// rnd.Next(0, query.Count);
            Console.WriteLine(query);
        }

        [Test]
        public void GetFolderPath()
        {
            var parts = FolderDAO.GetParentFolders(15);
        }


        [Test]
        public void GetFilesCountTest()
        {
            //  int count =   FolderDAO.GetFilesCount(1);
            //  Console.WriteLine(count);
        }

        [Test]
        public void GetFolderByIDTest()
        {
            var folder = FolderDAO.GetFolder(1);
            Console.WriteLine(folder.Title);
        }

        [Test]
        public void SaveOrUpdateFolderTest()
        {
            SecurityContext.AuthenticateMe("alex.bannov@avsmedia.com", "111111");

            var commonDocumentsID = FolderDAO.GetFolderIDCommon(false);
            object folderID = 0;

            DemoData.ListFolders[0].ParentFolderID = commonDocumentsID;
            DemoData.ListFolders[1].ParentFolderID = commonDocumentsID;

            folderID = FolderDAO.SaveFolder(DemoData.ListFolders[0]);
            folderID = FolderDAO.SaveFolder(DemoData.ListFolders[1]);

            DemoData.ListFolders[2].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[2]);

            DemoData.ListFolders[3].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[3]);

            DemoData.ListFolders[4].ParentFolderID = commonDocumentsID;
            folderID = FolderDAO.SaveFolder(DemoData.ListFolders[4]);

            DemoData.ListFolders[5].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[5]);
            DemoData.ListFolders[6].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[6]);

            DemoData.ListFolders[7].ParentFolderID = commonDocumentsID;
            folderID = FolderDAO.SaveFolder(DemoData.ListFolders[7]);

            DemoData.ListFolders[8].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[8]);
            DemoData.ListFolders[9].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[9]);

            DemoData.ListFolders[10].ParentFolderID = commonDocumentsID;
            folderID = FolderDAO.SaveFolder(DemoData.ListFolders[10]);

            DemoData.ListFolders[11].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[11]);

            DemoData.ListFolders[12].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[12]);
            DemoData.ListFolders[13].ParentFolderID = folderID;
            folderID = FolderDAO.SaveFolder(DemoData.ListFolders[13]);

            DemoData.ListFolders[14].ParentFolderID = folderID;
            folderID = FolderDAO.SaveFolder(DemoData.ListFolders[14]);

            DemoData.ListFolders[15].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[15]);

            DemoData.ListFolders[16].ParentFolderID = folderID;
            FolderDAO.SaveFolder(DemoData.ListFolders[16]);

            Console.WriteLine(folderID);
        }

        [Test]
        public void DeleteFolderTest()
        {
            // FolderDAO.DeleteFolder(1);
            var folder = FolderDAO.GetFolder(1);
            Assert.IsNull(folder, "Папка не удалена");
        }
    }
}
#endif