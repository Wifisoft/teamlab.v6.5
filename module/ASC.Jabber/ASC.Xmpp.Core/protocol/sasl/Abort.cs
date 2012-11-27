// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Abort.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.sasl
{
    /// <summary>
    ///   Summary description for Abort.
    /// </summary>
    public class Abort : Element
    {
        public Abort()
        {
            TagName = "abort";
            Namespace = Uri.SASL;
        }
    }
}