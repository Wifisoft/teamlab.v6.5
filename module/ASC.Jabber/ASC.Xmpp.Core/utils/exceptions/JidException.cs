// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="JidException.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region file header

#endregion

#region file header

#endregion

using System;

namespace ASC.Xmpp.Core.utils.exceptions
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class JidException : Exception
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public JidException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"> </param>
        public JidException(string msg) : base(msg)
        {
        }

        #endregion
    }
}