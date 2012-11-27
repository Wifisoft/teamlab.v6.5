namespace BasecampRestAPI
{
	public interface IToDoList
	{
		int ID { get; }
		//string Name { get; set; }
		//string Description { get; set; }
		int ProjectID { get; }
		int MilestoneID { get; }
		//int Position { get; }
		//bool Private { get; set; }
		//bool Tracked { get; set; }
		IToDoListItem[] Items { get; }
	}
}