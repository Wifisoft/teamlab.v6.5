#region Usings

using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "time", Namespace = "")]
    public class TimeWrapper
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 5)]
        public ApiDateTime Date { get; set; }

        [DataMember(Order = 6)]
        public float Hours { get; set; }

        [DataMember(Order = 6)]
        public string Note { get; set; }

        [DataMember(Order = 7)]
        public int RelatedProject { get; set; }

        [DataMember(Order = 7)]
        public int RelatedTask { get; set; }

        [DataMember(Order = 7)]
        public string RelatedTaskTitle { get; set; }

        [DataMember(Order = 51)]
        public EmployeeWraper CreatedBy { get; set; }

        [DataMember]
        public bool CanEdit { get; set; }


        private TimeWrapper()
        {
        }

        public TimeWrapper(TimeSpend timeSpend)
        {
            Date = (ApiDateTime)timeSpend.Date;
            Hours = timeSpend.Hours;
            Id = timeSpend.ID;
            Note = timeSpend.Note;
            CreatedBy = EmployeeWraper.Get(timeSpend.Person);
            RelatedProject = timeSpend.Task.Project.ID;
            RelatedTask = timeSpend.Task.ID;
            RelatedTaskTitle = timeSpend.Task.Title;
            CanEdit = ProjectSecurity.CanEdit(timeSpend);
        }


        public static TimeWrapper GetSample()
        {
            return new TimeWrapper
                       {
                           Id = 10,
                           Date = (ApiDateTime) DateTime.Now,
                           Hours = 3.5F,
                           Note = "Sample note",
                           RelatedProject = 123,
                           RelatedTask = 13456,
                           RelatedTaskTitle = "Sample task",
                           CreatedBy = EmployeeWraper.GetSample(),
                       };
        }
    }
}
