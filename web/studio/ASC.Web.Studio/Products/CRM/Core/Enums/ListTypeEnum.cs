#region Import

using System.ComponentModel;
using ASC.Web.CRM.Classes;

#endregion

namespace ASC.CRM.Core
{
    public enum ListType
    {
        ContactStatus = 1,
        TaskCategory = 2,
        HistoryCategory = 3
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum HistoryCategorySystem
    {
        TaskClosed = -1,
        FilesUpload = -2
    }

}
