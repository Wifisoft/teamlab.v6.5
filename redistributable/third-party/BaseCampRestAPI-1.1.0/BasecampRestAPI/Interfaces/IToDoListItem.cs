using System;

namespace BasecampRestAPI
{
	public interface IToDoListItem
	{
		int ID { get; }
		string Content { get; set; }
		//int Position { get; }
		DateTime CreatedOn { get; }
		int CreatorID { get; }
		bool Completed { get; set; }
		//int CommentsCount { get; }
		string ResponsiblePartyType { get; set; }
		int ResponsiblePartyID { get; }
		//DateTime CompletedOn { get; set; }
		//int CompleterID { get; set; }
		//ITimeEntry[] TimeEntries { get; }
		IComment[] RecentComments { get; }

	}
}