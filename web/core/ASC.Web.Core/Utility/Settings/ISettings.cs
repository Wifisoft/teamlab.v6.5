using System;

namespace ASC.Web.Core.Utility.Settings
{
    public interface ISettings
    {
        Guid ID { get; }
        ISettings GetDefault();
    }
}
