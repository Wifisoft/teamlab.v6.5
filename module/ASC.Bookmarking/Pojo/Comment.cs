using System;
using System.Collections.Generic;
namespace ASC.Bookmarking.Pojo
{
	public class Comment
	{
		public virtual Guid ID { get; set; }		

		public virtual Guid UserID { get; set; }

		public virtual string Content { get; set; }

		public virtual DateTime Datetime { get; set; }

		public virtual String Parent { get; set; }

		public virtual long BookmarkID { get; set; }

		public virtual bool Inactive { get; set; }

		// override object.Equals
		public override bool Equals(object obj)
		{
			//       
			// See the full list of guidelines at
			//   http://go.microsoft.com/fwlink/?LinkID=85237  
			// and also the guidance for operator== at
			//   http://go.microsoft.com/fwlink/?LinkId=85238
			//

			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var c = obj as Comment;
			if (ID.Equals(c.ID))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (GetType().FullName + "|" +
					ID.ToString()).GetHashCode();
		}						
	}
}
