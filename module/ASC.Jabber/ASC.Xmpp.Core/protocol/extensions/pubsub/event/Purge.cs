// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Purge.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.@event
{
    public class Purge : Element
    {
        #region << Constructors >>

        public Purge()
        {
            TagName = "purge";
            Namespace = Uri.PUBSUB_EVENT;
        }

        public Purge(string node) : this()
        {
            Node = node;
        }

        #endregion

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }
    }
}