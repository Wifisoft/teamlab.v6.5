// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Gone.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.chatstates
{
    /// <summary>
    ///   User has effectively ended their participation in the chat session. User has not interacted with the chat interface, system, or device for a relatively long period of time (e.g., 2 minutes), or has terminated the chat interface (e.g., by closing the chat window).
    /// </summary>
    public class Gone : Element
    {
        /// <summary>
        /// </summary>
        public Gone()
        {
            TagName = Chatstate.gone.ToString();
            ;
            Namespace = Uri.CHATSTATES;
        }
    }
}