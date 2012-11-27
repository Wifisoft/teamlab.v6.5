using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AppLimit.CloudComputing.SharpBox.StorageProvider.SkyDrive.Authorization
{
    [DataContract]
    internal class OAuth20Token : ICloudStorageAccessToken
    {
        [DataMember(Name = "access_token")]
        public String AccessToken { get; set; }

        [DataMember(Name = "refresh_token")]
        public String RefreshToken { get; set; }

        [DataMember(Name = "token_type")]
        public String TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public double ExpiresIn { get; set; }

        [DataMember(Name = "client_id")]
        public String ClientID { get; set; }

        [DataMember(Name = "client_secret")]
        public String ClientSecret { get; set; }

        [DataMember(Name = "redirect_uri")]
        public String RedirectUri { get; set; }

        [DataMember(Name = "timestamp")]
        public DateTime? Timestamp { get; set; }

        public bool IsExpired
        {
            get
            {
                if (Timestamp.HasValue && !ExpiresIn.Equals(default(double)))
                    return DateTime.UtcNow > Timestamp + TimeSpan.FromSeconds(ExpiresIn);
                return true;
            }
        }

        public static OAuth20Token Deserialize(String json)
        {
            var serializer = new DataContractJsonSerializer(typeof(OAuth20Token));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (OAuth20Token)serializer.ReadObject(ms);
            }
        }

        public String Serialize()
        {
            var serializer = new DataContractJsonSerializer(typeof(OAuth20Token));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, this);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public override string ToString()
        {
            //Other fields can be changed because of refresh token and then cache key will be lost
            return String.Format("{{\"client_id\"={0},\"redirect_uri\"={1}}}", ClientID, RedirectUri); 
        }
    }
}
