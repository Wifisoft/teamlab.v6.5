// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="PrivacyIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.privacy
{
    /// <summary>
    ///   Summary description for PrivateIq.
    /// </summary>
    public class PrivacyIq : IQ
    {
        private readonly Privacy m_Privacy = new Privacy();

        public PrivacyIq()
        {
            base.Query = m_Privacy;
            GenerateId();
        }

        public PrivacyIq(IqType type)
            : this()
        {
            Type = type;
        }

        public PrivacyIq(IqType type, Jid to)
            : this(type)
        {
            To = to;
        }

        public PrivacyIq(IqType type, Jid to, Jid from)
            : this(type, to)
        {
            From = from;
        }

        public new Privacy Query
        {
            get { return m_Privacy; }
        }
    }
}