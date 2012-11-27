using System;
using System.Xml;

namespace BasecampRestAPI
{
	public class Milestone : IMilestone
	{
		private readonly IBaseCamp _camp;

		public static IMilestone GetInstance(IBaseCamp camp, XmlNode node)
		{
			return new Milestone(camp, node);
		}
		private Milestone(IBaseCamp camp, XmlNode node)
		{
			_camp = camp;
			ID = XmlHelpers.ParseInteger(node, "id");
			Title = XmlHelpers.ChildNodeText(node, "title");
			CreatedOn = XmlHelpers.ParseDateTime(node, "created-on");
			Deadline = XmlHelpers.ParseDateTime(node, "deadline", CreatedOn);
			Title = XmlHelpers.ChildNodeText(node, "title");
			//WantsNotification = XmlHelpers.ParseBool(node, "wants-notification");
			//ResponsiblePartyID = XmlHelpers.ParseInteger(node, "responsible-party-id");
			Completed = XmlHelpers.ParseBool(node, "completed");
			//CreatorID = XmlHelpers.ParseInteger(node, "creator-id");
			ProjectID = XmlHelpers.ParseInteger(node, "project-id");
		}

		#region Implementation of IMilestone

		public int ID { get; private set; }
		public bool Completed { get; private set; }
		public DateTime CreatedOn { get; private set; }
		public DateTime Deadline { get; set; }
		public string Title { get; set; }
		//public bool WantsNotification { get; private set; }
        //public int ResponsiblePartyID { get; private set; }
		//public int CreatorID { get; private set; }
        public int ProjectID { get; private set; }

		#endregion
	}
}
