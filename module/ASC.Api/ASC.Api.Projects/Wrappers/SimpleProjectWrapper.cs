using System;
using System.Runtime.Serialization;
using ASC.Projects.Core.Domain;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "project", Namespace = "")]
    public class SimpleProjectWrapper
    {

        [DataMember(Order = 60)]
        public int Id { get; set; }

        [DataMember(Order = 61)]
        public string Title { get; set; }

        [DataMember(Order = 62)]
        public int Status { get; set; }

        ///<summary>
        ///</summary>
        public SimpleProjectWrapper()
        {
            
        }

        ///<summary>
        ///</summary>
        public SimpleProjectWrapper(Project project)
        {
            Id = project.ID;
            Title = project.Title;
            Status = (int)project.Status;
        }

        ///<summary>
        ///</summary>
        ///<param name="projectId"></param>
        ///<param name="projectTitle"></param>
        public SimpleProjectWrapper(int projectId, string projectTitle)
        {
            Id = projectId;
            Title = projectTitle;
        }

        public static SimpleProjectWrapper GetSample()
        {
            return new SimpleProjectWrapper(123,"Sample project");
        }
    }
}