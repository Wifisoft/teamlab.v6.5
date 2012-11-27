-- core_acl
CREATE TABLE IF NOT EXISTS `core_acl` (
  `tenant` int(11) NOT NULL,
  `subject` varchar(38) NOT NULL,
  `action` varchar(38) NOT NULL,
  `object` varchar(255) NOT NULL DEFAULT '',
  `acetype` int(11) NOT NULL,
  PRIMARY KEY (`tenant`,`subject`,`action`,`object`)
);
-- core_alias
CREATE TABLE IF NOT EXISTS `core_alias` (
  `uniq_id` varchar(100) NOT NULL,
  `alias` varchar(50) NOT NULL,
  `tenant_id` int(10) NOT NULL,
  PRIMARY KEY (`uniq_id`),
  KEY `alias` (`alias`,`tenant_id`)
);
-- core_group
CREATE TABLE IF NOT EXISTS `core_group` (
  `tenant` int(11) NOT NULL,
  `id` varchar(38) NOT NULL,
  `name` varchar(128) NOT NULL,
  `categoryid` varchar(38) DEFAULT NULL,
  `parentid` varchar(38) DEFAULT NULL,
  `removed` int(11) NOT NULL DEFAULT '0',
  `last_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `parentid` (`tenant`,`parentid`),
  KEY `last_modified` (`last_modified`)
);
-- core_logging
CREATE TABLE IF NOT EXISTS `core_logging` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `user_id` varchar(38) NOT NULL,
  `user_email` varchar(50) DEFAULT NULL,
  `caller_ip` varchar(50) DEFAULT NULL,
  `tenant_id` int(10) NOT NULL,
  `action` text NOT NULL,
  `timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
);
-- core_settings
CREATE TABLE IF NOT EXISTS `core_settings` (
  `tenant` int(11) NOT NULL,
  `id` varchar(128) NOT NULL,
  `value` mediumblob NOT NULL,
  PRIMARY KEY (`tenant`,`id`)
);
-- core_subscription
CREATE TABLE IF NOT EXISTS `core_subscription` (
  `tenant` int(11) NOT NULL,
  `source` varchar(38) NOT NULL,
  `action` varchar(128) NOT NULL,
  `recipient` varchar(38) NOT NULL,
  `object` varchar(128) NOT NULL,
  `unsubscribed` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`tenant`,`source`,`action`,`recipient`,`object`)
);
-- core_subscriptionmethod
CREATE TABLE IF NOT EXISTS `core_subscriptionmethod` (
  `tenant` int(11) NOT NULL,
  `source` varchar(38) NOT NULL,
  `action` varchar(128) NOT NULL,
  `recipient` varchar(38) NOT NULL,
  `sender` varchar(1024) NOT NULL,
  PRIMARY KEY (`tenant`,`source`,`action`,`recipient`)
);
-- core_user
CREATE TABLE IF NOT EXISTS `core_user` (
  `tenant` int(11) NOT NULL,
  `id` varchar(38) NOT NULL,
  `username` varchar(255) NOT NULL,
  `firstname` varchar(64) NOT NULL,
  `lastname` varchar(64) NOT NULL,
  `sex` int(11) DEFAULT NULL,
  `bithdate` datetime DEFAULT NULL,
  `status` int(11) NOT NULL DEFAULT '1',
  `activation_status` int(11) NOT NULL DEFAULT '0',
  `email` varchar(255) DEFAULT NULL,
  `workfromdate` datetime DEFAULT NULL,
  `terminateddate` datetime DEFAULT NULL,
  `title` varchar(64) DEFAULT NULL,
  `department` varchar(128) DEFAULT NULL,
  `culture` varchar(20) DEFAULT NULL,
  `contacts` varchar(1024) DEFAULT NULL,
  `phone` varchar(255) DEFAULT NULL,
  `phone_activation` int(11) NOT NULL DEFAULT '0',
  `location` varchar(255) DEFAULT NULL,
  `notes` varchar(512) DEFAULT NULL,
  `removed` int(11) NOT NULL DEFAULT '0',
  `create_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `last_modified` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `last_modified` (`last_modified`),
  KEY `username` (`tenant`,`username`),
  KEY `email` (`email`)
);
-- core_usergroup
CREATE TABLE IF NOT EXISTS `core_usergroup` (
  `tenant` int(11) NOT NULL,
  `userid` varchar(38) NOT NULL,
  `groupid` varchar(38) NOT NULL,
  `ref_type` int(11) NOT NULL,
  `removed` int(11) NOT NULL DEFAULT '0',
  `last_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`tenant`,`userid`,`groupid`,`ref_type`),
  KEY `last_modified` (`last_modified`)
);
-- core_userphoto
CREATE TABLE IF NOT EXISTS `core_userphoto` (
  `tenant` int(11) NOT NULL,
  `userid` varchar(38) NOT NULL,
  `photo` mediumblob NOT NULL,
  PRIMARY KEY (`userid`),
  KEY `tenant` (`tenant`)
);
-- core_usersecurity
CREATE TABLE IF NOT EXISTS `core_usersecurity` (
  `tenant` int(11) NOT NULL,
  `userid` varchar(38) NOT NULL,
  `pwdhash` varchar(512) DEFAULT NULL,
  `pwdhashsha512` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`userid`),
  KEY `pwdhash` (`pwdhash`(255)),
  KEY `tenant` (`tenant`)
);
-- notify_info
CREATE TABLE IF NOT EXISTS `notify_info` (
  `notify_id` int(10) NOT NULL,
  `state` int(10) NOT NULL DEFAULT '0',
  `attempts` int(10) NOT NULL DEFAULT '0',
  `modify_date` datetime NOT NULL,
  PRIMARY KEY (`notify_id`),
  KEY `state` (`state`)
);
-- notify_queue
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
-- tenants_forbiden
CREATE TABLE IF NOT EXISTS `tenants_forbiden` (
  `address` varchar(50) NOT NULL,
  PRIMARY KEY (`address`)
);
-- tenants_quota
CREATE TABLE IF NOT EXISTS `tenants_quota` (
  `tenant` int(10) NOT NULL,
  `name` varchar(128) DEFAULT NULL,
  `description` varchar(128) DEFAULT NULL,
  `max_file_size` bigint(20) NOT NULL DEFAULT '0',
  `max_total_size` bigint(20) NOT NULL DEFAULT '0',
  `active_users` int(10) NOT NULL DEFAULT '0',
  `features` text,
  `price` decimal(10,2) NOT NULL DEFAULT '0.00',
  `avangate_id` varchar(128) DEFAULT NULL,
  `https_enable` int(10) NOT NULL DEFAULT '0',
  `security_enable` int(10) NOT NULL DEFAULT '0',
  `sms_auth` int(10) NOT NULL DEFAULT '0',
  `branding` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`tenant`)
);
-- tenants_quotarow
CREATE TABLE IF NOT EXISTS `tenants_quotarow` (
  `tenant` int(11) NOT NULL,
  `path` varchar(255) NOT NULL,
  `counter` bigint(20) NOT NULL DEFAULT '0',
  `tag` varchar(1024) DEFAULT NULL,
  `last_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`tenant`,`path`),
  KEY `last_modified` (`last_modified`)
);
-- tenants_tariff
CREATE TABLE IF NOT EXISTS `tenants_tariff` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `tenant` int(10) NOT NULL,
  `tariff` int(10) NOT NULL,
  `stamp` datetime NOT NULL,
  `comment` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `tenant` (`tenant`)
);
-- tenants_tariffcoupon
CREATE TABLE IF NOT EXISTS `tenants_tariffcoupon` (
  `coupon` varchar(128) NOT NULL,
  `tariff` int(11) NOT NULL,
  `tariff_period` int(11) NOT NULL,
  `valid_from` datetime DEFAULT NULL,
  `valid_to` datetime DEFAULT NULL,
  PRIMARY KEY (`coupon`)
);
-- tenants_template_subscription
CREATE TABLE IF NOT EXISTS `tenants_template_subscription` (
  `source` varchar(38) NOT NULL,
  `action` varchar(128) NOT NULL,
  `recipient` varchar(38) NOT NULL,
  `object` varchar(128) NOT NULL DEFAULT '',
  `unsubscribed` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`source`,`action`,`recipient`,`object`)
);
-- tenants_template_subscriptionmethod
CREATE TABLE IF NOT EXISTS `tenants_template_subscriptionmethod` (
  `source` varchar(38) NOT NULL,
  `action` varchar(128) NOT NULL,
  `recipient` varchar(38) NOT NULL,
  `sender` varchar(1024) NOT NULL,
  PRIMARY KEY (`source`,`action`,`recipient`)
);
-- tenants_tenants
CREATE TABLE IF NOT EXISTS `tenants_tenants` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `alias` varchar(100) NOT NULL,
  `mappeddomain` varchar(100) DEFAULT NULL,
  `version` int(10) NOT NULL DEFAULT '1',
  `language` char(10) NOT NULL DEFAULT 'en-US',
  `timezone` varchar(50) DEFAULT NULL,
  `trusteddomains` varchar(1024) DEFAULT NULL,
  `trusteddomainsenabled` int(10) NOT NULL DEFAULT '1',
  `status` int(11) NOT NULL DEFAULT '0',
  `statuschanged` datetime DEFAULT NULL,
  `creationdatetime` datetime NOT NULL,
  `owner_id` varchar(38) DEFAULT NULL,
  `public` int(10) NOT NULL DEFAULT '0',
  `publicvisibleproducts` varchar(1024) DEFAULT NULL,
  `last_modified` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `alias` (`alias`),
  KEY `last_modified` (`last_modified`),
  KEY `mappeddomain` (`mappeddomain`),
  KEY `version` (`version`)
);
-- tenants_version
CREATE TABLE IF NOT EXISTS `tenants_version` (
  `id` int(10) NOT NULL,
  `version` varchar(64) NOT NULL,
  `url` varchar(64) NOT NULL,
  `visible` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
);

