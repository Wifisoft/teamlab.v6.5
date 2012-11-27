// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="BrowseIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.browse
{
    /// <summary>
    ///   Summary description for BrowseIq.
    /// </summary>
    public class BrowseIq : IQ
    {
        private readonly Browse m_Browse = new Browse();

        public BrowseIq()
        {
            base.Query = m_Browse;
            GenerateId();
        }

        public BrowseIq(IqType type) : this()
        {
            Type = type;
        }

        public BrowseIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public BrowseIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Browse Query
        {
            get { return m_Browse; }
        }
    }
}