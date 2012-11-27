using System;
using System.Collections.Generic;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class IssuesEngine
    {
        private IIssueDao dao;


        public IssuesEngine(IDaoFactory factory)
        {
            dao = factory.GetIssueDao();
        }


        public List<Issue> GetIssues(int projectId)
        {
            return dao.GetIssues(projectId);
        }

        public Issue GetIssue(int issueId)
        {
            return dao.GetIssue(issueId);
        }

        public Issue SaveIssue(Issue issue)
        {
            if (issue == null) throw new ArgumentNullException("issue");

            issue.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            issue.LastModifiedOn = TenantUtil.DateTimeNow();
            if (issue.CreateBy == default(Guid)) issue.CreateBy = SecurityContext.CurrentAccount.ID;
            if (issue.CreateOn == default(DateTime)) issue.CreateOn = TenantUtil.DateTimeNow();
            return dao.SaveIssue(issue);
        }

        public void RemoveIssue(int issueId)
        {
            dao.RemoveIssue(issueId);
        }

        public bool IsExist(int issueId)
        {
            return dao.IsExist(issueId);
        }
    }
}
