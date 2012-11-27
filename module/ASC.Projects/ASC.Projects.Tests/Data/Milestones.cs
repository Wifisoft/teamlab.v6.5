using System;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Data;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    [TestFixture]
    public class Milestones
    {
        [Test]
        public void Test()
        {
            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(0);


            IDaoFactory daoFactory = new DaoFactory("projects", 0);


            var result = daoFactory.GetReportDao().BuildMilestonesReport(new ReportFilter());


            Console.WriteLine(result.Count);

        }

        [Test]
        public void MilestoneDeadline()
        {
            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(0);

            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            var milestones = daoFactory.GetMilestoneDao().GetByDeadLine(DateTime.Now);


            Console.WriteLine(milestones.Count);

        }
    }
}
