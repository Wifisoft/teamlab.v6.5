// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Address.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.multicast
{
    public class Address : Element
    {
        public Address()
        {
            TagName = "address";
            Namespace = protocol.Uri.ADDRESS;
        }

        public AddressType Type
        {
            get { return (AddressType) GetAttributeEnum("type", typeof (AddressType)); }

            set { SetAttribute("type", value.ToString()); }
        }

        public Jid Jid
        {
            get { return GetAttributeJid("jid"); }
            set { SetAttribute("jid", value); }
        }

        public bool Delivered
        {
            get { return GetAttributeBool("delivered"); }
            set { SetAttribute("delivered", value); }
        }

        public string Uri
        {
            get { return GetAttribute("uri"); }
            set { SetAttribute("uri", value); }
        }

        public string Desc
        {
            get { return GetAttribute("desc"); }
            set { SetAttribute("desc", value); }
        }

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }
    }
}