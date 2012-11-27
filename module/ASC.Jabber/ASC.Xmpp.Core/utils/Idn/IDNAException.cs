// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="IDNAException.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region file header

#endregion

using System;

namespace ASC.Xmpp.Core.utils.Idn
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class IDNAException : Exception
    {
        #region Members

        /// <summary>
        /// </summary>
        public static string CONTAINS_ACE_PREFIX = "ACE prefix (xn--) not allowed.";

        /// <summary>
        /// </summary>
        public static string CONTAINS_HYPHEN = "Leading or trailing hyphen not allowed.";

        /// <summary>
        /// </summary>
        public static string CONTAINS_NON_LDH = "Contains non-LDH characters.";

        /// <summary>
        /// </summary>
        public static string TOO_LONG = "String too long.";

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="m"> </param>
        public IDNAException(string m) : base(m)
        {
        }

        // TODO
        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public IDNAException(StringprepException e) : base(string.Empty, e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public IDNAException(PunycodeException e) : base(string.Empty, e)
        {
        }

        #endregion
    }
}