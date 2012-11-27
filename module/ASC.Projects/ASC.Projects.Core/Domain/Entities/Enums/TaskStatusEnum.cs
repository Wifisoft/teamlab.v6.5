using System.ComponentModel;

namespace ASC.Projects.Core.Domain
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum TaskStatus
    {
        Open = 1,
        Closed = 2
    }
}