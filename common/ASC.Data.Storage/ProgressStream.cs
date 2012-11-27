using System;
using System.IO;

namespace ASC.Data.Storage
{
    public class ProgressStream : Stream
    {
        #region Delegates

        public delegate void ReadProgressDelegate(ProgressStream sender, int progress);

        #endregion

        private readonly Stream _inner;
        private readonly long _innerLength = long.MaxValue;

        public ProgressStream(Stream inner)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            _inner = inner;
            try
            {
                _innerLength = _inner.Length;
            }
            catch (Exception)
            {
            }
        }

        public override bool CanRead
        {
            get { return _inner.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _inner.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _inner.CanWrite; }
        }

        public override long Length
        {
            get { return _inner.Length; }
        }

        public override long Position
        {
            get { return _inner.Position; }
            set { _inner.Position = value; }
        }

        public event ReadProgressDelegate OnReadProgress;

        public void InvokeOnReadProgress(int progress)
        {
            ReadProgressDelegate handler = OnReadProgress;
            if (handler != null) handler(this, progress);
        }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readed = _inner.Read(buffer, offset, count);
            OnReadProgress(this, (int) (_inner.Position/(double) _innerLength*100));
            return readed;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
        }
    }
}