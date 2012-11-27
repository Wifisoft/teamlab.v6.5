// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="IQ.cs">
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
    ///   Summary description for Iq.
    /// </summary>
    public class IQ : client.IQ
    {
        #region << Constructors >>

        public IQ()
        {
            Namespace = Uri.ACCEPT;
        }

        public IQ(IqType type) : base(type)
        {
            Namespace = Uri.ACCEPT;
        }

        public IQ(Jid from, Jid to) : base(from, to)
        {
            Namespace = Uri.ACCEPT;
        }

        public IQ(IqType type, Jid from, Jid to) : base(type, from, to)
        {
            Namespace = Uri.ACCEPT;
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