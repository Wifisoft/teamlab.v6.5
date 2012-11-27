// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Actor.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.x.muc
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class Actor : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Actor()
        {
            TagName = "actor";
            Namespace = Uri.MUC_USER;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public Jid Jid
        {
            get { return GetAttributeJid("jid"); }

            set { SetAttribute("jid", value); }
        }

        #endregion
    }
}