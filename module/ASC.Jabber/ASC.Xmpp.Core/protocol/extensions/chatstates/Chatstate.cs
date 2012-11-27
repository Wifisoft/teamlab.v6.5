// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Chatstate.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.extensions.chatstates
{
    /// <summary>
    ///   Enumeration of supported Chatstates (JEP-0085)
    /// </summary>
    public enum Chatstate
    {
        /// <summary>
        ///   No Chatstate at all
        /// </summary>
        None,

        /// <summary>
        ///   Active Chatstate
        /// </summary>
        active,

        /// <summary>
        ///   Inactive Chatstate
        /// </summary>
        inactive,

        /// <summary>
        ///   Composing Chatstate
        /// </summary>
        composing,

        /// <summary>
        ///   Gone Chatstate
        /// </summary>
        gone,

        /// <summary>
        ///   Paused Chatstate
        /// </summary>
        paused
    }
}