// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="TimeIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.time
{
    /// <summary>
    ///   Summary description for TimeIq.
    /// </summary>
    public class TimeIq : IQ
    {
        private readonly Time m_Time = new Time();

        public TimeIq()
        {
            base.Query = m_Time;
            GenerateId();
        }

        public TimeIq(IqType type) : this()
        {
            Type = type;
        }

        public TimeIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public TimeIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Time Query
        {
            get { return m_Time; }
        }
    }
}