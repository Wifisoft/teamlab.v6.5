-- projects_comments
CREATE TABLE IF NOT EXISTS "projects_comments" (
  "id" char(38) NOT NULL COLLATE NOCASE,
  "content" text COLLATE NOCASE,
  "inactive" INTEGER NOT NULL DEFAULT 0,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "parent_id" char(38) DEFAULT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  "target_uniq_id" varchar(50) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("id")
);
CREATE INDEX IF NOT EXISTS "projects_comments_target_uniq_id" ON "projects_comments" ("tenant_id","target_uniq_id");


-- projects_events
CREATE TABLE IF NOT EXISTS "projects_events" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "last_modified_on" datetime DEFAULT NULL,
  "last_modified_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "from_date" datetime NOT NULL,
  "to_date" datetime NOT NULL,
  "project_id" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_events_project_id" ON "projects_events" ("tenant_id","project_id");


-- projects_following_project_participant
CREATE TABLE IF NOT EXISTS "projects_following_project_participant" (
  "project_id" INTEGER NOT NULL,
  "participant_id" char(38) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("participant_id","project_id")
);
CREATE INDEX IF NOT EXISTS "projects_following_project_participant_project_id" ON "projects_following_project_participant" ("project_id");


-- projects_issues
CREATE TABLE IF NOT EXISTS "projects_issues" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "issue_id" varchar(64) NOT NULL COLLATE NOCASE,
  "project_id" INTEGER NOT NULL,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "create_by" varchar(38) NOT NULL COLLATE NOCASE,
  "last_modified_on" datetime DEFAULT NULL,
  "last_modified_by" varchar(40) DEFAULT NULL COLLATE NOCASE,
  "detected_in_version" varchar(64) NOT NULL COLLATE NOCASE,
  "corrected_in_version" varchar(64) DEFAULT NULL COLLATE NOCASE,
  "priority" INTEGER NOT NULL,
  "assigned_on" varchar(38) NOT NULL COLLATE NOCASE,
  "status" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_issues_project_id" ON "projects_issues" ("tenant_id","project_id");


-- projects_messages
CREATE TABLE IF NOT EXISTS "projects_messages" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime NOT NULL,
  "last_modified_on" datetime DEFAULT NULL,
  "last_modified_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "content" TEXT COLLATE NOCASE,
  "project_id" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_messages_tenant_id" ON "projects_messages" ("tenant_id");
CREATE INDEX IF NOT EXISTS "projects_messages_project_id" ON "projects_messages" ("project_id");


-- projects_milestones
CREATE TABLE IF NOT EXISTS "projects_milestones" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "deadline" datetime NOT NULL,
  "responsible_id" char(38) DEFAULT NULL COLLATE NOCASE,
  "status" INTEGER NOT NULL,
  "status_changed" datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  "project_id" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL,
  "is_notify" INTEGER NOT NULL DEFAULT 0,
  "is_key" INTEGER DEFAULT 0,
  "create_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "create_on" datetime DEFAULT NULL,
  "last_modified_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "last_modified_on" datetime DEFAULT NULL
);
CREATE INDEX IF NOT EXISTS "projects_milestones_tenant_id" ON "projects_milestones" ("tenant_id");
CREATE INDEX IF NOT EXISTS "projects_milestones_project_id" ON "projects_milestones" ("project_id");


-- projects_project_change_request
CREATE TABLE IF NOT EXISTS "projects_project_change_request" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "project_status" INTEGER DEFAULT NULL,
  "is_edit_request" INTEGER DEFAULT NULL,
  "project_id" INTEGER DEFAULT NULL,
  "template_id" INTEGER NOT NULL DEFAULT 0,
  "title" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "private" INTEGER NOT NULL DEFAULT 0,
  "responsible_id" char(38) NOT NULL COLLATE NOCASE,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime DEFAULT NULL,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_project_change_request_tenant_id" ON "projects_project_change_request" ("tenant_id");


-- projects_project_participant
CREATE TABLE IF NOT EXISTS "projects_project_participant" (
  "project_id" INTEGER NOT NULL,
  "participant_id" char(38) NOT NULL COLLATE NOCASE,
  "security" INTEGER NOT NULL DEFAULT 0,
  "created" timestamp NOT NULL DEFAULT '2000-01-01 00:00:00',
  "updated" timestamp NOT NULL DEFAULT '2000-01-01 00:00:00',
  "removed" INTEGER NOT NULL DEFAULT 0,
  "tenant" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("project_id","participant_id")
);
CREATE INDEX IF NOT EXISTS "projects_project_participant_participant_id" ON "projects_project_participant" ("participant_id");


