// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="SessionIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.session
{
    /// <summary>
    ///   Starting the session, this is done after resource binding
    /// </summary>
    public class SessionIq : IQ
    {
        /*
		SEND:	<iq xmlns="jabber:client" id="agsXMPP_2" type="set" to="jabber.ru">
					<session xmlns="urn:ietf:params:xml:ns:xmpp-session" />
				</iq>
		RECV:	<iq xmlns="jabber:client" from="jabber.ru" type="result" id="agsXMPP_2">
					<session xmlns="urn:ietf:params:xml:ns:xmpp-session" />
				</iq> 
		 */
        private readonly Session m_Session = new Session();

        public SessionIq()
        {
            GenerateId();
            AddChild(m_Session);
        }

        public SessionIq(IqType type) : this()
        {
            Type = type;
        }

        public SessionIq(IqType type, Jid to) : this()
        {
            Type = type;
            To = to;
        }
    }
}