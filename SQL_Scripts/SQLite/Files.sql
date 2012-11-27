-- files_bunch_objects
CREATE TABLE IF NOT EXISTS "files_bunch_objects" (
  "tenant_id" INTEGER NOT NULL,
  "right_node" varchar(255) NOT NULL COLLATE NOCASE,
  "left_node" varchar(255) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("tenant_id","right_node")
);
CREATE INDEX IF NOT EXISTS "files_bunch_objects_left_node" ON "files_bunch_objects" ("left_node");


-- files_converts
CREATE TABLE IF NOT EXISTS "files_converts" (
  "input" varchar(50) NOT NULL COLLATE NOCASE,
  "output" varchar(50) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("input","output")
);


-- files_file
CREATE TABLE IF NOT EXISTS "files_file" (
  "id" INTEGER NOT NULL,
  "version" INTEGER NOT NULL,
  "current_version" INTEGER NOT NULL DEFAULT 0,
  "folder_id" INTEGER NOT NULL DEFAULT 0,
  "title" varchar(400) NOT NULL COLLATE NOCASE,
  "content_type" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "content_length" INTEGER NOT NULL DEFAULT 0,
  "file_status" INTEGER NOT NULL DEFAULT 0,
  "category" INTEGER NOT NULL DEFAULT 0,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "modified_by" char(38) NOT NULL COLLATE NOCASE,
  "modified_on" datetime NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "converted_type" varchar(10) DEFAULT NULL COLLATE NOCASE,
  PRIMARY KEY ("tenant_id","id","version")
);
CREATE INDEX IF NOT EXISTS "files_file_modified_on" ON "files_file" ("modified_on");
CREATE INDEX IF NOT EXISTS "files_file_folder_id" ON "files_file" ("folder_id");
CREATE INDEX IF NOT EXISTS "files_file_id" ON "files_file" ("id");


-- files_folder
CREATE TABLE IF NOT EXISTS "files_folder" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "parent_id" INTEGER NOT NULL DEFAULT 0,
  "title" varchar(400) NOT NULL COLLATE NOCASE,
  "folder_type" INTEGER NOT NULL DEFAULT 0,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "modified_by" char(38) NOT NULL COLLATE NOCASE,
  "modified_on" datetime NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "foldersCount" INTEGER NOT NULL DEFAULT 0,
  "filesCount" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "files_folder_parent_id" ON "files_folder" ("tenant_id","parent_id");


-- files_folder_tree
CREATE TABLE IF NOT EXISTS "files_folder_tree" (
  "folder_id" INTEGER NOT NULL,
  "parent_id" INTEGER NOT NULL,
  "level" INTEGER NOT NULL,
  PRIMARY KEY ("parent_id","folder_id")
);
CREATE INDEX IF NOT EXISTS "files_folder_tree_folder_id" ON "files_folder_tree" ("folder_id");


-- files_security
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


-- files_tag
CREATE TABLE IF NOT EXISTS "files_tag" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "name" varchar(255) NOT NULL COLLATE NOCASE,
  "owner" varchar(38) NOT NULL COLLATE NOCASE,
  "flag" INTEGER NOT NULL DEFAULT 0,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "files_tag_name" ON "files_tag" ("tenant_id","owner","name","flag");


-- files_tag_link
CREATE TABLE IF NOT EXISTS "files_tag_link" (
  "tenant_id" INTEGER NOT NULL,
  "tag_id" INTEGER NOT NULL,
  "entry_type" INTEGER NOT NULL,
  "entry_id" varchar(32) NOT NULL COLLATE NOCASE,
  "create_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "create_on" datetime DEFAULT NULL,
  PRIMARY KEY ("tenant_id","tag_id","entry_id","entry_type")
);


-- files_thirdparty_account
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


-- files_thirdparty_id_mapping
CREATE TABLE IF NOT EXISTS "files_thirdparty_id_mapping" (
  "hash_id" char(32) NOT NULL COLLATE NOCASE,
  "id" text NOT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  PRIMARY KEY ("hash_id")
);
CREATE INDEX IF NOT EXISTS "files_thirdparty_id_mapping_index_1" ON "files_thirdparty_id_mapping" ("tenant_id","hash_id");



