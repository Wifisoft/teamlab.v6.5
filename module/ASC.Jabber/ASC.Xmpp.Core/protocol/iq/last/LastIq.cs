// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="LastIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.last
{
    /// <summary>
    ///   Summary description for LastIq.
    /// </summary>
    public class LastIq : IQ
    {
        private readonly Last m_Last = new Last();

        public LastIq()
        {
            base.Query = m_Last;
            GenerateId();
        }

        public LastIq(IqType type) : this()
        {
            Type = type;
        }

        public LastIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public LastIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Last Query
        {
            get { return m_Last; }
        }
    }
}