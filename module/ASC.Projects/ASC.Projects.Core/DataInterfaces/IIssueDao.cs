using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IIssueDao
    {
        List<Issue> GetIssues(int projectId);

        Issue GetIssue(int id);

        Issue SaveIssue(Issue issue);

        void RemoveIssue(int id);

        bool IsExist(int id);
    }
}
