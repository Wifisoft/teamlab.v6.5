using System;
using System.Runtime.Serialization;
using ASC.Blogs.Core.Domain;

namespace ASC.Api.Blogs
{
    [DataContract(Name = "tag", Namespace = "")]
    public class BlogTagWrapper
    {

        public BlogTagWrapper(TagStat tagStat)
        {
            Name = tagStat.Name;
            Count = tagStat.Count;
        }

        private BlogTagWrapper()
        {
        }

        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 10)]
        public int Count { get; set; }

        public static BlogTagWrapper GetSample()
        {
            return new BlogTagWrapper()
                       {
                           Count = 10,
                           Name = "Sample tag"
                       };
        }
    }
}