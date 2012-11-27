// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="PingIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.extensions.ping
{
    /// <summary>
    /// </summary>
    public class PingIq : IQ
    {
        private readonly Ping m_Ping = new Ping();

        #region << Constructors >>

        public PingIq()
        {
            base.Query = m_Ping;
            GenerateId();
        }

        public PingIq(Jid to) : this()
        {
            To = to;
        }

        public PingIq(Jid to, Jid from) : this()
        {
            To = to;
            From = from;
        }

        #endregion

        public new Ping Query
        {
            get { return m_Ping; }
        }
    }
}