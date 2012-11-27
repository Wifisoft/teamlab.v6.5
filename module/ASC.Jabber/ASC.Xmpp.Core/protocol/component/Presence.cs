// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Presence.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region Using directives

using ASC.Xmpp.Core.protocol.client;

#endregion

namespace ASC.Xmpp.Core.protocol.component
{
    /// <summary>
    ///   Summary description for Presence.
    /// </summary>
    public class Presence : client.Presence
    {
        #region << Constructors >>

        public Presence()
        {
            Namespace = Uri.ACCEPT;
        }

        public Presence(ShowType show, string status) : this()
        {
            Show = show;
            Status = status;
        }

        public Presence(ShowType show, string status, int priority) : this(show, status)
        {
            Priority = priority;
        }

        #endregion

        /// <summary>
        ///   Error Child Element
        /// </summary>
        public new Error Error
        {
            get { return SelectSingleElement(typeof (Error)) as Error; }
            set
            {
                if (HasTag(typeof (Error)))
                    RemoveTag(typeof (Error));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}