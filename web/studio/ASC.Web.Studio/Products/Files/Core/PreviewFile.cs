using System;

namespace ASC.Files.Core
{
    public class PreviewFile
    {
        public long Id { get; set; }
        public long Version { get; set; }
        public long TenantId { get; set; }
        public bool IsConverted { get; set; }
        public int Failed { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
    }
}