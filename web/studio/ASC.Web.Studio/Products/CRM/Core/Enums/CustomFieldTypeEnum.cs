using System.ComponentModel;
using ASC.Web.CRM.Classes;

namespace ASC.CRM.Core
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CustomFieldType
    {
        TextField = 0,
        TextArea = 1,
        SelectBox = 2,
        CheckBox = 3,
        Heading = 4,
        Date = 5
    }
}