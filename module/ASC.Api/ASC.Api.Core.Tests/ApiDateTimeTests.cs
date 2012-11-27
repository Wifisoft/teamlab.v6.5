using System;
using ASC.Specific;
using NUnit.Framework;

namespace ASC.Api.Core.Tests
{
    [TestFixture]
    public class ApiDateTimeTests
    {
        [Test]
        public void TestParsing()
        {
            const string parseTime = "2012-01-11T07:01:00.0000001Z";
            var apiDateTime1 = ApiDateTime.Parse(parseTime);
            var dateTime = (DateTime) apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind,utcTime.Kind);
            Assert.AreEqual(dateTime,utcTime);
            Assert.AreEqual(apiDateTime1.ToString(),parseTime);
        }

        [Test]
        public void TestNull()
        {
            var apiDateTime = (ApiDateTime) null;
        }

        [Test]
        public void TestLocal2()
        {

            var apiDateTime = new ApiDateTime(DateTime.Now,TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
            var stringv =apiDateTime.ToString();
        }


        [Test]
        public void TestParsing2()
        {
            const string parseTime = "2012-01-31T20:00:00.0000000Z";
            var apiDateTime1 = ApiDateTime.Parse(parseTime);
            var dateTime = (DateTime)apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind, utcTime.Kind);
            Assert.AreEqual(dateTime, utcTime);
            Assert.AreEqual(apiDateTime1.ToString(), parseTime);
        }

        [Test]
        public void TestUtc()
        {
            var apiDateTime1 = new ApiDateTime(DateTime.Now,TimeZoneInfo.Utc);
            var dateTime = (DateTime)apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind, utcTime.Kind);
            Assert.AreEqual(dateTime, utcTime);
        }

        [Test]
        public void TestLocal()
        {
            var apiDateTime1 = new ApiDateTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
            var dateTime = (DateTime)apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind, utcTime.Kind);
            Assert.AreEqual(dateTime, utcTime);
        }

        [Test]
        public void Test00()
        {
            var apiDateTime1 = new ApiDateTime(DateTime.Now);
            var stringrep = apiDateTime1.ToString();
            var dateTime = (DateTime)apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind, utcTime.Kind);
            Assert.AreEqual(dateTime, utcTime);
        }
    }
}