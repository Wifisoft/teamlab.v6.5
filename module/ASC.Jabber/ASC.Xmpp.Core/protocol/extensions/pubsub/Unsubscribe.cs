// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Unsubscribe.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
        
        Example 38. Entity unsubscribes from a node

        <iq type='set'
            from='francisco@denmark.lit/barracks'
            to='pubsub.shakespeare.lit'
            id='unsub1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub'>
             <unsubscribe
                 node='blogs/princely_musings'
                 jid='francisco@denmark.lit'/>
          </pubsub>
        </iq>
    
    */

    // looks exactly the same as subscribe, but has an additional Attribute subid

    public class Unsubscribe : Subscribe
    {
        #region << Constructors >>

        public Unsubscribe()
        {
            TagName = "unsubscribe";
        }

        public Unsubscribe(string node, Jid jid) : this()
        {
            Node = node;
            Jid = jid;
        }

        public Unsubscribe(string node, Jid jid, string subid)
            : this(node, jid)
        {
            SubId = subid;
        }

        #endregion

        public string SubId
        {
            get { return GetAttribute("subid"); }
            set { SetAttribute("subid", value); }
        }
    }
}