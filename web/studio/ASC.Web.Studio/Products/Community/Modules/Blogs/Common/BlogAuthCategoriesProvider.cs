using ASC.Common.Security.Authorizing;

namespace ASC.Web.Community.Blogs
{
	public class BlogAuthCategoriesProvider : IAuthCategoriesProvider {

		public virtual AuthCategory[] GetAuthCategories() {
			return ASC.Blogs.Core.Constants.AuthorizingCategories;
		}
	}
}
