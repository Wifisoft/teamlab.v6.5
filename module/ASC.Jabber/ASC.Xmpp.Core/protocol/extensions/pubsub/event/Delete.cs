// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Delete.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.@event
{
    public class Delete : Element
    {
        #region << Constructors >>

        public Delete()
        {
            TagName = "delete";
            Namespace = Uri.PUBSUB_EVENT;
        }

        public Delete(string node) : this()
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