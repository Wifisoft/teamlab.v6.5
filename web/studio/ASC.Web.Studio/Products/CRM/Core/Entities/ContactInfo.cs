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

namespace ASC.CRM.Core
{


    public class ContactInfo : DomainObject
    {
       

        public int ContactID { get; set; }

        public ContactInfoType InfoType { get; set; }

        public int Category { get; set; }

        public String Data { get; set; }

        public bool IsPrimary { get; set; }

        #region Methods


        public static int GetDefaultCategory(ContactInfoType infoTypeEnum)
        {
            switch (infoTypeEnum)
            {
                case ContactInfoType.Phone:
                    return (int)PhoneCategory.Work;
                case ContactInfoType.Address:
                    return (int)AddressCategory.Work;
                default:
                    return (int)ContactInfoBaseCategory.Work;
            }
        }

        public String CategoryToString()
        {
            switch (InfoType)
            {
                case ContactInfoType.Phone:
                    return ((PhoneCategory)Category).ToLocalizedString();
                case ContactInfoType.Address:
                    return ((AddressCategory)Category).ToLocalizedString();
                default:
                    return ((ContactInfoBaseCategory)Category).ToLocalizedString();
            }
        }

        public static Type GetCategory(ContactInfoType infoType)
        {
            switch (infoType)
            {
                case ContactInfoType.Phone:
                    return typeof(PhoneCategory);
                case ContactInfoType.Address:
                    return typeof(AddressCategory);
                default:
                    return typeof(ContactInfoBaseCategory);
            }
        }

        #endregion
    }
}