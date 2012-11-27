using System;
using System.Collections.Generic;
using System.IO;
using AppLimit.CloudComputing.SharpBox.Common.Cache;
using AppLimit.CloudComputing.SharpBox.Common.IO;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider
{
    public class CachedServiceWrapper : IStorageProviderService
    {
        private readonly IStorageProviderService _service;
        private static readonly CachedDictionary<ICloudFileSystemEntry> FsCache = new CachedDictionary<ICloudFileSystemEntry>("sbox-fs",
            System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(120), (x) => true);//TODO: To config!

        static CachedServiceWrapper()
        {
            
        }

        public CachedServiceWrapper(IStorageProviderService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            _service = service;
        }

        public bool VerifyAccessTokenType(ICloudStorageAccessToken token)
        {
            return _service.VerifyAccessTokenType(token);
        }

        public IStorageProviderSession CreateSession(ICloudStorageAccessToken token, ICloudStorageConfiguration configuration)
        {
            return _service.CreateSession(token, configuration);
        }

        public void CloseSession(IStorageProviderSession session)
        {
            FsCache.Reset(GetSessionKey(session));
            _service.CloseSession(session);
        }

        public ICloudFileSystemEntry RequestResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            var cacheKey = GetSessionKey(session);
            return FsCache.Get(cacheKey, GetCacheUrl(session,Name,parent), () => _service.RequestResource(session, Name, parent));
        }


        private string GetSessionKey(IStorageProviderSession session)
        {
            return session.GetType().Name + " " + session.SessionToken;
        }

        private static string GetCacheUrl(IStorageProviderSession session, string Name, ICloudFileSystemEntry parent)
        {
            // build the url            
            String targeturl = PathHelper.Combine(session.ServiceConfiguration.ServiceLocator.ToString(),
                                                  GenericHelper.GetResourcePath(parent)).TrimEnd('/');

            // finalize the url 
            //if (!(parent is ICloudDirectoryEntry))
                //targeturl = targeturl.TrimEnd('/');

            // add the additional Path 
            if (Name != null)
                targeturl = PathHelper.Combine(targeturl, Name);
            return targeturl;
        }


        public void RefreshResource(IStorageProviderSession session, ICloudFileSystemEntry resource)
        {
            var cacheKey = GetSessionKey(session);
            var refreshed = FsCache.Get(cacheKey, GetCacheUrl(session,null,resource),null) as BaseFileEntry;

            if (refreshed == null || ((refreshed is BaseDirectoryEntry) && (refreshed as BaseDirectoryEntry).HasChildrens == nChildState.HasNotEvaluated))
            {
                _service.RefreshResource(session,resource);
                //TODO: Optimize caching we don't need to stroe whole resource for it. Only a flag that it's already refreshed not so long time ago
                //Do cache it here
                FsCache.Add(cacheKey, GetCacheUrl(session, null, resource),resource);
            }
            //else if (!ReferenceEquals(refreshed,resource))
            //{
                //TODO: Leave blanc for now.
                //var resourseBase = resource as BaseFileEntry;
                //if (resourseBase!=null)
                //{
                //    //Merge changes
                //    resourseBase.Parent = refreshed.Parent;
                //    resourseBase.IsDeleted = refreshed.IsDeleted;
                //    resourseBase.Length = refreshed.Length;
                //    resourseBase.Modified = refreshed.Modified;
                //    resourseBase.Name = refreshed.Name;

                //    if (resource is BaseDirectoryEntry && refreshed is BaseDirectoryEntry)
                //    {
                //        //Merge childs
                //        ((BaseDirectoryEntry) resource).ClearChilds();
                //        var resourseDir = ((BaseDirectoryEntry) resource);
                //        foreach (var child in ((BaseDirectoryEntry)refreshed).GetSubdirectoriesWithoutRefresh())
                //        {
                //            resourseDir.AddChild((BaseFileEntry)child);
                //        }
                //    }
                //}
            //}
            //If equal reference then ok
        }

        public ICloudFileSystemEntry CreateResource(IStorageProviderSession session, string Name, ICloudDirectoryEntry parent)
        {
            FsCache.Reset(GetSessionKey(session),GetCacheUrl(session,null,parent));
            return _service.CreateResource(session, Name, parent);
        }

        public bool DeleteResource(IStorageProviderSession session, ICloudFileSystemEntry entry)
        {
            if(entry.Parent != null)
                FsCache.Reset(GetSessionKey(session), GetCacheUrl(session, null, entry.Parent));
            FsCache.Reset(GetSessionKey(session), GetCacheUrl(session, null, entry));
            return _service.DeleteResource(session, entry);
        }

        public bool MoveResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            if (fsentry.Parent != null)
                FsCache.Reset(GetSessionKey(session), GetCacheUrl(session, null, fsentry.Parent));
            FsCache.Reset(GetSessionKey(session), GetCacheUrl(session, null, fsentry));
            FsCache.Reset(GetSessionKey(session), GetCacheUrl(session, null, newParent));
            return _service.MoveResource(session, fsentry, newParent);
        }

        public bool CopyResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, ICloudDirectoryEntry newParent)
        {
            FsCache.Reset(GetSessionKey(session), GetCacheUrl(session, null, newParent));
            return _service.CopyResource(session, fsentry, newParent);
        }

        public bool RenameResource(IStorageProviderSession session, ICloudFileSystemEntry fsentry, string newName)
        {
            if (fsentry.Parent != null)
                FsCache.Reset(GetSessionKey(session), GetCacheUrl(session, null, fsentry.Parent));
            FsCache.Reset(GetSessionKey(session), GetCacheUrl(session, null, fsentry)); 
            return _service.RenameResource(session, fsentry, newName);
        }

        public void StoreToken(IStorageProviderSession session, Dictionary<string, string> tokendata, ICloudStorageAccessToken token)
        {
            _service.StoreToken(session, tokendata, token);
        }

        public ICloudStorageAccessToken LoadToken(Dictionary<string, string> tokendata)
        {
            return _service.LoadToken(tokendata);
        }

        public string GetResourceUrl(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, string additionalPath)
        {
            return _service.GetResourceUrl(session, fileSystemEntry, additionalPath);
        }

        public void DownloadResourceContent(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, Stream targetDataStream, FileOperationProgressChanged progressCallback, object progressContext)
        {
            _service.DownloadResourceContent(session, fileSystemEntry, targetDataStream, progressCallback, progressContext);
        }

        public Stream CreateDownloadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry)
        {
            return _service.CreateDownloadStream(session, fileSystemEntry);
        }

        public void CommitStreamOperation(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, nTransferDirection Direction, Stream NotDisposedStream)
        {
            _service.CommitStreamOperation(session, fileSystemEntry, Direction, NotDisposedStream);
        }

        public void UploadResourceContent(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, Stream targetDataStream, FileOperationProgressChanged progressCallback, object progressContext)
        {
            _service.UploadResourceContent(session, fileSystemEntry, targetDataStream, progressCallback, progressContext);
        }

        public Stream CreateUploadStream(IStorageProviderSession session, ICloudFileSystemEntry fileSystemEntry, long uploadSize)
        {
            return _service.CreateUploadStream(session, fileSystemEntry, uploadSize);
        }

        public bool SupportsDirectRetrieve
        {
            get { return _service.SupportsDirectRetrieve; }
        }
    }
}