using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Core.Users;

namespace ASC.Web.Studio.Core.Users
{
    public class UserTransferData
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string HashId { get; set; }

        public Guid UserId { get; set; }

        public string ValidationKey { get; set; }

        public MobilePhoneActivationStatus MobilePhoneActivationStatus { get; set; }
    }
}
