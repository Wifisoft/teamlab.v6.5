using System;
using System.Globalization;
using System.Web;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Community.News.Code
{
	public class RequestInfo
	{
		public Guid UserId = Guid.Empty;

		public string UserIdAttribute = string.Empty;

		public UserInfo User = null;

		public bool HasUser = false;

		public RequestInfo(HttpRequest request)
		{
			try
			{
				if (!string.IsNullOrEmpty(request["uid"]))
				{
					UserId = new Guid(request["uid"]);
					if (UserId != Guid.Empty)
					{
						HasUser = true;
						UserIdAttribute = string.Format(CultureInfo.CurrentCulture, "&uid={0}", UserId);
						User = CoreContext.UserManager.GetUsers(UserId);
					}
				}
			}
			catch { }
		}
	}
}
