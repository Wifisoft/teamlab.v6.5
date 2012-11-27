using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Core.Users;
using ASC.Specific;
using ASC.Web.Core.Users;

namespace ASC.Api.Employee
{
    ///<summary>
    ///</summary>
    [DataContract(Name = "person", Namespace = "")]
    public class EmployeeWraperFull:EmployeeWraper
    {
        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string FirstName { get; set; }
        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string LastName { get; set; }


        ///<summary>
        ///</summary>
        [DataMember(Order = 2)]
        public string UserName { get; set; }


        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string Email { get; set; }

        [DataMember(Order = 12)]
        protected List<Contact> Contacts { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public ApiDateTime Birthday { get; set; }
        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string Sex { get; set; }
        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public EmployeeStatus Status { get; set; }
        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public ApiDateTime Terminated { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string Department { get; set; }
        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public ApiDateTime WorkFrom { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 20)]
        public List<GroupWrapperSummary> Groups { get; set; }
        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string Location { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string Notes { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 20)]
        public string AvatarMedium { get; set; }

        [DataMember(Order = 20)]
        public string Avatar { get; set; }

        ///<summary>
        ///</summary>
        public EmployeeWraperFull()
        {
            
        }

        ///<summary>
        ///</summary>
        ///<param name="userInfo"></param>
        ///<exception cref="ArgumentException"></exception>
        public EmployeeWraperFull(UserInfo userInfo):base(userInfo)
        {
            if (userInfo==Core.Users.Constants.LostUser)
                throw new ArgumentException("user not found");

            Id = userInfo.ID;
            UserName = userInfo.UserName;
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            Birthday = (ApiDateTime) userInfo.BirthDate;
            Sex = userInfo.Sex.HasValue ? userInfo.Sex.Value ? "male" : "female" : "";
            Status = userInfo.Status;
            Terminated = (ApiDateTime) userInfo.TerminatedDate;
            Title = userInfo.Title;
            Department = userInfo.Department;
            WorkFrom = (ApiDateTime) userInfo.WorkFromDate;
            Email = userInfo.Email;
            Location = userInfo.Location;
            Notes = userInfo.Notes;
            Email = userInfo.Email;
            FillConacts(userInfo);
 
            Groups = Core.CoreContext.UserManager.GetUserGroups(userInfo.ID).Select(x=>new GroupWrapperSummary(x)).ToList();

            try
            {
                AvatarSmall = UserPhotoManager.GetSmallPhotoURL(userInfo.ID);
                AvatarMedium = UserPhotoManager.GetMediumPhotoURL(userInfo.ID);
                Avatar = UserPhotoManager.GetBigPhotoURL(userInfo.ID);
            }
            catch (Exception)
            {
                
            }
        }

        private void FillConacts(UserInfo userInfo)
        {
            Contacts = new List<Contact>();
            for (var i = 0; i < userInfo.Contacts.Count; i += 2)
            {
                if (i + 1 < userInfo.Contacts.Count)
                {
                    Contacts.Add(new Contact(userInfo.Contacts[i], userInfo.Contacts[i + 1]));
                }
            }
        }

        public static new EmployeeWraperFull GetSample()
        {
            return new EmployeeWraperFull()
            {
                Avatar = "url to big avatar",
                AvatarSmall = "url to small avatar",
                Contacts =  new List<Contact>() {new Contact("GTalk", "mike@gmail.com") },
                Email = "mike@gmail.com",
                FirstName = "Mike",
                Id = Guid.NewGuid(),
                UserName = "Mike.Zanyatski",
                LastName = "Zanyatski",
                Title = "Manager",
                Groups = new List<GroupWrapperSummary>() { GroupWrapperSummary.GetSample() },
                AvatarMedium                = "url to medium avatar",
                Birthday                = new ApiDateTime(new DateTime(1917,11,7)),
                Department                = "Marketing",
                Location                = "Palo Alto",
                Notes                = "Notes to worker",
                Sex                = "male",
                Status                = EmployeeStatus.Active,
                WorkFrom = new ApiDateTime(new DateTime(1945, 5, 9)),
                Terminated = new ApiDateTime(new DateTime(2029, 12, 12)),
            };
        }
    }
}