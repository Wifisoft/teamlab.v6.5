using System;
using System.IO;
using AppLimit.CloudComputing.SharpBox.StorageProvider.API;
using System.Threading;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.BaseObjects
{
    internal class BaseFileEntryDataTransfer : ICloudFileDataTransfer
    {
        private ICloudFileSystemEntry _fsEntry;
        private IStorageProviderService _service;
        private IStorageProviderSession _session;

        private class BaseFileEntryDataTransferAsyncContext
        {
            public Object ProgressContext { get; set; }
            public FileOperationProgressChanged ProgressCallback { get; set; }
        }

        public BaseFileEntryDataTransfer(ICloudFileSystemEntry fileSystemEntry, IStorageProviderService service, IStorageProviderSession session)
        {
            _fsEntry = fileSystemEntry;
            _session = session;
            _service = service;
        }

        #region ICloudFileDataTransfer Members

        public void Transfer(Stream targetDataStream, nTransferDirection direction)
        {
            Transfer(targetDataStream, direction, null, null);
        }

        public void Transfer(Stream targetDataStream, nTransferDirection direction, FileOperationProgressChanged progressCallback, object progressContext)
        {
            // call the transfer interface in the service provider
            if (direction == nTransferDirection.nUpload)
            {
                if (_session.ServiceConfiguration.Limits.MaxUploadFileSize != -1 && targetDataStream.Length > _session.ServiceConfiguration.Limits.MaxUploadFileSize)
                    throw new Exceptions.SharpBoxException(Exceptions.SharpBoxErrorCodes.ErrorLimitExceeded);
                else
                    _service.UploadResourceContent(_session, _fsEntry, targetDataStream, progressCallback, progressContext);
            }
            else
            {
                if (_session.ServiceConfiguration.Limits.MaxDownloadFileSize != -1 && _fsEntry.Length > _session.ServiceConfiguration.Limits.MaxDownloadFileSize)
                    throw new Exceptions.SharpBoxException(Exceptions.SharpBoxErrorCodes.ErrorLimitExceeded);
                else
                    _service.DownloadResourceContent(_session, _fsEntry, targetDataStream, progressCallback, progressContext);
            }
        }

        public void TransferAsyncProgress(Stream targetDataStream, nTransferDirection direction, FileOperationProgressChanged progressCallback, object progressContext)
        {
            var ctx = new BaseFileEntryDataTransferAsyncContext
                          {
                              ProgressCallback = progressCallback,
                              ProgressContext = progressContext
                          };

            Transfer(targetDataStream, direction, FileOperationProgressChangedAsyncHandler, ctx);
        }


        public Stream GetDownloadStream()
        {
            return _service.CreateDownloadStream(_session, _fsEntry);
        }

        public Stream GetUploadStream(long uploadSize)
        {
            return _service.CreateUploadStream(_session, _fsEntry, uploadSize);
        }

#if !WINDOWS_PHONE
        void ICloudFileDataTransfer.Serialize(System.Runtime.Serialization.IFormatter dataFormatter, object objectGraph)
        {
            using (var cache = new MemoryStream())
            {
                // serialize into the cache
                dataFormatter.Serialize(cache, objectGraph);

                // go to start
                cache.Position = 0;

                // transfer the cache
                _fsEntry.GetDataTransferAccessor().Transfer(cache, nTransferDirection.nUpload);
            }
        }

        object ICloudFileDataTransfer.Deserialize(System.Runtime.Serialization.IFormatter dataFormatter)
        {
            using (var cache = new MemoryStream())
            {
                // get the data
                _fsEntry.GetDataTransferAccessor().Transfer(cache, nTransferDirection.nDownload);

                // go to the start
                cache.Position = 0;

                // go ahead
                return dataFormatter.Deserialize(cache);
            }
        }
#endif

        #endregion

        #region Internal callbacks

        private void FileOperationProgressChangedAsyncHandler(object sender, FileDataTransferEventArgs e)
        {
            var ctx = e.CustomnContext as BaseFileEntryDataTransferAsyncContext;

            // define the thread
            ThreadPool.QueueUserWorkItem((object state) =>
                                             {
                                                 // change the transferevent args
                                                 var eAsync = e.Clone() as FileDataTransferEventArgs;
                                                 ctx.ProgressCallback(sender, eAsync);
                                             });
        }

        #endregion
    }
}