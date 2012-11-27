ALTER TABLE "crm_contact" ADD "display_name" varchar(255) DEFAULT NULL COLLATE NOCASE;
CREATE INDEX IF NOT EXISTS "crm_contact_display_name" ON "crm_contact" ("tenant_id","display_name");
CREATE INDEX IF NOT EXISTS "crm_contact_last_modifed_on" ON "crm_contact" ("last_modifed_on");

DROP INDEX IF EXISTS "crm_contact_info_IX_Contact";
CREATE INDEX IF NOT EXISTS "crm_contact_info_IX_Contact" ON "crm_contact_info"("tenant_id","contact_id");

ALTER TABLE "crm_field_description" RENAME TO "crm_field_description_old";
CREATE TABLE IF NOT EXISTS "crm_field_description" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "tenant_id" INTEGER NOT NULL,
  "label" varchar(255) NOT NULL COLLATE NOCASE,
  "type" INTEGER NOT NULL,
  "sort_order" INTEGER NOT NULL DEFAULT 0,
  "mask" text COLLATE NOCASE,
  "entity_type" INTEGER NOT NULL
);
DROP INDEX IF EXISTS "crm_field_description_entity_type";
CREATE INDEX IF NOT EXISTS "crm_field_description_entity_type" ON "crm_field_description" ("tenant_id","entity_type","sort_order");
INSERT INTO "crm_field_description" SELECT * FROM "crm_field_description_old";
DROP TABLE IF EXISTS "crm_field_description_old";

ALTER TABLE "crm_field_value" RENAME TO "crm_field_value_old";
CREATE TABLE IF NOT EXISTS "crm_field_value" (
  "value" varchar(255) NOT NULL COLLATE NOCASE,
  "entity_id" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "field_id" INTEGER NOT NULL,
  "entity_type" INTEGER NOT NULL,
  "last_modifed_on" datetime DEFAULT NULL,
  "last_modifed_by" char(38) DEFAULT NULL COLLATE NOCASE,
  PRIMARY KEY ("tenant_id","entity_id","entity_type","field_id")
);
DROP INDEX IF EXISTS "crm_field_value_field_id";
CREATE INDEX IF NOT EXISTS "crm_field_value_field_id" ON "crm_field_value" ("field_id");
INSERT INTO "crm_field_value" SELECT * FROM "crm_field_value_old";
DROP TABLE IF EXISTS "crm_field_value_old";

CREATE TABLE IF NOT EXISTS "crm_projects" (
  "project_id" INTEGER NOT NULL,
  "contact_id" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  PRIMARY KEY ("tenant_id","contact_id","project_id")
);
DROP INDEX IF EXISTS "crm_projects_project_id";
DROP INDEX IF EXISTS "crm_projects_contact_id";
CREATE INDEX IF NOT EXISTS "crm_projects_project_id" ON "crm_projects" ("tenant_id","project_id");
CREATE INDEX IF NOT EXISTS "crm_projects_contact_id" ON "crm_projects" ("tenant_id","contact_id");

DROP INDEX IF EXISTS "crm_task_IX_Contact";
CREATE INDEX IF NOT EXISTS "crm_task_IX_Contact" ON "crm_task"("tenant_id","contact_id");
DROP INDEX IF EXISTS "crm_task_IX_Entity"; 
CREATE INDEX IF NOT EXISTS "crm_task_IX_Entity" ON "crm_task"("tenant_id","entity_id","entity_type");

CREATE TABLE IF NOT EXISTS "crm_task_template" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "create_on" datetime NOT NULL,
  "create_by" char(38) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000' COLLATE NOCASE,
  "last_modifed_on" datetime NOT NULL,
  "last_modifed_by" char(38) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000' COLLATE NOCASE,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "category_id" INTEGER NOT NULL,
  "description" tinytext COLLATE NOCASE,
  "responsible_id" char(38) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000' COLLATE NOCASE,
  "is_notify" INTEGER NOT NULL,
  "offset" INTEGER NOT NULL DEFAULT 0,
  "sort_order" INTEGER NOT NULL DEFAULT 0,
  "deadLine_is_fixed" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "container_id" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "crm_task_template_template_id" ON "crm_task_template" ("tenant_id","container_id");

CREATE TABLE IF NOT EXISTS "crm_task_template_container" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(256) NOT NULL COLLATE NOCASE,
  "entity_type" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "create_on" datetime NOT NULL,
  "create_by" char(38) NOT NULL DEFAULT 0 COLLATE NOCASE,
  "last_modifed_on" datetime NOT NULL,
  "last_modifed_by" char(38) NOT NULL DEFAULT 0 COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "crm_task_template_container_entity_type" ON "crm_task_template_container" ("tenant_id","entity_type");

CREATE TABLE IF NOT EXISTS "crm_task_template_task" (
  "task_id" INTEGER NOT NULL,
  "task_template_id" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "crm_task_template_task_task_id" ON "crm_task_template_task" ("task_id");

DROP TABLE IF EXISTS "sm_facebookaccounts";
DROP TABLE IF EXISTS "sm_linkedinaccounts";
DROP TABLE IF EXISTS "sm_twitteraccounts";