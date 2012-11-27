namespace ASC.PhotoManager.Data
{
	using System;
	using System.Collections.Generic;
	using ASC.Common.Security;
	using ASC.Common.Security.Authorizing;

	public class Comment : ISecurityObject
	{
		public long Id
		{
			get;
			internal set;
		}

		public string Text
		{
			get;
			set;
		}

		public bool Inactive
		{
			get;
			set;
		}

		public string UserID
		{
			get;
			set;
		}

		public long ItemID
		{
			get;
			internal set;
		}

		public DateTime Timestamp
		{
			get;
			set;
		}

		public IList<Comment> Comments
		{
			get;
			private set;
		}

		public bool IsRead
		{
			get;
			internal set;
		}

		public long ParentId
		{
			get;
			set;
		}

		public Comment(long itemId)
		{
			ItemID = itemId;
            Timestamp = ASC.Core.Tenants.TenantUtil.DateTimeNow();
			Comments = new List<Comment>();
			IsRead = true;
		}

		#region ISecurityObjectId Members

		public Type ObjectType
		{
			get { return this.GetType(); }
		}

		public object SecurityId
		{
			get { return this.Id; }
		}

		#endregion

		#region ISecurityObjectProvider Members

		public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
		{
			var roles = new List<IRole>();

			if (Equals(account.ID, this.UserID))
			{
				roles.Add(Constants.Owner);
			}

			return roles;
		}

		public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
		{
            throw new NotImplementedException();
		}

		public bool InheritSupported
		{
			get { return false; }
		}

		public bool ObjectRolesSupported
		{
			get { return true; }
		}

		#endregion

		public override bool Equals(object obj)
		{
			var c = obj as Comment;
			if (c == null) return false;
			if (c.Id == 0 && Id == 0) return ReferenceEquals(c, this);
			return c.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id == 0 ? base.GetHashCode() : Id.GetHashCode();
		}

		public override string ToString()
		{
			return GetType().Name + ": " + this.Text;
		}
	}
}