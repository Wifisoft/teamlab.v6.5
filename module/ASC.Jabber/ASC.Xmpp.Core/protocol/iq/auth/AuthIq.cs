// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="AuthIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.auth
{
    /// <summary>
    ///   Summary description for AuthIq.
    /// </summary>
    public class AuthIq : IQ
    {
        private readonly Auth m_Auth = new Auth();

        public AuthIq()
        {
            base.Query = m_Auth;
            GenerateId();
        }

        public AuthIq(IqType type) : this()
        {
            Type = type;
        }

        public AuthIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public AuthIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Auth Query
        {
            get { return m_Auth; }
        }
    }
}