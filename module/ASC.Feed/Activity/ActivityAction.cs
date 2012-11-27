using System;

namespace ASC.Feed.Activity
{
    [Flags]
    public enum ActivityAction
    {
        Undefined=0,
        Created=1,
        Updated=2,
        Deleted=4,
        Commented=8,
        Periodic=16,
        Reply=32,
        Liked=64,
        Closed=128,
        Opened=256,
        Shared=512,
        All=int.MaxValue
    }
}