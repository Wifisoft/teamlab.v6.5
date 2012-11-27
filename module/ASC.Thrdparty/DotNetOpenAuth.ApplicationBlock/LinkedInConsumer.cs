using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace DotNetOpenAuth.ApplicationBlock
{
    public class LinkedInConsumer
    {
        /// <summary>
        /// The Consumer to use for accessing Google data APIs.
        /// </summary>
        public static readonly ServiceProviderDescription ServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint("https://api.linkedin.com/uas/oauth/requestToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("https://www.linkedin.com/uas/oauth/authenticate", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("https://api.linkedin.com/uas/oauth/accessToken", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
        };

        private static readonly MessageReceivingEndpoint ProfileEndpoint = new MessageReceivingEndpoint("http://api.linkedin.com/v1/people/~:(id,first-name,last-name,picture-url)", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest);


        //http://api.linkedin.com/v1/people/~
        public static void RequestAuthorization(WebConsumer consumer)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }
            Uri callback = Util.GetCallbackUrlFromContext().StripQueryArgumentsWithPrefix("oauth_");
            var request = consumer.PrepareRequestUserAuthorization(callback, null, null);
            consumer.Channel.Send(request);
        }

        public static string GetProfile(ConsumerBase consumer, string accessToken)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            var request = consumer.PrepareAuthorizedRequest(ProfileEndpoint, accessToken, new Dictionary<string, string>());
            var response = consumer.Channel.WebRequestHandler.GetResponse(request);
            string body = response.GetResponseReader().ReadToEnd();
            return body;
        }
    }
}