using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Forum;

namespace ASC.Api.Forums
{
    [DataContract(Name = "forum", Namespace = "")]
    public class ForumWrapper
    {
        [DataMember(Order = 100)]
        public List<ForumCategoryWrapper> Categories { get; set; }

        public ForumWrapper(IEnumerable<ThreadCategory> categories, IEnumerable<Thread> threads)
        {
            Categories = (from threadCategory in categories where threadCategory.Visible
                         select
                             new ForumCategoryWrapper(threadCategory,
                                                      from thread in threads
                                                      where thread.CategoryID == threadCategory.ID select thread)).ToList();
        }

        private ForumWrapper()
        {

        }

        public static ForumWrapper GetSample()
        {
            return new ForumWrapper(){Categories = new List<ForumCategoryWrapper>(){ForumCategoryWrapper.GetSample()}
            };
        }
    }
}