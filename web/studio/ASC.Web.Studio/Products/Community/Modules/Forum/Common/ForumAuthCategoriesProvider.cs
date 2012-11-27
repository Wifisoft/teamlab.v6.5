using ASC.Common.Module;
using ASC.Common.Security.Authorizing;

namespace ASC.Web.Community.Forum {

	/// <summary>
	/// 
	/// </summary>
	public class ForumAuthCategoriesProvider : IAuthCategoriesProvider {

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual AuthCategory[] GetAuthCategories() {
			return ASC.Forum.Module.Constants.AuthorizingCategories;
		}
	}
}
