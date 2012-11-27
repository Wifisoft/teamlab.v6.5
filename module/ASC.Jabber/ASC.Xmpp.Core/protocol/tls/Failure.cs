// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Failure.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.tls
{

    #region usings

    #endregion

    // Step 5 (alt): Server informs client that TLS negotiation has failed and closes both stream and TCP connection:

    // <failure xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>
    // </stream:stream>

    /// <summary>
    ///   Summary description for Failure.
    /// </summary>
    public class Failure : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Failure()
        {
            TagName = "failure";
            Namespace = Uri.TLS;
        }

        #endregion
    }
}