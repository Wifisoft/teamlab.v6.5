DROP TABLE IF EXISTS `comments`;
CREATE TABLE  `comments` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `parent_id` int(10) unsigned DEFAULT NULL,
  `content` tinytext NOT NULL,
  `author` varchar(45) NOT NULL,
  `deleted` bit(1) DEFAULT NULL,
  `security_id` varchar(45) DEFAULT NULL,
  `created` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `comment_key` varchar(45) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `Index_key` (`comment_key`),
  FULLTEXT KEY `Index_content` (`content`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `comment_readed`;
CREATE TABLE  `comment_readed` (
  `comment_id` int(10) unsigned NOT NULL,
  `user_id` varchar(45) NOT NULL,
  `timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`comment_id`,`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;