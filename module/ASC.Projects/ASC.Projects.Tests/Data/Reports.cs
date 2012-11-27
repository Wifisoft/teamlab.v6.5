using System;
using System.Reflection;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    public class Reports : TestBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void BuildMilestoneListReport()
        {
            var daoFactory = new DaoFactory("projects", 0);
            var result = daoFactory.GetReportDao().BuildMilestonesReport(new ReportFilter());
            Console.WriteLine(result.Count);
        }
    }
}
