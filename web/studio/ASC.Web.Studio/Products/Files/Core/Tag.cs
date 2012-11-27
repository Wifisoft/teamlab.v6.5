using System;

namespace ASC.Files.Core
{
    [Serializable]
    public class Tag
    {
        public string TagName { get; set; }

        public TagType TagType { get; set; }

        public Guid Owner { get; set; }

        public object EntryId { get; set; }

        public FileEntryType EntryType { get; set; }

        public int Id { get; set; }


        public Tag()
        {

        }

        public Tag(string name, TagType type, Guid owner)
            : this(name, type, owner, null)
        {
        }

        public Tag(string name, TagType type, Guid owner, FileEntry entry)
        {
            TagName = name;
            TagType = type;
            Owner = owner;
            if (entry != null)
            {
                EntryId = entry.ID;
                EntryType = entry is File ? FileEntryType.File : FileEntryType.Folder;
            }
        }


        public static Tag New(Guid owner, FileEntry file)
        {
            return new Tag("new", TagType.New, owner, file);
        }
    }

    [Flags]
    public enum TagType
    {
        New = 1,
        //Favorite = 2,
        System = 4
    }
}