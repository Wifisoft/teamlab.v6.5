// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="PubSubAction.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    public abstract class PubSubAction : Element
    {
        public PubSubAction()
        {
            Namespace = Uri.PUBSUB;
        }

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public Type Type
        {
            get { return (Type) GetAttributeEnum("type", typeof (Type)); }
            set
            {
                if (value == Type.NONE)
                    RemoveAttribute("type");
                else
                    SetAttribute("type", value.ToString());
            }
        }
    }
}