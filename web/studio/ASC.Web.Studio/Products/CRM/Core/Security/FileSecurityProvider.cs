#region Import

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security;
using System.Threading;
using ASC.CRM.Core.Entities;
using ASC.Common.Security;
using System.Linq;
using System.Linq.Expressions;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Users.Activity;
using Action = ASC.Common.Security.Authorizing.Action;
using Constants = ASC.Core.Users.Constants;
using SecurityContext = ASC.Core.SecurityContext;
using ASC.Web.Files.Api;
using ASC.Files.Core.Security;
using ASC.Files.Core;

#endregion

namespace ASC.CRM.Core
{

    public class FileSecurity : IFileSecurity
    {


        #region IFileSecurity Members

        public bool CanCreate(FileEntry file, Guid userId)
        {
            return true;
        }

        public bool CanDelete(FileEntry file, Guid userId)
        {
            return file.CreateBy == userId || file.ModifiedBy == userId || CRMSecurity.IsAdmin;

        }

        public bool CanEdit(FileEntry file, Guid userId)
        {
            return file.CreateBy == userId || file.ModifiedBy == userId || CRMSecurity.IsAdmin;
        }

        public bool CanRead(FileEntry file, Guid userId)
        {
            return true;
        }

        #endregion
    }

    public class FileSecurityProvider : IFileSecurityProvider
    {
        public IFileSecurity GetFileSecurity(string data)
        {
            return new FileSecurity();
        }

    }
}