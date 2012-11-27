using System;
using System.Collections.Generic;

namespace ASC.Bookmarking.Pojo
{
	public class Bookmark
	{
		public virtual long ID { get; set; }

		public virtual string URL { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }

		public virtual Guid UserCreatorID { get; set; }

		public IList<UserBookmark> UserBookmarks { get; set; }

		public IList<Comment> Comments { get; set; }

		public IList<Tag> Tags { get; set; }

		public Bookmark(string url, DateTime date, string name, string description)
		{
			this.URL = url;
			this.Date = date;
			this.Name = name;
			this.Description = description;

			InitCollections();
		}

		public Bookmark()
		{
			InitCollections();
		}

		private void InitCollections()
		{
			UserBookmarks = new List<UserBookmark>();
			Comments = new List<Comment>();
			Tags = new List<Tag>();
		}
	}
}
