using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Users;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Microsoft.Practices.ServiceLocation;

namespace ASC.Api.Employee
{
    ///<summary>
    ///</summary>
    [DataContract(Name = "person", Namespace = "")]
    public class EmployeeWraper
    {
        private readonly UserInfo _userInfo;

        ///<summary>
        ///</summary>
        protected EmployeeWraper()
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="userInfo"></param>
        protected EmployeeWraper(UserInfo userInfo)
        {
            _userInfo = userInfo;
            Id = userInfo.ID;


            Title = userInfo.Title;
            try
            {
                AvatarSmall = UserPhotoManager.GetSizedPhotoUrl(userInfo.ID, 64, 64);
            }
            catch
            {
                //Go to hell with these
            }
        }



        ///<summary>
        ///</summary>
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string DisplayName { get { return DisplayUserSettings.GetFullUserName(_userInfo); } }


        ///<summary>
        ///</summary>
        [DataMember(Order = 11)]
        public string Title { get; set; }


        ///<summary>
        ///</summary>
        [DataMember(Order = 20)]
        public string AvatarSmall { get; set; }


        ///<summary>
        ///</summary>
        ///<param name="userId"></param>
        ///<returns></returns>
        public static EmployeeWraper Get(Guid userId)
        {
            try
            {
                return Get(CoreContext.UserManager.GetUsers(userId));
            }
            catch (Exception)
            {
                return Get(Core.Users.Constants.LostUser);
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="userInfo"></param>
        ///<returns></returns>
        /// NOTE: static methods is a hell!!!
        public static EmployeeWraper Get(UserInfo userInfo)
        {
            EmployeeWraper employeeWraper;
            try
            {
                var cacheManager = ServiceLocator.Current.GetInstance<ICacheManager>();
                if (cacheManager != null)
                {
                    employeeWraper = (EmployeeWraper)cacheManager[userInfo.ID.ToString()];
                    if (employeeWraper == null)
                    {
                        employeeWraper = new EmployeeWraper(userInfo);
                        cacheManager.Add(userInfo.ID.ToString(), employeeWraper, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(1)));
                    }
                }
                else
                {
                    employeeWraper = new EmployeeWraper(userInfo);
                }
            }
            catch (NullReferenceException)
            {
                //Service locator is'n configured
                employeeWraper = new EmployeeWraper(userInfo);
            }
            return employeeWraper;
        }

        public static EmployeeWraper GetSample()
        {
            return new EmployeeWraper
                       {
                           AvatarSmall = "url to small avatar",
                           Title = "Manager",
                           Id = Guid.NewGuid(),

                       };
        }
    }
}