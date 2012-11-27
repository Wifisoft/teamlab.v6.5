
namespace ASC.Bookmarking.Pojo
{
	public class UserBookmarkTag
	{
		public virtual long UserBookmarkTagID { get; set; }

		public virtual long UserBookmarkID { get; set; }

		public virtual long TagID { get; set; }

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			
			var tag = obj as UserBookmarkTag;
			if (this.TagID.Equals(tag.TagID))
			{
				return true;
			}
			return false;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{			
			var hash = TagID.GetHashCode();
			return hash;
		}

	}
}
