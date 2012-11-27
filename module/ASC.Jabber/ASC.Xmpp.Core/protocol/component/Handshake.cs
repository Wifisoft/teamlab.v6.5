// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Handshake.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.utils;

namespace ASC.Xmpp.Core.protocol.component
{
    //<handshake>aaee83c26aeeafcbabeabfcbcd50df997e0a2a1e</handshake>

    /// <summary>
    ///   Handshake Element
    /// </summary>
    public class Handshake : Stanza
    {
        public Handshake()
        {
            TagName = "handshake";
            Namespace = Uri.ACCEPT;
        }

        public Handshake(string password, string streamid) : this()
        {
            SetAuth(password, streamid);
        }

        /// <summary>
        ///   Digest (Hash) for authentication
        /// </summary>
        public string Digest
        {
            get { return Value; }
            set { Value = value; }
        }

        public void SetAuth(string password, string streamId)
        {
            Value = Hash.Sha1Hash(streamId + password);
        }
    }
}