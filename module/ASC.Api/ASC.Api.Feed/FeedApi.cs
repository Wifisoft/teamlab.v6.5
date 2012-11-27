using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Api.Attributes;
using ASC.Api.Collections;

using ASC.Api.Impl;
using ASC.Feed.Activity;
using ASC.Feed.ActivityProvider;
using ASC.Feed.Utils;
using ASC.Specific;
using Microsoft.Practices.Unity;

namespace ASC.Api.Feed
{
    public class FeedApi : Interfaces.IApiEntryPoint
    {
        private readonly ApiContext _context;
        private readonly IUnityContainer _registrationContainer;

        public string Name
        {
            get { return "feed"; }
        }

        public FeedApi(ApiContext context, IUnityContainer registrationContainer)
        {
            _context = context;
            _registrationContainer = registrationContainer;
        }

        [Read("")]
        public object GetFeed(ApiDateTime from, ApiDateTime to, IEnumerable<string> sources, ActivityAction? actions, int? days)
        {
            var activityList = new List<Activity>();
            //resolve all IActivityProvider
            var activityProviders = _registrationContainer.ResolveAll<IActivityProvider>();
            if (activityProviders!=null)
            {
                DateTime? fromDate = null;
                if (from!=null)
                {
                    fromDate = from.UtcTime.Date;
                }
                DateTime? toDate = null;
                if (to != null)
                {
                    toDate = to.UtcTime.Date;
                }

                //Filter providers to allowed sources
                if (sources!=null && sources.Any())
                {
                    activityProviders = activityProviders.Where(x => sources.Contains(x.SourceName));
                }
                var range = ActivityDateUtil.GetRange(fromDate, toDate, days ?? 1);
                foreach (var activityProvider in activityProviders)
                {
                    activityList.AddRange(activityProvider.GetActivities(range, null, actions));
                }
            }

            return activityList
                .Select(x => new ApiActivity(x)).GroupBy(x => x.Source)
                .ToDictionary(x=>x.Key,y=>y.GroupBy(z=>z.ItemType)
                .ToDictionary(x=>x.Key??"",z=>z.OrderBy(x=>x.Priority)));
        }
    }
}
