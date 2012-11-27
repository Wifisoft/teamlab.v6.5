ALTER TABLE "calendar_calendar_user" RENAME TO "calendar_calendar_user_old";
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
DROP INDEX IF EXISTS "calendar_calendar_user_user_id";
CREATE INDEX IF NOT EXISTS "calendar_calendar_user_user_id" ON "calendar_calendar_user" ("user_id");
INSERT INTO "calendar_calendar_user" SELECT * FROM "calendar_calendar_user_old";
DROP TABLE IF EXISTS "calendar_calendar_user_old";

CREATE INDEX IF NOT EXISTS "calendar_notifications_event_id" ON "calendar_notifications"("event_id");