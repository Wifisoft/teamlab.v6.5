// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="DigestMD5Mechanism.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using ASC.Xmpp.Core.protocol.sasl;
using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.authorization.DigestMD5
{

    #region usings

    #endregion

    /// <summary>
    ///   Handels the SASL Digest MD5 authentication
    /// </summary>
    public class DigestMD5Mechanism : Mechanism
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="con"> </param>
        public override void Init()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public override void Parse(Node e)
        {
            if (e.GetType() == typeof (Challenge))
            {
                var c = e as Challenge;

                var step1 = new Step1(c.TextBase64);
                if (step1.Rspauth == null)
                {
                    // response xmlns="urn:ietf:params:xml:ns:xmpp-sasl">dXNlcm5hbWU9ImduYXVjayIscmVhbG09IiIsbm9uY2U9IjM4MDQzMjI1MSIsY25vbmNlPSIxNDE4N2MxMDUyODk3N2RiMjZjOWJhNDE2ZDgwNDI4MSIsbmM9MDAwMDAwMDEscW9wPWF1dGgsZGlnZXN0LXVyaT0ieG1wcC9qYWJiZXIucnUiLGNoYXJzZXQ9dXRmLTgscmVzcG9uc2U9NDcwMTI5NDU4Y2EwOGVjYjhhYTIxY2UzMDhhM2U5Nzc
                    var s2 = new Step2(step1, base.Username, base.Password, base.Server);
                    var r = new Response(s2.ToString());
                    //base.XmppClientConnection.Send(r);
                }
                else
                {
                    // SEND: <response xmlns="urn:ietf:params:xml:ns:xmpp-sasl"/>
                    //base.XmppClientConnection.Send(new Response());
                }
            }
        }

        #endregion
    }
}