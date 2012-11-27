using System.Web.UI;

namespace ASC.Web.Core.WebZones
{
    public interface IRenderCustomNavigation : IRenderWebItem
    {
        string RenderCustomNavigation(Page page);

        Control LoadCustomNavigationControl(Page page);
    }
}
