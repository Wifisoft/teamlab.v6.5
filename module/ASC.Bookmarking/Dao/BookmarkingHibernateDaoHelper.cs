using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace ASC.Bookmarking.Dao
{
	internal class BookmarkingHibernateDaoHelper
	{
		private const string PERSENT_SYMBOL = "%";

		internal static string ModifySearchParam(string searchParam)
		{
			return PERSENT_SYMBOL + searchParam + PERSENT_SYMBOL;
		}

		internal static string ModifySearchParamWithSpace(string searchParam)
		{
			return PERSENT_SYMBOL + " " + searchParam + PERSENT_SYMBOL;
		}

		internal static string ModifyEndOfSearchParam(string searchParam)
		{
			return searchParam + PERSENT_SYMBOL;
		}
	}
}
