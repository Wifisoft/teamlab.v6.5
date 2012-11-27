// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Subscriptions.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    public class Subscriptions : Element
    {
        /*
            Example 14. Entity requests all current subscriptions

            <iq type='get'
                from='francisco@denmark.lit/barracks'
                to='pubsub.shakespeare.lit'
                id='subscriptions1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <subscriptions/>
              </pubsub>
            </iq>
                       

            Example 15. Service returns all current subscriptions

            <iq type='result'
                from='pubsub.shakespeare.lit'
                to='francisco@denmark.lit'
                id='subscriptions1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub'>
                <subscriptions>
                  <subscription node='node1' jid='francisco@denmark.lit' subscription='subscribed'/>
                  <subscription node='node2' jid='francisco@denmark.lit' subscription='subscribed'/>
                  <subscription node='node5' jid='francisco@denmark.lit' subscription='unconfigured'/>
                  <subscription node='node6' jid='francisco@denmark.lit' subscription='pending'/>
                </subscriptions>
              </pubsub>
            </iq>
    
        */

        public Subscriptions()
        {
            TagName = "subscriptions";
            Namespace = Uri.PUBSUB;
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public Subscription AddSubscription()
        {
            var sub = new Subscription();
            AddChild(sub);
            return sub;
        }

        /// <summary>
        /// </summary>
        /// <param name="item"> </param>
        /// <returns> </returns>
        public Subscription AddSubscription(Subscription sub)
        {
            AddChild(sub);
            return sub;
        }

        public Subscription[] GetSubscriptions()
        {
            ElementList nl = SelectElements(typeof (Subscription));
            var items = new Subscription[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                items[i] = (Subscription) e;
                i++;
            }
            return items;
        }
    }
}