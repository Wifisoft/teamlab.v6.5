namespace ASC.Projects.Core.Domain
{
    public class Message : ProjectEntity
    {
        public string Content { get; set; }

        public int CommentsCount { get; set; }
    }
}
