// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Failure.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

// <failure xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//		<incorrect-encoding/>
// </failure>

namespace ASC.Xmpp.Core.protocol.sasl
{
    /// <summary>
    ///   Summary description for Failure.
    /// </summary>
    public class Failure : Element
    {
        public Failure()
        {
            TagName = "failure";
            Namespace = Uri.SASL;
        }

        public Failure(FailureCondition cond) : this()
        {
            Condition = cond;
        }

        public FailureCondition Condition
        {
            get
            {
                if (HasTag("aborted"))
                    return FailureCondition.aborted;
                else if (HasTag("incorrect-encoding"))
                    return FailureCondition.incorrect_encoding;
                else if (HasTag("invalid-authzid"))
                    return FailureCondition.invalid_authzid;
                else if (HasTag("invalid-mechanism"))
                    return FailureCondition.invalid_mechanism;
                else if (HasTag("mechanism-too-weak"))
                    return FailureCondition.mechanism_too_weak;
                else if (HasTag("not-authorized"))
                    return FailureCondition.not_authorized;
                else if (HasTag("temporary-auth-failure"))
                    return FailureCondition.temporary_auth_failure;
                else
                    return FailureCondition.UnknownCondition;
            }
            set
            {
                if (value == FailureCondition.aborted)
                    SetTag("aborted");
                else if (value == FailureCondition.incorrect_encoding)
                    SetTag("incorrect-encoding");
                else if (value == FailureCondition.invalid_authzid)
                    SetTag("invalid-authzid");
                else if (value == FailureCondition.invalid_mechanism)
                    SetTag("invalid-mechanism");
                else if (value == FailureCondition.mechanism_too_weak)
                    SetTag("mechanism-too-weak");
                else if (value == FailureCondition.not_authorized)
                    SetTag("not-authorized");
                else if (value == FailureCondition.temporary_auth_failure)
                    SetTag("temporary-auth-failure");
            }
        }
    }
}