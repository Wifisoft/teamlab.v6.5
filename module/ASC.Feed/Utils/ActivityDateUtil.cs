using System;
using System.Linq;

namespace ASC.Feed.Utils
{
    public static class ActivityDateUtil
    {
        public static DateTimeRange GetRange(DateTime? from, DateTime? to)
        {
            return GetRange(@from, to, 1);
        }

        public static DateTimeRange GetRange(DateTime? from, DateTime? to, int dayRange)
        {
            DateTime fromDate = DateTime.UtcNow.Date.AddDays(-dayRange);
            DateTime toDate = DateTime.UtcNow.Date.AddDays(+dayRange);
            if (from.HasValue)
            {
                fromDate = from.Value.Date;
            }
            if (to.HasValue)
            {
                toDate = to.Value.Date;
            }
            return new DateTimeRange(fromDate, toDate);
        }
    }
}