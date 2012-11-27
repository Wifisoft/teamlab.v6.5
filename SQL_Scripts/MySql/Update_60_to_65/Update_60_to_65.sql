ALTER TABLE `calendar_calendar_user` MODIFY `time_zone` varchar(255) DEFAULT 'UTC';
ALTER TABLE `calendar_calendar_user` ADD INDEX `user_id` (`user_id`);
ALTER TABLE `calendar_notifications` ADD INDEX `event_id` (`event_id`);
ALTER TABLE `core_user` MODIFY `username` varchar(255) NOT NULL AFTER `id`;
ALTER TABLE `core_user` MODIFY `email` varchar(255) DEFAULT NULL;
ALTER TABLE `core_user` ADD `phone` varchar(255) DEFAULT NULL AFTER `contacts`;
ALTER TABLE `core_user` ADD `phone_activation` int(11) NOT NULL DEFAULT '0' AFTER `phone`;
ALTER TABLE `core_user` MODIFY `last_modified` datetime NOT NULL;
ALTER TABLE `core_user` ADD `create_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP AFTER `removed`;
UPDATE `core_user` SET `create_on` = `last_modified`;
insert ignore into core_settings(tenant, id, value) values (-1, 'SmtpSettings', 0xF052E090A1A3750DADCD4E9961DA04AAF74059304D63A7395E32F788AA4732408A43879439C7045CA5A738039B4ED813D4978B53EEC54D2689FDF2AB9C1A71CBB3523A224DAC89FE455C368972B379590C3C0E53ECC55B1E7A026C822237D9894E9BF137D426F10E0C424F3E16F2A464);
ALTER TABLE `crm_contact` ADD `display_name` varchar(255) DEFAULT NULL AFTER `last_modifed_by`;
ALTER TABLE `crm_contact` ADD INDEX `display_name` (`tenant_id`,`display_name`);
ALTER TABLE `crm_contact` ADD INDEX `last_modifed_on` (`last_modifed_on`);
ALTER TABLE `crm_contact_info` DROP INDEX `IX_Contact`;
ALTER TABLE `crm_contact_info` ADD INDEX `IX_Contact` (`tenant_id`,`contact_id`);
ALTER TABLE `crm_field_description` MODIFY `mask` text;
ALTER TABLE `crm_field_description` DROP INDEX `entity_type`;
ALTER TABLE `crm_field_description` ADD INDEX `entity_type` (`tenant_id`,`entity_type`,`sort_order`);
ALTER TABLE `crm_field_value` DROP PRIMARY KEY;
ALTER TABLE `crm_field_value` ADD PRIMARY KEY (`tenant_id`,`entity_id`,`entity_type`,`field_id`);
ALTER TABLE `crm_field_value` ADD INDEX `field_id` (`field_id`);
CREATE TABLE IF NOT EXISTS `crm_projects` (
  `project_id` int(10) NOT NULL,
  `contact_id` int(10) NOT NULL,
  `tenant_id` int(10) NOT NULL,
  PRIMARY KEY (`tenant_id`,`contact_id`,`project_id`),
  KEY `project_id` (`tenant_id`,`project_id`),
  KEY `contact_id` (`tenant_id`,`contact_id`)
);
ALTER TABLE `crm_task` DROP INDEX `IX_Contact`;
ALTER TABLE `crm_task` ADD INDEX `IX_Contact` (`tenant_id`,`contact_id`);
ALTER TABLE `crm_task` DROP INDEX `IX_Entity`; 
ALTER TABLE `crm_task` ADD INDEX `IX_Entity` (`tenant_id`,`entity_id`,`entity_type`);
CREATE TABLE IF NOT EXISTS `crm_task_template` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `create_on` datetime NOT NULL,
  `create_by` char(38) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  `last_modifed_on` datetime NOT NULL,
  `last_modifed_by` char(38) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  `title` varchar(255) NOT NULL,
  `category_id` int(10) NOT NULL,
  `description` tinytext,
  `responsible_id` char(38) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  `is_notify` tinyint(4) NOT NULL,
  `offset` bigint(20) NOT NULL DEFAULT '0',
  `sort_order` int(11) NOT NULL DEFAULT '0',
  `deadLine_is_fixed` tinyint(4) NOT NULL,
  `tenant_id` int(10) NOT NULL,
  `container_id` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `template_id` (`tenant_id`,`container_id`)
);
CREATE TABLE IF NOT EXISTS `crm_task_template_container` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `title` varchar(256) NOT NULL,
  `entity_type` int(10) NOT NULL,
  `tenant_id` int(10) NOT NULL,
  `create_on` datetime NOT NULL,
  `create_by` char(38) NOT NULL DEFAULT '0',
  `last_modifed_on` datetime NOT NULL,
  `last_modifed_by` char(38) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `entity_type` (`tenant_id`,`entity_type`)
);
CREATE TABLE IF NOT EXISTS `crm_task_template_task` (
  `task_id` int(10) NOT NULL,
  `task_template_id` int(10) NOT NULL,
  `tenant_id` int(10) NOT NULL,
  KEY `task_id` (`task_id`)
);
CREATE TABLE IF NOT EXISTS `files_converts` (
  `input` varchar(50) NOT NULL,
  `output` varchar(50) NOT NULL,
  PRIMARY KEY (`input`,`output`)
);
ALTER TABLE `files_security` MODIFY `entry_id` varchar(32) NOT NULL;
ALTER TABLE `files_security` ADD `timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;
ALTER TABLE `files_tag_link` MODIFY `entry_id` varchar(32) NOT NULL AFTER `entry_type`;
ALTER TABLE `files_tag_link` ADD `create_by` char(38) DEFAULT NULL;
ALTER TABLE `files_tag_link` ADD `create_on` datetime DEFAULT NULL;
CREATE TABLE IF NOT EXISTS `files_thirdparty_account` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `provider` varchar(50) NOT NULL DEFAULT '0',
  `customer_title` varchar(400) NOT NULL,
  `user_name` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL,
  `token` text,
  `user_id` varchar(38) NOT NULL,
  `folder_type` int(11) NOT NULL DEFAULT '0',
  `create_on` datetime NOT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`id`)
);
CREATE TABLE IF NOT EXISTS `files_thirdparty_id_mapping` (
  `hash_id` char(32) NOT NULL,
  `id` text NOT NULL,
  `tenant_id` int(11) NOT NULL,
  PRIMARY KEY (`hash_id`),
  KEY `index_1` (`tenant_id`,`hash_id`)
);
CREATE TABLE IF NOT EXISTS `notify_info` (
  `notify_id` int(10) NOT NULL,
  `state` int(10) NOT NULL DEFAULT '0',
  `attempts` int(10) NOT NULL DEFAULT '0',
  `modify_date` datetime NOT NULL,
  PRIMARY KEY (`notify_id`),
  KEY `state` (`state`)
);
CREATE TABLE IF NOT EXISTS `notify_queue` (
  `notify_id` int(11) NOT NULL AUTO_INCREMENT,
  `tenant_id` int(11) NOT NULL,
  `sender` varchar(255) DEFAULT NULL,
  `reciever` varchar(255) DEFAULT NULL,
  `subject` varchar(1024) DEFAULT NULL,
  `content_type` varchar(64) DEFAULT NULL,
  `content` text,
  `sender_type` varchar(64) DEFAULT NULL,
  `reply_to` varchar(1024) DEFAULT NULL,
  `creation_date` datetime NOT NULL,
  PRIMARY KEY (`notify_id`)
);
ALTER TABLE `projects_milestones` MODIFY `description` text AFTER `title`;
ALTER TABLE `projects_milestones` ADD `responsible_id` char(38) DEFAULT NULL AFTER `deadline`;
ALTER TABLE `projects_milestones` ADD `status_changed` datetime NOT NULL DEFAULT '2000-01-01 00:00:00' AFTER `status`;
ALTER TABLE `projects_milestones` MODIFY `project_id` int(11) NOT NULL AFTER `status_changed`;
ALTER TABLE `projects_milestones` MODIFY `tenant_id` int(11) NOT NULL AFTER `project_id`;
ALTER TABLE `projects_milestones` MODIFY `is_notify` tinyint(1) NOT NULL DEFAULT '0' AFTER `tenant_id`;
ALTER TABLE `projects_milestones` MODIFY `is_key` tinyint(1) DEFAULT '0' AFTER `is_notify`;
ALTER TABLE `projects_milestones` MODIFY `last_modified_by` char(38) DEFAULT NULL AFTER `create_on`;
ALTER TABLE `projects_projects` ADD `status_changed` datetime NOT NULL DEFAULT '2000-01-01 00:00:00' AFTER `status`;
ALTER TABLE `projects_projects` DROP `previous_status`;
ALTER TABLE `projects_project_participant` MODIFY `removed` int(10) NOT NULL DEFAULT '0' AFTER `updated`;
ALTER TABLE `projects_project_participant` ADD `tenant` int(10) NOT NULL DEFAULT '0';
ALTER TABLE `projects_subtasks` ADD `status_changed` datetime NOT NULL DEFAULT '2000-01-01 00:00:00' AFTER `status`;
ALTER TABLE `projects_subtasks` MODIFY `tenant_id` int(11) NOT NULL AFTER `status_changed`;
ALTER TABLE `projects_subtasks` DROP `previous_status`;
ALTER TABLE `projects_tasks` ADD `status_changed` datetime NOT NULL DEFAULT '2000-01-01 00:00:00' AFTER `status`;
ALTER TABLE `projects_tasks` MODIFY `create_by` char(38) NOT NULL AFTER `deadline`;
ALTER TABLE `projects_tasks` MODIFY `create_on` datetime DEFAULT NULL AFTER `create_by`;
ALTER TABLE `projects_tasks` MODIFY `last_modified_by` char(38) DEFAULT NULL AFTER `create_on`;
ALTER TABLE `projects_tasks` MODIFY `last_modified_on` datetime DEFAULT NULL AFTER `last_modified_by`;
ALTER TABLE `projects_tasks` DROP `previous_status`;
CREATE TABLE IF NOT EXISTS `projects_tasks_dependence` (
  `task_id` int(10) NOT NULL DEFAULT '0',
  `parent_id` int(10) NOT NULL DEFAULT '0',
  `tenant_id` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`task_id`,`parent_id`,`tenant_id`)
);
ALTER TABLE `projects_tasks_recurrence` MODIFY `cron` varchar(255) DEFAULT NULL;
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
ALTER TABLE `projects_time_tracking` ADD `create_on` datetime DEFAULT NULL;
ALTER TABLE `projects_time_tracking` ADD `create_by` char(38) DEFAULT NULL;
DROP TABLE IF EXISTS `sm_facebookaccounts`;
DROP TABLE IF EXISTS `sm_linkedinaccounts`;
DROP TABLE IF EXISTS `sm_twitteraccounts`;
ALTER TABLE `tenants_quota` ADD `name` varchar(128) DEFAULT NULL AFTER `tenant`;
ALTER TABLE `tenants_quota` ADD `description` varchar(128) DEFAULT NULL AFTER `name`;
ALTER TABLE `tenants_quota` ADD `active_users` int(10) NOT NULL DEFAULT '0' AFTER `max_total_size`;
ALTER TABLE `tenants_quota` ADD `features` text AFTER `active_users`;
ALTER TABLE `tenants_quota` ADD `price` decimal(10,2) NOT NULL DEFAULT '0.00' AFTER `features`;
ALTER TABLE `tenants_quota` ADD `avangate_id` varchar(128) DEFAULT NULL AFTER `price`;
ALTER TABLE `tenants_quota` ADD `sms_auth` int(10) NOT NULL DEFAULT '0';
ALTER TABLE `tenants_quota` ADD `branding` int(10) NOT NULL DEFAULT '0';
CREATE TABLE IF NOT EXISTS `tenants_tariffcoupon` (
  `coupon` varchar(128) NOT NULL,
  `tariff` int(11) NOT NULL,
  `tariff_period` int(11) NOT NULL,
  `valid_from` datetime DEFAULT NULL,
  `valid_to` datetime DEFAULT NULL,
  PRIMARY KEY (`coupon`)
);
ALTER TABLE `tenants_tenants` MODIFY `alias` varchar(100) NOT NULL;
ALTER TABLE `tenants_tenants` MODIFY `mappeddomain` varchar(100) DEFAULT NULL;
ALTER TABLE `tenants_tenants` ADD `version` int(10) NOT NULL DEFAULT '1' AFTER `mappeddomain`;
ALTER TABLE `tenants_tenants` ADD INDEX `version` (`version`);
CREATE TABLE IF NOT EXISTS `tenants_version` (
  `id` int(10) NOT NULL,
  `version` varchar(64) NOT NULL,
  `url` varchar(64) NOT NULL,
  `visible` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
);
DROP TABLE IF EXISTS `webstudio_quicklinks`;
ALTER TABLE `webstudio_settings` MODIFY `Data` mediumtext NOT NULL;
CREATE TABLE IF NOT EXISTS `webstudio_user_birthday` (
  `tenant_id` int(10) NOT NULL,
  `subscriber_id` char(38) NOT NULL,
  `target_user_id` char(38) NOT NULL
);
replace into projects_tasks_responsible (task_id, responsible_id, tenant_id) select pt.id, pt.responsible_id, pt.tenant_id from projects_tasks as pt where pt.responsible_id <> '00000000-0000-0000-0000-000000000000';
update projects_tasks set status = 1 where status = 4;
update projects_tasks set status = 1 where status = 0;
update projects_milestones set responsible_id = create_by where responsible_id = '00000000-0000-0000-0000-000000000000' or responsible_id is null;

