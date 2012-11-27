using System;
using System.Collections.Generic;
using System.Xml;

namespace BasecampRestAPI
{
	public class BaseCamp : IBaseCamp
	{
		public static BaseCamp GetInstance(string url, string userName, string password)
		{
			return GetInstance(url, userName, password, ProductionWebRequestFactory.GetInstance());
		}

		public static BaseCamp GetInstance(string url, string userName, string password, IWebRequestFactory factory)
		{
			return GetInstance(RestWebService.GetInstance(url, userName, password, factory));
		}
		public static BaseCamp GetInstance(IRestWebService service)
		{
			return new BaseCamp(service);
		}

		private BaseCamp(IRestWebService service)
		{
			Service = service;
		}

		public IRestWebService Service { get; private set; }

		public IPerson[] People
		{
			get
			{
				List<IPerson> people = new List<IPerson>();
				foreach (XmlNode node in Service.GetRequestResponseElement("people.xml").ChildNodes)
				{
					people.Add(Person.GetInstance(this, node));
				}
				return people.ToArray();
			}
		}

		public IProject[] Projects
		{
			get
			{
				List<IProject> projects = new List<IProject>();
				foreach (XmlNode node in Service.GetRequestResponseElement("projects.xml").ChildNodes)
				{
					projects.Add(Project.GetInstance(this, node));
				}
				return projects.ToArray();
			}
		}


		public IProject GetProjectById(int id)
		{
			return Project.GetInstance(this, Service.GetRequestResponseElement(Project.PathForId(id)));
		}

		public ICategory GetCategoryById(int id)
		{
			return Category.GetInstance(this, Service.GetRequestResponseElement(Category.PathForId(id)));
		}

		public IPost GetMessageById(int id)
		{
			return Post.GetInstance(this, Service.GetRequestResponseElement(Post.PathForId(id)));
		}

		public IPerson GetPersonById(int id)
		{
			return Person.GetInstance(this, Service.GetRequestResponseElement(Person.PathForId(id)));
		}

		public IToDoListItem GetToDoListItemById(int id)
		{
            return ToDoListItem.GetInstance(this, Service.GetRequestResponseElement(ToDoListItem.PathForId(id)));
		}

		public IComment[] CommentsForResource(string resource, int id)
		{
			List<IComment> comments = new List<IComment>();
			string path = string.Format("{0}/{1}/comments.xml", resource, id);
			foreach (XmlNode node in Service.GetRequestResponseElement(path))
			{
				comments.Add(Comment.GetInstance(this, node));
			}
			return comments.ToArray();
		}
	}
}
