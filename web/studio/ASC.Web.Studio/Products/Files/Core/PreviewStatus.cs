using System.Runtime.Serialization;

namespace ASC.Files.Core
{
    [DataContract(Name = "previewStatus", Namespace = "")]
    public class PreviewStatus
    {
        public long FileId { get; set; }

        public long Version { get; set; }

        [DataMember(Name = "errors")]
        public int ErrorCount { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "converted")]
        public bool IsConverted { get; set; }

        [DataMember(Name = "processing")]
        public bool IsProcessing { get; set; }

        [DataMember(Name = "queue")]
        public bool IsInQueue { get; set; }

        public bool IsFailed
        {
            get { return ErrorCount > 0; }
        }
    }
}