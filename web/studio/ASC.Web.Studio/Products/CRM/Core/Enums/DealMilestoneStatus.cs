#region Import

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Web.CRM.Classes;

#endregion

namespace ASC.CRM.Core
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum DealMilestoneStatus
    {
        Open = 0,
        ClosedAndWon = 1,
        ClosedAndLost = 2
    }

}