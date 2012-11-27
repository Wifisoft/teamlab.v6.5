DROP TABLE IF EXISTS "webstudio_quicklinks";

ALTER TABLE "webstudio_settings" RENAME TO "webstudio_settings_old";
CREATE TABLE IF NOT EXISTS "webstudio_settings" (
  "TenantID" INTEGER NOT NULL,
  "ID" varchar(64) NOT NULL COLLATE NOCASE,
  "UserID" varchar(64) NOT NULL COLLATE NOCASE,
  "Data" TEXT NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("TenantID","ID","UserID")
);
INSERT INTO "webstudio_settings" SELECT * FROM "webstudio_settings_old";
DROP TABLE IF EXISTS "webstudio_settings_old";

CREATE TABLE IF NOT EXISTS "webstudio_user_birthday" (
  "tenant_id" INTEGER NOT NULL,
  "subscriber_id" char(38) NOT NULL COLLATE NOCASE,
  "target_user_id" char(38) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("tenant_id","subscriber_id","target_user_id")
);


