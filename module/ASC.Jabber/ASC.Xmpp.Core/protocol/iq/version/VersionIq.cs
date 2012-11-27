// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="VersionIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.version
{
    /// <summary>
    ///   Summary description for VersionIq.
    /// </summary>
    public class VersionIq : IQ
    {
        private readonly Version m_Version = new Version();

        public VersionIq()
        {
            base.Query = m_Version;
            GenerateId();
        }

        public VersionIq(IqType type) : this()
        {
            Type = type;
        }

        public VersionIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public VersionIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Version Query
        {
            get { return m_Version; }
        }
    }
}