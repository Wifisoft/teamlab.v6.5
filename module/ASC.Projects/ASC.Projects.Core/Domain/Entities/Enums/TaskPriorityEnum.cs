using System.ComponentModel;

namespace ASC.Projects.Core.Domain
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum TaskPriority
    {
        High = 1,
        Normal = 0,
        Low = -1
    }
}
