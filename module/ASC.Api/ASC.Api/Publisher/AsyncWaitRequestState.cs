using System;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace ASC.Api.Publisher
{
    internal class AsyncWaitRequestState : IAsyncResult
    {
        internal AsyncCallback Cb;
        private readonly HttpContext _ctx;
        internal object ExtraData;


        public AsyncWaitRequestState(HttpContext ctx,
                                 AsyncCallback cb,
                                 object extraData)
        {
            _ctx = ctx;
            Cb = cb;
            ExtraData = extraData;
        }
        
        public void OnCompleted()
        {
            if (Cb != null)
                Cb(this);
        }

        // IAsyncResult interface property implementations

        #region IAsyncResult Members

        public object AsyncState
        {
            get { return (ExtraData); }
        }

        public bool CompletedSynchronously
        {
            get { return (false); }
        }

        public bool IsCompleted
        {
            get { return true; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return null;
            }
        }

        public HttpContext Context
        {
            get { return _ctx; }
        }

        #endregion

    }
}