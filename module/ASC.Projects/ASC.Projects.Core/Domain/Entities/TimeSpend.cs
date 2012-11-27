using System;

namespace ASC.Projects.Core.Domain
{
    public class TimeSpend : DomainObject<Int32>
    {
        public Task Task { get; set; }

        public DateTime Date { get; set; }

        public float Hours { get; set; }

        public Guid Person { get; set; }

        public String Note { get; set; }
    }
}
