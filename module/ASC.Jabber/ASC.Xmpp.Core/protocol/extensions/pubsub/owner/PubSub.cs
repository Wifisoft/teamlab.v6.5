// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="PubSub.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.owner
{
    public class PubSub : Element
    {
        public PubSub()
        {
            TagName = "pubsub";
            Namespace = Uri.PUBSUB_OWNER;
        }

        public Delete Delete
        {
            get { return SelectSingleElement(typeof (Delete)) as Delete; }
            set
            {
                if (HasTag(typeof (Delete)))
                    RemoveTag(typeof (Delete));

                if (value != null)
                    AddChild(value);
            }
        }

        public Purge Purge
        {
            get { return SelectSingleElement(typeof (Purge)) as Purge; }
            set
            {
                if (HasTag(typeof (Purge)))
                    RemoveTag(typeof (Purge));

                if (value != null)
                    AddChild(value);
            }
        }

        public Subscribers Subscribers
        {
            get { return SelectSingleElement(typeof (Subscribers)) as Subscribers; }
            set
            {
                if (HasTag(typeof (Subscribers)))
                    RemoveTag(typeof (Subscribers));

                if (value != null)
                    AddChild(value);
            }
        }

        public Affiliates Affiliates
        {
            get { return SelectSingleElement(typeof (Affiliates)) as Affiliates; }
            set
            {
                if (HasTag(typeof (Affiliates)))
                    RemoveTag(typeof (Affiliates));

                if (value != null)
                    AddChild(value);
            }
        }

        public Configure Configure
        {
            get { return SelectSingleElement(typeof (Configure)) as Configure; }
            set
            {
                if (HasTag(typeof (Configure)))
                    RemoveTag(typeof (Configure));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}