using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Impl;
using ASC.Api.Utils;
using ASC.Common.Data;
using ASC.Core;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Model;

namespace ASC.Api.Photo
{
    ///<summary>
    ///Photos and Albums
    ///</summary>
    public class PhotoApi : Interfaces.IApiEntryPoint
    {
        private readonly ApiContext _context;

        public string Name
        {
            get { return "photo"; }
        }

        private IImageStorage _storage;
        private IImageStorage ImageStorage
        {
            get
            {
                if (_storage == null)
                {
                    _storage = StorageFactory.GetStorage();
                }
                return _storage;
            }
        }

        public PhotoApi(ApiContext context)
        {
            _context = context;
        }

		  ///<summary>
		  /// Returns the list of all events for the photo album with the event titles, date of creation and update, description and author details
		  ///</summary>
		  ///<short>
		  /// Events
		  ///</short>
		  ///<returns>list of events</returns>
        [Read("")]
        public IEnumerable<PhotoEventWrapper> GetEvents()
        {
            var events = ImageStorage.GetEvents((int) _context.StartIndex, (int) _context.Count);
            _context.SetDataPaginated();
            return events.Select(x => new PhotoEventWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all photo albums of the current user related to the event specified
		  ///</summary>
		  ///<short>
		  ///My albums
		  ///</short>
		  ///<param name="eventid">id of event</param>
        ///<returns>list of albums</returns>
        [Read(@"@self/{eventid:[0-9]+}")]
        public IEnumerable<PhotoAlbumWrapper> GetMyAlbums(long eventid)
        {
            var albums = ImageStorage.GetAlbums(eventid, SecurityContext.CurrentAccount.ID.ToString()).NotFoundIfNull();
            return albums.Select(x => new PhotoAlbumWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all photo albums containing the query string in its title, description, location or comments
		  ///</summary>
		  ///<short>
		  ///Search
		  ///</short>
		  ///<param name="query">search query</param>
        ///<returns>list of albums</returns>
        [Read("@search/{query}")]
        public IEnumerable<PhotoAlbumItemWrapper> SearchAlbums(string query)
        {
            var albums = ImageStorage.SearchAlbumItems(query);
            return albums.Select(x => new PhotoAlbumItemWrapper(x)).ToSmartList();
        }
		  ///<summary>
		  ///Returns the list of all photo albums of all users related to the event specified
		  ///</summary>
		  ///<short>
		  ///Albums
		  ///</short>
		  ///<param name="eventid">id of event</param>
        ///<returns>list of albums</returns>
        [Read("{eventid:[0-9]+}")]
        public IEnumerable<PhotoAlbumWrapper> GetAlbums(long eventid)
        {
            var albums = ImageStorage.GetAlbums(eventid, string.Empty).NotFoundIfNull();
            return albums.Select(x => new PhotoAlbumWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all photos for the specified event in the selected album
		  ///</summary>
		  ///<short>
		  ///Album photos
		  ///</short>
		  ///<param name="eventid">id of event</param>
        ///<param name="albumid">id of album</param>
        ///<returns>list of photos</returns>
        [Read("{eventid:[0-9]+}/{albumid:[0-9]+}")]
        public IEnumerable<PhotoAlbumItemWrapper> GetAlbumItems(long eventid, long albumid)
        {
            //TODO: think about not used parameter
            var @event = ImageStorage.GetEvent(eventid).NotFoundIfNull();//Just for check
            var items = ImageStorage.GetAlbumItems(ImageStorage.GetAlbum(albumid).NotFoundIfNull());
            return items.Select(x => new PhotoAlbumItemWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all comments for the specified photo in the selected album related with the specified event
		  ///</summary>
		  ///<short>
		  ///Photo comments
		  ///</short>
		  ///<param name="eventid">id of event</param>
        ///<param name="albumid">id of album</param>
        ///<param name="itemid">id of photo</param>
        ///<returns>list of comments</returns>
        [Read("{eventid:[0-9]+}/{albumid:[0-9]+}/{itemid:[0-9]+}/comment")]
        public IEnumerable<PhotoAlbumItemCommentWrapper> GetAlbumItemsComments(long eventid, long albumid, long itemid)
        {
            //TODO: think about not used parameter
            ImageStorage.GetEvent(eventid).NotFoundIfNull();//Just for check
            ImageStorage.GetAlbum(albumid).NotFoundIfNull();//Just for check
            var comments = ImageStorage.GetComments(itemid);
            return comments.Select(x => new PhotoAlbumItemCommentWrapper(x)).ToSmartList();
        }
    }
}
