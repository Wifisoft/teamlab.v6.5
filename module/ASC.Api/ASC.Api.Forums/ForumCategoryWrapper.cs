using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Forum;
using ASC.Specific;

namespace ASC.Api.Forums
{
    [DataContract(Name = "category", Namespace = "")]
    public class ForumCategoryWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public string Title { get; set; }

        [DataMember(Order = 2)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Updated { get; set; }


        [DataMember(Order = 10)]
        public string Description { get; set; }

        public ForumCategoryWrapper(ThreadCategory category, IEnumerable<Thread> threads)
        {
            Title = category.Title;
            Updated = Created = (ApiDateTime)category.CreateDate;

            Description = category.Description;
            Threads = (from thread in threads where thread.IsApproved select new ForumThreadWrapper(thread)).ToList();
        }

        private ForumCategoryWrapper()
        {
        }

        [DataMember(Order = 100)]
        public List<ForumThreadWrapper> Threads { get; set; }

        public static ForumCategoryWrapper GetSample()
        {
            return new ForumCategoryWrapper()
                       {
                           Created = (ApiDateTime)DateTime.Now, 
                           Description = "Sample category", 
                           Title = "Sample title", 
                           Updated = (ApiDateTime)DateTime.Now,
                           Threads = new List<ForumThreadWrapper>() { ForumThreadWrapper.GetSample()}
                       };
        }
    }
}