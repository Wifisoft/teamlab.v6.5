using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ASC.PhotoManager.Resources;
using ASC.Web.Core.ModuleManagement.Common;
using System.Collections.Generic;
using ASC.PhotoManager.Model;
using ASC.PhotoManager.Data;

namespace ASC.Web.Community.PhotoManager
{
    public class PhotoStatisticProvider 
        : IStatisticProvider
    {
        private long GetPhotoCount(Guid userID)
        {
            var storage = StorageFactory.GetStorage();
            long count = 0;
            foreach (Album album in storage.GetAlbums(0, userID.ToString()))
            {
                count += album.ImagesCount;
            }
            return count;
        }

        #region IStatisticProvider Members

        public List<StatisticItem> GetAllStatistic(Guid userID)
        {
            return new List<StatisticItem> { GetMainStatistic(userID) };
        }

        public StatisticItem GetMainStatistic(Guid userID)
        {
            return new StatisticItem()
            {
                Count = GetPhotoCount(userID),
                URL = ASC.PhotoManager.PhotoConst.UserPhotosPageUrl+userID.ToString(),
                Name = PhotoManagerResource.MainStatisticName
            };
        }

        #endregion
    }
}
