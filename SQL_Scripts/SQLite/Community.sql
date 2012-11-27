-- blogs_blogs
CREATE TABLE IF NOT EXISTS "blogs_blogs" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "name" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "user_id" char(38) NOT NULL COLLATE NOCASE,
  "group_id" char(38) NOT NULL COLLATE NOCASE,
  "Tenant" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "blogs_blogs_user_id" ON "blogs_blogs" ("Tenant","user_id");


-- blogs_comments
CREATE TABLE IF NOT EXISTS "blogs_comments" (
  "Tenant" INTEGER NOT NULL,
  "id" char(38) NOT NULL COLLATE NOCASE,
  "post_id" char(38) NOT NULL COLLATE NOCASE,
  "content" text COLLATE NOCASE,
  "created_by" char(38) NOT NULL COLLATE NOCASE,
  "created_when" datetime NOT NULL,
  "parent_id" char(38) DEFAULT NULL COLLATE NOCASE,
  "inactive" INTEGER DEFAULT NULL,
  PRIMARY KEY ("Tenant","id")
);
CREATE INDEX IF NOT EXISTS "blogs_comments_ixComments_Created" ON "blogs_comments" ("Tenant","created_when");
CREATE INDEX IF NOT EXISTS "blogs_comments_ixComments_PostId" ON "blogs_comments" ("Tenant","post_id");


-- blogs_posts
CREATE TABLE IF NOT EXISTS "blogs_posts" (
  "id" char(38) NOT NULL COLLATE NOCASE,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "content" TEXT NOT NULL COLLATE NOCASE,
  "created_by" char(38) NOT NULL COLLATE NOCASE,
  "created_when" datetime NOT NULL,
  "blog_id" INTEGER NOT NULL,
  "Tenant" INTEGER NOT NULL DEFAULT 0,
  "LastCommentId" char(38) DEFAULT NULL COLLATE NOCASE,
  "LastModified" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY ("Tenant","id")
);
CREATE INDEX IF NOT EXISTS "blogs_posts_ixPosts_CreatedBy" ON "blogs_posts" ("Tenant","created_by");
CREATE INDEX IF NOT EXISTS "blogs_posts_ixPosts_CreatedWhen" ON "blogs_posts" ("Tenant","created_when");
CREATE INDEX IF NOT EXISTS "blogs_posts_ixPosts_LastCommentId" ON "blogs_posts" ("Tenant","LastCommentId");


-- blogs_reviewposts
CREATE TABLE IF NOT EXISTS "blogs_reviewposts" (
  "post_id" char(38) NOT NULL COLLATE NOCASE,
  "reviewed_by" char(38) NOT NULL COLLATE NOCASE,
  "timestamp" datetime NOT NULL,
  "Tenant" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("Tenant","post_id","reviewed_by")
);


-- blogs_tags
CREATE TABLE IF NOT EXISTS "blogs_tags" (
  "post_id" varchar(38) NOT NULL COLLATE NOCASE,
  "name" varchar(255) NOT NULL COLLATE NOCASE,
  "Tenant" INTEGER NOT NULL,
  PRIMARY KEY ("Tenant","post_id","name")
);
CREATE INDEX IF NOT EXISTS "blogs_tags_name" ON "blogs_tags" ("name");


