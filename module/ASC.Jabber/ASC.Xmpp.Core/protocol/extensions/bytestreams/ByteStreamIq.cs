// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="ByteStreamIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.extensions.bytestreams
{
    /// <summary>
    ///   a Bytestream IQ
    /// </summary>
    public class ByteStreamIq : IQ
    {
        private readonly ByteStream m_ByteStream = new ByteStream();

        public ByteStreamIq()
        {
            base.Query = m_ByteStream;
            GenerateId();
        }

        public ByteStreamIq(IqType type) : this()
        {
            Type = type;
        }

        public ByteStreamIq(IqType type, Jid to)
            : this(type)
        {
            To = to;
        }

        public ByteStreamIq(IqType type, Jid to, Jid from)
            : this(type, to)
        {
            From = from;
        }

        public ByteStreamIq(IqType type, Jid to, Jid from, string Id)
            : this(type, to, from)
        {
            this.Id = Id;
        }

        public new ByteStream Query
        {
            get { return m_ByteStream; }
        }
    }
}