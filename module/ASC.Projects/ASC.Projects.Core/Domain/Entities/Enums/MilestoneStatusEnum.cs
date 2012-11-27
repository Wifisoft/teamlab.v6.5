using System.ComponentModel;

namespace ASC.Projects.Core.Domain
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum MilestoneStatus
    {
        Open = 0,
        Closed = 1
    }
}
