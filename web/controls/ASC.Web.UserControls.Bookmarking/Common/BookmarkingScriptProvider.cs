using System;
using System.Web.UI;
using ASC.Data.Storage;

namespace ASC.Web.UserControls.Bookmarking.Common
{
	public class BookmarkingScriptProvider : Control
	{

		protected override void OnLoad(EventArgs e)
		{
			//javascripts registration
			Page.ClientScript.RegisterClientScriptInclude(BookmarkingSettings.BookmarkingSctiptKey, Page.ClientScript.GetWebResourceUrl(this.GetType(), "ASC.Web.UserControls.Bookmarking.js.bookmarking.js"));

			Page.ClientScript.RegisterClientScriptInclude(BookmarkingSettings.BookmarkingTagsAutocompleteSctiptKey, WebPath.GetPath("js/tagsautocompletebox.js"));

			string script = @"
function createSearchHelper() {

	var ForumTagSearchHelper = new SearchHelper(
		'BookmarkTagsInput', 
		'tagAutocompleteItem',
		'tagAutocompleteSelectedItem', 
		'', 
		'', 
		'BookmarkPage', 
		'GetSuggest', 
		'', 
		true,
		false
	);
}
";
			Page.ClientScript.RegisterClientScriptBlock(typeof(string), "bookmarkingTagsAutocompleteInitScript", script, true);
		}
	}
}
