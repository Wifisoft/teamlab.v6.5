using System;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core
{
    [WebZoneAttribute(WebZoneType.Nowhere)]
    public interface IModule : IWebItem
    {
        string ModuleSysName { get; }

        new ModuleContext Context { get; }
    }
}
