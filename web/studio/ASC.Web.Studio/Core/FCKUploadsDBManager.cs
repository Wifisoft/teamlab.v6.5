using ASC.Common.Data;
using ASC.Web.Studio.Utility;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Web.Studio.Core
{
    class FCKUploadsDBManager
    {
        private static readonly string _databaseID = "webstudio";

        private static DbManager _dbManager
        {
            get
            {
                return DbManager.FromHttpContext(_databaseID);
            }
        }

        public static void SetUploadRelations(string storeDomain, string folderID, string itemID)
        {
            _dbManager.Connection.CreateCommand(@"insert into webstudio_fckuploads (TenantID, StoreDomain, FolderID, ItemID)
                                                      values (@tid, @sd, @fid, @iid)")
                                                            .AddParameter("tid", TenantProvider.CurrentTenantID)
                                                            .AddParameter("sd", storeDomain.ToLower())
                                                            .AddParameter("fid", folderID.ToLower())
                                                            .AddParameter("iid", itemID.ToLower()).ExecuteNonQuery();
        }

        public static string GetFolderID(string storeDomain, string itemID)
        { 
            return _dbManager.ExecuteScalar<string>(new SqlQuery("webstudio_fckuploads").Select("FolderID" )
                                            .Where(Exp.Eq("TenantID", TenantProvider.CurrentTenantID) &
                                                   Exp.Eq("StoreDomain", storeDomain.ToLower()) &
                                                   Exp.Eq("ItemID", itemID.ToLower())));
        }

        public static void RemoveUploadRelation(string storeDomain, string folderID, string itemID)
        { 
            _dbManager.ExecuteNonQuery(new SqlDelete("webstudio_fckuploads")
                                            .Where(Exp.Eq("TenantID", TenantProvider.CurrentTenantID) &
                                                   Exp.Eq("StoreDomain", storeDomain.ToLower()) &
                                                   Exp.Eq("FolderID", folderID.ToLower()) &
                                                   Exp.Eq("ItemID", itemID.ToLower())));
        }

        
    }
}
