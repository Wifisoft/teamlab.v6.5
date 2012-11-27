using System;
using System.Text;

namespace ASC.Web.Controls.FileUploader.HttpModule
{
    internal class EntityBodyChunkStateWaiter
    {
        private bool collectInput;
        private StringBuilder input;
        private EntityBodyChunkReader _reader;

        private int guardSize;
        private char[] guard;
        private int _waitIndex;


        internal event EventHandler<EventArgs> MeetGuard;

        internal EntityBodyChunkStateWaiter(string waitFor, bool collectInput)
        {

            this.collectInput = collectInput;

            if (collectInput)
                input = new StringBuilder();

            guardSize = waitFor.Length;
            guard = waitFor.ToCharArray();

            _waitIndex = 0;
        }

        internal string Value
        {
            get { return !collectInput ? string.Empty : input.ToString(); }
        }

        public int Index
        {
            get { return _reader.Index; }
        }

        public int CharFound
        {
            get { return _waitIndex; }
        }

        internal void Reset()
        {
            input = new StringBuilder();
            _waitIndex = 0;
        }

        internal void Wait(byte[] buffer, int offset)
        {
            this._reader = new EntityBodyChunkReader(buffer, offset);
            Wait();
        }

        internal void Wait(EntityBodyChunkStateWaiter waiter)
        {
            this._reader = waiter._reader;
            Wait();
        }

        internal void Wait()
        {
            while (_reader.Read())
            {

                char c = _reader.Current;

                if (collectInput)
                    input.Append(c);

                if (c != guard[_waitIndex])
                {
                    _waitIndex = 0;
                }
                else
                {
                    _waitIndex++;
                    if (_waitIndex == guard.Length)
                    {
                        if (MeetGuard != null)
                        {
                            if (collectInput && input.Length >= guardSize)
                            {
                                input.Remove(input.Length - guardSize, guardSize);
                            }
                            MeetGuard(this, new EventArgs());
                        }
                        break;
                    }
                }
            }
        }

    }
}