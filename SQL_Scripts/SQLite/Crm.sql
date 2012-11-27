-- crm_case
CREATE TABLE IF NOT EXISTS "crm_case" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "is_closed" INTEGER NOT NULL DEFAULT 0,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "last_modifed_on" datetime DEFAULT NULL,
  "last_modifed_by" char(38) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "crm_case_tenant_id" ON "crm_case" ("tenant_id");


-- crm_contact
CREATE TABLE IF NOT EXISTS "crm_contact" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "tenant_id" INTEGER NOT NULL,
  "is_company" INTEGER NOT NULL,
  "notes" text COLLATE NOCASE,
  "title" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "first_name" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "last_name" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "company_name" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "industry" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "status_id" INTEGER NOT NULL DEFAULT 0,
  "company_id" INTEGER NOT NULL,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "last_modifed_on" datetime DEFAULT NULL,
  "last_modifed_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "display_name" varchar(255) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "crm_contact_company_id" ON "crm_contact" ("tenant_id","company_id");
CREATE INDEX IF NOT EXISTS "crm_contact_display_name" ON "crm_contact" ("tenant_id","display_name");
CREATE INDEX IF NOT EXISTS "crm_contact_last_modifed_on" ON "crm_contact" ("last_modifed_on");


-- crm_contact_info
CREATE TABLE IF NOT EXISTS "crm_contact_info" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "data" text NOT NULL COLLATE NOCASE,
  "category" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "is_primary" INTEGER NOT NULL,
  "contact_id" INTEGER NOT NULL,
  "type" INTEGER NOT NULL,
  "last_modifed_on" datetime DEFAULT NULL,
  "last_modifed_by" char(38) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "crm_contact_info_IX_Contact" ON "crm_contact_info" ("tenant_id","contact_id");


-- crm_deal
CREATE TABLE IF NOT EXISTS "crm_deal" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "responsible_id" char(38) NOT NULL COLLATE NOCASE,
  "contact_id" INTEGER NOT NULL,
  "create_on" datetime NOT NULL,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "bid_currency" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "bid_value" decimal(50,9) NOT NULL DEFAULT '0.000000000',
  "bid_type" INTEGER NOT NULL DEFAULT 0,
  "deal_milestone_id" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "expected_close_date" datetime NOT NULL,
  "per_period_value" INTEGER NOT NULL DEFAULT 0,
  "deal_milestone_probability" INTEGER DEFAULT NULL,
  "last_modifed_on" datetime DEFAULT NULL,
  "last_modifed_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "actual_close_date" datetime DEFAULT NULL
);
CREATE INDEX IF NOT EXISTS "crm_deal_contact_id" ON "crm_deal" ("tenant_id","contact_id");


-- crm_deal_milestone
CREATE TABLE IF NOT EXISTS "crm_deal_milestone" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "color" varchar(50) NOT NULL DEFAULT 0 COLLATE NOCASE,
  "sort_order" INTEGER NOT NULL DEFAULT 0,
  "title" varchar(250) NOT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "probability" INTEGER NOT NULL DEFAULT 0,
  "status" INTEGER NOT NULL DEFAULT 0,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "crm_deal_milestone_tenant_id" ON "crm_deal_milestone" ("tenant_id");


-- crm_entity_contact
CREATE TABLE IF NOT EXISTS "crm_entity_contact" (
  "entity_id" INTEGER NOT NULL,
  "entity_type" INTEGER NOT NULL,
  "contact_id" INTEGER NOT NULL,
  PRIMARY KEY ("entity_id","entity_type","contact_id")
);
CREATE INDEX IF NOT EXISTS "crm_entity_contact_IX_Contact" ON "crm_entity_contact" ("contact_id");


-- crm_entity_tag
CREATE TABLE IF NOT EXISTS "crm_entity_tag" (
  "tag_id" INTEGER NOT NULL,
  "entity_id" INTEGER NOT NULL,
  "entity_type" INTEGER NOT NULL,
  PRIMARY KEY ("entity_id","entity_type","tag_id")
);
CREATE INDEX IF NOT EXISTS "crm_entity_tag_tag_id" ON "crm_entity_tag" ("tag_id");


