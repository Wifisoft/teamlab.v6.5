// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Inactive.cs">
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
    ///   User has not been actively participating in the chat session. User has not interacted with the chat interface for an intermediate period of time (e.g., 30 seconds).
    /// </summary>
    public class Inactive : Element
    {
        /// <summary>
        /// </summary>
        public Inactive()
        {
            TagName = Chatstate.inactive.ToString();
            ;
            Namespace = Uri.CHATSTATES;
        }
    }
}