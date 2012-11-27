// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="PubSubIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    public class PubSubIq : IQ
    {
        /*
            Example 1. Entity requests a new node with default configuration.

            <iq type="set"
                from="pgm@jabber.org"
                to="pubsub.jabber.org"
                id="create1">
                <pubsub xmlns="http://jabber.org/protocol/pubsub">
                    <create node="generic/pgm-mp3-player"/>
                    <configure/>
                </pubsub>
            </iq>
        */
        private readonly PubSub m_PubSub = new PubSub();

        #region << Constructors >>

        public PubSubIq()
        {
            GenerateId();
            AddChild(m_PubSub);
        }

        public PubSubIq(IqType type) : this()
        {
            Type = type;
        }

        public PubSubIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public PubSubIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        #endregion

        public PubSub PubSub
        {
            get { return m_PubSub; }
        }
    }
}