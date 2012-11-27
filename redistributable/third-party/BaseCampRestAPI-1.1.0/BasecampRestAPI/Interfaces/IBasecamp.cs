namespace BasecampRestAPI
{
	public interface IBaseCamp
	{
		IRestWebService Service { get; }
		IProject[] Projects { get; }
		IPerson[] People { get; }

		IProject GetProjectById(int id);
		IPerson GetPersonById(int id);
		IToDoListItem GetToDoListItemById(int id);
		ICategory GetCategoryById(int id);
		IPost GetMessageById(int id);
		IComment[] CommentsForResource(string resource, int id);
	}
}