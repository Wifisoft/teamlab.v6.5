// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Purge.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.owner
{
    // Only the Namespace is different to Purge in the Event Namespace
    public class Purge : @event.Purge
    {
        #region << Constructors >>

        public Purge()
        {
            Namespace = Uri.PUBSUB_OWNER;
        }

        public Purge(string node) : this()
        {
            Node = node;
        }

        #endregion
    }
}