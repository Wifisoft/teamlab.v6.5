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
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ContactInfoType
    {
        Phone,
        Email,
        Website,
        Skype,
        Twitter,
        LinkedIn,
        Facebook,
        Address,
        LiveJournal,
        MySpace,
        GMail,
        Blogger,
        Yahoo,
        MSN,
        ICQ,
        Jabber,
        AIM
    }
}