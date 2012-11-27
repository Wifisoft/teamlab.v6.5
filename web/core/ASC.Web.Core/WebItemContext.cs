using System;
using System.Collections.Generic;
using ASC.Common.Security.Authorizing;
using ASC.Web.Core.Subscriptions;
using ASC.Web.Core.Users.Activity;

namespace ASC.Web.Core
{
    public class WebItemContext
    {
        public IGlobalHandler GlobalHandler { get; set; }

        public SpaceUsageStatManager SpaceUsageStatManager { get; set; }

        public Func<string> GetCreateContentPageAbsoluteUrl { get; set; }

        public ISubscriptionManager SubscriptionManager { get; set; }

        public List<IUserActivityPublisher> UserActivityPublishers { get; set; }

        public Func<List<string>> UserOpportunities { get; set; }

        public Func<List<string>> AdminOpportunities { get; set; }

        public string SmallIconFileName { get; set; }

        public string DisabledIconFileName { get; set; }
        
        public string IconFileName { get; set; }

        public string LargeIconFileName { get; set; }

        public string ThemesFolderVirtualPath { get; set; }

        public string ImageFolder { get; set; }

        public string AssemblyName { get; set; }

        public Type HtmlInjectionProviderType { get; set; }

        public int DefaultSortOrder { get; set; }

        public bool UseEmbeddedMakeUp { get; set; }

        public bool SortDisabled { get; set; }

        public bool HasComplexHierarchyOfAccessRights { get; set; }

        public bool CanNotBeDisabled { get; set; }

        public WebItemContext()
        {
            UserActivityPublishers = new List<IUserActivityPublisher>();
            GetCreateContentPageAbsoluteUrl = () => string.Empty;
        }
    }
}
