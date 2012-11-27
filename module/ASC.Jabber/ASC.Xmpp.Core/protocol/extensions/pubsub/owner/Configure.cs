// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Configure.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.x.data;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.owner
{
    public class Configure : Element
    {
        #region << Constructor >>

        public Configure()
        {
            TagName = "configure";
            Namespace = Uri.PUBSUB_OWNER;
        }

        public Configure(string node)
        {
            Node = node;
        }

        #endregion

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        /// <summary>
        ///   The x-Data Element
        /// </summary>
        public Data Data
        {
            get { return SelectSingleElement(typeof (Data)) as Data; }
            set
            {
                if (HasTag(typeof (Data)))
                    RemoveTag(typeof (Data));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}