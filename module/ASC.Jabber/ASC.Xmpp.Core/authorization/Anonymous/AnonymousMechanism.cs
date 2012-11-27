// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="AnonymousMechanism.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.authorization.Anonymous
{

    #region usings

    #endregion

    /// <summary>
    ///   SALS ANONYMOUS Mechanism, this allows anonymous logins to a xmpp server.
    /// </summary>
    public class AnonymousMechanism : Mechanism
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="con"> </param>
        public override void Init()
        {
            //con.Send(new Auth(MechanismType.ANONYMOUS));
        }

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public override void Parse(Node e)
        {
            // *No Challenges* in SASL ANONYMOUS
        }

        #endregion

        /*
            S: <stream:features>
                    <mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
                        <mechanism>DIGEST-MD5<mechanism>
                        <mechanism>ANONYMOUS<mechanism>
                    </mechanisms>
               </stream:features>
            
            * So, the proper exchange for this ANONYMOUS mechanism would be:

            C: <auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='ANONYMOUS'/>
            S: <success xmlns='urn:ietf:params:xml:ns:xmpp-sasl'/>

            or, in case of the optional trace information:

            C: <auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='ANONYMOUS'>
                    c2lyaGM=
               </auth>
            S: <success xmlns='urn:ietf:params:xml:ns:xmpp-sasl'/>

        */
    }
}