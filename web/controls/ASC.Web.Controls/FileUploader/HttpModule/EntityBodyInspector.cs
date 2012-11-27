using System;
using System.Web;

namespace ASC.Web.Controls.FileUploader.HttpModule
{
    internal class EntityBodyInspector
    {
        private System.Text.Encoding _encoding;
        private EntityBodyChunkStateWaiter _current;
        private EntityBodyChunkStateWaiter _boundaryWaiter;
        private EntityBodyChunkStateWaiter _boundaryInfoWaiter;
        private EntityBodyChunkStateWaiter _formValueWaiter;

        private string _lastCdName;

        private UploadProgressStatistic statistic;

        internal EntityBodyInspector(HttpUploadWorkerRequest request)
        {
            statistic = new UploadProgressStatistic();
            
            statistic.TotalBytes = request.GetTotalEntityBodyLength();

            string contentType = request.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType);

            string boundary = string.Format("--{0}\r\n", UploadProgressUtils.GetBoundary(contentType));
            _encoding = System.Text.Encoding.UTF8;

            _boundaryWaiter = new EntityBodyChunkStateWaiter(boundary, false);
            _boundaryWaiter.MeetGuard += new EventHandler<EventArgs>(boundaryWaiter_MeetGuard);
            _current = _boundaryWaiter;

            _boundaryInfoWaiter = new EntityBodyChunkStateWaiter("\r\n\r\n", true);
            _boundaryInfoWaiter.MeetGuard += new EventHandler<EventArgs>(boundaryInfoWaiter_MeetGuard);

            _formValueWaiter = new EntityBodyChunkStateWaiter("\r\n", true);
            _formValueWaiter.MeetGuard += new EventHandler<EventArgs>(formValueWaiter_MeetGuard);

            _lastCdName = string.Empty;
        }

        internal void EndRequest()
        {
            statistic.EndUpload();
        }

        internal void Inspect(byte[] buffer, int offset, int size)
        {
            if (buffer == null)
                return;

            statistic.AddUploadedBytes(buffer.Length);
            Inspect(buffer, offset);
        }

        private void Inspect(byte[] buffer, int offset)
        {
            if (buffer == null)
                return;
            
            _current.Wait(buffer, offset);
        }

        void boundaryWaiter_MeetGuard(object sender, EventArgs e)
        {
            EntityBodyChunkStateWaiter sw = sender as EntityBodyChunkStateWaiter;
            sw.Reset();
            _current = _boundaryInfoWaiter;
            _current.Wait(sw);
        }

        void boundaryInfoWaiter_MeetGuard(object sender, EventArgs e)
        {
            EntityBodyChunkStateWaiter sw = sender as EntityBodyChunkStateWaiter;
            ContentDispositionInfo cdi = UploadProgressUtils.GetContentDisposition(sw.Value);
            sw.Reset();
            if (!cdi.IsFile)
            {
                _lastCdName = cdi.name;
                _current = _formValueWaiter;
                _current.Wait(sw);
            }
            else
            {
                statistic.BeginFileUpload(cdi.filename);
                _current = _boundaryWaiter;
                _current.Wait(sw);
            }
        }

        void formValueWaiter_MeetGuard(object sender, EventArgs e)
        {
            EntityBodyChunkStateWaiter sw = sender as EntityBodyChunkStateWaiter;

            string fieldValue = sw.Value;
            statistic.AddFormField(_lastCdName, fieldValue);

            sw.Reset();

            _current = _boundaryWaiter;
            _current.Wait();
        }
    }
}