CREATE TABLE IF NOT EXISTS "files_converts" (
  "input" varchar(50) NOT NULL COLLATE NOCASE,
  "output" varchar(50) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("input","output")
);


ALTER TABLE "files_security" RENAME TO "files_security_old";
CREATE TABLE IF NOT EXISTS "files_security" (
  "tenant_id" INTEGER NOT NULL,
  "entry_id" varchar(32) NOT NULL COLLATE NOCASE,
  "entry_type" INTEGER NOT NULL,
  "subject" char(38) NOT NULL COLLATE NOCASE,
  "owner" char(38) NOT NULL COLLATE NOCASE,
  "security" INTEGER NOT NULL,
  "timestamp" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY ("tenant_id","entry_id","entry_type","subject")
);

INSERT INTO "files_security" ("tenant_id","entry_id","entry_type","subject","owner","security") SELECT * FROM "files_security_old";

DROP TABLE IF EXISTS "files_security_old";


ALTER TABLE "files_tag_link" RENAME TO "files_tag_link_old";
CREATE TABLE IF NOT EXISTS "files_tag_link" (
  "tenant_id" INTEGER NOT NULL,
  "tag_id" INTEGER NOT NULL,
  "entry_type" INTEGER NOT NULL,
  "entry_id" varchar(32) NOT NULL COLLATE NOCASE,
  "create_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "create_on" datetime DEFAULT NULL,
  PRIMARY KEY ("tenant_id","tag_id","entry_id","entry_type")
);

INSERT INTO "files_tag_link" ("tenant_id","tag_id","entry_id","entry_type") SELECT * FROM "files_tag_link_old";

DROP TABLE IF EXISTS "files_tag_link_old";


CREATE TABLE IF NOT EXISTS "files_thirdparty_account" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "provider" varchar(50) NOT NULL DEFAULT 0 COLLATE NOCASE,
  "customer_title" varchar(400) NOT NULL COLLATE NOCASE,
  "user_name" varchar(100) NOT NULL COLLATE NOCASE,
  "password" varchar(100) NOT NULL COLLATE NOCASE,
  "token" text COLLATE NOCASE,
  "user_id" varchar(38) NOT NULL COLLATE NOCASE,
  "folder_type" INTEGER NOT NULL DEFAULT 0,
  "create_on" datetime NOT NULL,
  "tenant_id" INTEGER NOT NULL
);


CREATE TABLE IF NOT EXISTS "files_thirdparty_id_mapping" (
  "hash_id" char(32) NOT NULL COLLATE NOCASE,
  "id" text NOT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  PRIMARY KEY ("hash_id")
);
CREATE INDEX IF NOT EXISTS "files_thirdparty_id_mapping_index_1" ON "files_thirdparty_id_mapping" ("tenant_id","hash_id");

