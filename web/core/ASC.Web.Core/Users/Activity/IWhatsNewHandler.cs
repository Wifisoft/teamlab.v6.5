using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASC.Web.Core.Users.Activity
{
	public interface IWhatsNewHandler
	{
		IList<WhatsNewUserActivity> GetUserActivities(Guid? userID, DateTime startDate, DateTime endDate);
	}
}
