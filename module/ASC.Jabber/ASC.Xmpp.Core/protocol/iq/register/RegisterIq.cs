// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="RegisterIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.register
{
    /// <summary>
    ///   Used for registering new usernames on Jabber/XMPP Servers
    /// </summary>
    public class RegisterIq : IQ
    {
        private readonly Register m_Register = new Register();

        public RegisterIq()
        {
            base.Query = m_Register;
            GenerateId();
        }

        public RegisterIq(IqType type) : this()
        {
            Type = type;
        }

        public RegisterIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public RegisterIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Register Query
        {
            get { return m_Register; }
        }
    }
}