-- projects_project_tag
CREATE TABLE IF NOT EXISTS "projects_project_tag" (
  "tag_id" INTEGER NOT NULL,
  "project_id" INTEGER NOT NULL,
  PRIMARY KEY ("project_id","tag_id")
);


-- projects_project_tag_change_request
CREATE TABLE IF NOT EXISTS "projects_project_tag_change_request" (
  "tag_id" INTEGER NOT NULL,
  "project_id" INTEGER NOT NULL,
  PRIMARY KEY ("project_id","tag_id")
);


-- projects_projects
CREATE TABLE IF NOT EXISTS "projects_projects" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "status" INTEGER NOT NULL,
  "status_changed" datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  "title" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "responsible_id" char(38) NOT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  "private" INTEGER NOT NULL DEFAULT 0,
  "create_on" datetime DEFAULT NULL,
  "create_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "last_modified_on" datetime DEFAULT NULL,
  "last_modified_by" char(38) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "projects_projects_responsible_id" ON "projects_projects" ("responsible_id");
CREATE INDEX IF NOT EXISTS "projects_projects_tenant_id" ON "projects_projects" ("tenant_id");


-- projects_report_template
CREATE TABLE IF NOT EXISTS "projects_report_template" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "type" INTEGER NOT NULL,
  "name" varchar(1024) NOT NULL COLLATE NOCASE,
  "filter" text COLLATE NOCASE,
  "cron" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "create_on" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "create_by" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  "auto" INTEGER NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS "projects_report_template_tenant_id" ON "projects_report_template" ("tenant_id");


-- projects_review_entity_info
CREATE TABLE IF NOT EXISTS "projects_review_entity_info" (
  "user_id" varchar(40) NOT NULL COLLATE NOCASE,
  "entity_review" datetime DEFAULT NULL,
  "entity_uniqID" varchar(255) NOT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  PRIMARY KEY ("user_id","entity_uniqID")
);
CREATE INDEX IF NOT EXISTS "projects_review_entity_info_entity_uniqID" ON "projects_review_entity_info" ("tenant_id","entity_uniqID");


-- projects_subtasks
CREATE TABLE IF NOT EXISTS "projects_subtasks" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "Title" varchar(255) NOT NULL COLLATE NOCASE,
  "responsible_id" char(38) NOT NULL COLLATE NOCASE,
  "task_id" INTEGER NOT NULL,
  "status" INTEGER NOT NULL,
  "status_changed" datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  "tenant_id" INTEGER NOT NULL,
  "create_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "create_on" datetime DEFAULT NULL,
  "last_modified_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "last_modified_on" datetime DEFAULT NULL
);
CREATE INDEX IF NOT EXISTS "projects_subtasks_responsible_id" ON "projects_subtasks" ("responsible_id");
CREATE INDEX IF NOT EXISTS "projects_subtasks_task_id" ON "projects_subtasks" ("tenant_id","task_id");


-- projects_tags
CREATE TABLE IF NOT EXISTS "projects_tags" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "tenant_id" INTEGER DEFAULT NULL,
  "create_on" datetime DEFAULT NULL,
  "create_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "last_modified_on" datetime DEFAULT NULL,
  "last_modified_by" char(38) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "projects_tags_tenant_id" ON "projects_tags" ("tenant_id");


-- projects_tasks
CREATE TABLE IF NOT EXISTS "projects_tasks" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "responsible_id" char(38) NOT NULL COLLATE NOCASE,
  "priority" INTEGER NOT NULL,
  "status" INTEGER NOT NULL,
  "status_changed" datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  "project_id" INTEGER NOT NULL,
  "milestone_id" INTEGER DEFAULT NULL,
  "tenant_id" INTEGER NOT NULL,
  "sort_order" INTEGER NOT NULL DEFAULT 0,
  "deadline" datetime DEFAULT NULL,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" datetime DEFAULT NULL,
  "last_modified_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "last_modified_on" datetime DEFAULT NULL
);
CREATE INDEX IF NOT EXISTS "projects_tasks_tenant_id" ON "projects_tasks" ("tenant_id");
CREATE INDEX IF NOT EXISTS "projects_tasks_responsible_id" ON "projects_tasks" ("responsible_id");
CREATE INDEX IF NOT EXISTS "projects_tasks_project_id" ON "projects_tasks" ("project_id");
CREATE INDEX IF NOT EXISTS "projects_tasks_milestone_id" ON "projects_tasks" ("milestone_id");
CREATE INDEX IF NOT EXISTS "projects_tasks_deadline" ON "projects_tasks" ("deadline");


