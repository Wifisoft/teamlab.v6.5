// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Mechanism.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.authorization
{

    #region usings

    #endregion

    /// <summary>
    ///   Summary description for Mechanism.
    /// </summary>
    public abstract class Mechanism
    {
        #region Members

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// </summary>
        public string Username { // lower case that until i implement our c# port of libIDN
            get; set; }

        //public XmppClientConnection XmppClientConnection { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <summary>
        /// </summary>
        /// <param name="con"> </param>
        public abstract void Init();

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public abstract void Parse(Node e);

        #endregion
    }
}