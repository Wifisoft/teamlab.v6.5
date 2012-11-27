using System.ComponentModel;
using ASC.Web.CRM.Classes;

namespace ASC.CRM.Core
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ContactListViewType
    {
        All,
        Company,
        Person,
        WithOpportunity
    }

}







