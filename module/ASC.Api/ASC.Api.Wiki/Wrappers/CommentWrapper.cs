﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Specific;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Api.Employee;

namespace ASC.Api.Wiki.Wrappers
{
    [DataContract(Name = "comment", Namespace = "")]
    public class CommentWrapper
    {
        [DataMember(Order = 0)]
        public Guid Id { get; set; }

        [DataMember(Order = 1)]
        public Guid ParentId { get; set; }

        [DataMember(Order = 2)]
        public string Page { get; set; }

        [DataMember(Order = 3)]
        public string Content { get; set; }

        [DataMember(Order = 4)]
        public EmployeeWraper Author { get; set; }

        [DataMember(Order = 4)]
        public DateTime LastModified { get; set; }

        [DataMember(Order = 5)]
        public bool Inactive { get; set; }

        public CommentWrapper(Comment comment)
        {
            Id = comment.Id;
            ParentId = comment.ParentId;
            Page = comment.PageName;
            Content = comment.Body;
            Author = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(comment.UserId));
            LastModified = comment.Date;
            Inactive = comment.Inactive;
        }

        public CommentWrapper()
        {
            
        }

        public static CommentWrapper GetSample()
        {
            return new CommentWrapper
                       {
                           Author = EmployeeWraper.GetSample(),
                           Content = "Comment content",
                           Id = Guid.NewGuid(),
                           Page = "Some page",
                           Inactive = false,
                           LastModified = (ApiDateTime)DateTime.UtcNow,
                           ParentId = Guid.NewGuid()
                       };
        }
    }
}
