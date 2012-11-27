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
    [DataContract(Name = "folder", Namespace = "")]
    public class FolderWrapper : FileEntryWrapper
    {
        /// <summary>
        /// </summary>
        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public object ParentId { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public int FilesCount { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public int FoldersCount { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool IsShareable { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="folder"></param>
        public FolderWrapper(Folder folder):base(folder)
        {
            ParentId = folder.ParentFolderID;
            FilesCount = folder.TotalFiles;
            FoldersCount = folder.TotalSubFolders;
            IsShareable = folder.Shareable;
        }

        private FolderWrapper():base()
        {
 
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FolderWrapper GetSample()
        {
            return new FolderWrapper()
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
                           FilesCount = new Random().Next(), FoldersCount = new Random().Next(), ParentId = new Random().Next() ,IsShareable = false
                       };
        }
    }
}