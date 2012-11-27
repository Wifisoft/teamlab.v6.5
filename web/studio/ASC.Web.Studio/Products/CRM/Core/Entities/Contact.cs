#region Import

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ASC.Common.Security;
using ASC.Core.Users;
using ASC.Web.Core.Helpers;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.SocialMedia.LinkedIn;

#endregion

namespace ASC.CRM.Core.Entities
{

    [Serializable]
    public class Person : Contact
    {
        public Person()
        {
            FirstName = String.Empty;
            LastName = String.Empty;
            CompanyID = 0;
            JobTitle = String.Empty;
        }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public int CompanyID { get; set; }

        public String JobTitle { get; set; }

    }

    [Serializable]
    public class Company : Contact
    {
        public Company()
        {
            CompanyName = String.Empty;
        }

        public String CompanyName { get; set; }
    }

    public static class ContactExtension
    {

        public static String GetTitle(this Contact contact)
        {

            if (contact == null)
                return String.Empty;

            if (contact is Company)
            {
                var company = (Company)contact;

                return Convert.ToString(company.CompanyName);
            }

            var people = (Person)contact;

            return String.Format("{0} {1}", people.FirstName, people.LastName); 
        }

        public static String RenderLinkForCard(this Contact contact)
        {
            var isCompany = contact is Company;
            var popupID = Guid.NewGuid();

            return String.Format(@"
                <a class='linkMedium {0}' id='{5}' data-id='{2}'
                            href='default.aspx?{1}={2}{3}'>
                     {4}
                </a>",
                isCompany ? "crm-companyInfoCardLink" : "crm-peopleInfoCardLink",
                UrlConstant.ID, contact.ID,
                isCompany ? String.Empty : String.Format("&{0}=people", UrlConstant.Type),
                GetTitle(contact).HtmlEncode(), popupID);
        }

        public static String GetEmployeesCountString(this Contact contact)
        {
            if (contact is Person) return String.Empty;
            var count = Global.DaoFactory.GetContactDao().GetMembersCount(contact.ID);
            return count + " " + GrammaticalHelper.ChooseNumeralCase(count,
                                                    CRMContactResource.MembersNominative,
                                                    CRMContactResource.MembersGenitiveSingular,
                                                    CRMContactResource.MembersGenitivePlural);
        }

    }

    [Serializable]
    public abstract class Contact : DomainObject, ISecurityObjectId
    {
        protected Contact()
        {
            About = String.Empty;
            Industry = String.Empty;
            StatusID = 0;

        }

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public String About { get; set; }

        public String Industry { get; set; }

        public int StatusID { get; set; }

        public object SecurityId
        {
            get { return ID; }
        }

        public Type ObjectType
        {
            get { return GetType(); }
        }
    }
}