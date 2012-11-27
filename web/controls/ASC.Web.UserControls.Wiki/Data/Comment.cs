﻿using System;
using ASC.Core.Tenants;

namespace ASC.Web.UserControls.Wiki.Data
{
    public class Comment : IWikiObjectOwner
    {
        public Guid Id
        {
            get;
            set;
        }

        public Guid ParentId
        {
            get;
            set;
        }

        public string PageName
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }

        public Guid UserId
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public bool Inactive
        {
            get;
            set;
        }

        public Guid OwnerID
        {
            get { return UserId; }
        }

        public object GetObjectId()
        {
            return Id;
        }
    }
}
