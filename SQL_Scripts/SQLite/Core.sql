-- core_acl
CREATE TABLE IF NOT EXISTS "core_acl" (
  "tenant" INTEGER NOT NULL,
  "subject" varchar(38) NOT NULL COLLATE NOCASE,
  "action" varchar(38) NOT NULL COLLATE NOCASE,
  "object" varchar(255) NOT NULL DEFAULT '' COLLATE NOCASE,
  "acetype" INTEGER NOT NULL,
  PRIMARY KEY ("tenant","subject","action","object")
);


-- core_alias
CREATE TABLE IF NOT EXISTS "core_alias" (
  "uniq_id" varchar(100) NOT NULL COLLATE NOCASE,
  "alias" varchar(50) NOT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  PRIMARY KEY ("uniq_id")
);
CREATE INDEX IF NOT EXISTS "core_alias_alias" ON "core_alias" ("alias","tenant_id");


-- core_group
CREATE TABLE IF NOT EXISTS "core_group" (
  "tenant" INTEGER NOT NULL,
  "id" varchar(38) NOT NULL COLLATE NOCASE,
  "name" varchar(128) NOT NULL COLLATE NOCASE,
  "categoryid" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "parentid" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "removed" INTEGER NOT NULL DEFAULT 0,
  "last_modified" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY ("id")
);
CREATE INDEX IF NOT EXISTS "core_group_parentid" ON "core_group" ("tenant","parentid");
CREATE INDEX IF NOT EXISTS "core_group_last_modified" ON "core_group" ("last_modified");


-- core_logging
CREATE TABLE IF NOT EXISTS "core_logging" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "user_id" varchar(38) NOT NULL COLLATE NOCASE,
  "user_email" varchar(50) DEFAULT NULL COLLATE NOCASE,
  "caller_ip" varchar(50) DEFAULT NULL COLLATE NOCASE,
  "tenant_id" INTEGER NOT NULL,
  "action" text NOT NULL COLLATE NOCASE,
  "timestamp" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
);


-- core_settings
CREATE TABLE IF NOT EXISTS "core_settings" (
  "tenant" INTEGER NOT NULL,
  "id" varchar(128) NOT NULL COLLATE NOCASE,
  "value" BLOB NOT NULL,
  PRIMARY KEY ("tenant","id")
);


-- core_subscription
CREATE TABLE IF NOT EXISTS "core_subscription" (
  "tenant" INTEGER NOT NULL,
  "source" varchar(38) NOT NULL COLLATE NOCASE,
  "action" varchar(128) NOT NULL COLLATE NOCASE,
  "recipient" varchar(38) NOT NULL COLLATE NOCASE,
  "object" varchar(128) NOT NULL COLLATE NOCASE,
  "unsubscribed" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("tenant","source","action","recipient","object")
);


-- core_subscriptionmethod
CREATE TABLE IF NOT EXISTS "core_subscriptionmethod" (
  "tenant" INTEGER NOT NULL,
  "source" varchar(38) NOT NULL COLLATE NOCASE,
  "action" varchar(128) NOT NULL COLLATE NOCASE,
  "recipient" varchar(38) NOT NULL COLLATE NOCASE,
  "sender" varchar(1024) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("tenant","source","action","recipient")
);


-- core_user
CREATE TABLE IF NOT EXISTS "core_user" (
  "tenant" INTEGER NOT NULL,
  "id" varchar(38) NOT NULL COLLATE NOCASE,
  "username" varchar(255) NOT NULL COLLATE NOCASE,
  "firstname" varchar(64) NOT NULL COLLATE NOCASE,
  "lastname" varchar(64) NOT NULL COLLATE NOCASE,
  "sex" INTEGER DEFAULT NULL,
  "bithdate" datetime DEFAULT NULL,
  "status" INTEGER NOT NULL DEFAULT '1',
  "activation_status" INTEGER NOT NULL DEFAULT 0,
  "email" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "workfromdate" datetime DEFAULT NULL,
  "terminateddate" datetime DEFAULT NULL,
  "title" varchar(64) DEFAULT NULL COLLATE NOCASE,
  "department" varchar(128) DEFAULT NULL COLLATE NOCASE,
  "culture" varchar(20) DEFAULT NULL COLLATE NOCASE,
  "contacts" varchar(1024) DEFAULT NULL COLLATE NOCASE,
  "phone" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "phone_activation" INTEGER NOT NULL DEFAULT 0,
  "location" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "notes" varchar(512) DEFAULT NULL COLLATE NOCASE,
  "removed" INTEGER NOT NULL DEFAULT 0,
  "create_on" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "last_modified" datetime NOT NULL,
  PRIMARY KEY ("id")
);
CREATE INDEX IF NOT EXISTS "core_user_last_modified" ON "core_user" ("last_modified");
CREATE INDEX IF NOT EXISTS "core_user_username" ON "core_user" ("tenant","username");
CREATE INDEX IF NOT EXISTS "core_user_email" ON "core_user" ("email");


