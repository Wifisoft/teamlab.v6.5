// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Success.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

// <success xmlns='urn:ietf:params:xml:ns:xmpp-sasl'/>

namespace ASC.Xmpp.Core.protocol.sasl
{
    /// <summary>
    ///   Summary description for Success.
    /// </summary>
    public class Success : Element
    {
        public Success()
        {
            TagName = "success";
            Namespace = Uri.SASL;
        }
    }
}