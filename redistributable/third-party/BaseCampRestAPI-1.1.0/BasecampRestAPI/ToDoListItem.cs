using System;
using System.Collections.Generic;
using System.Xml;

namespace BasecampRestAPI
{
	/*
	<todo-item>
	  <id type="integer">#{id}</id>
	  <content>#{content}</content>
	  <position type="integer">#{position}</position>
	  <created-on type="datetime">#{created_on}</created-on>
	  <creator-id type="integer">#{creator_id}</creator-id>
	  <completed type="boolean">#{completed}</completed>
	  <comments-count type="integer">#{comments_count}</comments-count>

	  <!-- if the item has a responsible party -->
	  <responsible-party-type>#{responsible_party_type}</responsible-party-type>
	  <responsible-party-id type="integer">#{responsible_party_id}</responsible-party-id>

	  <!-- if the item has been completed -->
	  <completed-on type="datetime">#{completed_on}</completed-on>
	  <completer-id type="integer">#{completer_id}</completer-id>
	</todo-item>
	 */
	public class ToDoListItem : IToDoListItem
	{
		private readonly IBaseCamp _camp;
		private readonly IRestWebService _service;

		public static IToDoListItem GetInstance(IBaseCamp camp, XmlNode node)
		{
			return new ToDoListItem(camp, camp.Service, node);
		}
		private ToDoListItem(IBaseCamp camp, IRestWebService service, XmlNode node)
		{
			_camp = camp;
			_service = service;
			ID = XmlHelpers.ParseInteger(node, "id");
			Content = XmlHelpers.ChildNodeText(node, "content");
			//Position = XmlHelpers.ParseInteger(node, "position");
			CreatedOn = XmlHelpers.ParseDateTime(node, "created-on");
			CreatorID = XmlHelpers.ParseInteger(node, "creator-id");
			Completed = XmlHelpers.ParseBool(node, "completed");
			//CommentsCount = XmlHelpers.ParseInteger(node, "comments-count");
			if (node.SelectSingleNode("responsible-party-type") != null)
			{
				ResponsiblePartyType = XmlHelpers.ChildNodeText(node, "responsible-party-type");
				ResponsiblePartyID = XmlHelpers.ParseInteger(node, "responsible-party-id");
			}
			else
			{
				ResponsiblePartyType = string.Empty;
				ResponsiblePartyID = -1;
			}
			/*if (Completed)
			{
				CompletedOn = XmlHelpers.ParseDateTime(node, "completed-on", DateTime.MinValue);
				CompleterID = XmlHelpers.ParseInteger(node, "completer-id");
			}
			else
			{
				CompletedOn = DateTime.MinValue;
				CompleterID = -1;
			}*/
		}

		#region Implementation of IToDoListItem

		public int ID { get; private set; }
		public string Content { get; set; }
		//public int Position { get; private set; }
		public DateTime CreatedOn { get; private set; }
		public int CreatorID { get; private set; }
		public bool Completed { get; set; }
		//public int CommentsCount { get; private set; }
		public string ResponsiblePartyType { get; set; }
        public int ResponsiblePartyID { get; private set; }
		//public DateTime CompletedOn { get; set; }
        //public int CompleterID { get; private set; }
        public IComment[] RecentComments
		{
			get { return _camp.CommentsForResource("todo_items", ID); }
		}

		#endregion

        private string PathForAction(string actionPath)
        {
            return string.Format("todo_items/{0}/{1}", ID, actionPath);
        }

        public static string PathForId(int id)
        {
            return string.Format("todo_items/{0}.xml", id);
        }
	}
}