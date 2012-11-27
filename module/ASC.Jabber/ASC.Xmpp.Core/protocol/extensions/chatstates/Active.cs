// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Active.cs">
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
    ///   User is actively participating in the chat session. User accepts an initial content message, sends a content message, gives focus to the chat interface, or is otherwise paying attention to the conversation.
    /// </summary>
    public class Active : Element
    {
        /// <summary>
        /// </summary>
        public Active()
        {
            TagName = Chatstate.active.ToString();
            Namespace = Uri.CHATSTATES;
        }
    }
}