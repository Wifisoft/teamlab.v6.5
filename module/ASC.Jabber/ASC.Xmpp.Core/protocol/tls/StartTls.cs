// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="StartTls.cs">
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

    // Step 4: Client sends the STARTTLS command to server:
    // <starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>

    /// <summary>
    ///   Summary description for starttls.
    /// </summary>
    public class StartTls : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public StartTls()
        {
            TagName = "starttls";
            Namespace = Uri.TLS;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public bool Required
        {
            get { return HasTag("required"); }

            set
            {
                if (value == false)
                {
                    if (HasTag("required"))
                    {
                        RemoveTag("required");
                    }
                }
                else
                {
                    if (!HasTag("required"))
                    {
                        SetTag("required");
                    }
                }
            }
        }

        #endregion
    }
}