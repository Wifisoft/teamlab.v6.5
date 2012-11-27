using System;
using System.Web;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Model;
using ASC.Web.Studio.Utility;

namespace ASC.PhotoManager.Helpers
{
	public class RequestHelper
	{
		readonly HttpRequest _request;
		readonly IImageStorage _storage;

		long _eventID;
		long _albumID;
		long _albumItemID;

		Album _album;

		public RequestHelper(HttpRequest request, IImageStorage storage)
		{
			if (request == null) throw new ArgumentNullException("request");

			_request = request;
			_storage = storage;
			_ParseRequest();
		}


		public long EventId
		{
			get
			{
				return _eventID == 0 && Album != null ? Album.Event.Id : _eventID;
			}
		}

		private Album Album
		{
			get
			{
				if (_album == null)
				{
					if (_albumID != 0) _album = _storage.GetAlbum(_albumID);
					else if (_albumItemID != 0)
					{
						var item = _storage.GetAlbumItem(_albumItemID);
						if (item != null) _album = item.Album;
					}
				}
				return _album;
			}
		}


		private void _ParseRequest()
		{
			Int64.TryParse(_request[ASC.PhotoManager.PhotoConst.PARAM_EVENT], out _eventID);
			Int64.TryParse(_request[ASC.PhotoManager.PhotoConst.PARAM_ALBUM], out _albumID);
			Int64.TryParse(_request[ASC.PhotoManager.PhotoConst.PARAM_PHOTO], out _albumItemID);
		}
	}

	public class UrlHelper
	{
		public static string GetAbsoluteEventUrl(long eventID)
		{
			return CommonLinkUtility.GetFullAbsolutePath(
					String.Format("{0}?{1}={2}",
						ASC.PhotoManager.PhotoConst.ViewEventPageUrl,
						ASC.PhotoManager.PhotoConst.PARAM_EVENT,
						eventID)
				);
		}

		public static string GetAbsoluteAlbumUrl(long albumID)
		{
			return CommonLinkUtility.GetFullAbsolutePath(
					String.Format("{0}?{1}={2}",
						ASC.PhotoManager.PhotoConst.ViewAlbumPageUrl,
						ASC.PhotoManager.PhotoConst.PARAM_ALBUM,
						albumID)
				);
		}

		public static string GetAbsolutePhotoUrl(long photoID)
		{
			return CommonLinkUtility.GetFullAbsolutePath(
					String.Format("{0}?{1}={2}",
						ASC.PhotoManager.PhotoConst.ViewPhotoPageUrl,
						ASC.PhotoManager.PhotoConst.PARAM_PHOTO,
						photoID)
				);
		}
	}
}
