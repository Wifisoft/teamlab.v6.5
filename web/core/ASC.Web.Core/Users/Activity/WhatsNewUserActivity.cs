using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASC.Web.Core.Users.Activity
{
	public class WhatsNewUserActivity
	{

		public IList<string> BreadCrumbs { get; set; }

		public string Title { get; set; }

		public string URL { get; set; }

		public string UserName { get; set; }

		public string UserAbsoluteURL { get; set; }

		public DateTime Date { get; set; }

	}
}
