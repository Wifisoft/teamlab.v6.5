using System;
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Studio.Core
{
    [Serializable]
    [DataContract]
    public class StudioNotifyBarSettings : ISettings
    {
        [DataMember(Name = "ShowPromotions")]
        public bool ShowPromotions { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{D291A4C1-179D-4ced-895A-E094E809C859}"); }
        }

        public ISettings GetDefault()
        {
            return new StudioNotifyBarSettings() { ShowPromotions = true };
        }

        #endregion
    }
}