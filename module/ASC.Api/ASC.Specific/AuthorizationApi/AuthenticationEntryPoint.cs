using System;
using System.Security;
using System.Security.Authentication;
using ASC.Api.Attributes;
using ASC.Api.Utils;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Specific.AuthorizationApi
{
    /// <summary>
    /// Authorization for api
    /// </summary>
    public class AuthenticationEntryPoint : Api.Interfaces.IApiEntryPoint
    {
        /// <summary>
        /// Entry point name
        /// </summary>
        public string Name
        {
            get { return "authentication"; }
        }

        /// <summary>
        /// Gets authentication token for use in api authorization
        /// </summary>
        /// <short>
        /// Get token
        /// </short>
        /// <param name="userName">user name or email</param>
        /// <param name="password">password</param>
        /// <returns>tokent to use in 'Authorization' header when calling API methods</returns>
        /// <exception cref="AuthenticationException">Thrown when not authenticated</exception>
        [Create(@"",false)]//NOTE: this method doesn't requires auth!!!
        public AuthenticationTokenData AuthenticateMe(string userName, string password)
        {
            userName.ThrowIfNull(new ArgumentException("userName empty", "userName"));
            password.ThrowIfNull(new ArgumentException("password empty", "password"));
            var token = SecurityContext.AuthenticateMe(userName, password);
            if (string.IsNullOrEmpty(token))
                throw new AuthenticationException("User authentication failed");
            return new AuthenticationTokenData() {Expires = new ApiDateTime(DateTime.UtcNow.AddYears(1)),Token = token};
        }
    }
}