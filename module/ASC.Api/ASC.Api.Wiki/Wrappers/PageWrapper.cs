﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Employee;
using ASC.Specific;
using ASC.Web.UserControls.Wiki.Data;

namespace ASC.Api.Wiki.Wrappers
{
    [DataContract(Name = "page", Namespace = "")]
    public class PageWrapper
    {
        [DataMember(Order = 0)]
        public string Name { get; set; }

        [DataMember(Order = 1)]
        public string Content { get; set; }

        [DataMember(Order = 2)]
        public EmployeeWraper UpdatedBy { get; set; }

        [DataMember(Order = 3)]
        public DateTime Updated { get; set; }

        public PageWrapper(Page page)
        {
            Name = page.PageName;
            Content = page.Body;
            UpdatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(page.UserID));
            Updated = page.Date;
        }

        public PageWrapper()
        {
            
        }

        public static PageWrapper GetSample()
        {
            return new PageWrapper
            {
                Name = "Page name",
                Content = "Page content",
                UpdatedBy = EmployeeWraper.GetSample(),
                Updated = (ApiDateTime) DateTime.UtcNow
            };
        }
    }
}
