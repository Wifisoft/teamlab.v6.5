#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Web.CRM.Classes;
using Contact = ASC.CRM.Core.Entities.Contact;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    /// <summary>
    ///   Персона
    /// </summary>
    [DataContract(Name = "person", Namespace = "")]
    public class PersonWrapper : ContactWrapper
    {
        public PersonWrapper(int id) :
            base(id)
        {
        }

        public PersonWrapper(Person person)
            : base(person)
        {
            FirstName = person.FirstName;
            LastName = person.LastName;
            Title = person.JobTitle;
        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String FirstName { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String LastName { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWrapper Company { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        public new static PersonWrapper GetSample()
        {
            return new PersonWrapper(0)
                       {
                           IsPrivate = false,
                           IsCompany = false,
                           FirstName = "Tadjeddine",
                           LastName = "Bachir",
                           Company = CompanyWrapper.GetSample(),
                           Title = "Programmer",
                           About = "",
                           Created = (ApiDateTime) DateTime.UtcNow,
                           CreateBy = EmployeeWraper.GetSample()
                       };
        }
    }

    /// <summary>
    ///  Компания
    /// </summary>
    [DataContract(Name = "company", Namespace = "")]
    public class CompanyWrapper : ContactWrapper
    {
        public CompanyWrapper(int id) :
            base(id)
        {
        }

        public CompanyWrapper(Company company)
            : base(company)
        {
            CompanyName = company.CompanyName;
        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String CompanyName { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public IEnumerable<ContactBaseWrapper> Persons { get; set; }

        public new static CompanyWrapper GetSample()
        {
            return new CompanyWrapper(0)
                       {
                           IsPrivate = false,
                           IsCompany = true,
                           About = "",
                           CompanyName = "Food and Culture Project"
                       };
        }
    }

    [DataContract(Name = "contact", Namespace = "")]
    [KnownType(typeof (PersonWrapper))]
    [KnownType(typeof (CompanyWrapper))]
    public abstract class ContactWrapper : ContactBaseWrapper
    {
        protected ContactWrapper(int id)
            : base(id)
        {
        }

        protected ContactWrapper(Contact contact)
            : base(contact)
        {
            CreateBy = EmployeeWraper.Get(contact.CreateBy);
            Created = (ApiDateTime) contact.CreateOn;
            About = contact.About;
            Industry = contact.Industry;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<Address> Addresses { get; set; }

    

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String About { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Industry { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactType ContactType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<ContactInfoWrapper> CommonData { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<CustomFieldWrapper> CustomFields { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<String> Tags { get; set; }
            
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int TaskCount { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool HaveLateTasks { get; set; }
    }

    /// <summary>
    ///  Базовая информация о контакте
    /// </summary>
    [DataContract(Name = "contactBase", Namespace = "")]
    public class ContactBaseWrapper : ObjectWrapperBase
    {
        public ContactBaseWrapper(Contact contact)
            : base(contact.ID)
        {
            DisplayName = contact.GetTitle();
            IsPrivate = CRMSecurity.IsPrivate(contact);

            if (IsPrivate)
                AccessList = CRMSecurity.GetAccessSubjectTo(contact)
                             .Select(item => EmployeeWraper.Get(item.Key));

            SmallFotoUrl = ContactPhotoManager.GetSmallSizePhoto(contact.ID, contact is Company);
            IsCompany = contact is Company;
            CanEdit = CRMSecurity.CanEdit(contact);
        }


        protected ContactBaseWrapper(int contactID)
            : base(contactID)
        {
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String SmallFotoUrl { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public String DisplayName { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsCompany { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<EmployeeWraper> AccessList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsPrivate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }

        public static ContactBaseWrapper GetSample()
        {
            return new ContactBaseWrapper(0)
                       {
                           IsPrivate = false,
                           IsCompany = false,
                           DisplayName = "Tadjeddine Bachir",
                           SmallFotoUrl = "url to foto"
                       };
        }
    }
}