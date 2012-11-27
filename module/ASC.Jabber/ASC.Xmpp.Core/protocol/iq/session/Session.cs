// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Session.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.session
{
    /// <summary>
    ///   Summary description for Session.
    /// </summary>
    public class Session : Element
    {
        public Session()
        {
            TagName = "session";
            Namespace = Uri.SESSION;
        }
    }
}