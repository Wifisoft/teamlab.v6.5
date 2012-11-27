// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Paused.cs">
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
    ///   User had been composing but now has stopped. User was composing but has not interacted with the message input interface for a short period of time (e.g., 5 seconds).
    /// </summary>
    public class Paused : Element
    {
        /// <summary>
        /// </summary>
        public Paused()
        {
            TagName = Chatstate.paused.ToString();
            ;
            Namespace = Uri.CHATSTATES;
        }
    }
}