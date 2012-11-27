using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Data;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Data.DAO
{
    class IssueDAO : BaseDao, IIssueDao
    {
        private readonly string table = "projects_issues";

        private readonly string[] columns = new[]
        {
            "id", "project_id", "title", "description", "assigned_on", "detected_in_version", "corrected_in_version", "priority", "status", 
            "create_by", "create_on", "last_modified_by", "last_modified_on"
        };



        public IssueDAO(string dbId, int tenant)
            : base(dbId, tenant)
        {
        }

        public List<Issue> GetIssues(int projectId)
        {
            return DbManager
                .ExecuteList(Query(table).Select(columns).Where("project_id", projectId))
                .ConvertAll(r => ToIssue(r));
        }

        public Issue GetIssue(int id)
        {
            return DbManager
                .ExecuteList(Query(table).Select(columns).Where("ID", id))
                .ConvertAll(r => ToIssue(r))
                .SingleOrDefault();
        }

        public Issue SaveIssue(Issue issue)
        {
            var insert = Insert(table)
                .InColumns(columns)
                .Values(issue.ID, issue.ProjectID, issue.Title, issue.Description, issue.AssignedOn.ToString(), issue.DetectedInVersion, issue.CorrectedInVersion)
                .Values(issue.Priority, issue.Status)
                .Values(issue.CreateBy.ToString(), TenantUtil.DateTimeToUtc(issue.CreateOn), issue.LastModifiedBy.ToString(), TenantUtil.DateTimeToUtc(issue.LastModifiedOn))
                .Identity(1, 0, true);
            issue.ID = DbManager.ExecuteScalar<int>(insert);
            return issue;
        }

        public void RemoveIssue(int id)
        {
            DbManager.ExecuteNonQuery(Delete(table).Where("id", id));
        }

        public bool IsExist(int id)
        {
            return 0 < DbManager.ExecuteScalar<long>(Query(table).SelectCount().Where("id", id));
        }


        private Issue ToIssue(object[] r)
        {
            return new Issue()
            {
                ID = Convert.ToInt32(r[0]),
                ProjectID = Convert.ToInt32(r[1]),
                Title = (string)r[2],
                Description = (string)r[3],
                AssignedOn = ToGuid(r[4]),
                DetectedInVersion = (string)r[5],
                CorrectedInVersion = (string)r[6],
                Priority = (IssuePriority)Convert.ToInt32(r[7]),
                Status = (IssueStatus)Convert.ToInt32(r[8]),
                CreateBy = ToGuid(r[9]),
                CreateOn = TenantUtil.DateTimeFromUtc((DateTime)r[10]),
                LastModifiedBy = ToGuid(r[11]),
                LastModifiedOn = TenantUtil.DateTimeFromUtc((DateTime)r[12]),
            };
        }
    }
}
