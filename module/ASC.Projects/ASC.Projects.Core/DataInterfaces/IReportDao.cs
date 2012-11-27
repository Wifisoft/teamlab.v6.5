using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain.Reports;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IReportDao
    {
        IList<object[]> BuildMilestonesReport(ReportFilter filter);

        IList<object[]> BuildUsersWorkReport(ReportFilter filter);

        IList<object[]> BuildUsersStatisticsReport(ReportFilter filter);

        IList<object[]> BuildTaskListReport(ReportFilter filter);

        IList<object[]> BuildTimeReport(ReportFilter filter);


        IList<object[]> BuildProjectListReport(ReportFilter filter);

        IList<object[]> BuildProjectWithoutOpenMilestone(ReportFilter filter);

        IList<object[]> BuildProjectWithoutActiveTask(ReportFilter filter);


        List<ReportTemplate> GetTemplates(Guid userId);

        List<ReportTemplate> GetAutoTemplates();

        ReportTemplate GetTemplate(int id);

        ReportTemplate SaveTemplate(ReportTemplate template);

        void DeleteTemplate(int id);
    }
}
