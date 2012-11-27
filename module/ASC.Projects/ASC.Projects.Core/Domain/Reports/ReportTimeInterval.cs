
namespace ASC.Projects.Core.Domain.Reports
{
    public enum ReportTimeInterval
    {
        Absolute,
        Relative,

        Today,
        Yesterday,
        Tomorrow,

        CurrWeek,
        PrevWeek,
        NextWeek,

        CurrMonth,
        PrevMonth,
        NextMonth,

        CurrYear,
        PrevYear,
        NextYear,
    }
}
