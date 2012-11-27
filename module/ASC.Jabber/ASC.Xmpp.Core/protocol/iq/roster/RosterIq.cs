// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="RosterIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

// Request Roster:
// <iq id='someid' to='myjabber.net' type='get'>
//		<query xmlns='jabber:iq:roster'/>
// </iq>

namespace ASC.Xmpp.Core.protocol.iq.roster
{
    /// <summary>
    ///   Build a new roster query, jabber:iq:roster
    /// </summary>
    public class RosterIq : IQ
    {
        private readonly Roster m_Roster = new Roster();

        public RosterIq()
        {
            base.Query = m_Roster;
            GenerateId();
        }

        public RosterIq(IqType type) : this()
        {
            Type = type;
        }

        public new Roster Query
        {
            get { return m_Roster; }
        }
    }
}