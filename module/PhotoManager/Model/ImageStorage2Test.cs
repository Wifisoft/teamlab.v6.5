using ASC.Common.Data;
using ASC.Core;
using ASC.PhotoManager.Data;
using log4net.Config;
using NUnit.Framework;
using System;
using log4net.Appender;
using log4net.Layout;
using log4net.Filter;

namespace ASC.PhotoManager.Model
{
	[TestFixture]
	public class ImageStorage2Test
	{
		private IImageStorage store;

		public ImageStorage2Test()
		{
			var layout = new PatternLayout() { ConversionPattern = "%date [%thread] %-5level %logger [%ndc] - %message%newline" };
			BasicConfigurator.Configure(new TraceAppender() { Layout = layout, ImmediateFlush = true });

			DbRegistry.RegisterDatabase("photo", "System.Data.SQLite", @"Data Source=D:\Work\ASC\MainTrunk\web\studio\ASC.Web.Studio\Products\Community\Modules\PhotoManager\App_Data\images.db3");
			store = new ImageStorage2("photo", 0);
		}

		[Test]
		public void EventsTest()
		{
			var events = store.GetEvents(0, 3);
			var eventsCount = store.GetEventsCount();

			var event1 = new Event() { Name = "n1", Description = "d1", UserID = SecurityContext.CurrentAccount.ID.ToString() };
			store.SaveEvent(event1);
			store.SaveEvent(event1);
			var event2 = store.GetEvent(event1.Id);
			store.RemoveEvent(event1.Id);
		}

		[Test]
		public void AlbumsTest()
		{
			var albums = store.GetAlbums(0, null);
			var albumsCount = store.GetAlbumsCount(0, null);
			albums = store.GetLastAlbums(4);
			var album = new Album() { Caption = "aaa", Event = store.GetEvent(1) };
			store.SaveAlbum(album);
			store.SaveAlbum(album);
			store.RemoveAlbum(album.Id);
		}
	}
}
