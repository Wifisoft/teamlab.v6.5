using System;
using System.Runtime.Serialization;

namespace ASC.Notify.Messages
{
    [Serializable]
    [DataContract]
    public class NotifyMessage
    {
        [DataMember(Order = 1)]
        public int Tenant
        {
            get;
            set;
        }

        [DataMember(Order = 2)]
        public string Sender
        {
            get;
            set;
        }

        [DataMember(Order = 3)]
        public string From
        {
            get;
            set;
        }

        [DataMember(Order = 4)]
        public string To
        {
            get;
            set;
        }

        [DataMember(Order = 5)]
        public string ReplyTo
        {
            get;
            set;
        }

        [DataMember(Order = 6)]
        public string Subject
        {
            get;
            set;
        }

        [DataMember(Order = 7)]
        public string ContentType
        {
            get;
            set;
        }

        [DataMember(Order = 8)]
        public string Content
        {
            get;
            set;
        }

        [DataMember(Order = 9)]
        public DateTime CreationDate
        {
            get;
            set;
        }
    }
}
