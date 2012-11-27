using System;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "search", Namespace = "")]
    public class SearchWrapper
    {
        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }

        [DataMember(Order = 10)]
        public SearchItemWrapper[] Items  { get; set; }

        public SearchWrapper(SearchGroup searchGroup)
        {
            ProjectOwner = new SimpleProjectWrapper(searchGroup.ProjectID, searchGroup.ProjectTitle);
            if (searchGroup.Items != null) Items = searchGroup.Items.Select(x => new SearchItemWrapper(x)).ToArray();
        }

        private SearchWrapper()
        {

        }

        public static SearchWrapper GetSample()
        {
            return new SearchWrapper()
            {
                ProjectOwner = SimpleProjectWrapper.GetSample(),
                Items = new SearchItemWrapper[] { SearchItemWrapper.GetSample(), SearchItemWrapper.GetSample(), SearchItemWrapper.GetSample() }
            };

        }
    }
}