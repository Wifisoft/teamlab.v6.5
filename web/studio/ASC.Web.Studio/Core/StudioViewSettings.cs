using System;
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Studio.Core
{
    [Serializable]
    [DataContract]
    public class StudioViewSettings : ISettings
    {
        [DataMember(Name = "VisibleSidePanel")]
        public bool VisibleSidePanel { get; set; }

        [DataMember(Name = "LeftSidePanel")]
        public bool LeftSidePanel { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{F291A4C1-079D-4ced-895A-E094E809C858}"); }
        }

        public ISettings GetDefault()
        {
            return new StudioViewSettings {VisibleSidePanel = true, LeftSidePanel = false};
        }

        #endregion
    }
}