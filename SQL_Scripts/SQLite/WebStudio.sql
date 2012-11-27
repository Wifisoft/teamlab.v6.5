-- account_links
CREATE TABLE IF NOT EXISTS "account_links" (
  "id" varchar(200) NOT NULL COLLATE NOCASE,
  "uid" varchar(200) NOT NULL COLLATE NOCASE,
  "provider" char(32) DEFAULT NULL COLLATE NOCASE,
  "profile" text NOT NULL COLLATE NOCASE,
  "linked" datetime NOT NULL,
  PRIMARY KEY ("id","uid")
);
CREATE INDEX IF NOT EXISTS "account_links_uid" ON "account_links" ("uid");


-- webstudio_fckuploads
CREATE TABLE IF NOT EXISTS "webstudio_fckuploads" (
  "TenantID" INTEGER NOT NULL,
  "StoreDomain" varchar(50) NOT NULL COLLATE NOCASE,
  "FolderID" varchar(100) NOT NULL COLLATE NOCASE,
  "ItemID" varchar(100) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("TenantID","StoreDomain","FolderID","ItemID")
);


-- webstudio_settings
CREATE TABLE IF NOT EXISTS "webstudio_settings" (
  "TenantID" INTEGER NOT NULL,
  "ID" varchar(64) NOT NULL COLLATE NOCASE,
  "UserID" varchar(64) NOT NULL COLLATE NOCASE,
  "Data" TEXT NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("TenantID","ID","UserID")
);


-- webstudio_user_birthday
CREATE TABLE IF NOT EXISTS "webstudio_user_birthday" (
  "tenant_id" INTEGER NOT NULL,
  "subscriber_id" char(38) NOT NULL COLLATE NOCASE,
  "target_user_id" char(38) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("tenant_id","subscriber_id","target_user_id")
);


-- webstudio_useractivity
CREATE TABLE IF NOT EXISTS "webstudio_useractivity" (
  "ID" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "ProductID" char(38) NOT NULL COLLATE NOCASE,
  "ModuleID" char(38) NOT NULL COLLATE NOCASE,
  "UserID" char(38) NOT NULL COLLATE NOCASE,
  "ContentID" varchar(256) NOT NULL COLLATE NOCASE,
  "Title" varchar(4000) NOT NULL COLLATE NOCASE,
  "URL" varchar(4000) NOT NULL COLLATE NOCASE,
  "BusinessValue" INTEGER NOT NULL DEFAULT 0,
  "ActionType" INTEGER NOT NULL,
  "ActionText" varchar(256) NOT NULL COLLATE NOCASE,
  "ActivityDate" datetime NOT NULL,
  "ImageFileName" varchar(1024) DEFAULT NULL COLLATE NOCASE,
  "PartID" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "ContainerID" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "AdditionalData" varchar(256) DEFAULT NULL COLLATE NOCASE,
  "TenantID" INTEGER NOT NULL,
  "HtmlPreview" TEXT COLLATE NOCASE,
  "SecurityId" varchar(255) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "webstudio_useractivity_UserID" ON "webstudio_useractivity" ("UserID");
CREATE INDEX IF NOT EXISTS "webstudio_useractivity_actiontype" ON "webstudio_useractivity" ("TenantID","ActionType","ProductID");
CREATE INDEX IF NOT EXISTS "webstudio_useractivity_ProductID" ON "webstudio_useractivity" ("TenantID","ProductID","ModuleID");
CREATE INDEX IF NOT EXISTS "webstudio_useractivity_containerid" ON "webstudio_useractivity" ("ContainerID");
CREATE INDEX IF NOT EXISTS "webstudio_useractivity_ActivityDate" ON "webstudio_useractivity" ("ActivityDate");


-- webstudio_uservisit
CREATE TABLE IF NOT EXISTS "webstudio_uservisit" (
  "tenantid" INTEGER NOT NULL,
  "visitdate" datetime NOT NULL,
  "productid" varchar(38) NOT NULL COLLATE NOCASE,
  "userid" varchar(38) NOT NULL COLLATE NOCASE,
  "visitcount" INTEGER NOT NULL DEFAULT 0,
  "firstvisittime" datetime DEFAULT NULL,
  "lastvisittime" datetime DEFAULT NULL,
  PRIMARY KEY ("tenantid","visitdate","productid","userid")
);
CREATE INDEX IF NOT EXISTS "webstudio_uservisit_visitdate" ON "webstudio_uservisit" ("visitdate");


-- webstudio_widgetcontainer
CREATE TABLE IF NOT EXISTS "webstudio_widgetcontainer" (
  "ID" varchar(64) NOT NULL COLLATE NOCASE,
  "TenantID" INTEGER NOT NULL,
  "ContainerID" varchar(64) NOT NULL COLLATE NOCASE,
  "UserID" varchar(64) NOT NULL COLLATE NOCASE,
  "SchemaID" INTEGER NOT NULL,
  PRIMARY KEY ("ID")
);
CREATE INDEX IF NOT EXISTS "webstudio_widgetcontainer_ContainerID" ON "webstudio_widgetcontainer" ("TenantID","ContainerID");


-- webstudio_widgetstate
CREATE TABLE IF NOT EXISTS "webstudio_widgetstate" (
  "WidgetID" varchar(64) NOT NULL COLLATE NOCASE,
  "WidgetContainerID" varchar(64) NOT NULL COLLATE NOCASE,
  "ColumnID" INTEGER NOT NULL,
  "SortOrderInColumn" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("WidgetContainerID","WidgetID")
);



