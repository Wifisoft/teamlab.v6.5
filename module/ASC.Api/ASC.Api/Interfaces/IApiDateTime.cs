using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ASC.Api.Interfaces
{
    public interface IApiDateTime : IComparable<DateTime>
    {
        DateTime UtcTime { get;  }
        TimeSpan TimeZoneOffset { get; }
    }
}