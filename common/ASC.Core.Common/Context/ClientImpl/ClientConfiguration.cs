using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using ASC.Core.Configuration;
using ASC.Core.Tenants;

namespace ASC.Core
{
    class ClientConfiguration : IConfigurationClient
    {
        private readonly ITenantService tenantService;
        private readonly SmtpSettings configSmtpSettings;


        public bool Standalone
        {
            get { return ConfigurationManager.AppSettings["core.base-domain"] == "localhost"; }
        }

        public SmtpSettings SmtpSettings
        {
            get
            {
                return configSmtpSettings ?? Deserialize(GetSetting("SmtpSettings"));
            }
            set
            {
                if (configSmtpSettings != null)
                {
                    throw new InvalidOperationException("Mail Settings defined in the configuration file.");
                }
                SaveSetting("SmtpSettings", Serialize(value));
            }
        }


        public ClientConfiguration(ITenantService service)
        {
            this.tenantService = service;
            this.configSmtpSettings = GetSmtpSettingsFromConfig();
        }


        public void SaveSetting(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var data = value != null ? Crypto.GetV(Encoding.UTF8.GetBytes(value), 2, true) : null;
            tenantService.SetTenantSettings(Tenant.DEFAULT_TENANT, key, data);
        }

        public string GetSetting(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var data = tenantService.GetTenantSettings(Tenant.DEFAULT_TENANT, key);
            return data != null ? Encoding.UTF8.GetString(Crypto.GetV(data, 2, false)) : null;
        }


        private string Serialize(SmtpSettings smtp)
        {
            if (smtp == null) return null;
            return string.Join("#",
                new[] {
                        smtp.CredentialsDomain,
                        smtp.CredentialsUserName,
                        smtp.CredentialsUserPassword,
                        smtp.Host,
                        smtp.Port.HasValue ? smtp.Port.Value.ToString() : string.Empty,
                        smtp.SenderAddress,
                        smtp.SenderDisplayName,
                        smtp.EnableSSL.ToString()});
        }

        private SmtpSettings Deserialize(string value)
        {
            if (string.IsNullOrEmpty(value)) return new SmtpSettings();

            var props = value.Split(new[] { '#' }, StringSplitOptions.None);
            props = Array.ConvertAll(props, p => !string.IsNullOrEmpty(p) ? p : null);
            return new SmtpSettings
            {
                CredentialsDomain = props[0],
                CredentialsUserName = props[1],
                CredentialsUserPassword = props[2],
                Host = props[3],
                Port = String.IsNullOrEmpty(props[4]) ? null : (int?)Int32.Parse(props[4]),
                SenderAddress = props[5],
                SenderDisplayName = props[6],
                EnableSSL = 7 < props.Length && !string.IsNullOrEmpty(props[7]) && Convert.ToBoolean(props[7])
            };
        }

        private SmtpSettings GetSmtpSettingsFromConfig()
        {
            var smtpClient = new SmtpClient();
            if (!string.IsNullOrEmpty(smtpClient.Host))
            {
                // section /configuration/system.net/mailSettings/smtp not empty.
                var smtpSettings = new SmtpSettings();

                smtpSettings.Host = smtpClient.Host;
                smtpSettings.Port = smtpClient.Port;
                smtpSettings.EnableSSL = 400 < smtpClient.Port;

                var credentials = smtpClient.Credentials as NetworkCredential;
                if (credentials != null)
                {
                    smtpSettings.CredentialsDomain = credentials.Domain;
                    smtpSettings.CredentialsUserName = credentials.UserName;
                    smtpSettings.CredentialsUserPassword = credentials.Password;
                }

                var mailMessage = new MailMessage();
                if (mailMessage.From != null)
                {
                    smtpSettings.SenderAddress = mailMessage.From.Address;
                    smtpSettings.SenderDisplayName = mailMessage.From.DisplayName;
                }

                return smtpSettings;
            }
            else
            {
                return null;
            }
        }
    }
}
