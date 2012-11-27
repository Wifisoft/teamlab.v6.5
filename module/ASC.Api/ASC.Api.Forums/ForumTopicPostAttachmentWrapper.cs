using System;
using System.Runtime.Serialization;
using ASC.Forum;
using ASC.Specific;

namespace ASC.Api.Forums
{
    [DataContract(Name = "post", Namespace = "")]
    public class ForumTopicPostAttachmentWrapper : IApiSortableDate
    {
        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string ContentType { get; set; }

        [DataMember(Order = 3)]
        public int Size { get; set; }

        [DataMember(Order = 5)]
        public string Path { get; set; }

        public ForumTopicPostAttachmentWrapper(Attachment attachment)
        {
            ContentType = attachment.ContentType.ToString();
            Updated = Created = (ApiDateTime)attachment.CreateDate;

            Name = attachment.Name;
            Size = attachment.Size;
            Path = attachment.OffsetPhysicalPath;//TODO: add through datastorage
        }

        private ForumTopicPostAttachmentWrapper()
        {
        }

        public static ForumTopicPostAttachmentWrapper GetSample()
        {
            return new ForumTopicPostAttachmentWrapper()
                       {
                           ContentType = "image/jpeg",
                           Created = (ApiDateTime)DateTime.Now,
                           Updated = (ApiDateTime)DateTime.Now,
                           Name = "picture.jpg",
                           Path = "url to file",
                           Size = 122345
                       };
        }
    }
}