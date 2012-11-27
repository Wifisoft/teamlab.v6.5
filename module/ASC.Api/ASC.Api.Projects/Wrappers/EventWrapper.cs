using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "event", Namespace = "")]
    public class EventWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public int ProjectId { get; set; }

        [DataMember(Order = 9)]
        public string Title { get; set; }

        [DataMember(Order = 10)]
        public string Desription { get; set; }

        [DataMember(Order = 11)]
        public ApiDateTime From { get; set; }

        [DataMember(Order = 12)]
        public ApiDateTime To { get; set; }

        [DataMember(Order = 50)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 51)]
        public EmployeeWraper CreatedBy { get; set; }

        private ApiDateTime _updated;

        [DataMember(Order = 50)]
        public ApiDateTime Updated
        {
            get { return _updated >= Created ? _updated : Created; }
            set { _updated = value; }
        }


        [DataMember(Order = 41)]
        public EmployeeWraper UpdatedBy { get; set; }

        public EventWrapper(Event @event)
        {
            Created = (ApiDateTime) @event.CreateOn;
            CreatedBy = EmployeeWraper.Get(@event.CreateBy);
            Updated = (ApiDateTime) @event.LastModifiedOn;

            if (@event.CreateBy != @event.LastModifiedBy)
                UpdatedBy = EmployeeWraper.Get(@event.LastModifiedBy);
            Id = @event.ID;
            Desription = @event.Description;
            Title = @event.Title;
            ProjectId = @event.Project.ID;
            From = (ApiDateTime) @event.FromDate;
            To = (ApiDateTime) @event.ToDate;
        }

        private EventWrapper()
        {
           
        }

        public static EventWrapper GetSample()
        {
            return new EventWrapper()
                       {
                           Created = (ApiDateTime) DateTime.Now,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Desription = "sample description",
                           From = (ApiDateTime) DateTime.Now,
                           To = (ApiDateTime) DateTime.Now,
                           Id = 10,
                           ProjectId = 123,
                           Title = "Sample Title",
                           Updated = (ApiDateTime) DateTime.Now,
                           UpdatedBy = EmployeeWraper.GetSample()
                       };
        }
    }
}