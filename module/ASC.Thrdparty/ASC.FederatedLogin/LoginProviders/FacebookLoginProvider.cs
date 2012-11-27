using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ASC.FederatedLogin.Profile;
using ASC.Thrdparty.Configuration;
using DotNetOpenAuth.ApplicationBlock.Facebook;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Facebook;
using FacebookClient = DotNetOpenAuth.ApplicationBlock.FacebookClient;

namespace ASC.FederatedLogin.LoginProviders
{
    class FacebookLoginProvider : ILoginProvider
    {
        public LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params)
        {
            var builder = new UriBuilder(context.Request.GetUrlRewriter()) {Query = "p=" + context.Request["p"]};
            var oauth = new FacebookOAuthClient
            {
                AppId = KeyStorage.Get("facebookAppID"),
                AppSecret = KeyStorage.Get("facebookAppSecret"),
                RedirectUri = builder.Uri
            };
            FacebookOAuthResult result;
            if (FacebookOAuthResult.TryParse(context.Request.GetUrlRewriter(), out result))
            {
                if (result.IsSuccess)
                {
                    var accessToken = (Facebook.JsonObject)oauth.ExchangeCodeForAccessToken(result.Code);
                    var request = WebRequest.Create("https://graph.facebook.com/me?access_token=" + Uri.EscapeDataString((string)accessToken["access_token"]));
                    using (var response = request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            var graph = FacebookGraph.Deserialize(responseStream);
                            var profile = ProfileFromFacebook(graph);
                            return profile;
                        }
                    }
                }
                return LoginProfile.FromError(new Exception(result.ErrorReason));
            }
            //Maybe we didn't query
            var extendedPermissions = new[] { "email", "user_about_me" };
            var parameters = new Dictionary<string, object>
                                 {
                                     { "display", "popup" }
                                 };

            if (extendedPermissions.Length > 0)
            {
                var scope = new StringBuilder();
                scope.Append(string.Join(",", extendedPermissions));
                parameters["scope"] = scope.ToString();
            }
            var loginUrl = oauth.GetLoginUrl(parameters);
            context.Response.Redirect(loginUrl.ToString());
            return LoginProfile.FromError(new Exception("Failed to login with facebook"));




            //var client = new FacebookClient
            //                 {
            //                     ClientIdentifier = KeyStorage.Get("facebookAppID"),
            //                     ClientSecret = KeyStorage.Get("facebookAppSecret"),
            //                 };
            //try
            //{
            //    IAuthorizationState authorization = client.ProcessUserAuthorization(new HttpRequestInfo(context.Request));
            //    if (authorization == null)
            //    {
            //        // Kick off authorization request
            //        var scope = new List<string>()
            //                    {
            //                        "email,user_about_me",
            //                    };
            //        client.RequestUserAuthorization(scope, null, null);
            //    }
            //    else
            //    {
            //        var request = WebRequest.Create("https://graph.facebook.com/me?access_token=" + Uri.EscapeDataString(authorization.AccessToken));
            //        using (var response = request.GetResponse())
            //        {
            //            if (response != null)
            //                using (var responseStream = response.GetResponseStream())
            //                {
            //                    var graph = FacebookGraph.Deserialize(responseStream);
            //                    var profile = ProfileFromFacebook(graph);
            //                    return profile;
            //                }
            //        }
            //    }
            //}
            //catch (ProtocolException e)
            //{
            //    if (e.InnerException is WebException)
            //    {
            //        //Read stream
            //        var responce =
            //            new StreamReader((e.InnerException as WebException).Response.GetResponseStream()).ReadToEnd();
            //    }
            //    throw;
            //}

        }

        internal static LoginProfile ProfileFromFacebook(FacebookGraph graph)
        {
            var profile = new LoginProfile
                              {
                                  BirthDay = graph.Birthday,
                                  Link = graph.Link.ToString(),
                                  FirstName = graph.FirstName,
                                  LastName = graph.LastName,
                                  Gender = graph.Gender,
                                  DisplayName = graph.FirstName + graph.LastName,
                                  EMail = graph.Email,
                                  Id = graph.Id.ToString(),
                                  TimeZone = graph.Timezone,
                                  Locale = graph.Locale,
                                  Provider = ProviderConstants.Facebook,
                                  Avatar = string.Format("http://graph.facebook.com/{0}/picture", graph.Id)
                              };

            return profile;
        }
    }
}