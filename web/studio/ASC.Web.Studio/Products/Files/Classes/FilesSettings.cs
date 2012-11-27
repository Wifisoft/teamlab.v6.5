using System;
using System.Runtime.Serialization;
using ASC.Files.Core;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Files.Classes
{
    [Serializable]
    [DataContract]
    public class FilesSettings : ISettings
    {
        [DataMember(Name = "CompactViewFolder")]
        public bool CompactViewFolder { get; set; }

        [DataMember(Name = "ContentOrderBy")]
        public OrderBy ContentOrderBy { get; set; }

        public ISettings GetDefault()
        {
            return new FilesSettings
                       {
                           CompactViewFolder = true,
                           ContentOrderBy = new OrderBy(SortedByType.DateAndTime, false)
                       };
        }

        public Guid ID
        {
            get { return new Guid("{03B382BD-3C20-4f03-8AB9-5A33F016316E}"); }
        }
    }
}