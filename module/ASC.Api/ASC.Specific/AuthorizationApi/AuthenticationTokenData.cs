using System;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.Specific.AuthorizationApi
{
    [DataContract(Name = "token", Namespace = "")]
    public class AuthenticationTokenData
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }
        
        [DataMember(Order = 2)]
        public ApiDateTime Expires { get; set; }

        public static AuthenticationTokenData GetSample()
        {
            return new AuthenticationTokenData()
                       {Expires = new ApiDateTime(DateTime.UtcNow.AddYears(1)), Token = "sdjhfskjdhkqy739459234"};
        }
    }
}