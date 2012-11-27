// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Status.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.muc
{

    #region usings

    #endregion

    /*
    <x xmlns='http://jabber.org/protocol/muc#user'>
        <status code='100'/>
    </x>    
    */

    /// <summary>
    ///   Summary description for MucUser.
    /// </summary>
    public class Status : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Status()
        {
            TagName = "status";
            Namespace = Uri.MUC_USER;
        }

        /// <summary>
        /// </summary>
        /// <param name="code"> </param>
        public Status(StatusCode code) : this()
        {
            Code = code;
        }

        /// <summary>
        /// </summary>
        /// <param name="code"> </param>
        public Status(int code) : this()
        {
            SetAttribute("code", code);
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public StatusCode Code
        {
            get { return (StatusCode) GetAttributeEnum("code", typeof (StatusCode)); }

            set { SetAttribute("code", value.ToString()); }
        }

        #endregion
    }
}