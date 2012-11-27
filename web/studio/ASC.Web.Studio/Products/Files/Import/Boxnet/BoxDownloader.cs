using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Xml.Linq;
using ASC.Thrdparty.Configuration;

namespace ASC.Web.Files.Import.Boxnet
{
    public class BoxDownloader
    {
        public string AuthKey { get; set; }
        public const string BoxNetApiKeyName = "box.net";

        private string ApiKey
        {
            get { return KeyStorage.Get(BoxDownloader.BoxNetApiKeyName); }
        }

        private const string GetAuthTokenUrl =
            "https://www.box.net/api/1.0/rest?action=get_auth_token&api_key={0}&ticket={1}";
        private const string TreeUrl =
            "https://www.box.net/api/1.0/rest?action=get_account_tree&api_key={0}&auth_token={1}&folder_id=0&params[]=nozip";

        private BoxEntry _rootEntry = null;
        private const string DownloadUrl = "https://www.box.net/api/1.0/download/{0}/{1}";

        public BoxDownloader()
        {
            //Get from session
            if (!String.IsNullOrEmpty(BoxVariables.AuthToken))
            {
                AuthKey = BoxVariables.AuthToken;
            }
            else
            {
                if (String.IsNullOrEmpty(BoxVariables.AuthTicket))
                    throw new AuthenticationException("Ticket is null");
                //Try get key from ticket
                var responceData =
                    GetClient().DownloadString(string.Format(GetAuthTokenUrl, ApiKey, BoxVariables.AuthTicket));
                var response = XDocument.Parse(responceData).Element("response");
                if (response.Element("status").Value != "get_auth_token_ok")
                {
                    throw new InvalidOperationException("bad auth token status " +
                                                        response.Element("status").Value);
                }
                AuthKey = response.Element("auth_token").Value;
            }
        }

        public BoxDownloader(string authKey)
        {
            AuthKey = authKey;
        }

        public BoxEntry GetDocuments()
        {
            if (_rootEntry == null)
            {
                //Download xml
                var treeResponce = GetClient().DownloadData(string.Format(TreeUrl, ApiKey, AuthKey));
                var response = DocumentLoader.GetDoc(treeResponce).Element("response");
                //Check status
                if (response.Element("status").Value != "listing_ok")
                {
                    throw new InvalidOperationException("bad listing status " +
                                                        response.Element("status").Value);
                }
                var rootFolders = response.Element("tree").Elements("folder");

                //create root entry
                _rootEntry = new BoxEntry();
                foreach (var rootFolder in rootFolders)
                {
                    RetrieveTree(_rootEntry, rootFolder);
                }
            }
            return _rootEntry;
        }

        public Stream GetDocumentStream(BoxEntry document)
        {
            long size;
            return GetDocumentStream(document, out size);
        }

        public Stream GetDocumentStream(BoxEntry document, out long size)
        {
            if (document.IsFolder)
                throw new ArgumentException("entry must be a file not folder!");

            var url = string.Format(DownloadUrl, AuthKey, document.Id);
            var request = WebRequest.Create(url);
            var responce = request.GetResponse();
            size = responce.ContentLength;
            return responce != null ? responce.GetResponseStream() : null;
        }

        private static void RetrieveTree(BoxEntry boxEntry, XElement folderElement)
        {
            //select folders
            if (folderElement.Element("folders") != null)
            {
                var subFolders = folderElement.Element("folders").Elements("folder");
                foreach (var subFolder in subFolders)
                {
                    var entry = new BoxEntry
                                    {
                                        IsFolder = true,
                                        Title = subFolder.Attribute("name").Value,
                                        Id = subFolder.Attribute("id").Value
                                    };
                    boxEntry.Childs.Add(entry);
                    RetrieveTree(entry, subFolder);
                }
            }
            if (folderElement.Element("files") != null)
            {
                var files = folderElement.Element("files").Elements("file");
                boxEntry.Childs.AddRange(files.Select(file => new BoxEntry
                                                           {
                                                               IsFolder = false,
                                                               Title = file.Attribute("file_name").Value,
                                                               Id = file.Attribute("id").Value,
                                                               Size = int.Parse(file.Attribute("size").Value),
                                                               Published = FromUnixDate(long.Parse(file.Attribute("created").Value)),
                                                               Updated = FromUnixDate(long.Parse(file.Attribute("updated").Value)),
                                                           }));
            }
        }

        private static WebClient GetClient()
        {
            return new WebClient();
        }


        private static DateTime FromUnixDate(long value)
        {
            return new DateTime(1970, 1, 1).AddSeconds(value);
        }
    }
}