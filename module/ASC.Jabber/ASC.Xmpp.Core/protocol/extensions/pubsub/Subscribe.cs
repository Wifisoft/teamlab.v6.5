// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Subscribe.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    public class Subscribe : Element
    {
        #region << Constructors >>

        public Subscribe()
        {
            TagName = "subscribe";
            Namespace = Uri.PUBSUB;
        }

        public Subscribe(string node, Jid jid) : this()
        {
            Node = node;
            Jid = jid;
        }

        #endregion

        /*
        Example 25. Entity subscribes to a node

        <iq type="set"
            from="sub1@foo.com/home"
            to="pubsub.jabber.org"
            id="sub1">
          <pubsub xmlns="http://jabber.org/protocol/pubsub">
            <subscribe
                node="generic/pgm-mp3-player"
                jid="sub1@foo.com"/>
          </pubsub>
        </iq>
        */

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public Jid Jid
        {
            get
            {
                if (HasAttribute("jid"))
                    return new Jid(GetAttribute("jid"));
                else
                    return null;
            }
            set
            {
                if (value != null)
                    SetAttribute("jid", value.ToString());
                else
                    RemoveAttribute("jid");
            }
        }
    }
}