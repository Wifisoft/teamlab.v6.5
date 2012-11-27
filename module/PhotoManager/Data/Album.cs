namespace ASC.PhotoManager.Data
{
	#region

	using System;
	using System.Collections.Generic;
	using ASC.Common.Security;
	using ASC.Common.Security.Authorizing;

	#endregion

	public class Album : ISecurityObject
	{
		#region Properties

		public long Id
		{
			get;
			internal set;
		}

		public Event Event
		{
			get;
			set;
		}

		public string UserID
		{
			get;
			set;
		}

		public string Caption
		{
			get;
			set;
		}

		public AlbumItem FaceItem
		{
			get;
			set;
		}

		public DateTime LastUpdate
		{
			get;
			internal set;
		}

		public int ImagesCount
		{
			get;
			internal set;
		}

		public int ViewsCount
		{
			get;
			internal set;
		}

		public int CommentsCount
		{
			get;
			internal set;
		}

		internal long FaceImageId
		{
			get;
			set;
		}

		internal long EventId
		{
			get;
			set;
		}

		#endregion


		public Album()
		{
            LastUpdate = ASC.Core.Tenants.TenantUtil.DateTimeNow();
		}


		#region Methods

		#endregion

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
			if (!string.IsNullOrEmpty(this.UserID) && Equals(account.ID, new Guid(this.UserID)))
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
			var a = obj as Album;
			if (a == null) return false;
			if (a.Id == 0 && Id == 0) return ReferenceEquals(a, this);
			return a.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id == 0 ? base.GetHashCode() : Id.GetHashCode();
		}

		public override string ToString()
		{
			return GetType().Name + ": " + Id;
		}
	}
}