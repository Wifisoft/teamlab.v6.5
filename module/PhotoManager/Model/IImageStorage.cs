using System;
using System.Collections.Generic;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.PhotoManager.Data;

namespace ASC.PhotoManager.Model
{
	public interface IImageStorage
	{
		#region Events

		List<Event> GetEvents(int offset, int count);
		long GetEventsCount();

		Event GetEvent(long id);
		void SaveEvent(Event Event);
		void RemoveEvent(long id);

		#endregion


		#region Album

		List<Album> GetAlbums(long eventId, string userId);
		List<Album> GetLastAlbums(int count);
		long GetAlbumsCount(long eventId, string userId);

		Album GetAlbum(long id);
		void SaveAlbum(Album album);
        void SaveAlbum(Album album, IEnumerable<AlbumItem> newItems);
		void RemoveAlbum(long albumId);

		#endregion


		#region AlbumItem

		List<AlbumItem> GetAlbumItems(Album album);
		List<AlbumItem> GetAlbumItemsLastCommented(int start, int count);
		List<AlbumItem> SearchAlbumItems(string text);
		long GetAlbumItemsLastCommentedCount();

		AlbumItem GetAlbumItem(long id);
		AlbumItem GetAlbumItem(Album album, int index);
		void SaveAlbumItem(AlbumItem image);
		void ReadAlbumItem(long albumItemId, string userId);
		void RemoveAlbumItem(long albumItemId);

        List<Comment> GetLatestCommentsByImage(long ImageId, int count);

		#endregion


		#region Album Comment

		List<Comment> GetComments(long albumItemId);
		Comment GetComment(long id);
        long SaveComment(AlbumItem image, Comment newComment);
        void RemoveComment(long commentId);

		#endregion

        #region News

        List<CommentNews> GetComments(DateTime from, DateTime to);
	    List<AlbumItem> GetPhotos(DateTime from, DateTime to);

        #endregion

        #region Notify

        INotifySource NotifySource { get; }
		INotifyClient NotifyClient { get; }

		#endregion

	}
}
