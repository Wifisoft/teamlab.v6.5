using System;

namespace ASC.Projects.Core.Domain
{
    public class Comment : DomainObject<Guid>
    {
        public Guid Parent { get; set; }

        public string Content { get; set; }

        public bool Inactive { get; set; }


        public String TargetUniqID { get; set; }


        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }


        public override int GetHashCode()
        {
            return (GetType().FullName + "|" + Content + "|" + CreateBy.GetHashCode() + "|" + Parent.GetHashCode()).GetHashCode();
        }
    }
}