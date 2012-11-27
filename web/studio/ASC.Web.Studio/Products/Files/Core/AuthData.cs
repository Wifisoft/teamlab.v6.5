using System.Diagnostics;
using System.Runtime.Serialization;

namespace ASC.Files.Core
{
    [DataContract(Name = "AuthData", Namespace = "")]
    [DebuggerDisplay("{Login} {Password} {Token}")]
    public class AuthData
    {
        [DataMember(Name = "login")]
        public string Login { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }

        public AuthData(string token)
        {
            Token = token;
        }

        public AuthData(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public AuthData(string login, string password, string token)
        {
            Login = login;
            Password = password;
            Token = token;
        }
    }
}