using System.Runtime.Serialization;
using System.Diagnostics;

namespace ASC.Web.Files.Services.WCFService
{
    [DataContract(Name = "operation_result")]
    [DebuggerDisplay("Id = {Id}, Op = {OperationType}, Progress = {Progress}, Result = {Result}, Error = {Error}")]
    public class FileOperationResult
    {
        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        [DataMember(Name = "operation", IsRequired = false)]
        public FileOperationType OperationType { get; set; }

        [DataMember(Name = "progress", IsRequired = false)]
        public int Progress { get; set; }

        [DataMember(Name = "source", IsRequired = false)]
        public string Source { get; set; }

        [DataMember(Name = "result", IsRequired = false)]
        public string Result { get; set; }

        [DataMember(Name = "error", IsRequired = false)]
        public string Error { get; set; }

        [DataMember(Name = "processed", IsRequired = false)]
        public string Processed { get; set; }
    }
}