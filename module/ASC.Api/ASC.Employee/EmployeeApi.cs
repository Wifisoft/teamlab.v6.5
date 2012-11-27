using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using ASC.Api.Attributes;
using ASC.Api.Exceptions;
using ASC.Api.Impl;
using ASC.Core;
using ASC.Api.Collections;
using ASC.Core.Users;
using ASC.Specific;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core.Users;

namespace ASC.Api.Employee
{
    ///<summary>
    ///User profiles access
    ///</summary>
    public class EmployeeApi : Interfaces.IApiEntryPoint
    {
        private readonly ApiContext _context;

        ///<summary>
        ///</summary>
        public string Name
        {
            get { return "people"; }
        }


        public EmployeeApi(ApiContext context)
        {
            _context = context;
        }

        ///<summary>
        ///Returns the detailed information about the current user profile
        ///</summary>
        ///<short>
        ///My profile
        ///</short>
        ///<returns>Profile</returns>
        [Read("@self")]
        public EmployeeWraperFull GetMe()
        {
            return new EmployeeWraperFull(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID));
        }

        ///<summary>
        ///Returns the list of profiles for all portal users
        ///</summary>
        ///<short>
        ///All profiles
        ///</short>
        ///<returns>List of profiles</returns>
        /// <remarks>This method returns a partial profile. Use more specific method to get full profile</remarks>
        [Read("")]
        public IEnumerable<EmployeeWraperFull> GetAll()
        {
            var query = CoreContext.UserManager.GetUsers().Where(x => x.Status == EmployeeStatus.Active);
            if ("group".Equals(_context.FilterBy,StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(_context.FilterValue))
            {
                var groupId = new Guid(_context.FilterValue);
                //Filter by group
                query = query.Where(x => CoreContext.UserManager.IsUserInGroup(x.ID,groupId));
                _context.SetDataFiltered();
            }
            return query.Select(x => new EmployeeWraperFull(x)).ToSmartList();
        }

        ///<summary>
        ///Returns the detailed information about the profile of the user with the ID specified in the request
        ///</summary>
        ///<short>
        ///Specific profile
        ///</short>
        ///<param name="username">User name</param>
        ///<returns>User profile</returns>
        [Read("{username}")]
        public EmployeeWraperFull GetById(string username)
        {
            var user = CoreContext.UserManager.GetUserByUserName(username);
            if (user.ID == Core.Users.Constants.LostUser.ID)
            {
                //Try get by id
                try
                {
                    user = CoreContext.UserManager.GetUsers(new Guid(username));
                }
                catch{}
            }

            if (user.ID == Core.Users.Constants.LostUser.ID)
            {
                throw new ItemNotFoundException("User not found");
            }

            return new EmployeeWraperFull(user);
        }

        ///<summary>
        ///Returns the list of profiles for all portal users matching the search query
        ///</summary>
        ///<short>
        ///Search users
        ///</short>
        ///<param name="query">query</param>
        ///<returns>list of users</returns>
        [Read("@search/{query}")]
        public IEnumerable<EmployeeWraperFull> GetSearch(string query)
        {
            return CoreContext.UserManager.GetUsers().Where(x => x.Status == EmployeeStatus.Active &&
                ((x.FirstName != null && x.FirstName.IndexOf(query, StringComparison.OrdinalIgnoreCase) > -1) || (x.LastName != null && x.LastName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) ||
                (x.UserName != null && x.UserName.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Email != null && x.Email.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1) || (x.Contacts != null && x.Contacts.Any(y => y.IndexOf(query, StringComparison.OrdinalIgnoreCase) != -1))))
                .Select(x => new EmployeeWraperFull(x)).ToSmartList();
        }

        /// <summary>
        /// Adds a new portal user with the first and last name, email address and several optional parameters specified in the request
        /// </summary>
        /// <short>
        /// Add new user
        /// </short>
        /// <param name="email">Email</param>
        /// <param name="firstname">First name</param>
        /// <param name="lastname">Last name</param>
        /// <param name="department" optional="true">Department</param>
        /// <param name="title" optional="true">title</param>
        /// <param name="location" optional="true">location</param>
        /// <param name="sex" optional="true">Sex (male|female)</param>
        /// <param name="birthday" optional="true">Birthday</param>
        /// <param name="worksfrom" optional="true">Works from date. If not specified - current will be set</param>
        /// <param name="contacts">List of contacts</param>
        /// <param name="files">Avatar photos (upload using multipart/form-data)</param>
        /// <returns>Newly created user</returns>
        [Create("")]
        public EmployeeWraperFull AddMember(string email, string firstname, string lastname, Guid[] department, string title, string location, string sex, ApiDateTime birthday, ApiDateTime worksfrom, IEnumerable<Contact> contacts, IEnumerable<HttpPostedFileBase> files)
        {
            SecurityContext.DemandPermissions(Core.Users.Constants.Action_AddRemoveUser);
            var password = UserManagerWrapper.GeneratePassword();
            var user = new UserInfo();

            //Validate email
            var address = new MailAddress(email);
            user.Email = address.Address;
            //Set common fields
            user.FirstName = firstname;
            user.LastName = lastname;
            user.Title = title;
            user.Location = location;
            user.Sex = "male".Equals(sex, StringComparison.OrdinalIgnoreCase)
                           ? true
                           : ("female".Equals(sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null);

            user.BirthDate = birthday != null ? birthday.UtcTime.Date : (DateTime?)null;
            user.WorkFromDate = worksfrom != null ? worksfrom.UtcTime.Date : DateTime.UtcNow.Date;

            UpdateContacts(contacts, user);

            user = UserManagerWrapper.AddUser(user, password);

            UpdateDepartments(department, user);
            UpdatePhoto(files, user);
            return new EmployeeWraperFull(user);
        }

        private static void UpdateDepartments(IEnumerable<Guid> department, UserInfo user)
        {
            if (SecurityContext.CheckPermissions(Core.Users.Constants.Action_EditGroups))
            {
                if (department != null)
                {
                    var groups = CoreContext.UserManager.GetUserGroups(user.ID);
                    foreach (var groupInfo in groups)
                    {
                        CoreContext.UserManager.RemoveUserFromGroup(user.ID, groupInfo.ID);
                        user.Department = "";
                    }
                    foreach (var guid in department)
                    {
                        var userDepartment = CoreContext.GroupManager.GetGroupInfo(guid);
                        if (userDepartment != Core.Users.Constants.LostGroupInfo)
                        {
                            user.Department = userDepartment.Name;
                            user.Title = "";
                            CoreContext.UserManager.AddUserIntoGroup(user.ID, guid);
                        }
                    }
                }
            }
        }

        private static void UpdateContacts(IEnumerable<Contact> contacts, UserInfo user)
        {
            DeleteContacts(contacts, user);
            if (contacts != null)
            {
                foreach (var contact in contacts)
                {
                    user.Contacts.Add(contact.Type);
                    user.Contacts.Add(contact.Value);
                }
            }
        }

        private static void DeleteContacts(IEnumerable<Contact> contacts, UserInfo user)
        {
            if (contacts != null)
            {
                foreach (var contact in contacts)
                {
                    var index = user.Contacts.IndexOf(contact.Type);
                    if (index != -1)
                    {
                        //Remove existing
                        user.Contacts.RemoveRange(index, 2);
                    }
                }
            }
        }

        private static void UpdatePhoto(IEnumerable<HttpPostedFileBase> files, UserInfo user)
        {
            if (files != null)
            {
                var file = files.FirstOrDefault(x => x.ContentType.StartsWith("image/") && x.ContentLength > 0);
                if (file != null)
                {
                    if (file.InputStream.CanRead)
                    {
                        //Read a stream
                        var buffer = new byte[file.ContentLength];
                        file.InputStream.Read(buffer, 0, buffer.Length);
                        UserPhotoManager.SaveOrUpdatePhoto(user.ID, buffer);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the data for the selected portal user with the first and last name, email address and/or optional parameters specified in the request
        /// </summary>
        /// <short>
        /// Update user
        /// </short>
        /// <param name="userid">User ID to update</param>
        /// <param name="email">Email</param>
        /// <param name="firstname">First name</param>
        /// <param name="lastname">Last name</param>
        /// <param name="department" optional="true">Department</param>
        /// <param name="title" optional="true">title</param>
        /// <param name="location" optional="true">location</param>
        /// <param name="sex" optional="true">Sex (male|female)</param>
        /// <param name="birthday" optional="true">Birthday</param>
        /// <param name="worksfrom" optional="true">Works from date. If not specified - current will be set</param>
        /// <param name="contacts">List fo contacts</param>
        /// <param name="files">Avatar photos (upload using multipart/form-data)</param>
        /// <param name="disable"></param>
        /// <returns>Newly created user</returns>
        [Update("{userid}")]
        public EmployeeWraperFull UpdateMember(string userid, string email, string firstname, string lastname, Guid[] department, string title, string location, string sex, ApiDateTime birthday, ApiDateTime worksfrom, IEnumerable<Contact> contacts, IEnumerable<HttpPostedFileBase> files, bool? disable)
        {
            SecurityContext.DemandPermissions(Core.Users.Constants.Action_EditUser);

            var user = GetUserInfo(userid);

            //Update it

            //Validate email
            if (!string.IsNullOrEmpty(email))
            {
                var address = new MailAddress(email);
                user.Email = address.Address;
            }
            //Set common fields
            user.FirstName = firstname ?? user.FirstName;
            user.LastName = lastname ?? user.LastName;
            user.Title = title ?? user.Title;
            user.Location = location ?? user.Location;
            user.Sex = ("male".Equals(sex, StringComparison.OrdinalIgnoreCase)
                           ? true
                           : ("female".Equals(sex, StringComparison.OrdinalIgnoreCase) ? (bool?)false : null)) ?? user.Sex;

            user.BirthDate = birthday != null ? birthday.UtcTime.Date : user.BirthDate;
            user.WorkFromDate = worksfrom != null ? worksfrom.UtcTime.Date : user.WorkFromDate;
            //Update contacts
            UpdateContacts(contacts, user);
            UpdateDepartments(department, user);
            if (disable.HasValue)
            {
                user.Status = disable.Value ? EmployeeStatus.Terminated : EmployeeStatus.Active;
                user.TerminatedDate = disable.Value ? DateTime.UtcNow : (DateTime?)null;
            }

            CoreContext.UserManager.SaveUserInfo(user);
            return new EmployeeWraperFull(user);

        }

        /// <summary>
        /// Deletes the user with the ID specified in the request from the portal
        /// </summary>
        /// <short>
        /// Delete user
        /// </short>
        /// <param name="userid">ID of user to delete</param>
        /// <returns></returns>
        [Delete("{userid}")]
        public EmployeeWraperFull DeleteMember(string userid)
        {
            SecurityContext.DemandPermissions(Core.Users.Constants.Action_EditUser);

            var user = GetUserInfo(userid);

            user.Status = EmployeeStatus.Terminated;
            UserPhotoManager.RemovePhoto(Guid.Empty, user.ID);
            CoreContext.UserManager.DeleteUser(user.ID);
            return new EmployeeWraperFull(user);

        }

        /// <summary>
        /// Updates the specified user contact information merging the sent data with the present on the portal
        /// </summary>
        /// <short>
        /// Update user contacts
        /// </short>
        /// <param name="userid">User ID</param>
        /// <param name="contacts">Contacts list</param>
        /// <returns>Updated user profile</returns>
        [Update("{userid}/contacts")]
        public EmployeeWraperFull UpdateMemberContacts(string userid, IEnumerable<Contact> contacts)
        {
            var user = GetUserInfo(userid);
            UpdateContacts(contacts, user);
            CoreContext.UserManager.SaveUserInfo(user);
            return new EmployeeWraperFull(user);
        }

        /// <summary>
        /// Updates the specified user contact information changing the data present on the portal for the sent data
        /// </summary>
        /// <short>
        /// Set user contacts
        /// </short>
        /// <param name="userid">User ID</param>
        /// <param name="contacts">Contacts list</param>
        /// <returns>Updated user profile</returns>
        [Create("{userid}/contacts")]
        public EmployeeWraperFull SetMemberContacts(string userid, IEnumerable<Contact> contacts)
        {
            var user = GetUserInfo(userid);
            user.Contacts.Clear();
            UpdateContacts(contacts, user);
            CoreContext.UserManager.SaveUserInfo(user);
            return new EmployeeWraperFull(user);
        }

        /// <summary>
        /// Updates the specified user contact information deleting the data specified in the request from the portal
        /// </summary>
        /// <short>
        /// Delete user contacts
        /// </short>
        /// <param name="userid">User ID</param>
        /// <param name="contacts">Contacts list</param>
        /// <returns>Updated user profile</returns>
        [Delete("{userid}/contacts")]
        public EmployeeWraperFull DeleteMemberContacts(string userid, IEnumerable<Contact> contacts)
        {
            var user = GetUserInfo(userid);
            DeleteContacts(contacts, user);
            CoreContext.UserManager.SaveUserInfo(user);
            return new EmployeeWraperFull(user);
        }


        /// <summary>
        /// Updates the specified user photo with the image file posted with multipart/form-data
        /// </summary>
        /// <short>
        /// Update user photo
        /// </short>
        /// <param name="userid">ID of user</param>
        /// <param name="files">File posted with multipart/form-data</param>
        /// <returns></returns>
        [Update("{userid}/photo")]
        public EmployeeWraperFull UpdateMemberPhoto(string userid, IEnumerable<HttpPostedFileBase> files)
        {
            var user = GetUserInfo(userid);
            UpdatePhoto(files, user);
            CoreContext.UserManager.SaveUserInfo(user);
            return new EmployeeWraperFull(user);
        }

        /// <summary>
        /// Deletes the photo of the user with the ID specified in the request
        /// </summary>
        /// <short>
        /// Delete user photo
        /// </short>
        /// <param name="userid">ID of user</param>
        /// <returns></returns>
        [Delete("{userid}/photo")]
        public EmployeeWraperFull DeleteMemberPhoto(string userid)
        {
            var user = GetUserInfo(userid);
            UserPhotoManager.RemovePhoto(Guid.Empty, user.ID);
            CoreContext.UserManager.SaveUserInfo(user);
            return new EmployeeWraperFull(user);
        }


        private static UserInfo GetUserInfo(string userNameOrId)
        {
            UserInfo user;
            try
            {
                var userId = new Guid(userNameOrId);
                user = CoreContext.UserManager.GetUsers(userId);
            }
            catch (FormatException)
            {
                user = CoreContext.UserManager.GetUserByUserName(userNameOrId);
            }
            if (user == null || user.ID == Core.Users.Constants.LostUser.ID)
                throw new ItemNotFoundException("user not found");
            return user;
        }
    }
}
