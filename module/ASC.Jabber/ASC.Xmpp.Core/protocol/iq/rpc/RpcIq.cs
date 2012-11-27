// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="RpcIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.rpc
{
    /// <summary>
    ///   RpcIq.
    /// </summary>
    public class RpcIq : IQ
    {
        private readonly Rpc m_Rpc = new Rpc();

        public RpcIq()
        {
            base.Query = m_Rpc;
            GenerateId();
        }

        public RpcIq(IqType type) : this()
        {
            Type = type;
        }

        public RpcIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public RpcIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Rpc Query
        {
            get { return m_Rpc; }
        }
    }
}