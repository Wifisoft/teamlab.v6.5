using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ASC.Notify.Recipients;

namespace ASC.Core.Users
{
    [Serializable]
    public class UserInfo : IDirectRecipient, ICloneable
    {
        public UserInfo()
        {
            Status = EmployeeStatus.Active;
            ActivationStatus = EmployeeActivationStatus.NotActivated;
            Contacts = new List<string>();
            LastModified = DateTime.UtcNow;
        }


        public Guid ID
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public DateTime? BirthDate
        {
            get;
            set;
        }

        public bool? Sex
        {
            get;
            set;
        }

        public EmployeeStatus Status
        {
            get;
            set;
        }

        public EmployeeActivationStatus ActivationStatus
        {
            get;
            set;
        }

        public DateTime? TerminatedDate
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Department
        {
            get;
            set;
        }

        public DateTime? WorkFromDate
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public List<string> Contacts
        {
            get;
            private set;
        }

        public string Location
        {
            get;
            set;
        }

        public string Notes
        {
            get;
            set;
        }

        public bool Removed
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }

        public int Tenant
        {
            get;
            set;
        }

        public bool IsActive
        {
            get { return ActivationStatus == EmployeeActivationStatus.Activated; }
        }

        public string CultureName
        {
            get;
            set;
        }

        public string MobilePhone { get; set; }

        public MobilePhoneActivationStatus MobilePhoneActivationStatus
        {
            get;
            set;
        }
        

        public override string ToString()
        {
            return String.Format("{0} {1}", FirstName, LastName).Trim();
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var ui = obj as UserInfo;
            return ui != null && ID.Equals(ui.ID);
        }

        public CultureInfo GetCulture()
        {
            return string.IsNullOrEmpty(CultureName) ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(CultureName);
        }


        string[] IDirectRecipient.Addresses
        {
            get { return !string.IsNullOrEmpty(Email) ? new[] { Email } : new string[0]; }
        }

        public bool CheckActivation
        {
            get { return !IsActive;/*if user already active we don't need activation*/ }
        }

        string IRecipient.ID
        {
            get { return ID.ToString(); }
        }

        string IRecipient.Name
        {
            get { return ToString(); }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }


        internal string ContactsToString()
        {
            if (Contacts.Count == 0) return null;
            var sBuilder = new StringBuilder();
            foreach (string contact in Contacts)
            {
                sBuilder.AppendFormat("{0}|", contact);
            }
            return sBuilder.ToString();
        }

        internal UserInfo ContactsFromString(string contacts)
        {
            if (string.IsNullOrEmpty(contacts)) return this;
            Contacts.Clear();
            foreach (string contact in contacts.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Contacts.Add(contact);
            }
            return this;
        } 
    }
}