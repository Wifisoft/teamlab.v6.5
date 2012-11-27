using ASC.Web.Core.WebZones;

namespace ASC.Web.Core
{
    [WebZoneAttribute(WebZoneType.Nowhere)]
    public interface IAddon : IWebItem
    {
        new AddonContext Context { get; }

        void Init(AddonContext context);

        void Shutdown();
    }
}
