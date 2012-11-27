using System;

namespace ASC.Core.Users
{
    [Flags]
    public enum EmployeeActivationStatus
    {
        NotActivated = 0,
        Activated = 1,
        Pending = 2,
    }

    [Flags]
    public enum MobilePhoneActivationStatus
    {
        NotActivated = 0,
        Activated = 1
    }
}