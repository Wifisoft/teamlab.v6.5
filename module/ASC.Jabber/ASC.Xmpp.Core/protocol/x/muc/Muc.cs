// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Muc.cs">
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

    /*
     
        <x xmlns='http://jabber.org/protocol/muc'>
            <password>secret</password>
        </x>
     
     */

    /// <summary>
    ///   Summary description for MucUser.
    /// </summary>
    public class Muc : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Muc()
        {
            TagName = "x";
            Namespace = Uri.MUC;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The History object
        /// </summary>
        public History History
        {
            get { return SelectSingleElement(typeof (History)) as History; }

            set
            {
                if (HasTag(typeof (History)))
                {
                    RemoveTag(typeof (History));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        /// </summary>
        public string Password
        {
            get { return GetTag("password"); }

            set { SetTag("password", value); }
        }

        #endregion
    }
}