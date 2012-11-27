using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Bookmarking.Common
{
	public class BookmarkingSessionObject<T> where T : class, new()
	{
		public static T GetCurrentInstanse()
		{
			return BookmarkingBusinessFactory.GetObjectFromSession<T>();
		}
	}
}
