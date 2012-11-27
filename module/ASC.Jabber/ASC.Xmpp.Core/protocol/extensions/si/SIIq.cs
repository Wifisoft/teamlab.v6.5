// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="SIIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.extensions.si
{
    /*
    <iq id="jcl_18" to="xxx" type="result" from="yyy">
        <si xmlns="http://jabber.org/protocol/si">
            <feature xmlns="http://jabber.org/protocol/feature-neg">
                <x xmlns="jabber:x:data" type="submit">
                    <field var="stream-method">
                        <value>http://jabber.org/protocol/bytestreams</value>
                    </field>
                </x>
            </feature>
        </si>
    </iq>
 
    */

    /// <summary>
    /// </summary>
    public class SIIq : IQ
    {
        private readonly SI m_SI = new SI();

        public SIIq()
        {
            GenerateId();
            AddChild(m_SI);
        }

        public SIIq(IqType type)
            : this()
        {
            Type = type;
        }

        public SIIq(IqType type, Jid to)
            : this(type)
        {
            To = to;
        }

        public SIIq(IqType type, Jid to, Jid from)
            : this(type, to)
        {
            From = from;
        }

        public SI SI
        {
            get { return m_SI; }
        }
    }
}