using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Xml.XPath;
using ASC.Files.Core;

namespace ASC.Web.Files.Import
{
    internal class ZohoDocumentProvider : IDocumentProvider
    {
        private readonly string _ticket;

        public string Name
        {
            get { return "zoho"; }
        }

        private const string Scheme = "https"; //Fix for may 7 2011.

        public ZohoDocumentProvider(AuthData authData)
        {
            _ticket = Authenticate(authData.Login, authData.Password);
        }

        public IEnumerable<Document> GetDocuments()
        {
            return GetWriters()
                .Concat(GetSheets())
                .Concat(GetShows());
        }

        public Stream GetDocumentStream(string contentLink, out long size)
        {
            var request =
                WebRequest.Create(contentLink +
                                  string.Format("?apikey={0}&ticket={1}", ImportConfiguration.ZohoApiKey, _ticket));
            var responce = request.GetResponse();

            if (responce == null) throw new IOException("Responce stream empty");

            size = responce.ContentLength;
            return responce.GetResponseStream();
        }

        private IEnumerable<Document> GetWriters()
        {
            var url = string.Format(
                Scheme + "://export.writer.zoho.com/api/private/xml/documents?apikey={0}&ticket={1}",
                ImportConfiguration.ZohoApiKey, _ticket);
            var documents = XDocument.Load(url);
            CheckErrorAndThrow(documents);

            return documents
                .XPathSelectElements("//document")
                .Where(d => d.Element("documentId") != null)
                .Select(d =>
                        new Document
                            {
                                Id = d.Element("documentId").Value,
                                ContentLink =
                                    Scheme + "://export.writer.zoho.com/api/private/docx/download/" +
                                    d.Element("documentId").Value,
                                Title = d.ElementValueOrDefault("documentName") + ".docx",
                                CreateBy = d.ElementValueOrDefault("authorName"),
                                CreateOn = Utils.FromUnixTime(long.Parse(d.ElementValueOrDefault("created_date"))),
                                IsFolder = false,
                            });
        }

        private IEnumerable<Document> GetSheets()
        {
            var url = string.Format(Scheme + "://sheet.zoho.com/api/private/xml/books?apikey={0}&ticket={1}",
                                    ImportConfiguration.ZohoApiKey, _ticket);
            var documents = XDocument.Load(url);
            CheckErrorAndThrow(documents);

            return documents
                .XPathSelectElements("//workbook")
                .Where(d => d.Element("workbookId") != null)
                .Select(d =>
                        new Document
                            {
                                Id = d.Element("workbookId").Value,
                                ContentLink =
                                    Scheme + "://sheet.zoho.com/api/private/xlsx/download/" +
                                    d.Element("workbookId").Value,
                                Title = d.ElementValueOrDefault("workbookName") + ".xlsx",
                                CreateBy = d.ElementValueOrDefault("ownerName"),
                                CreateOn = Utils.FromUnixTime(long.Parse(d.ElementValueOrDefault("createdTime"))),
                                IsFolder = false
                            });
        }

        private IEnumerable<Document> GetShows()
        {
            var url = string.Format(Scheme + "://show.zoho.com/api/private/xml/presentations?apikey={0}&ticket={1}",
                                    ImportConfiguration.ZohoApiKey, _ticket);
            var documents = XDocument.Load(url);
            CheckErrorAndThrow(documents);

            return documents
                .XPathSelectElements("//presentation")
                .Where(d => d.Element("presentationId") != null)
                .Select(d =>
                        new Document
                            {
                                Id = d.Element("presentationId").Value,
                                ContentLink =
                                    Scheme + "://show.zoho.com/api/private/ppt/download/" +
                                    d.Element("presentationId").Value,
                                Title = d.ElementValueOrDefault("presentationName") + ".ppt",
                                CreateBy = d.ElementValueOrDefault("ownerName"),
                                CreateOn = Utils.FromUnixTime(long.Parse(d.ElementValueOrDefault("createdTime"))),
                                IsFolder = false
                            });
        }

        private static string Authenticate(string login, string password)
        {
            using (var webClient = new WebClient())
            {
                var address = string.Format(
                    "https://accounts.zoho.com/login?FROM_AGENT=true&LOGIN_ID={0}&PASSWORD={1}", login, password);
                var loginInfo = webClient.DownloadString(address)
                    .Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => s.Contains("=") && !s.StartsWith("#"))
                    .ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1]);

                if (loginInfo["RESULT"] == "FALSE") throw new UnauthorizedAccessException("Login failed.");
                return loginInfo["TICKET"];
            }
        }

        private static void CheckErrorAndThrow(XDocument document)
        {
            var err = document.XPathSelectElement("/response/error");
            if (err != null)
            {
                throw new Exception(string.Format("Code: {0}. {1}",
                                                  err.ElementValueOrDefault("code"),
                                                  err.ElementValueOrDefault("message", "not specified")));
            }
        }
    }
}