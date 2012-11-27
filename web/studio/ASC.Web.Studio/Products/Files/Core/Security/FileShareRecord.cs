using System;

namespace ASC.Files.Core.Security
{
    public class FileShareRecord
    {
        public int Tenant { get; set; }

        public object EntryId { get; set; }

        public FileEntryType EntryType { get; set; }

        public Guid Subject { get; set; }

        public Guid Owner { get; set; }

        public FileShare Share { get; set; }

        public int Level { get; set; }
    }
}