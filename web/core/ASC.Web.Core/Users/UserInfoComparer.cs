using System;
using System.Collections.Generic;
using ASC.Core.Users;

namespace ASC.Web.Core.Users
{
    public class UserInfoComparer : IComparer<UserInfo>
    {
        public static readonly IComparer<UserInfo> Default = new UserInfoComparer(UserSortOrder.DisplayName, false);


        public UserSortOrder SortOrder { get; set; }
        public bool Descending { get; set; }


        public UserInfoComparer(UserSortOrder sortOrder)
            : this(sortOrder, false)
        {
        }

        public UserInfoComparer(UserSortOrder sortOrder, bool descending)
        {
            SortOrder = sortOrder;
            Descending = descending;
        }


        public int Compare(UserInfo x, UserInfo y)
        {
            int result = 0;
            switch (SortOrder)
            {
                case UserSortOrder.DisplayName:
                    result = UserFormatter.Compare(x, y);
                    break;

                case UserSortOrder.Email:
                    result = String.Compare(x.Email, y.Email);
                    break;

                case UserSortOrder.Department:
                    result = String.Compare(x.Department, y.Department);
                    break;

                case UserSortOrder.Post:
                    result = String.Compare(x.Title, y.Title);
                    break;

                case UserSortOrder.Status:
                    result = String.Compare(x.Status.ToString(), y.Status.ToString());
                    break;

                case UserSortOrder.BirthDate:
                    result = x.BirthDate.GetValueOrDefault().CompareTo(y.BirthDate.GetValueOrDefault());
                    break;

                case UserSortOrder.WorkFromDate:
                    result = x.WorkFromDate.GetValueOrDefault().CompareTo(y.WorkFromDate.GetValueOrDefault());
                    break;
            }

            return !Descending ? result : -result;
        }
    }
}
