using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASC.Web.Studio.UserControls.Users
{
    [Serializable]
    public sealed class UserData
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
