using System;
using System.Collections.Generic;
using System.Web;
using ASC.Web.Core.Users.Activity;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.UserControls.Users.Activity;
using System.Linq;


namespace ASC.Web.Community.Product
{
	public class CommunityWhatsNewHandler : IWhatsNewHandler
	{
		#region IWhatsNewManager Members

		/// <summary>
		/// Gets specified user activities for the Community product.
		/// </summary>
		/// <param name="userID">UserID. If the UserID is null returns activities for all of the users.</param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public IList<WhatsNewUserActivity> GetUserActivities(Guid? userID, DateTime startDate, DateTime endDate)
		{
			var activities = UserActivityManager.GetUserActivities(TenantProvider.CurrentTenantID, null, CommunityProduct.ID, null, UserActivityConstants.ContentActionType, null, startDate, endDate);

			var userContentActivities = activities.ConvertAll(a => new UserContentActivity(a));

			userContentActivities = (from u in userContentActivities
									orderby u.ModuleName ascending, u.Date descending
									select u).ToList<UserContentActivity>();

			var whatsNewUserActivities = Convert(userContentActivities);			

			return whatsNewUserActivities;
		}

		private static IList<WhatsNewUserActivity> Convert(IList<UserContentActivity> userContentActivities)
		{
			var activities = new List<WhatsNewUserActivity>();			

			foreach (var userContentActivity in userContentActivities)
			{
				var breadcrumbs = new List<string>();
				breadcrumbs.Add(userContentActivity.ModuleName);

				var activity = new WhatsNewUserActivity()
				{
					UserName = userContentActivity.UserName,
					UserAbsoluteURL = userContentActivity.UserAbsoluteURL,
					Date = userContentActivity.Date,
					BreadCrumbs = breadcrumbs,
					Title = userContentActivity.Title,
					URL = userContentActivity.ActivityAbsoluteURL
				};

				activities.Add(activity);
			}

			return activities;
		}

		#endregion
	}
}
