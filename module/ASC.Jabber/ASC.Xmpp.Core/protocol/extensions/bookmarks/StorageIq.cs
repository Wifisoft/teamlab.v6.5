// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="StorageIq.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.@private;

namespace ASC.Xmpp.Core.protocol.extensions.bookmarks
{
    /// <summary>
    /// </summary>
    public class StorageIq : PrivateIq
    {
        public StorageIq()
        {
            Query.AddChild(new Storage());
        }

        public StorageIq(IqType type) : this()
        {
            Type = type;
        }

        public StorageIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public StorageIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }
    }
}