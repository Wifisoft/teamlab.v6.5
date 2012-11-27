using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Feed.Activity;
using ASC.Feed.Utils;

namespace ASC.Api.Employee.ActivityProviders
{
    public class BirthdayActivityProvider:ASC.Feed.ActivityProvider.IActivityProvider
    {
        public const string Birthday = "birthday";

        public string SourceName
        {
            get { return Birthday; }
        }

        public IEnumerable<Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions)
        {
            //Return birthdays
            return
                CoreContext.UserManager.GetUsers().Where(x => x.BirthDate.HasValue && range.In(x.BirthDate.Value)).
                    Select(x => new Activity(Birthday, null, x.BirthDate)
                                    {
                                        RelativeTo = x.ID, 
                                        Action = ActivityAction.Periodic
                                    }
                );
        }
    }
}