// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Request.cs">
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
    public class Request : Element
    {
        /*
         * <request xmlns='http://www.xmpp.org/extensions/xep-0184.html#ns'/>         
         */

        public Request()
        {
            TagName = "request";
            Namespace = Uri.MSG_RECEIPT;
        }
    }
}