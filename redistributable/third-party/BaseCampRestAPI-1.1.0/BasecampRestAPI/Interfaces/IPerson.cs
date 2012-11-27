using System;

namespace BasecampRestAPI
{
    public interface IPerson
    {
        int ID { get; }
        bool Administrator { get; }
        bool Deleted { get; }
        string EmailAddress { get; }
        string FirstName { get; }
        string LastName { get; }
        string PhoneNumberFax { get; }
        string PhoneNumberHome { get; }
        string PhoneNumberMobile { get; }
        string PhoneNumberOffice { get; }
        string PhoneNumberOfficeExt { get; }
        string UserName { get; }
        string Title { get; }
        string ImService { get; }
        string ImHandle { get; }

        int CanPost { get; }
        bool HasAccessToNewProjects { get; }
        string AvatarUrl { get; }
    }
}
