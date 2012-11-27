// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="TMToken.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

// <auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='DIGEST-MD5'/>

namespace ASC.Xmpp.Core.protocol.sasl
{
    /// <summary>
    ///   Summary description for Auth.
    /// </summary>
    public class TMToken : Element
    {
        public TMToken()
        {
            TagName = "x-tmtoken";
            Namespace = Uri.SASL;
        }
    }
}