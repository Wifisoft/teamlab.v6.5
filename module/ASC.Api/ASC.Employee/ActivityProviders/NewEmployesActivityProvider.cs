using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Feed.Activity;
using ASC.Feed.Utils;

namespace ASC.Api.Employee.ActivityProviders
{
    public class NewEmployesActivityProvider:ASC.Feed.ActivityProvider.IActivityProvider
    {
        public const string NewEmployes = "newemployee";

        public string SourceName
        {
            get { return NewEmployes; }
        }

        public IEnumerable<Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions)
        {
            return
                CoreContext.UserManager.GetUsers().Where(x => x.WorkFromDate.HasValue && range.In(x.WorkFromDate.Value)).
                    Select(x => new Activity(NewEmployes, null, x.WorkFromDate)
                    {
                        RelativeTo = x.ID,
                        Action = ActivityAction.Created
                    }
                );
        }
    }
}