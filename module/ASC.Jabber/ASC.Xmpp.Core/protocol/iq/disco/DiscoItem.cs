// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="DiscoItem.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.disco
{
    public enum DiscoAction
    {
        NONE = -1,
        remove,
        update
    }

    ///<summary>
    ///</summary>
    public class DiscoItem : Element
    {
        public DiscoItem()
        {
            TagName = "item";
            Namespace = Uri.DISCO_ITEMS;
        }

        public Jid Jid
        {
            get { return new Jid(GetAttribute("jid")); }
            set { SetAttribute("jid", value.ToString()); }
        }

        public string Name
        {
            get { return GetAttribute("name"); }
            set { SetAttribute("name", value); }
        }

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public DiscoAction Action
        {
            get { return (DiscoAction) GetAttributeEnum("action", typeof (DiscoAction)); }
            set
            {
                if (value == DiscoAction.NONE)
                    RemoveAttribute("action");
                else
                    SetAttribute("action", value.ToString());
            }
        }
    }
}