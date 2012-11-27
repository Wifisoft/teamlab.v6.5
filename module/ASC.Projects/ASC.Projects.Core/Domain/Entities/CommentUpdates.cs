using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASC.Projects.Core.Domain
{
    public class CommentUpdate
    {
        public Comment Comment { get; set; }
        public int CommentedId { get; set; }
        public string CommentedTitle { get; set; }
        public int ProjectId { get; set; }
        public EntityType CommentedType { get; set; }
    }
}
