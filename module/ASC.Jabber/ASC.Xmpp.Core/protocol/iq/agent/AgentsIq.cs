// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="AgentsIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

// Request Agents:
// <iq id='someid' to='myjabber.net' type='get'>
//		<query xmlns='jabber:iq:agents'/>
// </iq>

namespace ASC.Xmpp.Core.protocol.iq.agent
{
    /// <summary>
    ///   Summary description for AgentsIq.
    /// </summary>
    public class AgentsIq : IQ
    {
        private readonly Agents m_Agents = new Agents();

        public AgentsIq()
        {
            base.Query = m_Agents;
            GenerateId();
        }

        public AgentsIq(IqType type) : this()
        {
            Type = type;
        }

        public AgentsIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public AgentsIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Agents Query
        {
            get { return m_Agents; }
        }
    }
}