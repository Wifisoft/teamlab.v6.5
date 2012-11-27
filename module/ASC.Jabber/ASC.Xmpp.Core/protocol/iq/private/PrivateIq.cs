// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="PrivateIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.@private
{
    /// <summary>
    ///   Summary description for PrivateIq.
    /// </summary>
    public class PrivateIq : IQ
    {
        private readonly Private m_Private = new Private();

        public PrivateIq()
        {
            base.Query = m_Private;
            GenerateId();
        }

        public PrivateIq(IqType type) : this()
        {
            Type = type;
        }

        public PrivateIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public PrivateIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Private Query
        {
            get { return m_Private; }
        }
    }
}