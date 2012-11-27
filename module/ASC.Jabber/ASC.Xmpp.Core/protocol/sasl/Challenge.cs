// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Challenge.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

//<challenge xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//cmVhbG09InNvbWVyZWFsbSIsbm9uY2U9Ik9BNk1HOXRFUUdtMmhoIixxb3A9ImF1dGgi
//LGNoYXJzZXQ9dXRmLTgsYWxnb3JpdGhtPW1kNS1zZXNzCg==
//</challenge>

namespace ASC.Xmpp.Core.protocol.sasl
{
    /// <summary>
    ///   Summary description for Challenge.
    /// </summary>
    public class Challenge : Element
    {
        public Challenge()
        {
            TagName = "challenge";
            Namespace = Uri.SASL;
        }
    }
}