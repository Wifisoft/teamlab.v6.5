using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Employee;
using ASC.Specific;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Data;

namespace ASC.Api.Wiki.Wrappers
{
    [DataContract(Name = "file", Namespace = "")]
    public class FileWrapper
    {
        [DataMember(Order = 0)]
        public string Name { get; set; }

        [DataMember(Order = 1)]
        public EmployeeWraper UpdatedBy { get; set; }

        [DataMember(Order = 2)]
        public DateTime Updated { get; set; }

        [DataMember(Order = 3)]
        public string Location { get; set; }

        public FileWrapper(File file)
        {
            Name = file.FileName;
            UpdatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(file.UserID));
            Updated = file.Date;
            Location = file.FileLocation;
        }

        public FileWrapper()
        {
            
        }

        public static FileWrapper GetSample()
        {
            return new FileWrapper
                       {
                           Name = "File name",
                           Location = WikiEngine.GetFileLocation("File name"),
                           Updated = (ApiDateTime)DateTime.UtcNow,
                           UpdatedBy = EmployeeWraper.GetSample()
                       };
        }
    }
}
