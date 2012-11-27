using System;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Core;

namespace ASC.Web.Core.Helpers
{
    public class AuthorizationHelper
    {
        public static bool ProcessBasicAuthorization(HttpContext context)
        {
            string authCookie;
            return ProcessBasicAuthorization(context, out authCookie);
        }

        public static bool ProcessBasicAuthorization(HttpContext context, out string authCookie)
        {
            authCookie = null;
            try
            {
                //Try basic
                var authorization = context.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authorization))
                {
                    return false;
                }
                authorization = authorization.Trim();
                if (authorization.IndexOf("Basic", 0) != 0)
                {
                    return false;
                }

                // cut the word "basic" and decode from base64
                // get "username:password"
                var tempConverted = Convert.FromBase64String(authorization.Substring(6));
                var user = new ASCIIEncoding().GetString(tempConverted);

                // get "username"
                // get "password"
                var usernamePassword = user.Split(new[] { ':' });
                var username = usernamePassword[0];
                var password = usernamePassword[1];

                var userInfo = CoreContext.UserManager.GetUserByEmail(username);
                if (userInfo != null)
                {
                    authCookie = SecurityContext.AuthenticateMe(userInfo.ID.ToString(), password);
                }

            }
            catch (Exception) { }

            return SecurityContext.IsAuthenticated;
        }
    }
}