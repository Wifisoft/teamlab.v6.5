#if DEBUG
namespace ASC.Web.Files.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using NUnit.Framework;

    [TestFixture]
    public class WCFServieTest : BaseTest
    {

        [Test]
        public void SaveOrUpdateFileTest()
        {

            Console.WriteLine("testservice.svc/create POST:");
            Console.WriteLine("-----------------------");

            Uri address = new Uri("http://alex/asc/products/files/Services/WCFService/Service.svc/folders/put");

            // Create the web request  
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

            // Set type to POST  
            request.Method = "POST";
            request.ContentType = "application/xml; charset=utf-8";

            // Create the data we want to send  
            string data =
@"<folder>
    <parent_folder_id>14</parent_folder_id>
    <title>oijoijoij</title>
    <container_id>00000000-0000-0000-0000-000000000000</container_id>
</folder>";

            // Create a byte array of the data we want to send  
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data);

            // Set the content length in the request headers  
            request.ContentLength = byteData.Length;

            // Write data  
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Console application output  
                Console.WriteLine(reader.ReadToEnd());
            }

            Console.WriteLine();
            Console.WriteLine();

        }


    }
}
#endif