using System;
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Studio.Core
{
    [Serializable]
    [DataContract]
    public class StudioAdminMessageSettings : ISettings
    {
        [DataMember(Name = "Enable")]
        public bool Enable { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{28902650-58A9-11E1-B6A9-0F194924019B}"); }
        }

        public ISettings GetDefault()
        {
            return new StudioAdminMessageSettings { Enable = false };
        }

        #endregion
    }
}
