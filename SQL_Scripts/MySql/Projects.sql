-- projects_comments
CREATE TABLE IF NOT EXISTS `projects_comments` (
  `id` char(38) NOT NULL,
  `content` text,
  `inactive` tinyint(1) NOT NULL DEFAULT '0',
  `create_by` char(38) NOT NULL,
  `create_on` datetime NOT NULL,
  `parent_id` char(38) DEFAULT NULL,
  `tenant_id` int(11) NOT NULL,
  `target_uniq_id` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `target_uniq_id` (`tenant_id`,`target_uniq_id`)
);
-- projects_events
CREATE TABLE IF NOT EXISTS `projects_events` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) DEFAULT NULL,
  `create_by` char(38) NOT NULL,
  `create_on` datetime NOT NULL,
  `last_modified_on` datetime DEFAULT NULL,
  `last_modified_by` char(38) DEFAULT NULL,
  `from_date` datetime NOT NULL,
  `to_date` datetime NOT NULL,
  `project_id` int(11) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `project_id` (`tenant_id`,`project_id`)
);
-- projects_following_project_participant
CREATE TABLE IF NOT EXISTS `projects_following_project_participant` (
  `project_id` int(11) NOT NULL,
  `participant_id` char(38) NOT NULL,
  PRIMARY KEY (`participant_id`,`project_id`),
  KEY `project_id` (`project_id`)
);
-- projects_issues
CREATE TABLE IF NOT EXISTS `projects_issues` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `issue_id` varchar(64) NOT NULL,
  `project_id` int(11) NOT NULL,
  `title` varchar(255) NOT NULL,
  `description` text,
  `create_on` datetime NOT NULL,
  `create_by` varchar(38) NOT NULL,
  `last_modified_on` datetime DEFAULT NULL,
  `last_modified_by` varchar(40) DEFAULT NULL,
  `detected_in_version` varchar(64) NOT NULL,
  `corrected_in_version` varchar(64) DEFAULT NULL,
  `priority` int(11) NOT NULL,
  `assigned_on` varchar(38) NOT NULL,
  `status` int(11) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `project_id` (`tenant_id`,`project_id`)
);
-- projects_messages
CREATE TABLE IF NOT EXISTS `projects_messages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) DEFAULT NULL,
  `create_by` char(38) NOT NULL,
  `create_on` datetime NOT NULL,
  `last_modified_on` datetime DEFAULT NULL,
  `last_modified_by` char(38) DEFAULT NULL,
  `content` mediumtext,
  `project_id` int(11) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`),
  KEY `project_id` (`project_id`)
);
-- projects_milestones
CREATE TABLE IF NOT EXISTS `projects_milestones` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) DEFAULT NULL,
  `description` text,
  `deadline` datetime NOT NULL,
  `responsible_id` char(38) DEFAULT NULL,
  `status` int(11) NOT NULL,
  `status_changed` datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  `project_id` int(11) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  `is_notify` tinyint(1) NOT NULL DEFAULT '0',
  `is_key` tinyint(1) DEFAULT '0',
  `create_by` char(38) DEFAULT NULL,
  `create_on` datetime DEFAULT NULL,
  `last_modified_by` char(38) DEFAULT NULL,
  `last_modified_on` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`),
  KEY `project_id` (`project_id`)
);
-- projects_project_change_request
CREATE TABLE IF NOT EXISTS `projects_project_change_request` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_status` int(11) DEFAULT NULL,
  `is_edit_request` int(11) DEFAULT NULL,
  `project_id` int(11) DEFAULT NULL,
  `template_id` int(11) NOT NULL DEFAULT '0',
  `title` varchar(255) DEFAULT NULL,
  `description` text,
  `private` int(10) NOT NULL DEFAULT '0',
  `responsible_id` char(38) NOT NULL,
  `create_by` char(38) NOT NULL,
  `create_on` datetime DEFAULT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`)
);
-- projects_project_participant
CREATE TABLE IF NOT EXISTS `projects_project_participant` (
  `project_id` int(11) NOT NULL,
  `participant_id` char(38) NOT NULL,
  `security` int(10) NOT NULL DEFAULT '0',
  `created` timestamp NOT NULL DEFAULT '2000-01-01 00:00:00',
  `updated` timestamp NOT NULL DEFAULT '2000-01-01 00:00:00',
  `removed` int(10) NOT NULL DEFAULT '0',
  `tenant` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`project_id`,`participant_id`),
  KEY `participant_id` (`participant_id`)
);
-- projects_project_tag
CREATE TABLE IF NOT EXISTS `projects_project_tag` (
  `tag_id` int(11) NOT NULL,
  `project_id` int(11) NOT NULL,
  PRIMARY KEY (`project_id`,`tag_id`)
);
-- projects_project_tag_change_request
CREATE TABLE IF NOT EXISTS `projects_project_tag_change_request` (
  `tag_id` int(11) NOT NULL,
  `project_id` int(11) NOT NULL,
  PRIMARY KEY (`project_id`,`tag_id`)
);
-- projects_projects
CREATE TABLE IF NOT EXISTS `projects_projects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `status` int(11) NOT NULL,
  `status_changed` datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  `title` varchar(255) DEFAULT NULL,
  `description` text,
  `responsible_id` char(38) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  `private` int(10) NOT NULL DEFAULT '0',
  `create_on` datetime DEFAULT NULL,
  `create_by` char(38) DEFAULT NULL,
  `last_modified_on` datetime DEFAULT NULL,
  `last_modified_by` char(38) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `responsible_id` (`responsible_id`),
  KEY `tenant_id` (`tenant_id`)
);
-- projects_report_template
CREATE TABLE IF NOT EXISTS `projects_report_template` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` int(11) NOT NULL,
  `name` varchar(1024) NOT NULL,
  `filter` text,
  `cron` varchar(255) DEFAULT NULL,
  `create_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `create_by` varchar(38) DEFAULT NULL,
  `tenant_id` int(10) NOT NULL,
  `auto` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`)
);
-- projects_review_entity_info
CREATE TABLE IF NOT EXISTS `projects_review_entity_info` (
  `user_id` varchar(40) NOT NULL,
  `entity_review` datetime DEFAULT NULL,
  `entity_uniqID` varchar(255) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`user_id`,`entity_uniqID`),
  KEY `entity_uniqID` (`tenant_id`,`entity_uniqID`)
);
-- projects_subtasks
CREATE TABLE IF NOT EXISTS `projects_subtasks` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(255) NOT NULL,
  `responsible_id` char(38) NOT NULL,
  `task_id` int(11) NOT NULL,
  `status` int(11) NOT NULL,
  `status_changed` datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  `tenant_id` int(11) NOT NULL,
  `create_by` char(38) DEFAULT NULL,
  `create_on` datetime DEFAULT NULL,
  `last_modified_by` char(38) DEFAULT NULL,
  `last_modified_on` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `responsible_id` (`responsible_id`),
  KEY `task_id` (`tenant_id`,`task_id`)
);
-- projects_tags
CREATE TABLE IF NOT EXISTS `projects_tags` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) DEFAULT NULL,
  `tenant_id` int(11) DEFAULT NULL,
  `create_on` datetime DEFAULT NULL,
  `create_by` char(38) DEFAULT NULL,
  `last_modified_on` datetime DEFAULT NULL,
  `last_modified_by` char(38) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`)
);
-- projects_tasks
CREATE TABLE IF NOT EXISTS `projects_tasks` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) DEFAULT NULL,
  `description` text,
  `responsible_id` char(38) NOT NULL,
  `priority` int(11) NOT NULL,
  `status` int(11) NOT NULL,
  `status_changed` datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  `project_id` int(11) NOT NULL,
  `milestone_id` int(11) DEFAULT NULL,
  `tenant_id` int(11) NOT NULL,
  `sort_order` int(11) NOT NULL DEFAULT '0',
  `deadline` datetime DEFAULT NULL,
  `create_by` char(38) NOT NULL,
  `create_on` datetime DEFAULT NULL,
  `last_modified_by` char(38) DEFAULT NULL,
  `last_modified_on` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`),
  KEY `responsible_id` (`responsible_id`),
  KEY `project_id` (`project_id`),
  KEY `milestone_id` (`milestone_id`),
  KEY `deadline` (`deadline`)
);
-- projects_tasks_dependence
CREATE TABLE IF NOT EXISTS `projects_tasks_dependence` (
  `task_id` int(10) NOT NULL DEFAULT '0',
  `parent_id` int(10) NOT NULL DEFAULT '0',
  `tenant_id` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`task_id`,`parent_id`,`tenant_id`)
);
-- projects_tasks_recurrence
CREATE TABLE IF NOT EXISTS `projects_tasks_recurrence` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `task_id` int(11) NOT NULL,
  `cron` varchar(255) DEFAULT NULL,
  `start_date` datetime NOT NULL,
  `end_date` datetime NOT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `task_id` (`tenant_id`,`task_id`)
);
-- projects_tasks_responsible
CREATE TABLE IF NOT EXISTS `projects_tasks_responsible` (
  `tenant_id` int(11) NOT NULL,
  `task_id` int(11) NOT NULL,
  `responsible_id` char(38) NOT NULL,
  PRIMARY KEY (`tenant_id`,`task_id`,`responsible_id`)
);
-- projects_tasks_trace
CREATE TABLE IF NOT EXISTS `projects_tasks_trace` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `task_id` int(11) NOT NULL,
  `action_date` datetime NOT NULL,
  `action_owner_id` char(38) NOT NULL,
  `status` int(11) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `action_owner_id` (`action_owner_id`),
  KEY `task_id` (`tenant_id`,`task_id`)
);
-- projects_template_message
CREATE TABLE IF NOT EXISTS `projects_template_message` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `project_id` int(11) NOT NULL,
  `text` mediumtext,
  `create_by` char(38) NOT NULL,
  `create_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_Project` (`tenant_id`,`project_id`)
);
-- projects_template_milestone
CREATE TABLE IF NOT EXISTS `projects_template_milestone` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `project_id` int(11) NOT NULL,
  `duration` int(11) NOT NULL,
  `flags` int(11) NOT NULL DEFAULT '0',
  `create_by` char(38) NOT NULL,
  `create_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_Project` (`tenant_id`,`project_id`)
);
-- projects_template_project
CREATE TABLE IF NOT EXISTS `projects_template_project` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `description` text,
  `responsible` char(38) DEFAULT NULL,
  `tags` varchar(1024) DEFAULT NULL,
  `team` text,
  `create_by` char(38) NOT NULL,
  `create_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`)
);
-- projects_template_task
CREATE TABLE IF NOT EXISTS `projects_template_task` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `project_id` int(11) NOT NULL,
  `description` text,
  `milestone_id` int(11) NOT NULL DEFAULT '0',
  `priority` int(11) NOT NULL DEFAULT '0',
  `sort_order` int(11) NOT NULL DEFAULT '0',
  `responsible` char(38) DEFAULT NULL,
  `create_by` char(38) NOT NULL,
  `create_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_Project` (`tenant_id`,`project_id`)
);
-- projects_templates
CREATE TABLE IF NOT EXISTS `projects_templates` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) DEFAULT NULL,
  `description` text,
  `create_by` char(38) NOT NULL,
  `last_modified_on` datetime DEFAULT NULL,
  `last_modified_by` char(38) DEFAULT NULL,
  `create_on` datetime DEFAULT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`)
);
-- projects_time_tracking
CREATE TABLE IF NOT EXISTS `projects_time_tracking` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `note` varchar(255) DEFAULT NULL,
  `date` datetime NOT NULL,
  `hours` float DEFAULT '0',
  `tenant_id` int(11) NOT NULL,
  `relative_task_id` int(11) DEFAULT NULL,
  `person_id` char(38) NOT NULL,
  `project_id` int(11) NOT NULL,
  `create_on` datetime DEFAULT NULL,
  `create_by` char(38) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `person_id` (`person_id`),
  KEY `project_id` (`project_id`),
  KEY `relative_task_id` (`tenant_id`,`relative_task_id`)
);

