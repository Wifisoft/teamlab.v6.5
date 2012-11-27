// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="PrivateLogItem.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.tm.history
{
    public class PrivateLogItem : Element
    {
        public PrivateLogItem()
        {
            TagName = "item";
            Namespace = Uri.X_TM_IQ_PRIVATELOG;
        }

        public Jid Jid
        {
            get { return GetAttributeJid("jid"); }
            set { SetAttribute("jid", value); }
        }

        public bool Log
        {
            get { return GetAttributeBool("log"); }
            set { SetAttribute("log", value); }
        }
    }
}