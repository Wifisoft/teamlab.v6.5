// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="BindIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.bind
{
    /// <summary>
    ///   Summary description for BindIq.
    /// </summary>
    public class BindIq : IQ
    {
        private readonly Bind m_Bind = new Bind();

        public BindIq()
        {
            GenerateId();
            AddChild(m_Bind);
        }

        public BindIq(IqType type) : this()
        {
            Type = type;
        }

        public BindIq(IqType type, Jid to) : this()
        {
            Type = type;
            To = to;
        }

        public BindIq(IqType type, Jid to, string resource) : this(type, to)
        {
            m_Bind.Resource = resource;
        }

        public new Bind Query
        {
            get { return m_Bind; }
        }
    }
}