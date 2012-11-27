using System;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.SocialMedia.Facebook
{
    class FacebookAccountInfoDao : BaseDao
    {
        public FacebookAccountInfoDao(int tenantID, String storageKey) : base(tenantID, storageKey) { }

        public void CreateNewAccountInfo(FacebookAccountInfo accountInfo)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DeleteAccountInfo(accountInfo.AssociatedID);

                SqlInsert cmdInsert = Insert("sm_facebookaccounts")
                           .ReplaceExists(true)
                           .InColumnValue("access_token", accountInfo.AccessToken)                           
                           .InColumnValue("user_id", accountInfo.UserID)
                           .InColumnValue("associated_id", accountInfo.AssociatedID)
                           .InColumnValue("user_name", accountInfo.UserName);

                DbManager.ExecuteNonQuery(cmdInsert);

                tx.Commit();
            }
        }

        public void DeleteAccountInfo(Guid associatedID)
        {
            DbManager.ExecuteNonQuery(Delete("sm_facebookaccounts").Where(Exp.Eq("associated_id", associatedID)));
        }

        public FacebookAccountInfo GetAccountInfo(Guid associatedID)
        {
            var accounts = DbManager.ExecuteList(GetAccountQuery(Exp.Eq("associated_id", associatedID)));
            return accounts.Count > 0 ? ToFacebookAccountInfo(accounts[0]) : null;
        }

        private SqlQuery GetAccountQuery(Exp where)
        {
            SqlQuery sqlQuery = Query("sm_facebookaccounts")
                .Select(
                    "access_token",
                    "user_id",
                    "associated_id",
                    "user_name"
                );

            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;
        }

        private static FacebookAccountInfo ToFacebookAccountInfo(object[] row)
        {
            FacebookAccountInfo accountInfo = new FacebookAccountInfo
            {
                AccessToken = Convert.ToString(row[0]),
                UserID = Convert.ToString(row[1]),
                AssociatedID = ToGuid(row[2]),
                UserName = Convert.ToString(row[3])
            };

            return accountInfo;
        }
    }
}
