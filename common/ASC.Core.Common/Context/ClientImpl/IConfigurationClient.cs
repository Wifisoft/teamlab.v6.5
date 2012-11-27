using System;
using ASC.Core.Configuration;

namespace ASC.Core
{
    public interface IConfigurationClient
    {
        SmtpSettings SmtpSettings
        {
            get;
            set;
        }

        bool Standalone
        {
            get;
        }


        string GetSetting(string key);

        void SaveSetting(string key, string value);
    }
}
