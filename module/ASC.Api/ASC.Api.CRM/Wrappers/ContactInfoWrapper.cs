#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.CRM.Core;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Api.CRM.Wrappers
{

    /// <summary>
    ///   Адресс
    /// </summary>
    [DataContract(Name = "address", Namespace = "")]
    public class Address
    {

        public Address()
        {
            
        }

        public Address(ContactInfo contactInfo)
        {
            if (contactInfo.InfoType != ContactInfoType.Address)
                throw new ArgumentException();

            City = JObject.Parse(contactInfo.Data)["city"].Value<String>();
            Country = JObject.Parse(contactInfo.Data)["country"].Value<String>();
            State = JObject.Parse(contactInfo.Data)["state"].Value<String>();
            Street = JObject.Parse(contactInfo.Data)["street"].Value<String>();
            Zip = JObject.Parse(contactInfo.Data)["zip"].Value<String>();

        }

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public String Street { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public String City { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public String State { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public String Zip { get; set; }

        [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public String Country { get; set; }

        public static Address GetSample()
        {
            return new Address
                       {
                           Country = "Latvia",
                           Zip = "LV-1021",
                           Street = "Lubanas st. 125a-25",
                           State = "",
                           City = "Riga"
                       };
        }
    }

    /// <summary>
    ///   Контактная информация
    /// </summary>
    [DataContract(Name = "commonDataItem", Namespace = "")]
    public class ContactInfoWrapper : ObjectWrapperBase
    {
        public ContactInfoWrapper()
            : base(0)
        {

        }

        public ContactInfoWrapper(int id)
            : base(id)
        {

        }

        public ContactInfoWrapper(ContactInfo contactInfo)
            : base(contactInfo.ID)
        {
            InfoType = contactInfo.InfoType;
            Category = contactInfo.Category;
            Data = contactInfo.Data;
            IsPrimary = contactInfo.IsPrimary;
            ID = contactInfo.ID;
        }


        [DataMember(Order = 1)]
        public ContactInfoType InfoType { get; set; }

        [DataMember(Order = 2)]
        public int Category { get; set; }

        [DataMember(Order = 3)]
        public String Data { get; set; }

        [DataMember(Order = 4)]
        public bool IsPrimary { get; set; }

        public static ContactInfoWrapper GetSample()
        {
            return new ContactInfoWrapper(0)
            {
                IsPrimary = true,
                Category = (int)ContactInfoBaseCategory.Home,
                Data = "support@teamlab.com",
                InfoType = ContactInfoType.Email
              
            };
        }

    }
}