-- projects_tasks_dependence
CREATE TABLE IF NOT EXISTS "projects_tasks_dependence" (
  "task_id" INTEGER NOT NULL DEFAULT 0,
  "parent_id" INTEGER NOT NULL DEFAULT 0,
  "tenant_id" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("task_id","parent_id","tenant_id")
);


-- projects_tasks_recurrence
CREATE TABLE IF NOT EXISTS "projects_tasks_recurrence" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "task_id" INTEGER NOT NULL,
  "cron" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "start_date" datetime NOT NULL,
  "end_date" datetime NOT NULL,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_tasks_recurrence_task_id" ON "projects_tasks_recurrence" ("tenant_id","task_id");


-- projects_tasks_responsible
CREATE TABLE IF NOT EXISTS "projects_tasks_responsible" (
  "tenant_id" INTEGER NOT NULL,
  "task_id" INTEGER NOT NULL,
  "responsible_id" char(38) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("tenant_id","task_id","responsible_id")
);


-- projects_tasks_trace
CREATE TABLE IF NOT EXISTS "projects_tasks_trace" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "task_id" INTEGER NOT NULL,
  "action_date" datetime NOT NULL,
  "action_owner_id" char(38) NOT NULL COLLATE NOCASE,
  "status" INTEGER NOT NULL,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_tasks_trace_action_owner_id" ON "projects_tasks_trace" ("action_owner_id");
CREATE INDEX IF NOT EXISTS "projects_tasks_trace_task_id" ON "projects_tasks_trace" ("tenant_id","task_id");


-- projects_template_message
CREATE TABLE IF NOT EXISTS "projects_template_message" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "project_id" INTEGER NOT NULL,
  "text" TEXT COLLATE NOCASE,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_template_message_FK_Project" ON "projects_template_message" ("tenant_id","project_id");


-- projects_template_milestone
CREATE TABLE IF NOT EXISTS "projects_template_milestone" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "project_id" INTEGER NOT NULL,
  "duration" INTEGER NOT NULL,
  "flags" INTEGER NOT NULL DEFAULT 0,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_template_milestone_FK_Project" ON "projects_template_milestone" ("tenant_id","project_id");


-- projects_template_project
CREATE TABLE IF NOT EXISTS "projects_template_project" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "responsible" char(38) DEFAULT NULL COLLATE NOCASE,
  "tags" varchar(1024) DEFAULT NULL COLLATE NOCASE,
  "team" text COLLATE NOCASE,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_template_project_tenant_id" ON "projects_template_project" ("tenant_id");


-- projects_template_task
CREATE TABLE IF NOT EXISTS "projects_template_task" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) NOT NULL COLLATE NOCASE,
  "project_id" INTEGER NOT NULL,
  "description" text COLLATE NOCASE,
  "milestone_id" INTEGER NOT NULL DEFAULT 0,
  "priority" INTEGER NOT NULL DEFAULT 0,
  "sort_order" INTEGER NOT NULL DEFAULT 0,
  "responsible" char(38) DEFAULT NULL COLLATE NOCASE,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "create_on" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_template_task_FK_Project" ON "projects_template_task" ("tenant_id","project_id");


-- projects_templates
CREATE TABLE IF NOT EXISTS "projects_templates" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "title" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "description" text COLLATE NOCASE,
  "create_by" char(38) NOT NULL COLLATE NOCASE,
  "last_modified_on" datetime DEFAULT NULL,
  "last_modified_by" char(38) DEFAULT NULL COLLATE NOCASE,
  "create_on" datetime DEFAULT NULL,
  "tenant_id" INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS "projects_templates_tenant_id" ON "projects_templates" ("tenant_id");


-- projects_time_tracking
CREATE TABLE IF NOT EXISTS "projects_time_tracking" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "note" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "date" datetime NOT NULL,
  "hours" float DEFAULT 0,
  "tenant_id" INTEGER NOT NULL,
  "relative_task_id" INTEGER DEFAULT NULL,
  "person_id" char(38) NOT NULL COLLATE NOCASE,
  "project_id" INTEGER NOT NULL,
  "create_on" datetime DEFAULT NULL,
  "create_by" char(38) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "projects_time_tracking_person_id" ON "projects_time_tracking" ("person_id");
CREATE INDEX IF NOT EXISTS "projects_time_tracking_project_id" ON "projects_time_tracking" ("project_id");
CREATE INDEX IF NOT EXISTS "projects_time_tracking_relative_task_id" ON "projects_time_tracking" ("tenant_id","relative_task_id");