-- core_usergroup
CREATE TABLE IF NOT EXISTS "core_usergroup" (
  "tenant" INTEGER NOT NULL,
  "userid" varchar(38) NOT NULL COLLATE NOCASE,
  "groupid" varchar(38) NOT NULL COLLATE NOCASE,
  "ref_type" INTEGER NOT NULL,
  "removed" INTEGER NOT NULL DEFAULT 0,
  "last_modified" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY ("tenant","userid","groupid","ref_type")
);
CREATE INDEX IF NOT EXISTS "core_usergroup_last_modified" ON "core_usergroup" ("last_modified");


-- core_userphoto
CREATE TABLE IF NOT EXISTS "core_userphoto" (
  "tenant" INTEGER NOT NULL,
  "userid" varchar(38) NOT NULL COLLATE NOCASE,
  "photo" BLOB NOT NULL,
  PRIMARY KEY ("userid")
);
CREATE INDEX IF NOT EXISTS "core_userphoto_tenant" ON "core_userphoto" ("tenant");


-- core_usersecurity
CREATE TABLE IF NOT EXISTS "core_usersecurity" (
  "tenant" INTEGER NOT NULL,
  "userid" varchar(38) NOT NULL COLLATE NOCASE,
  "pwdhash" varchar(512) DEFAULT NULL COLLATE NOCASE,
  "pwdhashsha512" varchar(512) DEFAULT NULL COLLATE NOCASE,
  PRIMARY KEY ("userid")
);
CREATE INDEX IF NOT EXISTS "core_usersecurity_pwdhash" ON "core_usersecurity" ("pwdhash");
CREATE INDEX IF NOT EXISTS "core_usersecurity_tenant" ON "core_usersecurity" ("tenant");


-- notify_info
CREATE TABLE IF NOT EXISTS "notify_info" (
  "notify_id" INTEGER NOT NULL,
  "state" INTEGER NOT NULL DEFAULT 0,
  "attempts" INTEGER NOT NULL DEFAULT 0,
  "modify_date" datetime NOT NULL,
  PRIMARY KEY ("notify_id")
);
CREATE INDEX IF NOT EXISTS "notify_info_state" ON "notify_info" ("state");


-- notify_queue
CREATE TABLE IF NOT EXISTS "notify_queue" (
  "notify_id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "tenant_id" INTEGER NOT NULL,
  "sender" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "reciever" varchar(255) DEFAULT NULL COLLATE NOCASE,
  "subject" varchar(1024) DEFAULT NULL COLLATE NOCASE,
  "content_type" varchar(64) DEFAULT NULL COLLATE NOCASE,
  "content" text COLLATE NOCASE,
  "sender_type" varchar(64) DEFAULT NULL COLLATE NOCASE,
  "reply_to" varchar(1024) DEFAULT NULL COLLATE NOCASE,
  "creation_date" datetime NOT NULL
);


-- tenants_forbiden
CREATE TABLE IF NOT EXISTS "tenants_forbiden" (
  "address" varchar(50) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("address")
);


-- tenants_quota
CREATE TABLE IF NOT EXISTS "tenants_quota" (
  "tenant" INTEGER NOT NULL,
  "name" varchar(128) DEFAULT NULL COLLATE NOCASE,
  "description" varchar(128) DEFAULT NULL COLLATE NOCASE,
  "max_file_size" INTEGER NOT NULL DEFAULT 0,
  "max_total_size" INTEGER NOT NULL DEFAULT 0,
  "active_users" INTEGER NOT NULL DEFAULT 0,
  "features" text COLLATE NOCASE,
  "price" decimal(10,2) NOT NULL DEFAULT '0.00',
  "avangate_id" varchar(128) DEFAULT NULL COLLATE NOCASE,
  "https_enable" INTEGER NOT NULL DEFAULT 0,
  "security_enable" INTEGER NOT NULL DEFAULT 0,
  "sms_auth" INTEGER NOT NULL DEFAULT 0,
  "branding" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("tenant")
);


