using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace ASC.Web.Core.Users.Activity
{
    public interface IUserActivityControlLoader
    {
        Control LoadControl(Guid userID);
    }
}
