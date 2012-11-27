#region Import

using System;
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

#endregion

namespace ASC.Web.CRM.Classes
{
    [Serializable]
    [DataContract]
    public class SMTPServerSetting
    {

        public SMTPServerSetting()
        {
            Host = String.Empty;
            Port = 0;
            EnableSSL = false;
            RequiredHostAuthentication = false;
            HostLogin = String.Empty;
            HostPassword = String.Empty;
            SenderDisplayName = String.Empty;
            SenderEmailAddress = String.Empty;
        }

        [DataMember]
        public String Host { get; set; }
       
        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public bool EnableSSL { get; set; }

        [DataMember]
        public bool RequiredHostAuthentication { get; set; }

        [DataMember]
        public String HostLogin { get; set; }

        [DataMember]
        public String HostPassword { get; set; }
        
        [DataMember]
        public String SenderDisplayName { get; set; }

        [DataMember]
        public String SenderEmailAddress { get; set; }

    }
    
    [Serializable]
    [DataContract]
    public class CRMSettings : ISettings
    {
        [DataMember(Name = "DefaultCurrency")]
        private string defaultCurrency;

        [DataMember]
        public SMTPServerSetting SMTPServerSetting { get; set; }
        
        [DataMember]
        public Guid WebFormKey { get; set; }
  
        public Guid ID
        {
            get { return new Guid("fdf39b9a-ec96-4eb7-aeab-63f2c608eada"); }
        }

        public CurrencyInfo DefaultCurrency
        {
            get { return CurrencyProvider.Get(defaultCurrency); }
            set { defaultCurrency = value.Abbreviation; }
        }

        [DataMember(Name = "IsConfiguredPortal")]
        public bool IsConfiguredPortal { get; set; }

        public ISettings GetDefault()
        {

            var languageName =  System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
           
            var findedCurrency =
                CurrencyProvider.GetAll().Find(item => String.Compare(item.CultureName, languageName, true) == 0);
            
            if (findedCurrency != null)
                return new CRMSettings
                           {
                               defaultCurrency = findedCurrency.Abbreviation, 
                               IsConfiguredPortal = false, 
                               WebFormKey = Guid.Empty,
                               SMTPServerSetting = new SMTPServerSetting()
                           };
            
            return new CRMSettings
                       {
                           defaultCurrency = "USD", 
                           IsConfiguredPortal = false, 
                           WebFormKey = Guid.Empty,
                           SMTPServerSetting = new SMTPServerSetting()
                       };
        }
    }
}