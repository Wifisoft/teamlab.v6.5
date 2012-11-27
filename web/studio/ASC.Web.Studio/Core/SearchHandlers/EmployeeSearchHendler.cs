using System;
using System.Collections.Generic;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio.Core.SearchHandlers
{
    public class EmployeeSearchHendler : BaseSearchHandlerEx
    {
        public override string AbsoluteSearchURL
        {
            get { return VirtualPathUtility.ToAbsolute("~/employee.aspx") + "?empid=&depid="; }
        }

        public override Guid ModuleID
        {
            get { return new Guid("86d29ee3-5e8b-4835-88fd-02b463de146f"); }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions {ImageFileName = "people_search_icon.png", PartID = Guid.Empty}; }
        }

        public override string PlaceVirtualPath
        {
            get { return "~/employee.aspx"; }
        }

        public override string SearchName
        {
            get { return Resource.EmployeesSearch; }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }

        public override SearchResultItem[] Search(string text)
        {
            var users = new List<UserInfo>();
            var result = new List<SearchResultItem>();

            users.AddRange(CoreContext.UserManager.Search(text, EmployeeStatus.Active));

            foreach (var user in users)
            {
                var sri = new SearchResultItem
                              {
                                  Name = user.DisplayUserName(false),
                                  Description = string.Format("{2}{1} {0}", user.Department, !String.IsNullOrEmpty(user.Department) ? "," : String.Empty, user.Title),
                                  //Description =
                                  //string.Format("{0}: {1}, {2}: {3}",CustomNamingPeople.Substitute<Resources.Resource>("Department"),user.Department,CustomNamingPeople.Substitute<Resources.Resource>("UserPost"),user.Title),
                                  URL = GetEmployeeUrl(user),
                                  Date = user.WorkFromDate,
                                  Additional = new Dictionary<string, object> {{"imageRef", user.GetSmallPhotoURL()}}
                              };
                result.Add(sri);
            }

            return result.ToArray();
        }

        protected string GetEmployeeUrl(UserInfo ui)
        {
            var currentProduct = UserOnlineManager.Instance.GetCurrentProduct();
            return CommonLinkUtility.GetUserProfile(ui.ID, currentProduct != null ? currentProduct.ID : Guid.Empty);
        }
    }
}