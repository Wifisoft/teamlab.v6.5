#if DEBUG
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ASC.Common.Data.Sql.Expressions;
using ASC.Common.Data.Sql;
using System.Data;

namespace ASC.Common.Tests.Data
{
    [TestFixture]
    public class ExpTest
    {
        [Test]
        public void JunctuinTest()
        {
            var exp = Exp.Eq("A", 0) & (Exp.Eq("B", 0) | Exp.Eq("C", 0));
            Assert.AreEqual(exp.ToString(), "A = ? and (B = ? or C = ?)");

            exp = Exp.Eq("A", 0) & (Exp.Eq("B", 0) & Exp.Eq("C", 0));
            Assert.AreEqual(exp.ToString(), "A = ? and B = ? and C = ?");

            exp = Exp.Eq("A", 0) | (Exp.Eq("B", 0) | Exp.Eq("C", 0));
            Assert.AreEqual(exp.ToString(), "A = ? or B = ? or C = ?");

            exp = (Exp.Eq("A", 0) & Exp.Eq("B", 0)) | Exp.Eq("C", 0);
            Assert.AreEqual(exp.ToString(), "(A = ? and B = ?) or C = ?");

            exp = (Exp.Eq("A", 0) & Exp.Eq("B", 0)) & Exp.Eq("C", 0) | Exp.Eq("D", 0);
            Assert.AreEqual(exp.ToString(), "(A = ? and B = ? and C = ?) or D = ?");

            exp = (Exp.Eq("A", 0) & Exp.Eq("B", 0)) | (Exp.Eq("C", 0) & Exp.Eq("D", 0));
            Assert.AreEqual(exp.ToString(), "(A = ? and B = ?) or (C = ? and D = ?)");

            exp = (Exp.Eq("A", 0) | Exp.Eq("B", 0)) & Exp.Eq("C", 0);
            Assert.AreEqual(exp.ToString(), "(A = ? or B = ?) and C = ?");

            exp = Exp.Eq("A", 0) | Exp.Eq("B", 0) & Exp.Eq("C", 0);//приоритет & выше |
            Assert.AreEqual(exp.ToString(), "A = ? or (B = ? and C = ?)");
        }

        [Test]
        public void QueryTtest()
        {
            var query = new SqlQuery("Table1 t1")
                .From(new SqlQuery("Table2").Select("Id"), "t2")
                .Select("t1.Name")
                .Where(Exp.EqColumns("t1.Id", "t2.Id"));

            Assert.AreEqual("select t1.Name from Table1 t1, (select Id from Table2) as t2 where t1.Id = t2.Id", query.ToString());
        }

        [Test]
        public void LGTest()
        {
            Assert.AreEqual("a < ?", Exp.Lt("a", 0).ToString());
            Assert.AreEqual("a <= ?", Exp.Le("a", 0).ToString());
            Assert.AreEqual("a > ?", Exp.Gt("a", 0).ToString());
            Assert.AreEqual("a >= ?", Exp.Ge("a", 0).ToString());
        }

        [Test]
        public void InTest()
        {
            Assert.AreEqual("a = ?", Exp.In("a", new[] { 1 }).ToString());
            Assert.AreEqual("1 = 0", Exp.In("a", new int[0]).ToString());
            Assert.AreEqual("a in (?,?)", Exp.In("a", new[] { 1, 2 }).ToString());
            Assert.AreEqual("a in (select c)", Exp.In("a", new SqlQuery().Select("c")).ToString());

            Assert.AreEqual("a <> ?", (!Exp.In("a", new[] { 1 })).ToString());
            Assert.AreEqual("1 <> 0", (!Exp.In("a", new int[0])).ToString());
            Assert.AreEqual("a not in (?,?)", (!Exp.In("a", new[] { 1, 2 })).ToString());
            Assert.AreEqual("a not in (select c)", (!Exp.In("a", new SqlQuery().Select("c"))).ToString());
        }

        [Test]
        public void SqlInsertTest()
        {
            var i = new SqlInsert("Table").InColumnValue("c1", 1);
            Assert.AreEqual("insert into Table(c1) values (?)", i.ToString());

            i = new SqlInsert("Table").ReplaceExists(true).InColumnValue("c1", 1);
            Assert.AreEqual("replace into Table(c1) values (?)", i.ToString());

            i = new SqlInsert("Table").IgnoreExists(true).InColumnValue("c1", 1);
            Assert.AreEqual("insert ignore into Table(c1) values (?)", i.ToString());
        }

        [Test]
        public void SqlUpdateTest()
        {
            var update = new SqlUpdate("Table")
                .Set("Column1", 1)
                .Set("Column1", 2)
                .Set("Column2", 3)
                .Set("Column3 = Column3 + 2");

            Assert.AreEqual("update Table set Column1 = ?, Column2 = ?, Column3 = Column3 + 2", update.ToString());
        }

        [Test]
        public void SqlUnionTest()
        {
            var union = new SqlQuery("t1").Select("c1").Where("c1", 4)
                .Union(new SqlQuery("t2").Select("c2").Where("c2", 7));

            Assert.AreEqual("select c1 from t1 where c1 = ? union select c2 from t2 where c2 = ?", union.ToString());
            Assert.AreEqual(4, union.GetParameters()[0]);
            Assert.AreEqual(7, union.GetParameters()[1]);
        }

        [Test]
        public void SqlCreateTableTest()
        {
            var q = new SqlCreate.Table("t1")
                    .AddColumn("c1", DbType.String, 255);
            Assert.AreEqual("create table t1 (c1 string(255) null);\r\n", q.ToString());
        }
    }
}
#endif