//-----------------------------------------------------------------------
// <copyright file="FacebookGraph.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DotNetOpenAuth.ApplicationBlock.Facebook
{
    [DataContract]
    public class FacebookGraph
    {
        private static readonly DataContractJsonSerializer JsonSerializer = new DataContractJsonSerializer(typeof(FacebookGraph));

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "link")]
        public Uri Link { get; set; }

        [DataMember(Name = "birthday")]
        public string Birthday { get; set; }

        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }

        [DataMember(Name = "locale")]
        public string Locale { get; set; }

        public static FacebookGraph Deserialize(string json)
        {
            if (String.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException("json");
            }

            return Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(json)));
        }

        public static FacebookGraph Deserialize(Stream jsonStream)
        {
            if (jsonStream == null)
            {
                throw new ArgumentNullException("jsonStream");
            }
            var reader = new StreamReader(jsonStream);
            var json = reader.ReadToEnd();
            return (FacebookGraph)JsonSerializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(json)));
        }
    }
}
