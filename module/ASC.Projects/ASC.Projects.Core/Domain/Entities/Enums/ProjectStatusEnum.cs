using System.ComponentModel;

namespace ASC.Projects.Core.Domain
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ProjectStatus
    {
        Open,
        Closed,
        Paused
    }
}