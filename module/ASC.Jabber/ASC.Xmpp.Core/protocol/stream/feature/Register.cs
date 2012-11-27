// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Register.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

//<stream:stream xmlns:stream='http://etherx.jabber.org/streams/'
//xmlns='jabber:client'
//from='somedomain'
//version='1.0'>
//<stream:features>
//...
//<register xmlns='http://jabber.org/features/iq-register'/>
//...

namespace ASC.Xmpp.Core.protocol.stream.feature
{
    /// <summary>
    /// </summary>
    public class Register : Element
    {
        public Register()
        {
            TagName = "register";
            Namespace = Uri.FEATURE_IQ_REGISTER;
        }
    }
}