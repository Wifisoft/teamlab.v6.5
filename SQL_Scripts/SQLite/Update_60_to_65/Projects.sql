ALTER TABLE "projects_milestones" RENAME TO "projects_milestones_old";
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
DROP INDEX IF EXISTS "projects_milestones_tenant_id";
DROP INDEX IF EXISTS "projects_milestones_project_id";
CREATE INDEX IF NOT EXISTS "projects_milestones_tenant_id" ON "projects_milestones" ("tenant_id");
CREATE INDEX IF NOT EXISTS "projects_milestones_project_id" ON "projects_milestones" ("project_id");

INSERT INTO "projects_milestones"("id","title","deadline","status","create_by","create_on","is_notify","last_modified_on","last_modified_by","project_id","tenant_id","is_key","description")
SELECT * FROM "projects_milestones_old";

DROP TABLE IF EXISTS "projects_milestones_old";


ALTER TABLE "projects_projects" RENAME TO "projects_projects_old";
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
DROP INDEX IF EXISTS "projects_projects_responsible_id";
DROP INDEX IF EXISTS "projects_projects_tenant_id";
CREATE INDEX IF NOT EXISTS "projects_projects_responsible_id" ON "projects_projects" ("responsible_id");
CREATE INDEX IF NOT EXISTS "projects_projects_tenant_id" ON "projects_projects" ("tenant_id");

INSERT INTO "projects_projects"("id","status","title","description","responsible_id","tenant_id","private","create_on","create_by","last_modified_on","last_modified_by")
SELECT "id","status","title","description","responsible_id","tenant_id","private","create_on","create_by","last_modified_on","last_modified_by"
FROM "projects_projects_old";

DROP TABLE IF EXISTS "projects_projects_old";


ALTER TABLE "projects_project_participant" RENAME TO "projects_project_participant_old";
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
DROP INDEX IF EXISTS "projects_project_participant_participant_id";
CREATE INDEX IF NOT EXISTS "projects_project_participant_participant_id" ON "projects_project_participant" ("participant_id");

INSERT INTO "projects_project_participant"("project_id","participant_id","security","removed","created","updated")
SELECT "project_id","participant_id","security","removed","created","updated"
FROM "projects_project_participant_old";

DROP TABLE IF EXISTS "projects_project_participant_old";


ALTER TABLE "projects_subtasks" RENAME TO "projects_subtasks_old";
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
DROP INDEX IF EXISTS "projects_subtasks_responsible_id";
DROP INDEX IF EXISTS "projects_subtasks_task_id";
CREATE INDEX IF NOT EXISTS "projects_subtasks_responsible_id" ON "projects_subtasks" ("responsible_id");
CREATE INDEX IF NOT EXISTS "projects_subtasks_task_id" ON "projects_subtasks" ("tenant_id","task_id");

INSERT INTO "projects_subtasks" ("id","Title","responsible_id","task_id","status","create_by","create_on","tenant_id","last_modified_by","last_modified_on")
SELECT "id","Title","responsible_id","task_id","status","create_by","create_on","tenant_id","last_modified_by","last_modified_on"
FROM "projects_subtasks_old";

DROP TABLE IF EXISTS "projects_subtasks_old";


ALTER TABLE "projects_tasks" RENAME TO "projects_tasks_old";
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
DROP INDEX IF EXISTS "projects_tasks_tenant_id";
DROP INDEX IF EXISTS "projects_tasks_responsible_id";
DROP INDEX IF EXISTS "projects_tasks_project_id";
DROP INDEX IF EXISTS "projects_tasks_milestone_id";
DROP INDEX IF EXISTS "projects_tasks_deadline";
CREATE INDEX IF NOT EXISTS "projects_tasks_tenant_id" ON "projects_tasks" ("tenant_id");
CREATE INDEX IF NOT EXISTS "projects_tasks_responsible_id" ON "projects_tasks" ("responsible_id");
CREATE INDEX IF NOT EXISTS "projects_tasks_project_id" ON "projects_tasks" ("project_id");
CREATE INDEX IF NOT EXISTS "projects_tasks_milestone_id" ON "projects_tasks" ("milestone_id");
CREATE INDEX IF NOT EXISTS "projects_tasks_deadline" ON "projects_tasks" ("deadline");

INSERT INTO "projects_tasks"("id","title","description","responsible_id","priority","status","create_by","last_modified_on","last_modified_by","create_on","project_id","milestone_id",
"tenant_id","sort_order","deadline")
SELECT "id","title","description","responsible_id","priority","status","create_by","last_modified_on","last_modified_by","create_on","project_id","milestone_id",
"tenant_id","sort_order","deadline"
FROM "projects_tasks_old";

DROP TABLE IF EXISTS "projects_tasks_old";


CREATE TABLE IF NOT EXISTS "projects_tasks_dependence" (
  "task_id" INTEGER NOT NULL DEFAULT 0,
  "parent_id" INTEGER NOT NULL DEFAULT 0,
  "tenant_id" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("task_id","parent_id","tenant_id")
);


ALTER TABLE "projects_tasks_recurrence" RENAME TO "projects_tasks_recurrence_old";
CREATE TABLE IF NOT EXISTS "projects_tasks_recurrence" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "task_id" INTEGER NOT NULL,
  "cron" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "start_date" datetime NOT NULL,
  "end_date" datetime NOT NULL,
  "tenant_id" INTEGER NOT NULL
);
DROP INDEX IF EXISTS "projects_tasks_recurrence_task_id";
CREATE INDEX IF NOT EXISTS "projects_tasks_recurrence_task_id" ON "projects_tasks_recurrence" ("tenant_id","task_id");

INSERT INTO "projects_tasks_recurrence" SELECT * FROM "projects_tasks_recurrence_old";

DROP TABLE IF EXISTS "projects_tasks_recurrence_old";


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


ALTER TABLE "projects_time_tracking" ADD "create_on" datetime DEFAULT NULL;
ALTER TABLE "projects_time_tracking" ADD "create_by" char(38) DEFAULT NULL COLLATE NOCASE;

insert or replace into projects_tasks_responsible (task_id, responsible_id, tenant_id) select pt.id, pt.responsible_id, pt.tenant_id from projects_tasks as pt where pt.responsible_id <> '00000000-0000-0000-0000-000000000000';
update projects_tasks set status = 1 where status = 4;
update projects_tasks set status = 1 where status = 0;
update projects_milestones set responsible_id = create_by where responsible_id = '00000000-0000-0000-0000-000000000000' or responsible_id is null;
