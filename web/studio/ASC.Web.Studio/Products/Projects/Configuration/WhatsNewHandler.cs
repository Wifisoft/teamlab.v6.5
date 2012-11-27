using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Users;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Core.Utility;
using ASC.Web.Projects.Classes;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects.Configuration
{
    public class WhatsNewHandler : IWhatsNewHandler, IUserActivityFilter
    {
        public IList<WhatsNewUserActivity> GetUserActivities(Guid? userID, DateTime startDate, DateTime endDate)
        {
            var result = new List<WhatsNewUserActivity>();

            var activities = UserActivityManager.GetUserActivities(TenantProvider.CurrentTenantID, null, ProductEntryPoint.ID, new[] { ProductEntryPoint.ID }, UserActivityConstants.ContentActionType, null, startDate, endDate).ToList();

            if (userID.HasValue)
            {
                var ids = Global.EngineFactory.GetProjectEngine()
                    .GetByParticipant(userID.Value)
                    .Select(p => p.ID)
                    .Union(Global.EngineFactory.GetParticipantEngine().GetFollowingProjects(userID.Value));
                activities = activities.FindAll(a => ids.Contains(Convert.ToInt32(a.ContainerID)));
            }

            if (activities.Count != 0)
            {
                activities.Sort((x, y) =>
                {
                    var proj1 = int.Parse(x.ContainerID);
                    var proj2 = int.Parse(y.ContainerID);
                    var compare = proj1.CompareTo(proj2);
                    return compare != 0 ? compare : DateTime.Compare(x.Date, y.Date);
                });

                activities.ForEach(a =>
                {
                    var parts = a.AdditionalData.Split(new[] { '|' });
                    if (3 <= parts.Length)
                    {
                        var projectTitle = parts[2];
                        var timeLineType = (EntityType)Enum.Parse(typeof(EntityType), parts[0]);

                        result.Add(new WhatsNewUserActivity
                        {
                            Title = a.Title,
                            Date = a.Date,
                            URL = CommonLinkUtility.GetFullAbsolutePath(a.URL),
                            UserAbsoluteURL = CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(a.UserID, CommonLinkUtility.GetProductID())),
                            UserName = DisplayUserSettings.GetFullUserName(a.UserID),
                            BreadCrumbs = new List<String> { string.Format("[{0}]", projectTitle), ResourceEnumConverter.ConvertToString(timeLineType) }
                        });
                    }
                });
            }
            return result;
        }

        public bool FilterActivity(UserActivity activity)
        {
            if (activity.ProductID == ProductEntryPoint.ID)
            {
                return TimeLinePublisher.IsAllowedToView(activity, Global.EngineFactory);
            }
            return true;
        }
    }
}