using System;
using System.Runtime.Serialization;
using ASC.Bookmarking.Pojo;

namespace ASC.Api.Bookmarks
{
    [DataContract(Name = "tag", Namespace = "")]
    public class TagWrapper
    {

        public TagWrapper(Tag tagStat)
        {
            Name = tagStat.Name;
            Count = tagStat.Populatiry;
        }

        private TagWrapper()
        {
        }

        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 10)]
        public long Count { get; set; }

        public static TagWrapper GetSample()
        {
            return new TagWrapper()
            {
                Count = 10,
                Name = "Sample tag"
            };
        }
    }
}