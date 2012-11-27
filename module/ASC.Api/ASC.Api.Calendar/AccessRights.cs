using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.Api.Calendar
{
    public class CalendarAccessRights
    {
        public static readonly Action FullAccessAction = new Action(
                                                        new Guid("{0d68b142-e20a-446e-a832-0d6b0b65a164}"),
                                                        "Full Access", false, false);
       
    }
}
