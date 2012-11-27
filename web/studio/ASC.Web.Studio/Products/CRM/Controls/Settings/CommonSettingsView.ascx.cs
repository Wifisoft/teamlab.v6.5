#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ASC.CRM.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.CRM.Classes;
using ASC.Web.Studio.Utility;
using AjaxPro;
using ASC.Common.Threading.Progress;

#endregion

namespace ASC.Web.CRM.Controls.Settings
{
    [AjaxNamespace("AjaxPro.CommonSettingsView")]
    public partial class CommonSettingsView : BaseUserControl
    {
        #region Property

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Settings/CommonSettingsView.ascx"); } }

        protected SMTPServerSetting Settings { get { return Global.TenantSettings.SMTPServerSetting; } }
        
        protected List<CurrencyInfo> AllCurrencyRates { get; set; }

        protected bool MobileVer = false;
        
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            Utility.RegisterTypeForAjax(typeof(CommonSettingsView));
            AllCurrencyRates = CurrencyProvider.GetAll().Where(n => n.IsConvertable).ToList();
        }

        [AjaxMethod]
        public IProgressItem StartExportData()
        {
            if (!CRMSecurity.IsAdmin)
                throw new Exception();

            return ExportToCSV.Start();
        }
        
        [AjaxMethod]
        public void SaveChangeSettings(String defaultCurrency)
        {
            if (!CRMSecurity.IsAdmin)
                throw new Exception();

            var tenantSettings = Global.TenantSettings;

            tenantSettings.DefaultCurrency = CurrencyProvider.Get(defaultCurrency);

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);

        }

        [AjaxMethod]
        public void SaveSMTPSettings(string host, int port, bool authentication, string hostLogin, string hostPassword, string senderDisplayName, string senderEmailAddress, bool enableSSL)
        {
            var crmSettings = Global.TenantSettings;

            crmSettings.SMTPServerSetting = new SMTPServerSetting
            {
                Host = host,
                Port = port,
                RequiredHostAuthentication = authentication,
                HostLogin = hostLogin,
                HostPassword = hostPassword,
                SenderDisplayName = senderDisplayName,
                SenderEmailAddress = senderEmailAddress,
                EnableSSL = enableSSL
            };

            SettingsManager.Instance.SaveSettings(crmSettings, TenantProvider.CurrentTenantID);
        }

        [AjaxMethod]
        public IProgressItem GetStatus()
        {
            return ExportToCSV.GetStatus();
        }

        [AjaxMethod]
        public IProgressItem Cancel()
        {
            ExportToCSV.Cancel();

            return GetStatus();
        }

        #endregion

    }
}