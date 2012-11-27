using System;
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Studio.Core
{
    [Serializable]
    [DataContract]
    public class StudioSmsNotificationSettings : ISettings
    {
        [DataMember(Name = "Enable")]
        public bool Enable { get; set; }

        [DataMember(Name = "Optional")]
        public bool Optional { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{2802df61-af0d-40d4-abc5-a8506a5352ff}"); }
        }

        public ISettings GetDefault()
        {
            return new StudioSmsNotificationSettings() { Enable = false, Optional = true };
        }

        #endregion
    }
}