using System;
using System.Xml;

namespace BasecampRestAPI
{
	/*
<comment>
  <id type="integer">#{id}</id>
  <author-id type="integer">#{author_id}</author-id>
  <commentable-id type="integer">#{commentable_id}</commentable-id>
  <commentable-type>#{commentable_type}</commentable-type>
  <body>#{body}</body>
  <emailed-from nil="true">#{emailed_from}</emailed-from>
  <attachments-count type="integer">#{attachments_count}</attachments-count>
  <created-at type="datetime">#{created_at}</created-at>
</comment>
	 */
	public class Comment : IComment
	{
		private readonly IBaseCamp _camp;

		public static IComment GetInstance(IBaseCamp camp, XmlNode node)
		{
			return new Comment(camp, node);
		}
		private Comment(IBaseCamp camp, XmlNode node)
		{
			_camp = camp;
			ID = XmlHelpers.ParseInteger(node, "id");
			AuthorID = XmlHelpers.ParseInteger(node, "author-id");
			//CommentableID = XmlHelpers.ParseInteger(node, "commentable-id");
			//CommentableType = XmlHelpers.ChildNodeText(node, "commentable-type");
			Body = XmlHelpers.ChildNodeText(node, "body");
			//EmailedFrom = XmlHelpers.ChildNodeText(node, "emailed-from");
			//AttachmentsCount = XmlHelpers.ParseInteger(node, "attachments-count");
			CreatedAt = XmlHelpers.ParseDateTime(node, "created-at");
		}

		#region Implementation of IComment

		public int ID { get; private set; }
		//public int CommentableID { get; private set; }
        public int AuthorID { get; private set; }
		//public string CommentableType { get; private set; }
		public string Body { get; private set; }
		//public string EmailedFrom { get; private set; }
		//public int AttachmentsCount { get; private set; }
		public DateTime CreatedAt { get; private set; }

		#endregion
	}
}