-- tenants_quotarow
CREATE TABLE IF NOT EXISTS "tenants_quotarow" (
  "tenant" INTEGER NOT NULL,
  "path" varchar(255) NOT NULL COLLATE NOCASE,
  "counter" INTEGER NOT NULL DEFAULT 0,
  "tag" varchar(1024) DEFAULT NULL COLLATE NOCASE,
  "last_modified" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY ("tenant","path")
);
CREATE INDEX IF NOT EXISTS "tenants_quotarow_last_modified" ON "tenants_quotarow" ("last_modified");


-- tenants_tariff
CREATE TABLE IF NOT EXISTS "tenants_tariff" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "tenant" INTEGER NOT NULL,
  "tariff" INTEGER NOT NULL,
  "stamp" datetime NOT NULL,
  "comment" varchar(255) DEFAULT NULL COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "tenants_tariff_tenant" ON "tenants_tariff" ("tenant");


-- tenants_tariffcoupon
CREATE TABLE IF NOT EXISTS "tenants_tariffcoupon" (
  "coupon" varchar(128) NOT NULL COLLATE NOCASE,
  "tariff" INTEGER NOT NULL,
  "tariff_period" INTEGER NOT NULL,
  "valid_from" datetime DEFAULT NULL,
  "valid_to" datetime DEFAULT NULL,
  PRIMARY KEY ("coupon")
);


-- tenants_template_subscription
CREATE TABLE IF NOT EXISTS "tenants_template_subscription" (
  "source" varchar(38) NOT NULL COLLATE NOCASE,
  "action" varchar(128) NOT NULL COLLATE NOCASE,
  "recipient" varchar(38) NOT NULL COLLATE NOCASE,
  "object" varchar(128) NOT NULL DEFAULT '' COLLATE NOCASE,
  "unsubscribed" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("source","action","recipient","object")
);


-- tenants_template_subscriptionmethod
CREATE TABLE IF NOT EXISTS "tenants_template_subscriptionmethod" (
  "source" varchar(38) NOT NULL COLLATE NOCASE,
  "action" varchar(128) NOT NULL COLLATE NOCASE,
  "recipient" varchar(38) NOT NULL COLLATE NOCASE,
  "sender" varchar(1024) NOT NULL COLLATE NOCASE,
  PRIMARY KEY ("source","action","recipient")
);


-- tenants_tenants
CREATE TABLE IF NOT EXISTS "tenants_tenants" (
  "id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
  "name" varchar(255) NOT NULL COLLATE NOCASE,
  "alias" varchar(100) NOT NULL COLLATE NOCASE,
  "mappeddomain" varchar(100) DEFAULT NULL COLLATE NOCASE,
  "version" INTEGER NOT NULL DEFAULT '1',
  "language" char(10) NOT NULL DEFAULT 'en-US' COLLATE NOCASE,
  "timezone" varchar(50) DEFAULT NULL COLLATE NOCASE,
  "trusteddomains" varchar(1024) DEFAULT NULL COLLATE NOCASE,
  "trusteddomainsenabled" INTEGER NOT NULL DEFAULT '1',
  "status" INTEGER NOT NULL DEFAULT 0,
  "statuschanged" datetime DEFAULT NULL,
  "creationdatetime" datetime NOT NULL,
  "owner_id" varchar(38) DEFAULT NULL COLLATE NOCASE,
  "public" INTEGER NOT NULL DEFAULT 0,
  "publicvisibleproducts" varchar(1024) DEFAULT NULL COLLATE NOCASE,
  "last_modified" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE UNIQUE INDEX IF NOT EXISTS "tenants_tenants_alias" ON "tenants_tenants" ("alias");
CREATE INDEX IF NOT EXISTS "tenants_tenants_last_modified" ON "tenants_tenants" ("last_modified");
CREATE INDEX IF NOT EXISTS "tenants_tenants_mappeddomain" ON "tenants_tenants" ("mappeddomain");
CREATE INDEX IF NOT EXISTS "tenants_tenants_version" ON "tenants_tenants" ("version");


-- tenants_version
CREATE TABLE IF NOT EXISTS "tenants_version" (
  "id" INTEGER NOT NULL,
  "version" varchar(64) NOT NULL COLLATE NOCASE,
  "url" varchar(64) NOT NULL COLLATE NOCASE,
  "visible" INTEGER NOT NULL DEFAULT 0,
  PRIMARY KEY ("id")
);



