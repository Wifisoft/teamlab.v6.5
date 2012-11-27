// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Stream.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.Base
{
    /// <summary>
    ///   Summary description for Stream.
    /// </summary>
    public class Stream : Stanza
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Stream()
        {
            TagName = "stream";
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The StreamID of the current JabberSession. Returns null when none available.
        /// </summary>
        public string StreamId
        {
            get { return GetAttribute("id"); }

            set { SetAttribute("id", value); }
        }

        /// <summary>
        ///   See XMPP-Core 4.4.1 "Version Support"
        /// </summary>
        public string Version
        {
            get { return GetAttribute("version"); }

            set { SetAttribute("version", value); }
        }

        #endregion
    }
}