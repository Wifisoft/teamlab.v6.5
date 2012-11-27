using System;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.SocialMedia.Twitter
{
    class TwitterAccountInfoDao : BaseDao
    {
        public TwitterAccountInfoDao(int tenantID, String storageKey) : base(tenantID, storageKey) { }

        public void CreateNewAccountInfo(TwitterAccountInfo accountInfo)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DeleteAccountInfo(accountInfo.AssociatedID);

                SqlInsert cmdInsert = Insert("sm_twitteraccounts")
                           .ReplaceExists(true)
                           .InColumnValue("access_token", accountInfo.AccessToken)
                           .InColumnValue("access_token_secret", accountInfo.AccessTokenSecret)
                           .InColumnValue("screen_name", accountInfo.ScreenName)
                           .InColumnValue("user_id", accountInfo.UserID)
                           .InColumnValue("associated_id", accountInfo.AssociatedID)
                           .InColumnValue("user_name", accountInfo.UserName);

                DbManager.ExecuteNonQuery(cmdInsert);

                tx.Commit();
            }
        }

        public void DeleteAccountInfo(Guid associatedID)
        {
            DbManager.ExecuteNonQuery(Delete("sm_twitteraccounts").Where(Exp.Eq("associated_id", associatedID)));
        }

        public TwitterAccountInfo GetAccountInfo(Guid associatedID)
        {
            var accounts = DbManager.ExecuteList(GetAccountQuery(Exp.Eq("associated_id", associatedID)));
            return accounts.Count > 0 ? ToTwitterAccountInfo(accounts[0]) : null;
        }

        private SqlQuery GetAccountQuery(Exp where)
        {
            SqlQuery sqlQuery = Query("sm_twitteraccounts")
                .Select(
                    "access_token",
                    "access_token_secret",
                    "screen_name",
                    "user_id",
                    "associated_id",
                    "user_name"
                );

            if (where != null)
                sqlQuery.Where(where);

            return sqlQuery;
        }

        private static TwitterAccountInfo ToTwitterAccountInfo(object[] row)
        {
            TwitterAccountInfo accountInfo = new TwitterAccountInfo
            {
                AccessToken = Convert.ToString(row[0]),
                AccessTokenSecret = Convert.ToString(row[1]),
                ScreenName = Convert.ToString(row[2]),
                UserID = Convert.ToDecimal(row[3]),
                AssociatedID = ToGuid(row[4]),
                UserName = (row[5] != null && row[5] != DBNull.Value) ? Convert.ToString(row[5]) : null
            };

            return accountInfo;
        }
    }
}
