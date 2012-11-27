using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Web.Files.Services.WCFService;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    [DataContract(Name = "content", Namespace = "")]
    public class FolderContentWrapper
    {
        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<FileWrapper> Files { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<FolderWrapper> Folders { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public FolderWrapper Current { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public object PathParts { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="folderItems"></param>
        public FolderContentWrapper(DataWrapper folderItems)
        {
            Files = folderItems.Files.Select(x => new FileWrapper(x)).ToList();
            Folders = folderItems.Folders.Select(x => new FolderWrapper(x)).ToList();
            Current = new FolderWrapper(folderItems.FolderInfo);
            PathParts = folderItems.FolderPathParts.Select(x => new { key = x.Key, path = x.Value });
        }

        private FolderContentWrapper()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FolderContentWrapper GetSample()
        {
            return new FolderContentWrapper()
            {
                Current = FolderWrapper.GetSample(),
                Files = new List<FileWrapper>(new[] { FileWrapper.GetSample(), FileWrapper.GetSample() }),
                Folders = new List<FolderWrapper>(new[] { FolderWrapper.GetSample(), FolderWrapper.GetSample() }),
                PathParts = new
                {
                    key = "Key",
                    path = "//path//to//folder"
                }
            };
        }
    }
}