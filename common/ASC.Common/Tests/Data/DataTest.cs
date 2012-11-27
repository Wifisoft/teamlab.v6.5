#if DEBUG
using System;
using System.Data;
using System.Data.SQLite;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using NUnit.Framework;

namespace ASC.Common.Tests.Data
{
    [TestFixture]
    public class DataTest
    {
        private string dbId = Guid.NewGuid().ToString();
        private string cs = "Data Source=dbtest.sqlite;Version=3";

        public DataTest()
        {
            DbRegistry.RegisterDatabase(dbId, new SQLiteFactory(), cs);
        }

        [Test]
        public void RegistryTest()
        {
            Assert.AreEqual(cs, DbRegistry.GetConnectionString(dbId));
            Assert.IsTrue(DbRegistry.IsDatabaseRegistered(dbId));
            Assert.IsNotNull(DbRegistry.CreateDbConnection(dbId));
        }

        [Test]
        public void DbTransactionTest()
        {
            var dbManager = new DbManager(dbId);
            dbManager.ExecuteNonQuery("create table if not exists a(c1 TEXT)", null);

            var tx = dbManager.BeginTransaction();
            dbManager.ExecuteNonQuery("insert into a(c1) values (?)", "s");
            dbManager.ExecuteNonQuery("insert into a(c1) values (?)", "s2");
            tx.Dispose();

            dbManager.ExecuteNonQuery("insert into a(c1) values (?)", "s3");
        }

        [Test]
        public void GroupConcatTest()
        {
            using (var connect = new SQLiteConnection("Data Source=:memory:"))
            {
                connect.Open();
                var command = new SQLiteCommand("create table a(c1 TEXT)", connect);
                command.ExecuteNonQuery();

                command.CommandText = "insert into a values (NULL);insert into a values ('x');insert into a values ('y');";
                command.ExecuteNonQuery();

                command.CommandText = "select group_concat(c1, 4) from a";
                var result1 = command.ExecuteScalar<string>();
                Assert.AreEqual("x4y", result1);

                command.CommandText = "select group_concat(c1) from a";
                var result2 = command.ExecuteScalar<string>();
                Assert.AreEqual("x,y", result2);

                command.CommandText = "select group_concat(c1) from a where 1 = 0";
                var result3 = command.ExecuteScalar<string>();
                Assert.AreEqual(null, result3);

                command.CommandText = "select group_concat(c1, NULL) from a";
                var result4 = command.ExecuteScalar<string>();
                Assert.AreEqual("x,y", result4);

                command.CommandText = "select concat(1, NULL, '4566')";
                var result5 = command.ExecuteScalar<string>();
                Assert.AreEqual("14566", result5);

                command.CommandText = "select concat()";
                var result6 = command.ExecuteScalar<string>();
                Assert.AreEqual(null, result6);

                command.CommandText = "select concat_ws(',', 45, 77)";
                var result7 = command.ExecuteScalar<string>();
                Assert.AreEqual("45,77", result7);
            }
        }
    }
}
#endif