using System;
using System.Collections.Generic;
using System.Xml;

namespace BasecampRestAPI
{
	/*
	<project>
	  <id type="integer">#{id}</id>
	  <name>#{name}</name>
	  <created-on type="datetime">#{created_on}</created-on>
	  <status>#{status}</status>
	  <last-changed-on type="datetiem">#{last_changed_on}</last-changed-on>
	  <company>
		<id type="integer">#{id}</id>
		<name>#{name}</name>
	  </company>

	  <!-- if user is administrator, or show_announcement is true -->
	  <announcement>#{announcement}</announcement>

	  <!-- if user is administrator -->
	  <start-page>#{start_page}</start-page>
	  <show-writeboards type="boolean">#{show_writeboards}</show-writeboards>
	  <show-announcement type="boolean">#{show_announcement}</show-announcement>
	</project>
	*/
	public class Project : IProject
	{
		public static string PathForId(int id)
		{
			return string.Format("projects/{0}.xml", id);
		}
        private string RequestPathForAction(string action)
		{
			return string.Format("projects/{0}/{1}", ID, action);
		}

		private readonly IRestWebService _service;
        private IBaseCamp Camp { get; set; }

		public static IProject GetInstance(IBaseCamp baseCamp, XmlNode node)
		{
			return new Project(baseCamp, baseCamp.Service, node);
		}
		private Project(IBaseCamp baseCamp, IRestWebService service, XmlNode node)
		{
			Camp = baseCamp;
			_service = service;
			ID = XmlHelpers.ParseInteger(node, "id");
			Name = XmlHelpers.ChildNodeText(node, "name");
            Description = XmlHelpers.ChildNodeText(node, "announcement");
			//CreatedOn = XmlHelpers.ParseDateTime(node, "created-on");
			//LastChangedOn = XmlHelpers.ParseDateTime(node, "last-changed-on", CreatedOn);
			Status = XmlHelpers.ChildNodeText(node, "status");
		}

		#region Implementation of IProject

		public int ID { get; private set; }
		public string Name { get; private set; }
        public string Description { get; private set; }
		//public DateTime CreatedOn { get; private set; }
		//public DateTime LastChangedOn { get; private set; }
		public string Status { get; private set; }
		public IPerson[] People
		{
			get
			{
                string requestPath = RequestPathForAction("people.xml");
                List<IPerson> people = new List<IPerson>();
                try
                {
                    foreach (XmlNode node in _service.GetRequestResponseElement(requestPath).ChildNodes)
                    {
                        people.Add(Person.GetInstance(Camp, node));
                    }
                }
                catch
                {
                    return people.ToArray();
                }
                return people.ToArray();
			}
		}
		public IToDoList[] ToDoLists
		{
			get
			{
                string requestPath = RequestPathForAction("todo_lists.xml?filter=all");
                List<IToDoList> lists = new List<IToDoList>();
                try
                {
                    foreach (XmlNode node in _service.GetRequestResponseElement(requestPath).ChildNodes)
                    {
                        lists.Add(ToDoList.GetInstance(Camp, node));
                    }
                }
                catch
                {
                    return lists.ToArray();
                }
                return lists.ToArray();
			}
		}
		public IMilestone[] Milestones
		{
            get
            {
                string requestPath = RequestPathForAction("milestones/list.xml");
                List<IMilestone> milestones = new List<IMilestone>();
                try
                {
                    foreach (XmlNode node in _service.GetRequestResponseElement(requestPath, "<request/>").ChildNodes)
                    {
                        milestones.Add(Milestone.GetInstance(Camp, node));
                    }
                }
                catch
                {
                    return milestones.ToArray();
                }
                return milestones.ToArray();
            }
		}
		public IPost[] RecentMessages
		{
			get
			{
				return MessagesForAction("posts.xml");
			}
		}
        public IAttachment[] Attachments
        {
            get
            {
                return ProjectAttachments("attachments.xml");
            }
        }
        public ITimeEntry[] TimeEntries
        {
            get
            {
                return ProjectTimeEntries("time_entries.xml");
            }
        }
        public ICategory[] Categories
        {
            get
            {
                return ProjectCategories("categories.xml");
            }
        }

		public IMilestone GetMilestoneById(int id)
		{
			foreach (IMilestone milestone in Milestones)
			{
				if (milestone.ID == id)
				{
					return milestone;
				}
			}
            return null;
		}
		public ICategory[] GetCategoriesForType(CategoryType type)
		{
			string requestPath = RequestPathForAction(string.Format("categories.xml?type={0}", type.ToString().ToLower()));
			List<ICategory> categories = new List<ICategory>();
            try
            {
                foreach (XmlNode node in _service.GetRequestResponseElement(requestPath).ChildNodes)
                {
                    categories.Add(Category.GetInstance(Camp, node));
                }
            }
            catch
            {
                return categories.ToArray();
            }
			return categories.ToArray();
		}
		public IPost[] GetMessagesForCategory(ICategory category)
		{
			return MessagesForAction(string.Format("cat/{0}/posts.xml", category.ID));
		}

		#endregion

		private IPost[] MessagesForAction(string action)
		{
            List<IPost> messages = new List<IPost>();
            try
            {
                foreach (XmlNode node in _service.GetRequestResponseElement(RequestPathForAction(action)).ChildNodes)
                {
                    messages.Add(Post.GetInstance(Camp, node));
                }
            }
            catch
            {
                return messages.ToArray();
            }
            return messages.ToArray();
		}
        private IAttachment[] ProjectAttachments(string action)
        {
            List<IAttachment> attachments = new List<IAttachment>();
            try
            {
                bool isCompleted = false;
                int count = 0;
                string parameter = string.Empty;

                while (!isCompleted)
                {
                    XmlNodeList list = _service.GetRequestResponseElement(RequestPathForAction(action + parameter)).ChildNodes;

                    foreach (XmlNode node in list)
                    {
                        attachments.Add(Attachment.GetInstance(Camp, node));
                    }

                    if (list.Count < 100)
                    {
                        isCompleted = true;
                    }
                    else
                    {
                        count = count + 100;
                        parameter = string.Format("?n={0}", count);
                    }
                }
            }
            catch
            {
                return attachments.ToArray();
            }
            return attachments.ToArray();
        }
        private ITimeEntry[] ProjectTimeEntries(string action)
        {
            List<ITimeEntry> timeEntries = new List<ITimeEntry>();
            try
            {
                XmlElement element = _service.GetRequestResponseElement(RequestPathForAction(action));
                foreach (XmlNode node in element.ChildNodes)
                {
                    timeEntries.Add(TimeEntry.GetInstance(Camp, node));
                }
            }
            catch
            {
                return timeEntries.ToArray();
            }
            return timeEntries.ToArray();
        }
        private ICategory[] ProjectCategories(string action)
        {
            List<ICategory> categories = new List<ICategory>();
            try
            {
                foreach (XmlNode node in _service.GetRequestResponseElement(RequestPathForAction(action)).ChildNodes)
                {
                    categories.Add(Category.GetInstance(Camp, node));
                }
            }
            catch
            {
                return categories.ToArray();
            }
            return categories.ToArray();
        }
		private static string GetNotificationXml(IEnumerable<IPerson> notifications)
		{
			string result = string.Empty;
			foreach (IPerson person in notifications)
			{
				result += string.Format("<notify>{0}</notify>\n", person.ID);
			}
			return result;
		}
	}
}