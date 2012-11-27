// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Error.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.component
{
    /// <summary>
    ///   Summary description for Error.
    /// </summary>
    public class Error : client.Error
    {
        public Error()
        {
            Namespace = Uri.ACCEPT;
        }

        public Error(int code)
            : base(code)
        {
            Namespace = Uri.ACCEPT;
        }

        public Error(ErrorCode code)
            : base(code)
        {
            Namespace = Uri.ACCEPT;
        }

        public Error(ErrorType type)
            : base(type)
        {
            Namespace = Uri.ACCEPT;
        }

        /// <summary>
        ///   Creates an error Element according the the condition The type attrib as added automatically as decribed in the XMPP specs This is the prefered way to create error Elements
        /// </summary>
        /// <param name="condition"> </param>
        public Error(ErrorCondition condition)
            : base(condition)
        {
            Namespace = Uri.ACCEPT;
        }
    }
}