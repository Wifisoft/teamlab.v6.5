using System.Collections.Generic;
using System.Web;
using ASC.Web.Core;

namespace ASC.Web.Community.Product
{
    public class CommunitySpaceUsageStatManager : SpaceUsageStatManager
    {
        public override List<SpaceUsageStatManager.UsageSpaceStatItem> GetStatData()
        {
            var data = new List<SpaceUsageStatManager.UsageSpaceStatItem>();

            foreach (var wi in WebItemManager.Instance.GetSubItems(CommunityProduct.ID, ItemAvailableState.All))
            {
                var size = this.GetUsedSize(wi.ID);
                if (size >= 0)
                {
                    data.Add(new UsageSpaceStatItem() { Name = wi.Name, ImgUrl = wi.GetIconAbsoluteURL(), SpaceUsage = size, Url = VirtualPathUtility.ToAbsolute(wi.StartURL) });
                }
            }
            return data;
        }
    }
}
