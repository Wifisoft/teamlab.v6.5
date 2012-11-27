using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AppLimit.CloudComputing.SharpBox.Common.Net.Web;
using AppLimit.CloudComputing.SharpBox.Exceptions;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;
using AppLimit.CloudComputing.SharpBox.StorageProvider.SkyDrive.Authorization;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.SkyDrive.Logic
{
    internal class SkyDriveStorageProviderService : GenericStorageProviderService
    {
        public void RefreshDirectoryContent(IStorageProviderSession session, ICloudDirectoryEntry directory)
        {
            String uri = String.Format(SkyDriveConstants.FilesAccessUrlFormat, directory.Id);
            uri = SignUri(session, uri);

            WebRequest request = WebRequest.Create(uri);
            WebResponse response = request.GetResponse();

            using (var rs = response.GetResponseStream())
            {
                if (rs == null) 
                    throw new SharpBoxException(SharpBoxErrorCodes.ErrorCouldNotRetrieveDirectoryList);
                
                String json = new StreamReader(rs).ReadToEnd();
                var childs = SkyDriveJsonParser.ParseListOfEntries(session, json)
                    .Select(x => x as BaseFileEntry).Where(x => x != null).ToArray();
                
                if (childs.Length == 0 || !(directory is BaseDirectoryEntry)) 
                    throw new SharpBoxException(SharpBoxErrorCodes.ErrorCouldNotRetrieveDirectoryList);
                
                var directoryBase = directory as BaseDirectoryEntry;
                directoryBase.AddChilds(childs);
            }

        }

        public override bool VerifyAccessTokenType(ICloudStorageAccessToken token)
        {
            return token is OAuth20Token;
        }

        public override IStorageProviderSession CreateSession(ICloudStorageAccessToken token, ICloudStorageConfiguration configuration)
        {
            return new SkyDriveStorageProviderSession(token, this, configuration);
        }

        public override ICloudFileSystemEntry RequestResource(IStorageProviderSession session, String nameOrID, ICloudDirectoryEntry parent)
        {
            /* In this method name could be either requested resource name or it's ID.
             * In first case just refresh the parent and then search child with an appropriate.
             * In second case it does not matter if parent is null or not a parent because we use only resource ID. */

            if (SkyDriveHelpers.IsResourceID(nameOrID) || nameOrID.Equals("/") && parent == null)
                //If request by ID or root folder requested
            {
                String uri =
                    SkyDriveHelpers.IsResourceID(nameOrID)
                        ? String.Format("{0}/{1}", SkyDriveConstants.BaseAccessUrl, nameOrID)
                        : SkyDriveConstants.RootAccessUrl;
                uri = SignUri(session, uri);

                WebRequest request = WebRequest.Create(uri);
                WebResponse response = request.GetResponse();

                using (var rs = response.GetResponseStream())
                {
                    if (rs != null)
                    {
                        String json = new StreamReader(rs).ReadToEnd();
                        ICloudFileSystemEntry entry = SkyDriveJsonParser.ParseSingleEntry(session, json);
                        return entry;
                    }
                }
            }
            else
            {
                String uri =
                    parent != null
                        ? String.Format(SkyDriveConstants.FilesAccessUrlFormat, parent.Id)
                        : SkyDriveConstants.RootAccessUrl + "/files";
                uri = SignUri(session, uri);

                WebRequest request = WebRequest.Create(uri);
                WebResponse response = request.GetResponse();

                using (var rs = response.GetResponseStream())
                {
                    if (rs != null)
                    {
                        String json = new StreamReader(rs).ReadToEnd();
                        var entry = SkyDriveJsonParser.ParseListOfEntries(session, json).FirstOrDefault(x => x.Name.Equals(nameOrID));
                        if (entry != null && parent != null && parent is BaseDirectoryEntry)
                            (parent as BaseDirectoryEntry).AddChild(entry as BaseFileEntry);
                        return entry;
                    }
                }
            }
            return null;
        }

        public override void RefreshResource(IStorageProviderSession session, ICloudFileSystemEntry resource)
        {
            //Request resource by ID and then update properties from requested
            var current = RequestResource(session, resource.Id, null);
            SkyDriveHelpers.CopyProperties(current, resource);

            if (resource is ICloudDirectoryEntry)
                RefreshDirectoryContent(session, resource as ICloudDirectoryEntry);
        }

        public override bool DeleteResource(IStorageProviderSession session, ICloudFileSystemEntry entry)
        {
            String uri = String.Format("{0}/{1}", SkyDriveConstants.BaseAccessUrl, entry.Id);
            uri = SignUri(session, uri);
            String json = PerformRequest(session, uri, "DELETE", null);
            
            if (json != null && !SkyDriveJsonParser.ContainsError(json, false))
            {
                var parent = entry.Parent as BaseDirectoryEntry;
                if (parent != null)
                    parent.RemoveChildById(entry.Id);
                return true;
            }

            return false;
        }

        public override ICloudFileSystemEntry CreateResource(IStorageProviderSession session, String name, ICloudDirectoryEntry parent)
        {
            if (name.Contains("/"))
                throw new SharpBoxException(SharpBoxErrorCodes.ErrorInvalidFileOrDirectoryName);

            String uri =
                parent != null
                    ? String.Format("{0}/{1}", SkyDriveConstants.BaseAccessUrl, parent.Id)
                    : SkyDriveConstants.RootAccessUrl;

            String data = String.Format("{{name: \"{0}\"}}", name);
            String json = PerformRequest(session, uri, "POST", data);
            var entry = SkyDriveJsonParser.ParseSingleEntry(session, json);
            
            var parentBase = parent as BaseDirectoryEntry;
            if (parentBase != null)
                parentBase.AddChild(entry as BaseFileEntry);
            
            return entry;
        }

        public override bool RenameResource(IStorageProviderSession session, ICloudFileSystemEntry entry, String newName)
        {
            if (entry.Name.Equals("/") || newName.Contains("/"))
                return false;

            String uri = String.Format("{0}/{1}", SkyDriveConstants.BaseAccessUrl, entry.Id);
            String data = String.Format("{{name: \"{0}\"}}", newName);
            String json = PerformRequest(session, uri, "PUT", data);
            
            if (json != null && !SkyDriveJsonParser.ContainsError(json, false))
            {
                var entryBase = entry as BaseFileEntry;
                if (entryBase != null)
                    entryBase.Name = newName;
                return true;
            }

            return false;
        }

        public override bool MoveResource(IStorageProviderSession session, ICloudFileSystemEntry entry, ICloudDirectoryEntry moveTo)
        {
            if (entry.Name.Equals("/"))
                return false;

            String uri = String.Format("{0}/{1}", SkyDriveConstants.BaseAccessUrl, entry.Id);
            String data = String.Format("{{destination: \"{0}\"}}", moveTo.Id);
            String json = PerformRequest(session, uri, "MOVE", data);

            if (json != null && !SkyDriveJsonParser.ContainsError(json, false))
            {
                var parent = entry.Parent as BaseDirectoryEntry;
                if (parent != null)
                    parent.RemoveChildById(entry.Id);
                var moveToBase = moveTo as BaseDirectoryEntry;
                if (moveToBase != null)
                    moveToBase.AddChild(entry as BaseFileEntry);
                return true;
            }

            return false;
        }

        public override Stream CreateDownloadStream(IStorageProviderSession session, ICloudFileSystemEntry entry)
        {
            if (entry is ICloudDirectoryEntry)
                throw new ArgumentException("Download operation can be perform for files only");
            String uri = String.Format("{0}/{1}/content", SkyDriveConstants.BaseAccessUrl, entry.Id);
            uri = SignUri(session, uri);
            WebRequest request = WebRequest.Create(uri);
            WebResponse response = request.GetResponse();
            return response.GetResponseStream();
        }

        public override Stream CreateUploadStream(IStorageProviderSession session, ICloudFileSystemEntry entry, long uploadSize)
        {
            if (entry is ICloudDirectoryEntry)
                throw new ArgumentException("Upload operation can be perform for files only");

            String uri = String.Format("{0}/{1}/files/{2}", SkyDriveConstants.BaseAccessUrl, entry.ParentID, entry.Name);
            uri = SignUri(session, uri);

            WebRequest request = WebRequest.Create(uri);
            request.Method = "PUT";
            request.ContentLength = uploadSize;

            if (entry is BaseFileEntry)
                (entry as BaseFileEntry).Length = uploadSize;

            var stream = new WebRequestStream(request.GetRequestStream(), request, null);
            stream.PushPostDisposeOperation(CommitUploadOperation, request, entry);

            return stream;
        }

        public override void CommitStreamOperation(IStorageProviderSession session, ICloudFileSystemEntry entry, nTransferDirection direction, Stream stream)
        {
        }

        public override bool SupportsDirectRetrieve
        {
            get { return true; }
        }

        public override void StoreToken(IStorageProviderSession session, Dictionary<String, String> tokendata, ICloudStorageAccessToken token)
        {
            if (token is OAuth20Token)
            {
                tokendata.Add(SkyDriveConstants.SerializedDataKey, (token as OAuth20Token).Serialize());   
            }
        }

        public override ICloudStorageAccessToken LoadToken(Dictionary<String, String> tokendata)
        {
            String type = tokendata[CloudStorage.TokenCredentialType];
            if (type.Equals(typeof(OAuth20Token).ToString()))
            {
                String json = tokendata[SkyDriveConstants.SerializedDataKey];
                return OAuth20Token.Deserialize(json);
            }
            return null;
        }

        private static String SignUri(IStorageProviderSession session, String uri)
        {
            var token = GetValidToken(session);
            var signedUri = String.Format("{0}?access_token={1}", uri, token.AccessToken);
            return signedUri;
        }

        private static OAuth20Token GetValidToken(IStorageProviderSession session)
        {
            var token = session.SessionToken as OAuth20Token;
            if (token == null) throw new ArgumentException("Can not retrieve valid oAuth 2.0 token from given session", "session");
            if (token.IsExpired)
            {
                token = (OAuth20Token)SkyDriveAuthorizationHelper.RefreshToken(token);
                var sdSession = session as SkyDriveStorageProviderSession;
                if (sdSession != null)
                {
                    sdSession.SessionToken = token;
                }
            }
            return token;
        }

        private static String PerformRequest(IStorageProviderSession session, String uri, String method, String data)
        {
            if (String.IsNullOrEmpty(method) && String.IsNullOrEmpty(data))
                method = "GET";
            if (String.IsNullOrEmpty(method))
                return null;

            if (method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                uri = SignUri(session, uri);

            WebRequest request = WebRequest.Create(uri);
            request.Method = method;

            if (!method.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                request.Headers.Add("Authorization", "Bearer " + GetValidToken(session).AccessToken);
            
            if(!String.IsNullOrEmpty(data))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                using (var rs = request.GetRequestStream())
                {
                    rs.Write(bytes, 0, bytes.Length);
                }
            }

            try
            {
                WebResponse response = request.GetResponse();
                using (var rs = response.GetResponseStream())
                {
                    if (rs != null)
                    {
                        return new StreamReader(rs).ReadToEnd();
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private static void CommitUploadOperation(object[] args)
        {
            var request = args[0] as WebRequest;
            var entry = args[1] as BaseFileEntry;

            if (request == null || entry == null)
                return;

            WebResponse response = request.GetResponse();
            using (var rs = response.GetResponseStream())
            {
                if (rs == null) return;
                String json = new StreamReader(rs).ReadToEnd();
                String id = SkyDriveJsonParser.ParseEntryID(json);
                entry.Id = id;
                entry.Modified = DateTime.UtcNow;

                var parent = entry.Parent as BaseDirectoryEntry;
                if (parent != null)
                {
                    parent.RemoveChildById(entry.Name);
                    parent.AddChild(entry);   
                }
            }
        }
    }
}
