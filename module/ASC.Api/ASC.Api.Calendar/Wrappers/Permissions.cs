using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ASC.Api.Calendar.Wrappers
{
    [DataContract(Name = "permissions", Namespace = "")]
    public class Permissions
    {
        [DataMember(Name = "users")]
        public List<UserParams> UserParams { get; set; }

        public Permissions()
        {
            this.UserParams = new List<UserParams>();
        }

        public static object GetSample()
        {
            return new { users = new List<object>(){ ASC.Api.Calendar.Wrappers.UserParams.GetSample() } };
        }
    }

    [DataContract(Name = "permissions", Namespace = "")]
    public class CalendarPermissions : Permissions
    {
        [DataMember(Name = "data")]
        public PublicItemCollection Data { get; set; }

        public new static object GetSample()
        {
            return new { data = PublicItemCollection.GetSample() };
        }
    }

    [DataContract(Name = "userparams", Namespace = "")]
    public class UserParams
    {
        [DataMember(Name="objectId")]
        public Guid Id{get; set;}

        [DataMember(Name="name")]
        public string Name{get; set;}

        public static object GetSample()
        {
            return new { objectId = "2fdfe577-3c26-4736-9df9-b5a683bb8520", name = "Valery Zykov" };
        }
    }
}
