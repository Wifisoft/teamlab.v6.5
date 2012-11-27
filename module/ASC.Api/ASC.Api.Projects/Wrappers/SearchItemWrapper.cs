using System;
using System.Runtime.Serialization;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    ///<summary>
    ///</summary>
    [DataContract(Name = "search_item", Namespace = "")]
    public class SearchItemWrapper
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 5)]
        public string Title { get; set; }

        [DataMember(Order = 10)]
        public string Description { get; set; }

        [DataMember(Order = 20)]
        public ApiDateTime Created { get; set; }

        public SearchItemWrapper(SearchItem searchItem)
        {
            Created = (ApiDateTime) searchItem.CreateOn;
            Description = searchItem.Description;
            Id = searchItem.ID;
            Title = searchItem.Title;
            EntityType = searchItem.EntityType;
        }

        private SearchItemWrapper()
        {

        }

        [DataMember(Order = 3)]
        public EntityType EntityType { get; set; }

        public static SearchItemWrapper GetSample()
        {
            return new SearchItemWrapper()
                       {
                           Created = (ApiDateTime) DateTime.Now,
                           Description = "Sample desription",
                           EntityType = ASC.Projects.Core.Domain.EntityType.Project,
                           Id = "345",
                           Title = "Sample title"
                       };
        }
    }
}