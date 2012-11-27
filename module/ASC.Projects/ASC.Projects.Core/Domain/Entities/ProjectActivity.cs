#region Usings

using System;

#endregion

namespace ASC.Projects.Core.Domain
{
    public class ProjectActivity : DomainObject<long>
    {
        public int Tenant { get; set; }

        public int ProjectId { get; set; }

        public string ProjectTitle { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string ActionText { get; set; }

        public DateTime Date { get; set; }

        public Guid UserId { get; set; }

        public string AdditionalData { get; set; }
    }
}
