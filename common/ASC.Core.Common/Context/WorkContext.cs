using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using ASC.Core.Notify;
using ASC.Core.Notify.Senders;
using ASC.Core.Tenants;
using ASC.Notify.Engine;
using log4net;
using Constants = ASC.Core.Configuration.Constants;
using NotifyContext = ASC.Notify.Context;

namespace ASC.Core
{
    public static class WorkContext
    {
        private static readonly object syncRoot = new object();
        private static bool notifyStarted;
        private static NotifyContext notifyContext;


        public static NotifyContext NotifyContext
        {
            get
            {
                NotifyStartUp();
                return notifyContext;
            }
        }

        public static string[] AvailableNotifySenders
        {
            get { return new[] { Constants.NotifyEMailSenderSysName, Constants.NotifyMessengerSenderSysName, }; }
        }

        public static string[] DefaultClientSenders
        {
            get { return new[] { Constants.NotifyEMailSenderSysName, }; }
        }


        private static void NotifyStartUp()
        {
            if (notifyStarted) return;
            lock (syncRoot)
            {
                if (notifyStarted) return;

                notifyContext = new NotifyContext();

                INotifySender jabberSender = new NotifyServiceSender();
                INotifySender emailSender = new NotifyServiceSender();

                var postman = ConfigurationManager.AppSettings["core.notify.postman"];

                if ("ases".Equals(postman, StringComparison.InvariantCultureIgnoreCase) || "smtp".Equals(postman, StringComparison.InvariantCultureIgnoreCase))
                {
                    jabberSender = new JabberSender();

                    var properties = new Dictionary<string, string>();
                    var s = CoreContext.Configuration.SmtpSettings;
                    properties["host"] = s.Host;
                    properties["port"] = s.Port.GetValueOrDefault(25).ToString();
                    properties["enableSsl"] = s.EnableSSL.ToString();
                    properties["userName"] = s.CredentialsUserName;
                    properties["password"] = s.CredentialsUserPassword;
                    properties["domain"] = s.CredentialsDomain;
                    if ("ases".Equals(postman, StringComparison.InvariantCultureIgnoreCase))
                    {
                        emailSender = new AWSSender();
                        properties["accessKey"] = ConfigurationManager.AppSettings["ses.accessKey"];
                        properties["secretKey"] = ConfigurationManager.AppSettings["ses.secretKey"];
                        properties["refreshTimeout"] = ConfigurationManager.AppSettings["ses.refreshTimeout"];
                    }
                    else
                    {
                        emailSender = new SmtpSender();
                    }
                    emailSender.Init(properties);
                }

                notifyContext.NotifyService.RegisterSender(Constants.NotifyEMailSenderSysName, new EmailSenderSink(emailSender));
                notifyContext.NotifyService.RegisterSender(Constants.NotifyMessengerSenderSysName, new JabberSenderSink(jabberSender));

                notifyContext.NotifyEngine.Scheduling = ConfigurationManager.AppSettings["core.notify.scheduling"] != "false";
                notifyContext.NotifyEngine.BeforeTransferRequest += NotifyEngine_BeforeTransferRequest;
                notifyContext.NotifyEngine.AfterTransferRequest += NotifyEngine_AfterTransferRequest;
                notifyStarted = true;
            }
        }

        private static void NotifyEngine_BeforeTransferRequest(NotifyEngine sender, NotifyRequest request)
        {
            request.Properties.Add("Tenant", CoreContext.TenantManager.GetCurrentTenant(false));
        }

        private static void NotifyEngine_AfterTransferRequest(NotifyEngine sender, NotifyRequest request)
        {
            var tenant = (request.Properties.Contains("Tenant") ? request.Properties["Tenant"] : null) as Tenant;
            if (tenant != null)
            {
                CoreContext.TenantManager.SetCurrentTenant(tenant);
            }
        }
    }
}