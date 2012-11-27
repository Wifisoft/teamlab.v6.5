// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Auth.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

// <auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='DIGEST-MD5'/>

namespace ASC.Xmpp.Core.protocol.sasl
{
    /// <summary>
    ///   Summary description for Auth.
    /// </summary>
    public class Auth : Element
    {
        public Auth()
        {
            TagName = "auth";
            Namespace = Uri.SASL;
        }

        public Auth(MechanismType type) : this()
        {
            MechanismType = type;
        }

        public Auth(MechanismType type, string text) : this(type)
        {
            Value = text;
        }


        public MechanismType MechanismType
        {
            get { return Mechanism.GetMechanismType(GetAttribute("mechanism")); }
            set { SetAttribute("mechanism", Mechanism.GetMechanismName(value)); }
        }
    }
}