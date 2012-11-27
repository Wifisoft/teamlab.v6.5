using System;
using System.Collections.Generic;
using System.Text;
using ASC.Common.Security.Authorizing;
using ASC.Bookmarking.Common;

namespace ASC.Bookmarking.Business.Permissions
{
	public class BookmarkingAuthCategoriesProvider : IAuthCategoriesProvider
	{	
		public AuthCategory[] GetAuthCategories()
		{
			return BookmarkingBusinessConstants.AuthorizingCategories;
		}
	}
}
