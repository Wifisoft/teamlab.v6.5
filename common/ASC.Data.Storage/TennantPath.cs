using System;
using System.Globalization;

namespace ASC.Data.Storage
{
    public class TennantPath
    {
        public static string CreatePath(string tenant)
        {
            if (tenant == null) throw new ArgumentNullException("tenant");
            //Try parse first
            long tennantId;
            if (long.TryParse(tenant, NumberStyles.Integer, CultureInfo.InvariantCulture, out tennantId))
            {
                return tennantId == 0
                           ? tennantId.ToString(CultureInfo.InvariantCulture)
                           : tennantId.ToString("00/00/00", CultureInfo.InvariantCulture);
                //Make path
            }
            return tenant;
        }
    }
}