// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Activate.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.bytestreams
{
    public class Activate : Element
    {
        public Activate()
        {
            TagName = "activate";
            Namespace = Uri.BYTESTREAMS;
        }

        public Activate(Jid jid) : this()
        {
            Jid = jid;
        }

        /// <summary>
        ///   the full JID of the Target to activate
        /// </summary>
        public Jid Jid
        {
            get
            {
                if (Value == null)
                    return null;
                else
                    return new Jid(Value);
            }
            set
            {
                if (value != null)
                    Value = value.ToString();
                else
                    Value = null;
            }
        }
    }
}