using System;
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Community.Wiki.Common
{
    [Serializable]
    [DataContract]
    public class WikiWidgetSettings : ISettings
    {
        [DataMember(Name = "NewPageCount")]
        public int NewPageCount { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{4D52DB1C-2441-46ba-9DB3-CEF649A6D510}"); }
        }

        public ISettings GetDefault()
        {
            return new WikiWidgetSettings() { NewPageCount = 3 };
        }

        #endregion
    }
}
