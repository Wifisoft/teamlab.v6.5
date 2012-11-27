using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Core.Users;

namespace ASC.Api.Employee
{
    ///<summary>
    ///</summary>
    [DataContract(Name = "group", Namespace = "")]
    public class GroupWrapperSummary
    {
        ///<summary>
        ///</summary>
        ///<param name="group"></param>
        public GroupWrapperSummary(GroupInfo group)
        {
            Id = group.ID;
            Name = group.Name;
            Manager = Core.CoreContext.UserManager.GetUsers(Core.CoreContext.UserManager.GetDepartmentManager(group.ID)).UserName;
        }

        protected GroupWrapperSummary()
        {
            
        }
        ///<summary>
        ///</summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }


        ///<summary>
        ///</summary>
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 9, EmitDefaultValue = true)]
        public string Manager { get; set; }

        public static GroupWrapperSummary GetSample()
        {
            return new GroupWrapperSummary() { Id = Guid.NewGuid(), Manager = "Jake.Zazhitski", Name="Group Name" };
        }
    }
}