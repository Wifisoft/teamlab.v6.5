using System;

namespace BasecampRestAPI
{
	public interface IPost
	{
		int ID { get; }
		string Title { get; }
		string Body { get; }
		DateTime PostedOn { get; }
		int ProjectID { get; }
		//int CategoryID { get; }
		int AuthorID { get; }
		//int MilestoneID { get; }
		//int CommentsCount { get; }
		//int AttachmentsCount { get; }
		//bool UseTextile { get; }
		//string ExtendedBody { get; }
		//string DisplayBody { get; }
		//string DisplayExtendedBody { get; }
		//bool Private { get; }
		IComment[] RecentComments { get; }
	}
}
