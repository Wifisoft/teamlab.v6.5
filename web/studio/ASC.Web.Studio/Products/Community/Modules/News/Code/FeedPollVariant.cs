using System;
using System.Collections.Generic;

namespace ASC.Web.Community.News.Code
{
	[Serializable]
	public class FeedPollVariant
	{
		public long ID
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public FeedPollVariant()
		{

		}

		public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var v = obj as FeedPollVariant;
			return obj != null && ID == v.ID;
		}
	}
}