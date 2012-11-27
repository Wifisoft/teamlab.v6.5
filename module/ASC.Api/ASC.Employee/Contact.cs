using System.Runtime.Serialization;

namespace ASC.Api.Employee
{
    [DataContract(Name = "contact", Namespace = "")]
    public class Contact
    {
        [DataMember(Order = 1)]
        public string Type { get; set; }
        [DataMember(Order = 2)]
        public string Value { get; set; }

        public Contact()
        {
            //For binder
        }

        public Contact(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}