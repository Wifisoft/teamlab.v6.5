// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Received.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.msgreceipts
{
    /// <summary>
    /// </summary>
    public class Received : Element
    {
        /*         
         * <received xmlns='http://www.xmpp.org/extensions/xep-0184.html#ns'/>
         */

        public Received()
        {
            TagName = "received";
            Namespace = Uri.MSG_RECEIPT;
        }
    }
}