using System;
using System.Collections.Generic;
using ASC.Feed.Activity;
using ASC.Feed.Utils;

namespace ASC.Feed.ActivityProvider
{
    public interface IActivityProvider
    {
        string SourceName { get; }

        IEnumerable<Activity.Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions);
    }
}