-- subscribe all users to what's new notifications
insert ignore into core_subscription(source, action, recipient, object, tenant) values('asc.web.studio','send_whats_new','abef62db-11a8-4673-9d32-ef1d8af19dc0','',-1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values('asc.web.studio','send_whats_new','abef62db-11a8-4673-9d32-ef1d8af19dc0','email.sender',-1);
-- subscribe all users to new events
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('6504977c-75af-4691-9099-084d3ddeea04','new feed','abef62db-11a8-4673-9d32-ef1d8af19dc0','',-1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values('6504977c-75af-4691-9099-084d3ddeea04','new feed','abef62db-11a8-4673-9d32-ef1d8af19dc0','email.sender|messanger.sender',-1);
-- subscribe all users to new blogs
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('6a598c74-91ae-437d-a5f4-ad339bd11bb2','new post','abef62db-11a8-4673-9d32-ef1d8af19dc0','',-1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values('6a598c74-91ae-437d-a5f4-ad339bd11bb2','new post','abef62db-11a8-4673-9d32-ef1d8af19dc0','email.sender|messanger.sender',-1);
-- subscribe all users to new forum
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('853b6eb9-73ee-438d-9b09-8ffeedf36234', 'new topic in forum', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('853b6eb9-73ee-438d-9b09-8ffeedf36234', 'new topic in forum', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
-- subscribe all users to photos
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('9d51954f-db9b-4aed-94e3-ed70b914e101', 'new photo uploaded', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('9d51954f-db9b-4aed-94e3-ed70b914e101', 'new photo uploaded', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
-- subscribe all users to bookmarks
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('28b10049-dd20-4f54-b986-873bc14ccfc7', 'new bookmark created', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('28b10049-dd20-4f54-b986-873bc14ccfc7', 'new bookmark created', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
-- subscribe all users to wiki
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('742cf945-cbbc-4a57-82d6-1600a12cf8ca', 'new wiki page', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('742cf945-cbbc-4a57-82d6-1600a12cf8ca', 'new wiki page', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
-- subscribe all users to documents
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('6fe286a4-479e-4c25-a8d9-0156e332b0c0', 'sharedocument', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('6fe286a4-479e-4c25-a8d9-0156e332b0c0', 'sharefolder', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6fe286a4-479e-4c25-a8d9-0156e332b0c0', 'sharedocument', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6fe286a4-479e-4c25-a8d9-0156e332b0c0', 'sharefolder', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6fe286a4-479e-4c25-a8d9-0156e332b0c0', 'updatedocument', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
-- subscribe all users to projects
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'invitetoproject', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'milestonedeadline', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'newcommentformessage', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'newcommentformilestone', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'newcommentfortask', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'projectcreaterequest', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'projecteditrequest', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'removefromproject', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'responsibleforproject', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'responsiblefortask', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('6045b68c-2c2e-42db-9e53-c272e814c4ad', 'taskclosed', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
-- subscribe all users to calendar events
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('40650da3-f7c1-424c-8c89-b9c115472e08', 'calendar_sharing', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('40650da3-f7c1-424c-8c89-b9c115472e08', 'event_alert', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('40650da3-f7c1-424c-8c89-b9c115472e08', 'calendar_sharing', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('40650da3-f7c1-424c-8c89-b9c115472e08', 'event_alert', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
-- subscribe admin group to admin notifications
insert ignore into core_subscription(source, action, recipient, object, tenant) values('asc.web.studio','admin_notify','cd84e66b-b803-40fc-99f9-b2969a54a1de','',-1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values('asc.web.studio','admin_notify','cd84e66b-b803-40fc-99f9-b2969a54a1de','email.sender',-1);
-- subscribe all users to crm
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'SetAccess', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'SetAccess', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'ResponsibleForTask', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'ResponsibleForTask', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'AddRelationshipEvent', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'AddRelationshipEvent', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'ExportCompleted', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'ExportCompleted', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);
insert ignore into core_subscription(source, action, recipient, object, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'CreateNewContact', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '', -1);
insert ignore into core_subscriptionmethod(source, action, recipient, sender, tenant) values ('13ff36fb-0272-4887-b416-74f52b0d0b02', 'CreateNewContact', 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'email.sender|messanger.sender', -1);

-- default permissions
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, '5d5b7260-f7f7-49f1-a1c9-95fbb6a12604', 'ef5e6790-f346-4b6e-b662-722bc28cb0db', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, '5d5b7260-f7f7-49f1-a1c9-95fbb6a12604', 'f11e8f3f-46e6-4e55-90e3-09c22ec565bd', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '088d5940-a80f-4403-9741-d610718ce95c', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '08d66144-e1c9-4065-9aa1-aa4bba0a7bc8', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '08d75c97-cf3f-494b-90d1-751c941fe2dd', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '0d1f72a8-63da-47ea-ae42-0900e4ac72a9', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '13e30b51-5b4d-40a5-8575-cb561899eeb1', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '19f658ae-722b-4cd8-8236-3ad150801d96', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '2c6552b3-b2e0-4a00-b8fd-13c161e337b1', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '388c29d3-c662-4a61-bf47-fc2f7094224a', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '40bf31f4-3132-4e76-8d5c-9828a89501a3', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '49ae8915-2b30-4348-ab74-b152279364fb', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '9018c001-24c2-44bf-a1db-d1121a570e74', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '948ad738-434b-4a88-8e38-7569d332910a', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', '9d75a568-52aa-49d8-ad43-473756cd8903', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'a362fe79-684e-4d43-a599-65bc1f4e167f', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'c426c349-9ad4-47cd-9b8f-99fc30675951', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'd11ebcb9-0e6e-45e6-a6d0-99c41d687598', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'd1f3b53d-d9e2-4259-80e7-d24380978395', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'd49f4e30-da10-4b39-bc6d-b41ef6e039d3', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'd852b66f-6719-45e1-8657-18f0bb791690', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'e0759a42-47f0-4763-a26a-d5aa665bec35', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'e37239bd-c5b5-4f1e-a9f8-3ceeac209615', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'fbc37705-a04c-40ad-a68c-ce2f0423f397', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'abef62db-11a8-4673-9d32-ef1d8af19dc0', 'fcac42b8-9386-48eb-a938-d19b3c576912', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'ba74ca02-873f-43dc-8470-8620c156bc67', '13e30b51-5b4d-40a5-8575-cb561899eeb1', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'ba74ca02-873f-43dc-8470-8620c156bc67', '49ae8915-2b30-4348-ab74-b152279364fb', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'ba74ca02-873f-43dc-8470-8620c156bc67', '63e9f35f-6bb5-4fb1-afaa-e4c2f4dec9bd', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'ba74ca02-873f-43dc-8470-8620c156bc67', '9018c001-24c2-44bf-a1db-d1121a570e74', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'ba74ca02-873f-43dc-8470-8620c156bc67', 'd1f3b53d-d9e2-4259-80e7-d24380978395', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'ba74ca02-873f-43dc-8470-8620c156bc67', 'e0759a42-47f0-4763-a26a-d5aa665bec35', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'ba74ca02-873f-43dc-8470-8620c156bc67', 'e37239bd-c5b5-4f1e-a9f8-3ceeac209615', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'ba74ca02-873f-43dc-8470-8620c156bc67', 'f11e88d7-f185-4372-927c-d88008d2c483', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'ba74ca02-873f-43dc-8470-8620c156bc67', 'f11e8f3f-46e6-4e55-90e3-09c22ec565bd', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '00e7dfc5-ac49-4fd3-a1d6-98d84e877ac4', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '14be970f-7af5-4590-8e81-ea32b5f7866d', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '18ecc94d-6afa-4994-8406-aee9dff12ce2', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '298530eb-435e-4dc6-a776-9abcd95c70e9', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '430eaf70-1886-483c-a746-1a18e3e6bb63', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '557d6503-633b-4490-a14c-6473147ce2b3', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '724cbb75-d1c9-451e-bae0-4de0db96b1f7', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '7cb5c0d1-d254-433f-abe3-ff23373ec631', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '91b29dcd-9430-4403-b17a-27d09189be88', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', 'a18480a4-6d18-4c71-84fa-789888791f45', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', 'b630d29b-1844-4bda-bbbe-cf5542df3559', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', 'c62a9e8d-b24c-4513-90aa-7ff0f8ba38eb', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', 'd7cdb020-288b-41e5-a857-597347618533', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, '712d9ec3-5d2b-4b13-824f-71f00191dcca', 'e0759a42-47f0-4763-a26a-d5aa665bec35', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '0000ffff-ae36-4d2e-818d-726cb650aeb7', 'ASC.Web.Studio.Core.TcpIpFilterSecurityObject|0000:0000:0000:0000:0000:0000:0000:0000', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'bba32183-a14d-48ed-9d39-c6b4d8925fbf', '0d68b142-e20a-446e-a832-0d6b0b65a164', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '6f05c382-8bca-4469-9424-c807a98c40d7', '', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '77777777-32ae-425f-99b5-83176061d1ae', 'ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|1e04460243b54d7982f3fd6208a11960', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '77777777-32ae-425f-99b5-83176061d1ae', 'ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|6743007c6f954d208c88a8601ce5e76d', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '77777777-32ae-425f-99b5-83176061d1ae', 'ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|e67be73df9ae4ce18fec1880cb518cb4', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '77777777-32ae-425f-99b5-83176061d1ae', 'ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|ea942538e68e49079394035336ee0ba8', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '77777777-32ae-425f-99b5-83176061d1ae', 'ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|32d24cb57ece46069c9419216ba42086', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '77777777-32ae-425f-99b5-83176061d1ae', 'ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|bf88953e3c434850a3fbb1e43ad53a3e', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '77777777-32ae-425f-99b5-83176061d1ae', 'ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|2a9230378b2d487b9a225ac0918acf3f', 0);
insert ignore into core_acl (tenant, subject, action, object, acetype) values (-1, 'c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e', '77777777-32ae-425f-99b5-83176061d1ae', 'ASC.Web.Core.WebItemSecurity+WebItemSecurityObject|f4d98afdd336433287783c6945c81ea0', 0);

-- restricted tenant names
insert ignore into tenants_forbiden (address) values ('about');
insert ignore into tenants_forbiden (address) values ('api');
insert ignore into tenants_forbiden (address) values ('asset');
insert ignore into tenants_forbiden (address) values ('audio');
insert ignore into tenants_forbiden (address) values ('aws');
insert ignore into tenants_forbiden (address) values ('blogs');
insert ignore into tenants_forbiden (address) values ('business');
insert ignore into tenants_forbiden (address) values ('buzz');
insert ignore into tenants_forbiden (address) values ('calendar');
insert ignore into tenants_forbiden (address) values ('client');
insert ignore into tenants_forbiden (address) values ('clients');
insert ignore into tenants_forbiden (address) values ('community');
insert ignore into tenants_forbiden (address) values ('data');
insert ignore into tenants_forbiden (address) values ('db');
insert ignore into tenants_forbiden (address) values ('dev');
insert ignore into tenants_forbiden (address) values ('developer');
insert ignore into tenants_forbiden (address) values ('developers');
insert ignore into tenants_forbiden (address) values ('doc');
insert ignore into tenants_forbiden (address) values ('docs');
insert ignore into tenants_forbiden (address) values ('download');
insert ignore into tenants_forbiden (address) values ('downloads');
insert ignore into tenants_forbiden (address) values ('e-mail');
insert ignore into tenants_forbiden (address) values ('feed');
insert ignore into tenants_forbiden (address) values ('feeds');
insert ignore into tenants_forbiden (address) values ('file');
insert ignore into tenants_forbiden (address) values ('files');
insert ignore into tenants_forbiden (address) values ('flash');
insert ignore into tenants_forbiden (address) values ('forum');
insert ignore into tenants_forbiden (address) values ('forumsforumblog');
insert ignore into tenants_forbiden (address) values ('help');
insert ignore into tenants_forbiden (address) values ('jabber');
insert ignore into tenants_forbiden (address) values ('localhost');
insert ignore into tenants_forbiden (address) values ('mail');
insert ignore into tenants_forbiden (address) values ('management');
insert ignore into tenants_forbiden (address) values ('manual');
insert ignore into tenants_forbiden (address) values ('media');
insert ignore into tenants_forbiden (address) values ('movie');
insert ignore into tenants_forbiden (address) values ('music');
insert ignore into tenants_forbiden (address) values ('my');
insert ignore into tenants_forbiden (address) values ('nct');
insert ignore into tenants_forbiden (address) values ('net');
insert ignore into tenants_forbiden (address) values ('network');
insert ignore into tenants_forbiden (address) values ('new');
insert ignore into tenants_forbiden (address) values ('news');
insert ignore into tenants_forbiden (address) values ('office');
insert ignore into tenants_forbiden (address) values ('online-help');
insert ignore into tenants_forbiden (address) values ('onlinehelp');
insert ignore into tenants_forbiden (address) values ('organizer');
insert ignore into tenants_forbiden (address) values ('plan');
insert ignore into tenants_forbiden (address) values ('plans');
insert ignore into tenants_forbiden (address) values ('press');
insert ignore into tenants_forbiden (address) values ('project');
insert ignore into tenants_forbiden (address) values ('projects');
insert ignore into tenants_forbiden (address) values ('radio');
insert ignore into tenants_forbiden (address) values ('reg');
insert ignore into tenants_forbiden (address) values ('registration');
insert ignore into tenants_forbiden (address) values ('rss');
insert ignore into tenants_forbiden (address) values ('security');
insert ignore into tenants_forbiden (address) values ('share');
insert ignore into tenants_forbiden (address) values ('source');
insert ignore into tenants_forbiden (address) values ('stat');
insert ignore into tenants_forbiden (address) values ('static');
insert ignore into tenants_forbiden (address) values ('stream');
insert ignore into tenants_forbiden (address) values ('support');
insert ignore into tenants_forbiden (address) values ('talk');
insert ignore into tenants_forbiden (address) values ('task');
insert ignore into tenants_forbiden (address) values ('tasks');
insert ignore into tenants_forbiden (address) values ('teamlab');
insert ignore into tenants_forbiden (address) values ('time');
insert ignore into tenants_forbiden (address) values ('tools');
insert ignore into tenants_forbiden (address) values ('user-manual');
insert ignore into tenants_forbiden (address) values ('usermanual');
insert ignore into tenants_forbiden (address) values ('video');
insert ignore into tenants_forbiden (address) values ('wave');
insert ignore into tenants_forbiden (address) values ('wiki');
insert ignore into tenants_forbiden (address) values ('wikis');

-- fix crm aces
update core_acl set object = replace(object,'People','Person') where object like 'ASC.CRM.Core.Entities.People%';