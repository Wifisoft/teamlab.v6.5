#if DEBUG
namespace ASC.Web.Files.Tests
{
    using System;
    using System.Net;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class FilesTest : BaseTest
    {
        [Test]
        public void UploadFileTest()
        {
            var req = (HttpWebRequest)HttpWebRequest.Create("http://alex/asc/products/files/Services/WCFService/Service.svc/folders/files?id=1");

            req.Method = "POST";
            req.ContentType = "application/octet-stream";
            var reqStream = req.GetRequestStream();

            var fileToSend = System.IO.File.ReadAllBytes(@"c:\1.odt");

            reqStream.Write(fileToSend, 0, fileToSend.Length);
            reqStream.Close();

            var resp = (HttpWebResponse)req.GetResponse();

            var sr = new System.IO.StreamReader(resp.GetResponseStream(), Encoding.Default);
            var count = 0;
            var ReadBuf = new char[1024];
            do
            {
                count = sr.Read(ReadBuf, 0, 1024);
                if (0 != count)
                {
                    Console.WriteLine(new string(ReadBuf));
                }

            } while (count > 0);
            Console.WriteLine("Client: Receive Response HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
        }

        [Test]
        public void SaveFileTest()
        {
            Console.WriteLine(FileDAO.SaveFile(DemoData.SampleFile));
        }

        [Test]
        public void GetFileTest()
        {
            var file = FileDAO.GetFile(DemoData.SampleFile.ID, DemoData.SampleFile.Version);
            Console.WriteLine(file.Title);
        }

        [Test]
        public void GetLastVerisonTest()
        {
            var file = FileDAO.GetFile(DemoData.SampleFile.ID);
            Console.WriteLine(file.Title);
        }

        [Test]
        public void DeleteFileWithVersionsTest(int fileID)
        {
            // FileDAO.DeleteFileWithVersions(DemoData.SampleFile.ID);
            var file = FileDAO.GetFile(DemoData.SampleFile.ID);
            Assert.IsNull(file, "Удаление прошло корректно");
        }
    }
}
#endif