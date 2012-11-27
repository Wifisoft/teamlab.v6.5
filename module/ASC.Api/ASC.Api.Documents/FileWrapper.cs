using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Specific;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    [DataContract(Name = "file", Namespace = "")]
    public class FileWrapper : FileEntryWrapper
    {
        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public object FolderId { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public int Version { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public String ContentLength { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public FileStatus FileStatus { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public String ViewUrl { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="file"></param>
        public FileWrapper(File file):base(file)
        {


            FolderId = file.FolderID;
            Version = file.Version;
            ContentLength = file.ContentLengthString;
            FileStatus = file.FileStatus;
            try
            {
                ViewUrl = file.ViewUrl;
            }
            catch (Exception)
            {
                //Don't catch anything here because of httpcontext
            }
        }

        private FileWrapper()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FileWrapper GetSample()
        {
            return new FileWrapper()
                       {
                           Access = FileShare.ReadWrite,
                           Updated = (ApiDateTime) DateTime.UtcNow,
                           Created = (ApiDateTime) DateTime.UtcNow,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Id = new Random().Next(),
                           RootFolderType = FolderType.BUNCH,
                           SharedByMe = false,
                           Title = "Some titile",
                           UpdatedBy = EmployeeWraper.GetSample(),
                           ContentLength = 12345.ToString(),FileStatus = FileStatus.IsNew,FolderId = 12334,Version = 3,ViewUrl = "http://www.teamlab.com/viewfile?fileid=2221"
                       };
        }
    }
}