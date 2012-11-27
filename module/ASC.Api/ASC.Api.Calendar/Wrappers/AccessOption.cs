using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ASC.Api.Calendar.Wrappers
{
    [DataContract(Name = "sharingOption", Namespace = "")]
    public class AccessOption
    {
        [DataMember(Name = "id", Order = 10)]
        public string Id { get; set; }

        [DataMember(Name = "name", Order = 20)]
        public string Name { get; set; }

        [DataMember(Name = "defaultAction", Order = 30)]
        public bool Default { get; set; }

        public static AccessOption ReadOption
        {
            get { return new AccessOption() { Id = "read", Default = true, Name= Resources.CalendarApiResource.ReadOption }; }
        }

        public static AccessOption FullAccessOption
        {
            get { return new AccessOption() { Id = "full_access", Name = Resources.CalendarApiResource.FullAccessOption }; }
        }

        public static AccessOption OwnerOption
        {
            get { return new AccessOption() { Id = "owner", Name = Resources.CalendarApiResource.OwnerOption }; }
        }


        public static List<AccessOption> CalendarStandartOptions {
            get {
                 return new List<AccessOption>(){ReadOption, FullAccessOption};
            }
        }

        public static object GetSample()
        {
            return new { id = "read", name = "Read only", defaultAction = true };
        }
    }
}
