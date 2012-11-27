// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Delete.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.owner
{
    // Only the Namespace is different to Delete in the event Namespace

    public class Delete : @event.Delete
    {
        #region << Constructors >>

        public Delete()
        {
            Namespace = Uri.PUBSUB_OWNER;
        }

        public Delete(string node)
        {
            Node = node;
        }

        #endregion
    }
}