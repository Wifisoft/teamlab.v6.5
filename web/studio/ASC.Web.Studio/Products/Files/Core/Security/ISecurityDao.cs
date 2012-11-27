using System;
using System.Collections.Generic;

namespace ASC.Files.Core.Security
{
    public interface ISecurityDao : IDisposable
    {
        void SetShare(FileShareRecord r);

        IEnumerable<FileShareRecord> GetShares(IEnumerable<Guid> subjects);

        IEnumerable<FileShareRecord> GetShares(params FileEntry[] entry);

        void RemoveSubject(Guid subject);
    }
}