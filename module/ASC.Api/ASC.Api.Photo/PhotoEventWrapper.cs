using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.PhotoManager.Data;
using ASC.Specific;

namespace ASC.Api.Photo
{
    [DataContract(Name = "event", Namespace = "")]
    public class PhotoEventWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public long Id { get; set; }

        [DataMember(Order = 2)]
        public string Title { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper Author { get; set; }


        public PhotoEventWrapper(Event @event)
        {
            Id = @event.Id;
            Title = @event.Name;
            Updated = Created = (ApiDateTime)@event.Timestamp;

            Author = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(new Guid(@event.UserID)));
            Description = @event.Description;
        }

        private PhotoEventWrapper()
        {

        }

        public static PhotoEventWrapper GetSample()
        {
            return new PhotoEventWrapper()
                       {
                           Author = EmployeeWraper.GetSample(),
                           Created = (ApiDateTime) DateTime.Now,
                           Updated = (ApiDateTime) DateTime.Now,
                           Description = "Sample description",
                           Id = 10,
                           Title = "Sample title"
                       };
        }
    }
}