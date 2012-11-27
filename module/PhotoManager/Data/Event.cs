using System;
using System.Collections.Generic;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.PhotoManager.Data
{
	public class Event : ISecurityObject
	{
		public long Id
		{
			get;
			internal set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string UserID
		{
			get;
			set;
		}

		public DateTime Timestamp
		{
			get;
			set;
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


		public Event()
		{
            Timestamp = ASC.Core.Tenants.TenantUtil.DateTimeNow();
		}


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
			var e = obj as Event;
			if (e == null) return false;
			if (e.Id == 0 && Id == 0) return ReferenceEquals(e, this);
			return e.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id == 0 ? base.GetHashCode() : Id.GetHashCode();
		}

		public override string ToString()
		{
			return GetType().Name + ": " + this.Name;
		}
	}
}
