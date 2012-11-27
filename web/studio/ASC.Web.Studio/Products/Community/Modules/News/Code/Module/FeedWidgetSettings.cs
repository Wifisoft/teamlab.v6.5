using System;
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Community.News.Code.Module
{
    [Serializable]
    [DataContract]
    public class FeedWidgetSettings : ISettings
    {
        [DataMember(Name = "NewsCount")]
        public int NewsCount { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{35BB3AFA-E250-4d44-9E14-DC7D3168A72E}"); }
        }

        public ISettings GetDefault()
        {
            return new FeedWidgetSettings() { NewsCount = 3 };
        }

        #endregion
    }
}