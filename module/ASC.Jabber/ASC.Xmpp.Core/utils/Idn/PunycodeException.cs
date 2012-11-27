// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="PunycodeException.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System;

namespace ASC.Xmpp.Core.utils.Idn
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class PunycodeException : Exception
    {
        #region Members

        /// <summary>
        /// </summary>
        public static string BAD_INPUT = "Bad input.";

        /// <summary>
        /// </summary>
        public static string OVERFLOW = "Overflow.";

        #endregion

        #region Constructor

        /// <summary>
        ///   Creates a new PunycodeException.
        /// </summary>
        /// <param name="message"> message </param>
        public PunycodeException(string message) : base(message)
        {
        }

        #endregion
    }
}