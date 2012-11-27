-- account_links
CREATE TABLE IF NOT EXISTS `account_links` (
  `id` varchar(200) NOT NULL,
  `uid` varchar(200) NOT NULL,
  `provider` char(32) DEFAULT NULL,
  `profile` text NOT NULL,
  `linked` datetime NOT NULL,
  PRIMARY KEY (`id`,`uid`),
  KEY `uid` (`uid`)
);
-- webstudio_fckuploads
CREATE TABLE IF NOT EXISTS `webstudio_fckuploads` (
  `TenantID` int(11) NOT NULL,
  `StoreDomain` varchar(50) NOT NULL,
  `FolderID` varchar(100) NOT NULL,
  `ItemID` varchar(100) NOT NULL,
  PRIMARY KEY (`TenantID`,`StoreDomain`,`FolderID`,`ItemID`)
);
-- webstudio_settings
CREATE TABLE IF NOT EXISTS `webstudio_settings` (
  `TenantID` int(11) NOT NULL,
  `ID` varchar(64) NOT NULL,
  `UserID` varchar(64) NOT NULL,
  `Data` mediumtext NOT NULL,
  PRIMARY KEY (`TenantID`,`ID`,`UserID`)
);
-- webstudio_user_birthday
CREATE TABLE IF NOT EXISTS `webstudio_user_birthday` (
  `tenant_id` int(10) NOT NULL,
  `subscriber_id` char(38) NOT NULL,
  `target_user_id` char(38) NOT NULL
);
-- webstudio_useractivity
CREATE TABLE IF NOT EXISTS `webstudio_useractivity` (
  `ID` int(10) NOT NULL AUTO_INCREMENT,
  `ProductID` char(38) NOT NULL,
  `ModuleID` char(38) NOT NULL,
  `UserID` char(38) NOT NULL,
  `ContentID` varchar(256) NOT NULL,
  `Title` varchar(4000) NOT NULL,
  `URL` varchar(4000) NOT NULL,
  `BusinessValue` int(10) NOT NULL DEFAULT '0',
  `ActionType` int(10) NOT NULL,
  `ActionText` varchar(256) NOT NULL,
  `ActivityDate` datetime NOT NULL,
  `ImageFileName` varchar(1024) DEFAULT NULL,
  `PartID` varchar(38) DEFAULT NULL,
  `ContainerID` varchar(38) DEFAULT NULL,
  `AdditionalData` varchar(256) DEFAULT NULL,
  `TenantID` int(10) NOT NULL,
  `HtmlPreview` mediumtext,
  `SecurityId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `UserID` (`UserID`),
  KEY `actiontype` (`TenantID`,`ActionType`,`ProductID`),
  KEY `ProductID` (`TenantID`,`ProductID`,`ModuleID`),
  KEY `containerid` (`ContainerID`),
  KEY `ActivityDate` (`ActivityDate`)
);
-- webstudio_uservisit
CREATE TABLE IF NOT EXISTS `webstudio_uservisit` (
  `tenantid` int(11) NOT NULL,
  `visitdate` datetime NOT NULL,
  `productid` varchar(38) NOT NULL,
  `userid` varchar(38) NOT NULL,
  `visitcount` int(11) NOT NULL DEFAULT '0',
  `firstvisittime` datetime DEFAULT NULL,
  `lastvisittime` datetime DEFAULT NULL,
  PRIMARY KEY (`tenantid`,`visitdate`,`productid`,`userid`),
  KEY `visitdate` (`visitdate`)
);
-- webstudio_widgetcontainer
CREATE TABLE IF NOT EXISTS `webstudio_widgetcontainer` (
  `ID` varchar(64) NOT NULL,
  `TenantID` int(11) NOT NULL,
  `ContainerID` varchar(64) NOT NULL,
  `UserID` varchar(64) NOT NULL,
  `SchemaID` int(11) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `ContainerID` (`TenantID`,`ContainerID`)
);
-- webstudio_widgetstate
CREATE TABLE IF NOT EXISTS `webstudio_widgetstate` (
  `WidgetID` varchar(64) NOT NULL,
  `WidgetContainerID` varchar(64) NOT NULL,
  `ColumnID` int(11) NOT NULL,
  `SortOrderInColumn` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`WidgetContainerID`,`WidgetID`)
);

