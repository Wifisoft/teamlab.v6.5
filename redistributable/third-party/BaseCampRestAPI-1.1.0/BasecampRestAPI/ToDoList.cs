using System;
using System.Collections.Generic;
using System.Xml;

namespace BasecampRestAPI
{
	/*
		<todo-list>
		  <id type="integer">#{id}</id>
		  <name>#{name}</name>
		  <description>#{description}</description>
		  <project-id type="integer">#{project_id}</project-id>
		  <milestone-id type="integer">#{milestone_id}</milestone-id>
		  <position type="integer">#{position}</position>

		  <!-- if user can see private lists -->
		  <private type="boolean">#{private}</private>

		  <!-- if the account supports time tracking -->
		  <tracked type="boolean">#{tracked}</tracked>

		  <!-- if todo-items are included in the response -->
		  <todo-items type="array">
			<todo-item>
			  ...
			</todo-item>
			<todo-item>
			  ...
			</todo-item>
			...
		  </todo-items>
		</todo-list>
	 */

	public class ToDoList : IToDoList
	{
		private readonly IBaseCamp _camp;
        private readonly IRestWebService _service;

        public static ToDoList GetInstance(IBaseCamp baseCamp, XmlNode node)
		{
			return new ToDoList(baseCamp, baseCamp.Service, node);
		}
		private ToDoList(IBaseCamp baseCamp, IRestWebService service, XmlNode node)
		{
			_camp = baseCamp;
			_service = service;
			ID = XmlHelpers.ParseInteger(node, "id");
			//Name = XmlHelpers.ChildNodeText(node, "name");
			//Description = XmlHelpers.ChildNodeText(node, "description");
			ProjectID = XmlHelpers.ParseInteger(node, "project-id");
			MilestoneID = XmlHelpers.ParseInteger(node, "milestone-id");
			//Position = XmlHelpers.ParseInteger(node, "position");
			//Private = XmlHelpers.ParseBool(node, "private");
			//Tracked = XmlHelpers.ParseBool(node, "tracked");
		}
		

		#region Implementation of IToDoList

		public int ID { get; private set; }
		//public string Name { get; set; }
		//public string Description { get; set; }
		public int ProjectID { get; private set; }
        public int MilestoneID { get; private set; }
		//public int Position { get; private set; }
		//public bool Private { get; set; }
		//public bool Tracked { get; set; }
		public IToDoListItem[] Items
		{
			get
			{
				List<IToDoListItem> items = new List<IToDoListItem>();
				foreach (XmlNode node in _service.GetRequestResponseElement(string.Format("todo_lists/{0}/todo_items.xml", ID)))
				{
					items.Add(ToDoListItem.GetInstance(_camp, node));
				}
				return items.ToArray();
			}
		}

		#endregion

		public static string PathForId(int id)
		{
			return string.Format("todo_lists/{0}.xml", id);
		}
	}
}
