#region Import

using System.ComponentModel;
using ASC.Web.CRM.Classes;

#endregion

namespace ASC.CRM.Core
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum EntityType
    {
        Any  = -1,
        Contact = 0,
        Opportunity = 1,
        RelationshipEvent = 2,
        Task = 3,
        Company = 4,
        Person = 5,
        File = 6,
        Case = 7
    }
}