-- bookmarking_bookmark
CREATE TABLE IF NOT EXISTS "bookmarking_bookmark" (
  "ID" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "URL" text COLLATE NOCASE,
  "Date" datetime DEFAULT NULL,
  "Name" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "Description" text COLLATE NOCASE,
  "UserCreatorID" char(38) DEFAULT NULL COLLATE NOCASE,
  "Tenant" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "bookmarking_bookmark_Tenant" ON "bookmarking_bookmark" ("Tenant");


-- bookmarking_bookmarktag
CREATE TABLE IF NOT EXISTS "bookmarking_bookmarktag" (
  "BookmarkID" INTEGER NOT NULL,
  "TagID" INTEGER NOT NULL,
  "Tenant" INTEGER NOT NULL,
  PRIMARY KEY ("BookmarkID","TagID")
);
CREATE INDEX IF NOT EXISTS "bookmarking_bookmarktag_Tenant" ON "bookmarking_bookmarktag" ("Tenant");


-- bookmarking_comment
CREATE TABLE IF NOT EXISTS "bookmarking_comment" (
  "ID" char(38) NOT NULL COLLATE NOCASE,
  "UserID" char(38) DEFAULT NULL COLLATE NOCASE,
  "Content" text COLLATE NOCASE,
  "Datetime" datetime DEFAULT NULL,
  "Parent" char(38) DEFAULT NULL COLLATE NOCASE,
  "BookmarkID" INTEGER DEFAULT NULL,
  "Inactive" INTEGER DEFAULT NULL,
  "Tenant" INTEGER NOT NULL,
  PRIMARY KEY ("ID")
);
CREATE INDEX IF NOT EXISTS "bookmarking_comment_IndexCommentBookmarkID" ON "bookmarking_comment" ("Tenant","BookmarkID");


-- bookmarking_tag
CREATE TABLE IF NOT EXISTS "bookmarking_tag" (
  "TagID" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Name" varchar(255) NOT NULL COLLATE NOCASE,
  "Tenant" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "bookmarking_tag_Name" ON "bookmarking_tag" ("Tenant","Name");


-- bookmarking_userbookmark
CREATE TABLE IF NOT EXISTS "bookmarking_userbookmark" (
  "UserBookmarkID" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "UserID" char(38) DEFAULT NULL COLLATE NOCASE,
  "DateAdded" datetime DEFAULT NULL,
  "Name" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "Description" text COLLATE NOCASE,
  "BookmarkID" INTEGER NOT NULL,
  "Raiting" INTEGER NOT NULL DEFAULT 0,
  "Tenant" INTEGER NOT NULL,
  "LastModified" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS "bookmarking_userbookmark_LastModified" ON "bookmarking_userbookmark" ("Tenant","LastModified");
CREATE INDEX IF NOT EXISTS "bookmarking_userbookmark_BookmarkID" ON "bookmarking_userbookmark" ("BookmarkID");


-- bookmarking_userbookmarktag
CREATE TABLE IF NOT EXISTS "bookmarking_userbookmarktag" (
  "UserBookmarkID" INTEGER NOT NULL,
  "TagID" INTEGER NOT NULL,
  "Tenant" INTEGER NOT NULL,
  PRIMARY KEY ("UserBookmarkID","TagID")
);
CREATE INDEX IF NOT EXISTS "bookmarking_userbookmarktag_Tenant" ON "bookmarking_userbookmarktag" ("Tenant");


-- events_comment
CREATE TABLE IF NOT EXISTS "events_comment" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Feed" INTEGER NOT NULL,
  "Comment" text NOT NULL COLLATE NOCASE,
  "Parent" INTEGER NOT NULL DEFAULT 0,
  "Date" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "Creator" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "Inactive" INTEGER NOT NULL DEFAULT 0,
  "Tenant" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "events_comment_Tenant" ON "events_comment" ("Tenant");


-- events_feed
CREATE TABLE IF NOT EXISTS "events_feed" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "FeedType" INTEGER NOT NULL DEFAULT '1',
  "Caption" text NOT NULL COLLATE NOCASE,
  "Text" text COLLATE NOCASE,
  "Date" datetime NOT NULL,
  "Creator" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "Tenant" INTEGER NOT NULL DEFAULT 0,
  "LastModified" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS "events_feed_LastModified" ON "events_feed" ("Tenant","LastModified");


-- events_poll
CREATE TABLE IF NOT EXISTS "events_poll" (
  "Id" INTEGER NOT NULL,
  "PollType" INTEGER NOT NULL DEFAULT 0,
  "StartDate" datetime NOT NULL,
  "EndDate" datetime NOT NULL,
  "Tenant" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("Id")
);
CREATE INDEX IF NOT EXISTS "events_poll_Tenant" ON "events_poll" ("Tenant");


-- events_pollanswer
CREATE TABLE IF NOT EXISTS "events_pollanswer" (
  "Variant" INTEGER NOT NULL,
  "User" varchar(64) NOT NULL COLLATE NOCASE,
  "Tenant" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("Variant","User")
);
CREATE INDEX IF NOT EXISTS "events_pollanswer_Tenant" ON "events_pollanswer" ("Tenant");


-- events_pollvariant
CREATE TABLE IF NOT EXISTS "events_pollvariant" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Poll" INTEGER NOT NULL,
  "Name" varchar(1024) NOT NULL COLLATE NOCASE,
  "Tenant" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "events_pollvariant_Poll" ON "events_pollvariant" ("Tenant","Poll");


-- events_reader
CREATE TABLE IF NOT EXISTS "events_reader" (
  "Feed" INTEGER NOT NULL,
  "Reader" varchar(38) NOT NULL COLLATE NOCASE,
  "Tenant" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("Feed","Reader")
);
CREATE INDEX IF NOT EXISTS "events_reader_Tenant" ON "events_reader" ("Tenant");


-- forum_answer
CREATE TABLE IF NOT EXISTS "forum_answer" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "question_id" INTEGER NOT NULL,
  "create_date" datetime DEFAULT NULL,
  "user_id" char(38) NOT NULL COLLATE NOCASE,
  "TenantID" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "forum_answer_TenantID" ON "forum_answer" ("TenantID");


-- forum_answer_variant
CREATE TABLE IF NOT EXISTS "forum_answer_variant" (
  "answer_id" INTEGER NOT NULL,
  "variant_id" INTEGER NOT NULL,
  PRIMARY KEY ("answer_id","variant_id")
);


-- forum_attachment
CREATE TABLE IF NOT EXISTS "forum_attachment" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "name" varchar(500) NOT NULL COLLATE NOCASE,
  "post_id" INTEGER NOT NULL,
  "size" INTEGER NOT NULL DEFAULT 0,
  "download_count" INTEGER NOT NULL DEFAULT 0,
  "content_type" INTEGER NOT NULL DEFAULT 0,
  "mime_content_type" varchar(100) DEFAULT NULL COLLATE NOCASE,
  "create_date" datetime DEFAULT NULL,
  "path" varchar(1000) NOT NULL COLLATE NOCASE,
  "TenantID" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "forum_attachment_post_id" ON "forum_attachment" ("TenantID","post_id");


-- forum_category
CREATE TABLE IF NOT EXISTS "forum_category" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(500) NOT NULL COLLATE NOCASE,
  "description" varchar(500) NOT NULL DEFAULT '' COLLATE NOCASE,
  "sort_order" INTEGER NOT NULL DEFAULT 0,
  "create_date" datetime NOT NULL,
  "poster_id" char(38) NOT NULL COLLATE NOCASE,
  "TenantID" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "forum_category_TenantID" ON "forum_category" ("TenantID");


-- forum_lastvisit
CREATE TABLE IF NOT EXISTS "forum_lastvisit" (
  "tenantid" INTEGER NOT NULL,
  "user_id" char(38) NOT NULL COLLATE NOCASE,
  "thread_id" INTEGER NOT NULL,
  "last_visit" datetime NOT NULL,
  PRIMARY KEY ("user_id","thread_id")
);
CREATE INDEX IF NOT EXISTS "forum_lastvisit_tenantid" ON "forum_lastvisit" ("tenantid");


-- forum_post
CREATE TABLE IF NOT EXISTS "forum_post" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "topic_id" INTEGER NOT NULL,
  "poster_id" char(38) NOT NULL COLLATE NOCASE,
  "create_date" datetime NOT NULL,
  "subject" varchar(500) NOT NULL DEFAULT '' COLLATE NOCASE,
  "text" TEXT NOT NULL COLLATE NOCASE,
  "edit_date" datetime DEFAULT NULL,
  "edit_count" INTEGER NOT NULL DEFAULT 0,
  "is_approved" INTEGER NOT NULL DEFAULT 0,
  "parent_post_id" INTEGER NOT NULL DEFAULT 0,
  "formatter" INTEGER NOT NULL DEFAULT 0,
  "editor_id" char(38) DEFAULT NULL COLLATE NOCASE,
  "TenantID" INTEGER NOT NULL DEFAULT 0,
  "LastModified" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS "forum_post_LastModified" ON "forum_post" ("TenantID","LastModified");
CREATE INDEX IF NOT EXISTS "forum_post_topic_id" ON "forum_post" ("TenantID","topic_id");


-- forum_question
CREATE TABLE IF NOT EXISTS "forum_question" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "topic_id" INTEGER NOT NULL,
  "type" INTEGER NOT NULL DEFAULT 0,
  "name" varchar(500) NOT NULL COLLATE NOCASE,
  "create_date" datetime NOT NULL,
  "TenantID" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "forum_question_topic_id" ON "forum_question" ("TenantID","topic_id");


-- forum_tag
CREATE TABLE IF NOT EXISTS "forum_tag" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "name" varchar(200) NOT NULL COLLATE NOCASE,
  "is_approved" INTEGER NOT NULL DEFAULT 0,
  "TenantID" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "forum_tag_TenantID" ON "forum_tag" ("TenantID");


-- forum_thread
CREATE TABLE IF NOT EXISTS "forum_thread" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(500) NOT NULL COLLATE NOCASE,
  "description" varchar(500) NOT NULL DEFAULT '' COLLATE NOCASE,
  "sort_order" INTEGER NOT NULL DEFAULT 0,
  "category_id" INTEGER NOT NULL,
  "topic_count" INTEGER NOT NULL DEFAULT 0,
  "post_count" INTEGER NOT NULL DEFAULT 0,
  "is_approved" INTEGER NOT NULL DEFAULT 0,
  "TenantID" INTEGER NOT NULL DEFAULT 0,
  "recent_post_id" INTEGER NOT NULL DEFAULT 0,
  "recent_topic_id" INTEGER NOT NULL DEFAULT 0,
  "recent_post_date" datetime DEFAULT NULL,
  "recent_poster_id" char(38) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "forum_thread_TenantID" ON "forum_thread" ("TenantID");


-- forum_topic
CREATE TABLE IF NOT EXISTS "forum_topic" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "thread_id" INTEGER NOT NULL,
  "title" varchar(500) NOT NULL COLLATE NOCASE,
  "type" INTEGER NOT NULL DEFAULT 0,
  "create_date" datetime NOT NULL,
  "view_count" INTEGER NOT NULL DEFAULT 0,
  "post_count" INTEGER NOT NULL DEFAULT 0,
  "recent_post_id" INTEGER NOT NULL DEFAULT 0,
  "is_approved" INTEGER NOT NULL DEFAULT 0,
  "poster_id" char(38) DEFAULT NULL COLLATE NOCASE,
  "sticky" INTEGER NOT NULL DEFAULT 0,
  "closed" INTEGER DEFAULT 0,
  "question_id" varchar(45) DEFAULT 0 COLLATE NOCASE,
  "TenantID" INTEGER NOT NULL DEFAULT 0,
  "LastModified" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS "forum_topic_LastModified" ON "forum_topic" ("TenantID","LastModified");


-- forum_topic_tag
CREATE TABLE IF NOT EXISTS "forum_topic_tag" (
  "topic_id" INTEGER NOT NULL,
  "tag_id" INTEGER NOT NULL,
  PRIMARY KEY ("topic_id","tag_id")
);


-- forum_topicwatch
CREATE TABLE IF NOT EXISTS "forum_topicwatch" (
  "TenantID" INTEGER NOT NULL,
  "UserID" char(38) NOT NULL COLLATE NOCASE,
  "TopicID" INTEGER NOT NULL,
  "ThreadID" INTEGER NOT NULL,
  PRIMARY KEY ("TenantID","UserID","TopicID")
);


-- forum_variant
CREATE TABLE IF NOT EXISTS "forum_variant" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "name" varchar(200) NOT NULL COLLATE NOCASE,
  "question_id" INTEGER NOT NULL,
  "sort_order" INTEGER NOT NULL DEFAULT 0
);


-- photo_album
CREATE TABLE IF NOT EXISTS "photo_album" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Caption" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "Event" INTEGER NOT NULL,
  "User" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "FaceImage" INTEGER NOT NULL DEFAULT 0,
  "Timestamp" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "ImagesCount" INTEGER NOT NULL DEFAULT 0,
  "ViewsCount" INTEGER NOT NULL DEFAULT 0,
  "CommentsCount" INTEGER NOT NULL DEFAULT 0,
  "Tenant" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "photo_album_Photo_Album_Index1" ON "photo_album" ("Tenant","Event");


-- photo_comment
CREATE TABLE IF NOT EXISTS "photo_comment" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Text" text NOT NULL COLLATE NOCASE,
  "User" varchar(38) NOT NULL COLLATE NOCASE,
  "Timestamp" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "Image" INTEGER NOT NULL,
  "Parent" INTEGER NOT NULL DEFAULT 0,
  "Inactive" INTEGER NOT NULL DEFAULT 0,
  "Tenant" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "photo_comment_Photo_Comment_Index1" ON "photo_comment" ("Tenant","Image");


-- photo_event
CREATE TABLE IF NOT EXISTS "photo_event" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Name" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "Description" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "User" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "Timestamp" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "Tenant" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "photo_event_Tenant" ON "photo_event" ("Tenant");


-- photo_image
CREATE TABLE IF NOT EXISTS "photo_image" (
  "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Album" INTEGER NOT NULL,
  "Name" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "Description" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "Location" varchar(1024) DEFAULT NULL COLLATE NOCASE,
  "Timestamp" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "User" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "ThumbnailWidth" INTEGER NOT NULL DEFAULT 0,
  "ThumbnailHeight" INTEGER NOT NULL DEFAULT 0,
  "PreviewWidth" INTEGER NOT NULL DEFAULT 0,
  "PreviewHeight" INTEGER NOT NULL DEFAULT 0,
  "CommentsCount" INTEGER NOT NULL DEFAULT 0,
  "ViewsCount" INTEGER NOT NULL DEFAULT 0,
  "Tenant" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "photo_image_Photo_Image_Index1" ON "photo_image" ("Tenant","Album");


-- photo_imageview
CREATE TABLE IF NOT EXISTS "photo_imageview" (
  "Tenant" INTEGER NOT NULL,
  "Image" INTEGER NOT NULL,
  "User" varchar(38) NOT NULL COLLATE NOCASE,
  "Timestamp" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY ("Tenant","Image","User")
);


-- wiki_categories
CREATE TABLE IF NOT EXISTS "wiki_categories" (
  "Tenant" INTEGER NOT NULL,
  "CategoryName" varchar(240) NOT NULL COLLATE NOCASE,
  "PageName" varchar(240) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("Tenant","CategoryName","PageName")
);
CREATE INDEX IF NOT EXISTS "wiki_categories_PageName" ON "wiki_categories" ("Tenant","PageName");


-- wiki_comments
CREATE TABLE IF NOT EXISTS "wiki_comments" (
  "Id" char(38) NOT NULL COLLATE NOCASE,
  "ParentId" char(38) NOT NULL COLLATE NOCASE,
  "PageName" varchar(255) NOT NULL COLLATE NOCASE,
  "Body" text NOT NULL COLLATE NOCASE,
  "UserId" char(38) NOT NULL COLLATE NOCASE,
  "Date" datetime NOT NULL,
  "Inactive" INTEGER NOT NULL,
  "Tenant" INTEGER NOT NULL,
  PRIMARY KEY ("Id")
);
CREATE INDEX IF NOT EXISTS "wiki_comments_PageName" ON "wiki_comments" ("Tenant","PageName");


-- wiki_files
CREATE TABLE IF NOT EXISTS "wiki_files" (
  "Tenant" INTEGER NOT NULL,
  "FileName" varchar(255) NOT NULL COLLATE NOCASE,
  "Version" INTEGER NOT NULL,
  "UploadFileName" text NOT NULL COLLATE NOCASE,
  "UserID" char(38) NOT NULL COLLATE NOCASE,
  "Date" datetime NOT NULL,
  "FileLocation" text NOT NULL COLLATE NOCASE,
  "FileSize" INTEGER NOT NULL,
  PRIMARY KEY ("Tenant","FileName","Version")
);


-- wiki_pages
CREATE TABLE IF NOT EXISTS "wiki_pages" (
  "tenant" INTEGER NOT NULL,
  "pagename" varchar(255) NOT NULL COLLATE NOCASE,
  "version" INTEGER NOT NULL,
  "modified_by" char(38) NOT NULL COLLATE NOCASE,
  "modified_on" datetime NOT NULL,
  PRIMARY KEY ("tenant","pagename")
);
CREATE INDEX IF NOT EXISTS "wiki_pages_modified_on" ON "wiki_pages" ("tenant","modified_on");


-- wiki_pages_history
CREATE TABLE IF NOT EXISTS "wiki_pages_history" (
  "tenant" INTEGER NOT NULL,
  "pagename" varchar(255) NOT NULL COLLATE NOCASE,
  "version" INTEGER NOT NULL,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "body" TEXT COLLATE NOCASE,
  PRIMARY KEY ("tenant","pagename","version")
);



