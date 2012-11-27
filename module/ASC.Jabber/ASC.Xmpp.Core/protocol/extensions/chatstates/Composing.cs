// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Composing.cs">
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
    ///   User is composing a message. User is interacting with a message input interface specific to this chat session (e.g., by typing in the input area of a chat window).
    /// </summary>
    public class Composing : Element
    {
        public Composing()
        {
            TagName = Chatstate.composing.ToString();
            ;
            Namespace = Uri.CHATSTATES;
        }
    }
}