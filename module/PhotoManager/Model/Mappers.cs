using System;
using ASC.PhotoManager.Data;
using ASC.Common.Data;
using System.Collections.Generic;
using System.Drawing;

namespace ASC.PhotoManager.Model
{
	static class Mappers
	{
		private static string[] eventColumns = new[] 
		{ 
			"Id", "Name", "Description", "User", "Timestamp" 
		};

		private static string[] albumColumns = new[] 
		{ 
			"Id", "Caption", "User", "Event", "FaceImage", "Timestamp", "ImagesCount", "ViewsCount", "CommentsCount" 
		};

		private static string[] imageColumns = new[] 
		{ 
			"Id", "Album", "Name", "Description", "Location", "Timestamp", "User", 
			"ThumbnailWidth", "ThumbnailHeight", /*"OriginalWidth", "OriginalHeight",*/ "PreviewWidth", "PreviewHeight", 
			"ViewsCount", "CommentsCount" 
		};

		private static string[] commentColumns = new[] 
		{ 
			"Id", "Text", "User", "Timestamp", "Image", "Parent", "Inactive"
		};

		public static string[] EventColumns
		{
			get { return eventColumns; }
		}

		public static string[] AlbumColumns
		{
			get { return albumColumns; }
		}

		public static string[] ImageColumns
		{
			get { return imageColumns; }
		}

		public static string[] CommentColumns
		{
			get { return commentColumns; }
		}


		public static Event ToEvent(object[] r)
		{
			return new Event()
			{
				Id = Convert.ToInt64(r[0]),
				Name = Convert.ToString(r[1]),
				Description = Convert.ToString(r[2]),
				UserID = Convert.ToString(r[3]),
				Timestamp = ASC.Core.Tenants.TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[4])),
			};
		}

		public static Album ToAlbum(object[] r)
		{
			return new Album()
			{
				Id = Convert.ToInt64(r[0]),
				Caption = Convert.ToString(r[1]),
				UserID = Convert.ToString(r[2]),
				EventId = Convert.ToInt64(r[3]),
				FaceImageId = Convert.ToInt64(r[4]),
				LastUpdate = ASC.Core.Tenants.TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[5])),
				ImagesCount = ToInt32(r[6]),
				ViewsCount = ToInt32(r[7]),
				CommentsCount = ToInt32(r[8]),
			};
		}

		public static AlbumItem ToImage(object[] r)
		{
			return new AlbumItem()
			{
				Id = Convert.ToInt64(r[0]),
				AlbumId = Convert.ToInt64(r[1]),
				Name = Convert.ToString(r[2]),
				Description = Convert.ToString(r[3]),
				Location = Convert.ToString(r[4]),
				Timestamp = ASC.Core.Tenants.TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[5])),
				UserID = Convert.ToString(r[6]),
				ThumbnailSize = new Size(ToInt32(r[7]), ToInt32(r[8])),
				//OriginalSize = new Size(ToInt32(r[9]), ToInt32(r[10])),
				PreviewSize = new Size(ToInt32(r[9]), ToInt32(r[10])),
				ViewsCount = ToInt32(r[11]),
				CommentsCount = ToInt32(r[12]),
			};
		}

		public static Comment ToComment(object[] r)
		{
			return new Comment(Convert.ToInt64(r[4]))
			{
				Id = Convert.ToInt64(r[0]),
				Text = Convert.ToString(r[1]),
				UserID = Convert.ToString(r[2]),
				Timestamp = ASC.Core.Tenants.TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[3])),
				ParentId = Convert.ToInt64(r[5]),
				Inactive = Convert.ToBoolean(r[6]),
			};
		}

        public static CommentNews ToCommentNews(object[] row)
        {
            return new CommentNews
            {
                Comment = Mappers.ToComment(row),
                Image = new AlbumItem()
                {
                    Name = Convert.ToString(row[7]),
                    Location = Convert.ToString(row[8]),
                    UserID = Convert.ToString(row[9]),
                    AlbumId = Convert.ToInt64(row[10])
                }
            };
        }

        public static AlbumItem ToImageNews(object[] row)
        {
            return new AlbumItem
            {
                Id = Convert.ToInt64(row[0]),
                AlbumId = Convert.ToInt64(row[1]),
                Name = Convert.ToString(row[2]),
                Description = Convert.ToString(row[3]),
                Location = Convert.ToString(row[4]),
                Timestamp = Convert.ToDateTime(row[5]),
                UserID = Convert.ToString(row[6]),
                Album = new Album()
                {
                    Id = Convert.ToInt64(row[1]),
                    Caption = Convert.ToString(row[7]),
                    FaceItem = new AlbumItem()
                    {
                        AlbumId = Convert.ToInt64(row[1]),
                        Location = Convert.ToString(row[8]),
                        UserID = Convert.ToString(row[9])
                    }
                }
            };
        }

		private static int ToInt32(object int32)
		{
			return (int)Convert.ToInt64(int32);
		}
	}
}
