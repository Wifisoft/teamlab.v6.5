// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="DiscoInfoIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.disco
{
    /// <summary>
    ///   Discovering Information About a Jabber Entity
    /// </summary>
    public class DiscoInfoIq : IQ
    {
        private readonly DiscoInfo m_DiscoInfo = new DiscoInfo();

        public DiscoInfoIq()
        {
            base.Query = m_DiscoInfo;
            GenerateId();
        }

        public DiscoInfoIq(IqType type) : this()
        {
            Type = type;
        }

        public new DiscoInfo Query
        {
            get { return m_DiscoInfo; }
        }
    }
}