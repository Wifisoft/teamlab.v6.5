using System;
using System.Collections;
using System.Collections.Generic;
using ASC.Notify.Messages;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

namespace ASC.Notify.Engine
{
    public class NotifyRequest
    {
        public INotifySource NotifySource { get; internal set; }

        public INotifyAction NotifyAction { get; internal set; }

        public string ObjectID { get; internal set; }

        public IRecipient Recipient { get; internal set; }

        public List<ITagValue> Arguments { get; internal set; }

        public SendNoticeCallback NoticeCallback { get; internal set; }

        public string CurrentSender { get; internal set; }

        public INoticeMessage CurrentMessage { get; internal set; }

        public Hashtable Properties { get; private set; }


        public NotifyRequest(INotifySource notifySource, INotifyAction action, string objectID, IRecipient recipient)
        {
            if (notifySource == null) throw new ArgumentNullException("notifySource");
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");

            Properties = new Hashtable();
            Arguments = new List<ITagValue>();
            RequaredTags = new List<ITag>();
            Interceptors = new List<ISendInterceptor>();

            NotifySource = notifySource;
            Recipient = recipient;
            NotifyAction = action;
            ObjectID = objectID;
            NoticeCallback = null;

            IsNeedRetriveTags = true;
            IsNeedPatternFormatting = true;
            IsNeedCheckSubscriptions = true;
        }

        #region internal fields

        internal string[] SenderNames { get; set; }

        internal IPattern[] Patterns { get; set; }

        internal List<ITag> RequaredTags { get; set; }

        internal List<ISendInterceptor> Interceptors { get; set; }

        #endregion

        #region helpers

        internal bool IsNeedRetriveSenders
        {
            get { return SenderNames == null; }
        }

        internal bool IsNeedRetrivePatterns
        {
            get { return Patterns == null; }
        }

        internal bool IsNeedRetriveTags
        {
            get;
            set;
        }

        internal bool IsNeedPatternFormatting
        {
            get;
            set;
        }

        internal bool IsNeedCheckSubscriptions
        {
            get;
            set;
        }

        internal bool Intercept(InterceptorPlace place)
        {
            var result = false;
            foreach (var interceptor in Interceptors)
            {
                if ((interceptor.PreventPlace & place) == place)
                {
                    try
                    {
                        result = interceptor.PreventSend(this, place) || result;
                    }
                    catch { }
                }
            }
            return result;
        }

        internal IPattern GetSenderPattern(string senderName)
        {
            if (SenderNames == null || Patterns == null ||
                SenderNames.Length == 0 || Patterns.Length == 0 ||
                SenderNames.Length != Patterns.Length)
            {
                return null;
            }

            int index = Array.IndexOf(SenderNames, senderName);
            if (index < 0)
            {
                throw new ApplicationException(String.Format("Sender with tag {0} dnot found", senderName));
            }
            return Patterns[index];
        }

        internal NotifyRequest Split(IRecipient recipient)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            var newRequest = new NotifyRequest(NotifySource, NotifyAction, ObjectID, recipient);
            newRequest.NoticeCallback = NoticeCallback;
            newRequest.SenderNames = SenderNames;
            newRequest.Patterns = Patterns;
            newRequest.Arguments = new List<ITagValue>(Arguments);
            newRequest.RequaredTags = RequaredTags;
            newRequest.CurrentSender = CurrentSender;
            newRequest.CurrentMessage = CurrentMessage;
            newRequest.Interceptors.AddRange(Interceptors);
            return newRequest;
        }

        internal NoticeMessage CreateMessage(IDirectRecipient recipient)
        {
            return new NoticeMessage(recipient, NotifyAction, ObjectID);
        }

        #endregion
    }
}