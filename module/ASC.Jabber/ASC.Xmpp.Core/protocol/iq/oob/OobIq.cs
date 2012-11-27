// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="OobIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.oob
{
    /// <summary>
    ///   Summary description for OobIq.
    /// </summary>
    public class OobIq : IQ
    {
        private readonly Oob m_Oob = new Oob();

        public OobIq()
        {
            base.Query = m_Oob;
            GenerateId();
        }

        public OobIq(IqType type) : this()
        {
            Type = type;
        }

        public OobIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public OobIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Oob Query
        {
            get { return m_Oob; }
        }
    }
}