// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="XmlRpcException.cs">
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
    public class XmlRpcException : Exception
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public XmlRpcException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"> </param>
        public XmlRpcException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="code"> </param>
        /// <param name="msg"> </param>
        public XmlRpcException(int code, string msg) : base(msg)
        {
            Code = code;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public int Code { get; set; }

        #endregion
    }
}