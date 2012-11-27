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
    [TypeConverter(typeof (LocalizedEnumConverter))]
    public enum ContactInfoBaseCategory
    {
        Home,
        Work,
        Other
    }

    [TypeConverter(typeof (LocalizedEnumConverter))]
    public enum PhoneCategory
    {
        Home,
        Work,
        Mobile,
        Fax,
        Direct,
        Other
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum AddressPart
    {
        Street,
        City,
        State,
        Zip,
        Country
    }

    [TypeConverter(typeof (LocalizedEnumConverter))]
    public enum AddressCategory
    {
        Home,
        Postal,
        Office,
        Billing,
        Other, 
        Work
    }
}