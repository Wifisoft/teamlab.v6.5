-- calendar_calendar_item
CREATE TABLE IF NOT EXISTS "calendar_calendar_item" (
  "calendar_id" INTEGER NOT NULL,
  "item_id" char(38) NOT NULL COLLATE NOCASE,
  "is_group" smallint(2) NOT NULL DEFAULT 0,
  PRIMARY KEY ("calendar_id","item_id","is_group")
);


-- calendar_calendar_user
CREATE TABLE IF NOT EXISTS "calendar_calendar_user" (
  "calendar_id" INTEGER NOT NULL DEFAULT 0,
  "ext_calendar_id" varchar(50) NOT NULL DEFAULT '' COLLATE NOCASE,
  "user_id" char(38) NOT NULL COLLATE NOCASE,
  "hide_events" smallint(2) NOT NULL DEFAULT 0,
  "is_accepted" smallint(2) NOT NULL DEFAULT 0,
  "text_color" varchar(50) NOT NULL COLLATE NOCASE,
  "background_color" varchar(50) NOT NULL COLLATE NOCASE,
  "is_new" smallint(2) NOT NULL DEFAULT 0,
  "alert_type" smallint(6) NOT NULL DEFAULT 0,
  "name" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "time_zone" varchar(255) DEFAULT 'UTC' COLLATE NOCASE,
  PRIMARY KEY ("calendar_id","ext_calendar_id","user_id")
);
CREATE INDEX IF NOT EXISTS "calendar_calendar_user_user_id" ON "calendar_calendar_user" ("user_id");


-- calendar_calendars
CREATE TABLE IF NOT EXISTS "calendar_calendars" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "owner_id" char(38) NOT NULL COLLATE NOCASE,
  "name" varchar(255) NOT NULL COLLATE NOCASE,
  "description" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "tenant" INTEGER NOT NULL,
  "text_color" varchar(50) NOT NULL DEFAULT '#000000' COLLATE NOCASE,
  "background_color" varchar(50) NOT NULL DEFAULT '#fa9191' COLLATE NOCASE,
  "alert_type" smallint(6) NOT NULL DEFAULT 0,
  "time_zone" varchar(255) NOT NULL DEFAULT 'UTC' COLLATE NOCASE,
  "ical_url" TEXT COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "calendar_calendars_owner_id" ON "calendar_calendars" ("tenant","owner_id");


-- calendar_event_item
CREATE TABLE IF NOT EXISTS "calendar_event_item" (
  "event_id" INTEGER NOT NULL,
  "item_id" char(38) NOT NULL COLLATE NOCASE,
  "is_group" smallint(2) NOT NULL DEFAULT 0,
  PRIMARY KEY ("event_id","item_id","is_group")
);


-- calendar_event_user
CREATE TABLE IF NOT EXISTS "calendar_event_user" (
  "event_id" INTEGER NOT NULL,
  "user_id" char(38) NOT NULL COLLATE NOCASE,
  "alert_type" smallint(6) NOT NULL DEFAULT 0,
  "is_unsubscribe" smallint(2) NOT NULL DEFAULT 0,
  PRIMARY KEY ("event_id","user_id")
);


-- calendar_events
CREATE TABLE IF NOT EXISTS "calendar_events" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "tenant" INTEGER NOT NULL,
  "name" varchar(255) NOT NULL COLLATE NOCASE,
  "description" text NOT NULL COLLATE NOCASE,
  "calendar_id" INTEGER NOT NULL,
  "start_date" datetime NOT NULL,
  "end_date" datetime NOT NULL,
  "all_day_long" smallint(6) NOT NULL DEFAULT 0,
  "repeat_type" smallint(6) NOT NULL DEFAULT 0,
  "owner_id" char(38) NOT NULL COLLATE NOCASE,
  "alert_type" smallint(6) NOT NULL DEFAULT 0,
  "rrule" varchar(255) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "calendar_events_calendar_id" ON "calendar_events" ("tenant","calendar_id");


-- calendar_notifications
CREATE TABLE IF NOT EXISTS "calendar_notifications" (
  "user_id" char(38) NOT NULL COLLATE NOCASE,
  "event_id" INTEGER NOT NULL,
  "notify_date" datetime NOT NULL,
  "tenant" INTEGER NOT NULL,
  "alert_type" smallint(2) NOT NULL,
  "repeat_type" smallint(2) NOT NULL DEFAULT 0,
  "time_zone" varchar(255) NOT NULL DEFAULT 'UTC' COLLATE NOCASE,
  "rrule" varchar(255) DEFAULT NULL COLLATE NOCASE,
  PRIMARY KEY ("user_id","event_id")
);
CREATE INDEX IF NOT EXISTS "calendar_notifications_event_id" ON "calendar_notifications" ("event_id");



