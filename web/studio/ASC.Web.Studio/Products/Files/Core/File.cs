using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Files.Core
{
    [Flags]
    [DataContract]
    public enum FileStatus
    {
        [EnumMember] None = 0x0,

        [EnumMember] IsEditing = 0x1,

        [EnumMember] IsNew = 0x2
    }

    [DataContract(Name = "file", Namespace = "")]
    [DebuggerDisplay("{Title} ({ID} v{Version})")]
    public class File : FileEntry
    {
        private FileStatus _status;

        public File()
        {
            Version = 1;
        }

        public object FolderID { get; set; }

        [DataMember(EmitDefaultValue = true, Name = "version", IsRequired = false)]
        public int Version { get; set; }

        public long ContentLength { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "content_length", IsRequired = true)]
        public String ContentLengthString
        {
            get { return FileSizeComment.FilesSizeToString(ContentLength); }
            set { }
        }

        public String ContentType { get; set; }

        public FilterType FilterType
        {
            get
            {
                switch (FileUtility.GetFileTypeByFileName(Title))
                {
                    case FileType.Image:
                        return FilterType.ImagesOnly;
                    case FileType.Document:
                        return FilterType.DocumentsOnly;
                    case FileType.Presentation:
                        return FilterType.PresentationsOnly;
                    case FileType.Spreadsheet:
                        return FilterType.SpreadsheetsOnly;
                }

                return FilterType.None;
            }
        }

        [DataMember(EmitDefaultValue = true, Name = "file_status", IsRequired = false)]
        public FileStatus FileStatus
        {
            get { return GetFileStatus(ID, _status); }
            set { _status = value; }
        }

        public String ThumbnailURL { get; set; }

        public String FileUri
        {
            get { return CommonLinkUtility.GetFileDownloadUrl(ID); }
            set { }
        }

        public String ViewUrl
        {
            get { return CommonLinkUtility.GetFileViewUrl(ID); }
            set { }
        }

        public string ConvertedType { get; set; }

        public object NativeAccessor { get; set; }

        public static FileStatus GetFileStatus(object id, FileStatus currentStatus)
        {
            if (!FileLocker.IsLocked(id))
            {
                currentStatus &= (~FileStatus.IsEditing);
            }
            else
            {
                currentStatus |= FileStatus.IsEditing;
            }
            return currentStatus;
        }
    }
}