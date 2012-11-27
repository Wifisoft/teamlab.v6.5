using System.Runtime.Serialization;

namespace ASC.Api.Comments.Model
{
    [DataContract(Name = "comment_count", Namespace = "")]
    public class CommentCount
    {
        public CommentCount(long total, long readed)
        {
            Total = total;
            Readed = readed;
        }

        [DataMember(Order = 1)]
        public long Total { get; set; }

        [DataMember(Order = 2)]
        public long Readed { get; set; }
    }
}