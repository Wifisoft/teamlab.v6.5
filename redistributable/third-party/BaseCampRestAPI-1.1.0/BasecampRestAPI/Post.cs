using System;
using System.Collections.Generic;
using System.Xml;

namespace BasecampRestAPI
{
	/*
	<post>
		<id type="integer">#{id}</id>
		<title>#{title}</title>
		<body>#{body}</body>
		<posted-on type="datetime">#{posted_on}</posted-on>
		<project-id type="integer">#{project_id}</project-id>
		<category-id type="integer">#{category_id}</category-id>
		<author-id type="integer">#{author_id}</author-id>
		<milestone-id type="integer">#{milestone_id}</milestone-id>
		<comments-count type="integer">#{comments_count}</comments-count>
		<attachments-count type="integer">#{attachments_count}</attachments-count>
		<use-textile type="boolean">#{use_textile}</use-textile>
		<extended-body>#{extended_body}</extended-body>
		<display-body>#{display_body}</display-body>
		<display-extended-body>#{display_extended_body}</display-extended-body>

		<!-- if user can see private posts -->
		<private type="boolean">#{private}</private>
	</post>
	 * */
	public class Post : IPost
	{
		private readonly IBaseCamp _camp;

		public static IPost GetInstance(IBaseCamp camp, XmlNode node)
		{
			return new Post(camp, node);
		}
		private Post(IBaseCamp camp, XmlNode node)
		{
			_camp = camp;
			ID = XmlHelpers.ParseInteger(node, "id");
			Title = XmlHelpers.ChildNodeText(node, "title");
			Body = XmlHelpers.ChildNodeText(node, "body");
			PostedOn = XmlHelpers.ParseDateTime(node, "posted-on");
			ProjectID = XmlHelpers.ParseInteger(node, "project-id");
			//CategoryID = XmlHelpers.ParseInteger(node, "category-id");
			AuthorID = XmlHelpers.ParseInteger(node, "author-id");
			//MilestoneID = XmlHelpers.ParseInteger(node, "milestone-id");
			//CommentsCount = XmlHelpers.ParseInteger(node, "comments-count");
			//AttachmentsCount = XmlHelpers.ParseInteger(node, "attachments-count");
			//UseTextile = XmlHelpers.ParseBool(node, "use-textile");
			//ExtendedBody = XmlHelpers.ChildNodeText(node, "extended-body");
			//DisplayBody = XmlHelpers.ChildNodeText(node, "display-body");
			//DisplayExtendedBody = XmlHelpers.ChildNodeText(node, "display-extended-body");
			//Private = XmlHelpers.ParseBool(node, "private");
		}

		#region Implementation of IPost

		public int ID { get; private set; }
		public string Title { get; private set; }
		public string Body { get; private set; }
		public DateTime PostedOn { get; private set; }
        public int ProjectID { get; private set; }
        //public int CategoryID { get; private set; }
        public int AuthorID { get; private set; }
        //public int MilestoneID { get; private set; }
		//public int CommentsCount { get; private set; }
		//public int AttachmentsCount { get; private set; }
		//public bool UseTextile { get; private set; }
		//public string ExtendedBody { get; private set; }
		//public string DisplayBody { get; private set; }
		//public string DisplayExtendedBody { get; private set; }
		//public bool Private { get; private set; }
		public IComment[] RecentComments
		{
			get { return _camp.CommentsForResource("posts", ID); }
		}

		#endregion

		public static string PathForId(int id)
		{
			return string.Format("posts/{0}.xml", id);
		}
	}
}
