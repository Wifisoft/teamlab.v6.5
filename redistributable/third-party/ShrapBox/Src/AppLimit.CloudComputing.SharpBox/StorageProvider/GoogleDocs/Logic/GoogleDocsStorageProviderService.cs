using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using AppLimit.CloudComputing.SharpBox.Common.Extensions;
using AppLimit.CloudComputing.SharpBox.Common.Net.Web;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth.Context;
using AppLimit.CloudComputing.SharpBox.Common.Net.oAuth.Token;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;
using Microsoft.Win32;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.GoogleDocs.Logic
{
    internal class GoogleDocsStorageProviderService : GenericStorageProviderService
    {
        public enum RemoveMode
        {
            Trash,
            Delete,
            FromParentCollection
        };

        #region GoogleDocs Specific

        public void RefreshDirectoryContent(IStorageProviderSession session, BaseDirectoryEntry entry)
        {
            if (entry == null)
                return;

            var url = String.Format(GoogleDocsConstants.GoogleDocsContentsUrlFormat, entry.Id.ReplaceFirst("_", "%3a"));
            var parameters = new Dictionary<string, string> { { "max-results", "1000" } };
            try
            {
                while (!String.IsNullOrEmpty(url))
                {
                    var request = CreateWebRequest(session, url, "GET", parameters);
                    var response = (HttpWebResponse)request.GetResponse();
                    var rs = response.GetResponseStream();

                    var feedXml = new StreamReader(rs).ReadToEnd();
                    var childs = GoogleDocsXmlParser.ParseEntriesXml(session, feedXml);
                    entry.AddChilds(childs);

                    url = GoogleDocsXmlParser.ParseNext(feedXml);
                }
            }
            catch (WebException)
            {

            }
        }

        public bool RemoveResource(IStorageProviderSession session, ICloudFileSystemEntry resource, RemoveMode mode)
        {
            String url;
            Dictionary<String, String> parameters = null;

            if (mode == RemoveMode.FromParentCollection)
            {
                var pId = (resource.Parent != null ? resource.Parent.Id : GoogleDocsConstants.RootFolderId).ReplaceFirst("_", "%3a");
                url = String.Format("{0}/{1}/contents/{2}", GoogleDocsConstants.GoogleDocsFeedUrl, pId, resource.Id.ReplaceFirst("_", "%3a"));
            }
            else
            {
                url = String.Format(GoogleDocsConstants.GoogleDocsResourceUrlFormat, resource.Id.ReplaceFirst("_", "%3a"));
                parameters = new Dictionary<string, string> {{"delete", "true"}};
            }

            var request = CreateWebRequest(session, url, "DELETE", parameters);
            request.Headers.Add("If-Match", "*");

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
            }
            catch (WebException)
            {
            }

            return false;
        }

        public bool AddToCollection(IStorageProviderSession session, ICloudFileSystemEntry resource, ICloudDirectoryEntry collection)
        {
            var url = String.Format(GoogleDocsConstants.GoogleDocsContentsUrlFormat, collection.Id.ReplaceFirst("_", "%3a"));
            var request = CreateWebRequest(session, url, "POST", null);
            GoogleDocsXmlParser.WriteAtom(request, GoogleDocsXmlParser.EntryElement(null, GoogleDocsXmlParser.IdElement(resource.Id.ReplaceFirst("_", "%3a"))));

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.Created)
                    return true;
            }
            catch (WebException)
            {
            }

            return false;
        }

        #endregion

        #region GenericStorageProviderService members

        public override bool VerifyAccessTokenType(ICloudStorageAccessToken token)
        {
            return token is GoogleDocsToken;
        }

        public override IStorageProviderSession CreateSession(ICloudStorageAccessToken token, ICloudStorageConfiguration configuration)
        {
            var gdToken = token as GoogleDocsToken;
            return new GoogleDocsStorageProviderSession(this,
                                                        configuration,
                                                        new OAuthConsumerContext(gdToken.ConsumerKey, gdToken.ConsumerSecret),
                                                        gdToken);
        }

        public override ICloudFileSystemEntry RequestResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            if (GoogleDocsResourceHelper.IsNoolOrRoot(parent) && GoogleDocsResourceHelper.IsRootName(Name))
            {
                var root = new BaseDirectoryEntry("/", 0, DateTime.Now, session.Service, session) {Id = GoogleDocsConstants.RootFolderId};
                root.SetPropertyValue(GoogleDocsConstants.ResCreateMediaProperty, GoogleDocsConstants.RootResCreateMediaUrl);
                RefreshDirectoryContent(session, root);
                return root;
            }

            if (Name.Equals("/"))
            {
                RefreshDirectoryContent(session, parent as BaseDirectoryEntry);
            }

            if (GoogleDocsResourceHelper.IsResorceId(Name))
            {
                var url = String.Format(GoogleDocsConstants.GoogleDocsResourceUrlFormat, Name.ReplaceFirst("_", "%3a"));
                var request = CreateWebRequest(session, url, "GET", null);
                try
                {
                    var response = (HttpWebResponse) request.GetResponse();
                    var rs = response.GetResponseStream();

                    var xml = new StreamReader(rs).ReadToEnd();
                    var entry = GoogleDocsXmlParser.ParseEntriesXml(session, xml).FirstOrDefault();

                    if(entry == null)
                        throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);

                    if (parent != null)
                        (parent as BaseDirectoryEntry).AddChild(entry);

                    var dirEntry = entry as BaseDirectoryEntry;

                    if (dirEntry == null)
                        return entry;

                    RefreshDirectoryContent(session, dirEntry);
                    return dirEntry;
                }
                catch (WebException)
                {
                    throw new SharpBoxException(SharpBoxErrorCodes.ErrorFileNotFound);
                }
            }

            throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);
        }

        public override void RefreshResource(IStorageProviderSession session, ICloudFileSystemEntry resource)
        {
            if (resource == null)
                return;

            if (resource.Id.Equals(GoogleDocsConstants.RootFolderId))
            {
                RefreshDirectoryContent(session, resource as BaseDirectoryEntry);
                return;
            }

            var resourceUrl = String.Format(GoogleDocsConstants.GoogleDocsResourceUrlFormat, resource.Id.ReplaceFirst("_", "%3a"));
            var request = CreateWebRequest(session, resourceUrl, "GET", null);
            request.Headers.Add("If-None-Match", resource.GetPropertyValue(GoogleDocsConstants.EtagProperty));

            try
            {
                var response = (HttpWebResponse) request.GetResponse();

                if (response.StatusCode != HttpStatusCode.NotModified)
                {
                    var s = response.GetResponseStream();
                    var xml = new StreamReader(s).ReadToEnd();

                    GoogleDocsResourceHelper.UpdateResourceByXml(session, out resource, xml);
                }

                var dirEntry = resource as BaseDirectoryEntry;

                if (dirEntry == null || dirEntry.HasChildrens == nChildState.HasChilds)
                    return;

                RefreshDirectoryContent(session, dirEntry);
            }
            catch (WebException)
            {
                
            }
        }

        public override ICloudFileSystemEntry CreateResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            if (String.IsNullOrEmpty(Name))
            {
                throw new ArgumentException("Name cannot be empty");
            }

            var url = GoogleDocsResourceHelper.IsNoolOrRoot(parent)
                ? GoogleDocsConstants.GoogleDocsFeedUrl
                : String.Format(GoogleDocsConstants.GoogleDocsContentsUrlFormat, parent.Id.ReplaceFirst("_", "%3a"));

            var request = CreateWebRequest(session, url, "POST", null);
            GoogleDocsXmlParser.WriteAtom(request, GoogleDocsXmlParser.EntryElement(GoogleDocsXmlParser.CategoryElement(), GoogleDocsXmlParser.TitleElement(Name)));

            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var rs = response.GetResponseStream();

                    var xml = new StreamReader(rs).ReadToEnd();
                    var entry = GoogleDocsXmlParser.ParseEntriesXml(session, xml).First();
                    if (parent != null)
                        (parent as BaseDirectoryEntry).AddChild(entry);

                    return entry;
                }
            }
            catch (WebException)
            {
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorCreateOperationFailed);
            }

            return null;
        }

        public override bool RenameResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, string newName)
        {
            if (String.IsNullOrEmpty(newName) || GoogleDocsResourceHelper.IsNoolOrRoot(fsentry))
                return false;

            var url = String.Format(GoogleDocsConstants.GoogleDocsResourceUrlFormat, fsentry.Id.ReplaceFirst("_", "%3a"));

            var request = CreateWebRequest(session, url, "PUT", null);
            request.Headers.Add("If-Match", fsentry.GetPropertyValue(GoogleDocsConstants.EtagProperty));
            GoogleDocsXmlParser.WriteAtom(request, GoogleDocsXmlParser.EntryElement(GoogleDocsXmlParser.TitleElement(newName)));

            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //check if extension added
                    if (!(fsentry is ICloudDirectoryEntry) && String.IsNullOrEmpty(Path.GetExtension(newName)))
                        newName = newName + Path.GetExtension(fsentry.Name);
                    (fsentry as BaseFileEntry).Name = newName;
                    return true;
                }
            }
            catch (WebException)
            { 
            }

            return false;
        }

        public override bool DeleteResource(IStorageProviderSession session, ICloudFileSystemEntry entry)
        {
            if (GoogleDocsResourceHelper.IsNoolOrRoot(entry))
                return false;

            if (RemoveResource(session, entry, RemoveMode.Delete))
            {
                (entry as BaseFileEntry).IsDeleted = true;
                if (entry.Parent != null)
                    (entry.Parent as BaseDirectoryEntry).RemoveChildById(entry.Id);

                return true;
            }

            return false;
        }

        public override bool MoveResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            if (fsentry == null || newParent == null || GoogleDocsResourceHelper.IsNoolOrRoot(fsentry))
                return false;

            if(RemoveResource(session, fsentry, RemoveMode.FromParentCollection) && AddToCollection(session, fsentry, newParent))
            {
                if (fsentry.Parent != null)
                    (fsentry.Parent as BaseDirectoryEntry).RemoveChild(fsentry as BaseFileEntry);
                (newParent as BaseDirectoryEntry).AddChild(fsentry as BaseFileEntry);

                return true;
            }

            return false;
        }

        public override bool CopyResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            if (AddToCollection(session, fsentry, newParent))
            {
                (newParent as BaseDirectoryEntry).AddChild(fsentry as BaseFileEntry);
                return true;
            }
            return false;
        }

        public override Stream CreateDownloadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry)
        {
            var url = fileSystemEntry.GetPropertyValue(GoogleDocsConstants.DownloadUrlProperty);
          
            var format = GoogleDocsResourceHelper.GetExtensionByKind(fileSystemEntry.GetPropertyValue(GoogleDocsConstants.KindProperty));
            if (!String.IsNullOrEmpty(format))
            {
                url = String.Format("{0}&exportFormat={1}&format={1}", url, format);
            }

            var request = CreateWebRequest(session, url, "GET", null, true);

            MemoryStream stream = null;
            try
            {
                var response = request.GetResponse();
                using (var rs = response.GetResponseStream())
                {
                    stream = new MemoryStream();
                    rs.CopyTo(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (WebException)
            {
            }

            return stream;
        }

        public override void CommitStreamOperation(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, nTransferDirection Direction, Stream NotDisposedStream)
        {
        }

        public override Stream CreateUploadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, long uploadSize)
        {
            String url;
            bool update = false;

            String type = GetMimeType(fileSystemEntry.Name);

            if (GoogleDocsResourceHelper.IsResorceId(fileSystemEntry.Id)) //update existent
            {
                var ext = GoogleDocsResourceHelper.GetExtensionByKind(fileSystemEntry.GetPropertyValue(GoogleDocsConstants.KindProperty));
                if (!String.IsNullOrEmpty(ext))
                {
                    //for google docs kind we previously add an extension that don't needed anymore
                    var index = fileSystemEntry.Name.IndexOf('.' + ext, StringComparison.Ordinal);
                    (fileSystemEntry as BaseFileEntry).Name = fileSystemEntry.Name.Substring(0, index);
                }

                url = fileSystemEntry.GetPropertyValue(GoogleDocsConstants.ResEditMediaProperty);
                update = true;

                if (String.IsNullOrEmpty(url))
                    throw new HttpException(403, "User not authorize to update resource");
            }
            else
            {
                url =  fileSystemEntry.Parent.GetPropertyValue(GoogleDocsConstants.ResCreateMediaProperty) + "?convert=false";
            }

            //first initiate resumable upload request
            var initRequest = CreateWebRequest(session, url, update ? "PUT" : "POST", null);
            initRequest.Headers.Add("X-Upload-Content-Length", uploadSize.ToString(CultureInfo.InvariantCulture));
            initRequest.Headers.Add("X-Upload-Content-Type", type);
            if (update)
            {
                initRequest.Headers.Add("If-Match", "*");
            }
            GoogleDocsXmlParser.WriteAtom(initRequest, GoogleDocsXmlParser.EntryElement(null, GoogleDocsXmlParser.TitleElement(fileSystemEntry.Name)));

            var response = initRequest.GetResponse();

            //secondly create request to obtained url
            var uplRequest = CreateWebRequest(session, response.Headers["Location"], "PUT", null);
            uplRequest.ContentLength = uploadSize;
            uplRequest.ContentType = type;
            uplRequest.Headers.Add("Content-Range", String.Format("bytes {0}-{1}/{2}", 0, uploadSize - 1, uploadSize));

            var wrStream = new WebRequestStream(uplRequest.GetRequestStream(), uplRequest, null);
            wrStream.PushPostDisposeOperation(CommitUploadOperation, session, uplRequest, fileSystemEntry);

            return wrStream;
        }

        public override bool SupportsDirectRetrieve
        {
            get { return false; }
        }

        private void CommitUploadOperation(object[] args)
        {
            var session = args[0] as IStorageProviderSession;
            var request = args[1] as WebRequest;
            var entry = args[2] as BaseFileEntry;

            var responce = (HttpWebResponse)request.GetResponse();
            var stream = responce.GetResponseStream();

            var xml = new StreamReader(stream).ReadToEnd();

            var uploaded = GoogleDocsXmlParser.ParseEntriesXml(session, xml).First();

            BaseDirectoryEntry parent = null;
            if (entry.Parent != null)
            {
                parent = entry.Parent as BaseDirectoryEntry;
                parent.RemoveChild(entry); //this may be virtual one
            }

            //copy uploaded entry's info to old one
            entry.Name = uploaded.Name;
            entry.Id = uploaded.Id;
            entry.Length = uploaded.Length;
            entry.Modified = uploaded.Modified;
            entry.SetPropertyValue(GoogleDocsConstants.DownloadUrlProperty, uploaded.GetPropertyValue(GoogleDocsConstants.DownloadUrlProperty));
            entry.SetPropertyValue(GoogleDocsConstants.EtagProperty, uploaded.GetPropertyValue(GoogleDocsConstants.EtagProperty));
            entry.SetPropertyValue(GoogleDocsConstants.KindProperty, uploaded.GetPropertyValue(GoogleDocsConstants.KindProperty));
            entry.SetPropertyValue(GoogleDocsConstants.ResEditMediaProperty, uploaded.GetPropertyValue(GoogleDocsConstants.ResCreateMediaProperty));

            if (parent != null)
                parent.AddChild(entry);
        }

        #endregion

        #region oAuth

        public override void StoreToken(IStorageProviderSession session, Dictionary<string, string> tokendata, ICloudStorageAccessToken token)
        {
            if (token is GoogleDocsRequestToken)
            {
                var requestToken = token as GoogleDocsRequestToken;
                tokendata.Add(GoogleDocsConstants.TokenGoogleDocsAppKey, requestToken.RealToken.TokenKey);
                tokendata.Add(GoogleDocsConstants.TokenGoogleDocsAppSecret, requestToken.RealToken.TokenSecret);
            }
            else if (token is GoogleDocsToken)
            {
                var gdtoken = token as GoogleDocsToken;
                tokendata.Add(GoogleDocsConstants.TokenGoogleDocsAppKey, gdtoken.ConsumerKey);
                tokendata.Add(GoogleDocsConstants.TokenGoogleDocsAppSecret, gdtoken.ConsumerSecret);
                tokendata.Add(GoogleDocsConstants.TokenGoogleDocsUsername, gdtoken.TokenKey);
                tokendata.Add(GoogleDocsConstants.TokenGoogleDocsPassword, gdtoken.TokenSecret);
            }
        }

        public override ICloudStorageAccessToken LoadToken(Dictionary<string, string> tokendata)
        {
            String type = tokendata[CloudStorage.TokenCredentialType];

            if (type.Equals(typeof (GoogleDocsToken).ToString()))
            {
                var tokenKey = tokendata[GoogleDocsConstants.TokenGoogleDocsUsername];
                var tokenSecret = tokendata[GoogleDocsConstants.TokenGoogleDocsPassword];
                var consumerKey = tokendata[GoogleDocsConstants.TokenGoogleDocsAppKey];
                var consumerSecret = tokendata[GoogleDocsConstants.TokenGoogleDocsAppSecret];

                return new GoogleDocsToken(tokenKey, tokenSecret, consumerKey, consumerSecret);
            }

            if (type.Equals(typeof (GoogleDocsRequestToken).ToString()))
            {
                var tokenKey = tokendata[GoogleDocsConstants.TokenGoogleDocsAppKey];
                var tokenSecret = tokendata[GoogleDocsConstants.TokenGoogleDocsAppSecret];

                return new GoogleDocsRequestToken(new OAuthToken(tokenKey, tokenSecret));
            }

            throw new InvalidCastException("Token type not supported through this provider");
        }

        #endregion

        #region Helpers

        private static String GetMimeType(String file)
        {
            return Common.Net.MimeMapping.GetMimeMapping(file);
        }

        private static WebRequest CreateWebRequest(IStorageProviderSession storageProviderSession, String url, String method, Dictionary<String, String> parameters)
        {
            return CreateWebRequest(storageProviderSession, url, method, parameters, false);
        }

        private static WebRequest CreateWebRequest(IStorageProviderSession storageProviderSession, String url, String method, Dictionary<String, String> parameters, bool oAuthParamsAsHeader)
        {
            var session = storageProviderSession as GoogleDocsStorageProviderSession;
            var configuration = session.ServiceConfiguration as GoogleDocsConfiguration;
            WebRequest request;
            if (!oAuthParamsAsHeader)
            {
                request = OAuthService.CreateWebRequest(url, method, null, null, session.Context, (GoogleDocsToken)session.SessionToken, parameters);
            }
            else
            {
                request = WebRequest.Create(url);
                String oAuthHeader = OAuthService.GetOAuthAuthorizationHeader(url, session.Context, (GoogleDocsToken)session.SessionToken, null, method);
                request.Headers.Add("Authorization", oAuthHeader);
            }

            //using API's 3.0 version
            request.Headers.Add("GData-Version", configuration.GDataVersion);

            return request;
        }

        private static OAuthService OAuthService
        {
            get { return new OAuthService(); }
        }

        #endregion
    }
}