using System;
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Community.Bookmarking.Widget
{
    [Serializable]
    [DataContract]
    public class BookmarkingWidgetSettings : ISettings
    {
        [DataMember(Name = "MaxCountBookmarks")]
        public int MaxCountBookmarks { get; set; }

        #region ISettings Members

        public ISettings GetDefault()
        {
            return new BookmarkingWidgetSettings() { MaxCountBookmarks = 3 };
        }

        public Guid ID
        {
            get
            {
                return new Guid("{B028A9F5-28EF-43e8-950E-C205774E4553}");
            }
        }

        #endregion
    }
}
