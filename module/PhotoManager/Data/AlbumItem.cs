namespace ASC.PhotoManager.Data
{
	#region

	using System;
	using System.Collections.Generic;
	using ASC.Common.Security;
	using ASC.Common.Security.Authorizing;
	using System.Drawing;

	#endregion

	public class AlbumItem : ISecurityObject
	{
		#region Constants

		public static readonly AlbumItem Empty = new AlbumItem();

		private const string thumb_suffix = "thumb";
		private const string preview_suffix = "preview";
		private const string jpeg_extension = "jpg";

		#endregion

		#region Properties

		public long Id
		{
			get;
			internal set;
		}

        //public string ExpandedStore
        //{
        //    get { return GetFilePath(string.Empty); }
        //}

		public string ExpandedStoreThumb
		{
			get { return GetFilePath(thumb_suffix); }
		}

		public string ExpandedStorePreview
		{
			get { return GetFilePath(preview_suffix); }
		}

		public string Location
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string UserID
		{
			get;
			set;
		}
		public string Description
		{
			get;
			set;
		}
        //public Size OriginalSize
        //{
        //    get;
        //    set;
        //}
		public Size PreviewSize
		{
			get;
			set;
		}
		public Size ThumbnailSize
		{
			get;
			set;
		}
		public Album Album
		{
			get;
			internal set;
		}
		public DateTime Timestamp
		{
			get;
			set;
		}

		public int CommentsCount
		{
			get;
			internal set;
		}

		public int ViewsCount
		{
			get;
			internal set;
		}

		internal long AlbumId
		{
			get;
			set;
		}

		#endregion


		internal AlbumItem()
		{
			ThumbnailSize = Size.Empty;
			PreviewSize = Size.Empty;
			//OriginalSize = Size.Empty;
		}

		public AlbumItem(Album album)
			: this()
		{
			if (album == null) throw new ArgumentNullException("album");
			Album = album;
		}


		#region Utility methods

		private string GetFilePath(string locationSuffix)
		{
            if (AlbumId != 0)
                return string.Format("{0}/{1}/{2}{3}.{4}", UserID, AlbumId, Location, locationSuffix, jpeg_extension);
			return Album != null ?
				string.Format("{0}/{1}/{2}{3}.{4}", UserID, Album.Id, Location, locationSuffix, /*string.IsNullOrEmpty(this.Description) ? */jpeg_extension /*: this.Description*/) :
				string.Empty;
		}

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

			if (!string.IsNullOrEmpty(this.UserID) && Equals(account.ID, new Guid(UserID)))
			{
				roles.Add(Constants.Owner);
			}

			return roles;
		}

		public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
		{
			return this.Album;
		}

		public bool InheritSupported
		{
			get { return true; }
		}

		public bool ObjectRolesSupported
		{
			get { return true; }
		}

		#endregion


		public override bool Equals(object obj)
		{
			var ai = obj as AlbumItem;
			if (ai == null) return false;
			if (ai.Id == 0 && Id == 0) return ReferenceEquals(ai, this);
			return ai.Id == Id;
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