-- crm_field_description
CREATE TABLE IF NOT EXISTS "crm_field_description" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "tenant_id" INTEGER NOT NULL,
  "label" varchar(255) NOT NULL COLLATE NOCASE,
  "type" INTEGER NOT NULL,
  "sort_order" INTEGER NOT NULL DEFAULT 0,
  "mask" text COLLATE NOCASE,
  "entity_type" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "crm_field_description_entity_type" ON "crm_field_description" ("tenant_id","entity_type","sort_order");


-- crm_field_value
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
CREATE INDEX IF NOT EXISTS "crm_field_value_field_id" ON "crm_field_value" ("field_id");


-- crm_list_item
CREATE TABLE IF NOT EXISTS "crm_list_item" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "sort_order" INTEGER NOT NULL DEFAULT 0,
  "color" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "additional_params" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  "list_type" INTEGER DEFAULT NULL,
  "description" varchar(255) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "crm_list_item_list_type" ON "crm_list_item" ("tenant_id","list_type");


-- crm_projects
CREATE TABLE IF NOT EXISTS "crm_projects" (
  "project_id" INTEGER NOT NULL,
  "contact_id" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  PRIMARY KEY ("tenant_id","contact_id","project_id")
);
CREATE INDEX IF NOT EXISTS "crm_projects_project_id" ON "crm_projects" ("tenant_id","project_id");
CREATE INDEX IF NOT EXISTS "crm_projects_contact_id" ON "crm_projects" ("tenant_id","contact_id");


-- crm_relationship_event
CREATE TABLE IF NOT EXISTS "crm_relationship_event" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "contact_id" INTEGER NOT NULL,
  "content" text COLLATE NOCASE,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "entity_type" INTEGER NOT NULL,
  "entity_id" INTEGER NOT NULL,
  "category_id" INTEGER NOT NULL,
  "last_modifed_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "last_modifed_on" datetime DEFAULT NULL,
  "have_files" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "crm_relationship_event_tenant_id" ON "crm_relationship_event" ("tenant_id");
CREATE INDEX IF NOT EXISTS "crm_relationship_event_IX_Contact" ON "crm_relationship_event" ("contact_id");
CREATE INDEX IF NOT EXISTS "crm_relationship_event_IX_Entity" ON "crm_relationship_event" ("entity_id","entity_type");


-- crm_tag
CREATE TABLE IF NOT EXISTS "crm_tag" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  "entity_type" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "crm_tag_tenant_id" ON "crm_tag" ("tenant_id");


-- crm_task
CREATE TABLE IF NOT EXISTS "crm_task" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "deadline" datetime NOT NULL,
  "responsible_id" char(38) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000' COLLATE NOCASE,
  "contact_id" INTEGER NOT NULL DEFAULT '-1',
  "is_closed" INTEGER NOT NULL DEFAULT 0,
  "tenant_id" INTEGER NOT NULL,
  "entity_type" INTEGER NOT NULL,
  "entity_id" INTEGER NOT NULL,
  "category_id" INTEGER NOT NULL DEFAULT 0,
  "create_on" datetime NOT NULL,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "last_modifed_on" datetime DEFAULT NULL,
  "last_modifed_by" char(38) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "crm_task_responsible_id" ON "crm_task" ("tenant_id","responsible_id");
CREATE INDEX IF NOT EXISTS "crm_task_IX_Contact" ON "crm_task" ("tenant_id","contact_id");
CREATE INDEX IF NOT EXISTS "crm_task_IX_Entity" ON "crm_task" ("tenant_id","entity_id","entity_type");


-- crm_task_template
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


-- crm_task_template_container
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


-- crm_task_template_task
CREATE TABLE IF NOT EXISTS "crm_task_template_task" (
  "task_id" INTEGER NOT NULL,
  "task_template_id" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "crm_task_template_task_task_id" ON "crm_task_template_task" ("task_id");



