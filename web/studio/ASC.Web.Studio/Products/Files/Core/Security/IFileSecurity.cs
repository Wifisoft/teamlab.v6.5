using System;

namespace ASC.Files.Core.Security
{
    public interface IFileSecurity
    {
        bool CanRead(FileEntry file, Guid userId);

        bool CanCreate(FileEntry file, Guid userId);

        bool CanEdit(FileEntry file, Guid userId);

        bool CanDelete(FileEntry file, Guid userId);
    }
}