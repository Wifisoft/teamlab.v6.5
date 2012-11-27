-- crm_case
CREATE TABLE IF NOT EXISTS `crm_case` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `is_closed` tinyint(1) NOT NULL DEFAULT '0',
  `create_by` char(38) NOT NULL,
  `create_on` datetime NOT NULL,
  `tenant_id` int(11) NOT NULL,
  `last_modifed_on` datetime DEFAULT NULL,
  `last_modifed_by` char(38) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`)
);
-- crm_contact
CREATE TABLE IF NOT EXISTS `crm_contact` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tenant_id` int(11) NOT NULL,
  `is_company` tinyint(1) NOT NULL,
  `notes` text,
  `title` varchar(255) DEFAULT NULL,
  `first_name` varchar(255) DEFAULT NULL,
  `last_name` varchar(255) DEFAULT NULL,
  `company_name` varchar(255) DEFAULT NULL,
  `industry` varchar(255) DEFAULT NULL,
  `status_id` int(11) NOT NULL DEFAULT '0',
  `company_id` int(11) NOT NULL,
  `create_by` char(38) NOT NULL,
  `create_on` datetime NOT NULL,
  `last_modifed_on` datetime DEFAULT NULL,
  `last_modifed_by` char(38) DEFAULT NULL,
  `display_name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `company_id` (`tenant_id`,`company_id`),
  KEY `display_name` (`tenant_id`,`display_name`),
  KEY `last_modifed_on` (`last_modifed_on`)
);
-- crm_contact_info
CREATE TABLE IF NOT EXISTS `crm_contact_info` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `data` text NOT NULL,
  `category` int(255) NOT NULL,
  `tenant_id` int(255) NOT NULL,
  `is_primary` tinyint(4) NOT NULL,
  `contact_id` int(11) NOT NULL,
  `type` int(255) NOT NULL,
  `last_modifed_on` datetime DEFAULT NULL,
  `last_modifed_by` char(38) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `IX_Contact` (`tenant_id`,`contact_id`)
);
-- crm_deal
CREATE TABLE IF NOT EXISTS `crm_deal` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `description` text,
  `responsible_id` char(38) NOT NULL,
  `contact_id` int(11) NOT NULL,
  `create_on` datetime NOT NULL,
  `create_by` char(38) NOT NULL,
  `bid_currency` varchar(255) DEFAULT NULL,
  `bid_value` decimal(50,9) NOT NULL DEFAULT '0.000000000',
  `bid_type` int(11) NOT NULL DEFAULT '0',
  `deal_milestone_id` int(11) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  `expected_close_date` datetime NOT NULL,
  `per_period_value` int(11) NOT NULL DEFAULT '0',
  `deal_milestone_probability` int(11) DEFAULT NULL,
  `last_modifed_on` datetime DEFAULT NULL,
  `last_modifed_by` char(38) DEFAULT NULL,
  `actual_close_date` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `contact_id` (`tenant_id`,`contact_id`)
);
-- crm_deal_milestone
CREATE TABLE IF NOT EXISTS `crm_deal_milestone` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `color` varchar(50) NOT NULL DEFAULT '0',
  `sort_order` int(10) NOT NULL DEFAULT '0',
  `title` varchar(250) NOT NULL,
  `description` text,
  `probability` int(10) NOT NULL DEFAULT '0',
  `status` int(10) NOT NULL DEFAULT '0',
  `tenant_id` int(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`)
);
-- crm_entity_contact
CREATE TABLE IF NOT EXISTS `crm_entity_contact` (
  `entity_id` int(11) NOT NULL,
  `entity_type` int(11) NOT NULL,
  `contact_id` int(11) NOT NULL,
  PRIMARY KEY (`entity_id`,`entity_type`,`contact_id`),
  KEY `IX_Contact` (`contact_id`)
);
-- crm_entity_tag
CREATE TABLE IF NOT EXISTS `crm_entity_tag` (
  `tag_id` int(11) NOT NULL,
  `entity_id` int(11) NOT NULL,
  `entity_type` int(10) NOT NULL,
  PRIMARY KEY (`entity_id`,`entity_type`,`tag_id`),
  KEY `tag_id` (`tag_id`)
);
-- crm_field_description
CREATE TABLE IF NOT EXISTS `crm_field_description` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tenant_id` int(11) NOT NULL,
  `label` varchar(255) NOT NULL,
  `type` int(11) NOT NULL,
  `sort_order` int(11) NOT NULL DEFAULT '0',
  `mask` text,
  `entity_type` int(255) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `entity_type` (`tenant_id`,`entity_type`,`sort_order`)
);
-- crm_field_value
CREATE TABLE IF NOT EXISTS `crm_field_value` (
  `value` varchar(255) NOT NULL,
  `entity_id` int(11) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  `field_id` int(11) NOT NULL,
  `entity_type` int(10) NOT NULL,
  `last_modifed_on` datetime DEFAULT NULL,
  `last_modifed_by` char(38) DEFAULT NULL,
  PRIMARY KEY (`tenant_id`,`entity_id`,`entity_type`,`field_id`),
  KEY `field_id` (`field_id`)
);
-- crm_list_item
CREATE TABLE IF NOT EXISTS `crm_list_item` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `sort_order` int(11) NOT NULL DEFAULT '0',
  `color` varchar(255) DEFAULT NULL,
  `additional_params` varchar(255) DEFAULT NULL,
  `tenant_id` int(11) NOT NULL,
  `list_type` int(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `list_type` (`tenant_id`,`list_type`)
);
-- crm_projects
CREATE TABLE IF NOT EXISTS `crm_projects` (
  `project_id` int(10) NOT NULL,
  `contact_id` int(10) NOT NULL,
  `tenant_id` int(10) NOT NULL,
  PRIMARY KEY (`tenant_id`,`contact_id`,`project_id`),
  KEY `project_id` (`tenant_id`,`project_id`),
  KEY `contact_id` (`tenant_id`,`contact_id`)
);
-- crm_relationship_event
CREATE TABLE IF NOT EXISTS `crm_relationship_event` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `contact_id` int(11) NOT NULL,
  `content` text,
  `create_by` char(38) NOT NULL,
  `create_on` datetime NOT NULL,
  `tenant_id` int(11) NOT NULL,
  `entity_type` int(11) NOT NULL,
  `entity_id` int(11) NOT NULL,
  `category_id` int(11) NOT NULL,
  `last_modifed_by` char(38) DEFAULT NULL,
  `last_modifed_on` datetime DEFAULT NULL,
  `have_files` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`),
  KEY `IX_Contact` (`contact_id`),
  KEY `IX_Entity` (`entity_id`,`entity_type`)
);
-- crm_tag
CREATE TABLE IF NOT EXISTS `crm_tag` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `tenant_id` int(11) NOT NULL,
  `entity_type` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant_id` (`tenant_id`)
);
-- crm_task
CREATE TABLE IF NOT EXISTS `crm_task` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `description` text,
  `deadline` datetime NOT NULL,
  `responsible_id` char(38) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  `contact_id` int(11) NOT NULL DEFAULT '-1',
  `is_closed` int(1) NOT NULL DEFAULT '0',
  `tenant_id` int(11) NOT NULL,
  `entity_type` int(11) NOT NULL,
  `entity_id` int(11) NOT NULL,
  `category_id` int(11) NOT NULL DEFAULT '0',
  `create_on` datetime NOT NULL,
  `create_by` char(38) NOT NULL,
  `last_modifed_on` datetime DEFAULT NULL,
  `last_modifed_by` char(38) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `responsible_id` (`tenant_id`,`responsible_id`),
  KEY `IX_Contact` (`tenant_id`,`contact_id`),
  KEY `IX_Entity` (`tenant_id`,`entity_id`,`entity_type`)
);
-- crm_task_template
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
-- crm_task_template_container
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
-- crm_task_template_task
CREATE TABLE IF NOT EXISTS `crm_task_template_task` (
  `task_id` int(10) NOT NULL,
  `task_template_id` int(10) NOT NULL,
  `tenant_id` int(10) NOT NULL,
  KEY `task_id` (`task_id`)
);

