using System;
using ASC.Common.Data.Sql;
using ASC.Projects.Core.DataInterfaces;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;
using ASC.Core.Tenants;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Projects.Data.DAO
{
    class ParticipantDao : BaseDao, IParticipantDao
    {
        public ParticipantDao(string dbId, int tenant) : base(dbId, tenant) { }


        public int[] GetFollowingProjects(Guid participant)
        {
            return DbManager
                .ExecuteList(new SqlQuery("projects_following_project_participant").Select("project_id").Where("participant_id", participant.ToString()))
                .ConvertAll(r => Convert.ToInt32(r[0]))
                .ToArray();
        }

        public int[] GetMyProjects(Guid participant)
        {
            return DbManager
                .ExecuteList(new SqlQuery("projects_project_participant").Select("project_id").Where("participant_id", participant.ToString()))
                .ConvertAll(r => Convert.ToInt32(r[0]))
                .ToArray();
        }
        public List<int> GetInterestedProjects(Guid participant)
        {
            var union_q = new SqlQuery("projects_following_project_participant").Select("project_id").Where("participant_id", participant.ToString())
                        .Union(new SqlQuery("projects_project_participant").Select("project_id").Where("participant_id", participant.ToString()));

            return DbManager.ExecuteList(union_q)
                    .ConvertAll(r => Convert.ToInt32(r[0]));
        }

        public void AddToFollowingProjects(int project, Guid participant)
        {
            DbManager.ExecuteNonQuery(
                new SqlInsert("projects_following_project_participant", true)
                .InColumnValue("project_id", project)
                .InColumnValue("participant_id", participant.ToString()));
        }

        public void RemoveFromFollowingProjects(int project, Guid participant)
        {
            DbManager.ExecuteNonQuery(
                new SqlDelete("projects_following_project_participant")
                .Where("project_id", project)
                .Where("participant_id", participant.ToString()));
        }


        public DateTime? WhenReaded(Guid participant, string uniqueId)
        {
            var res = (DateTime?)DbManager.ExecuteScalar(
                Query("projects_review_entity_info")
                .Select("entity_review")
                .Where("user_id", participant.ToString())
                .Where("entity_uniqID", uniqueId));

            return res.HasValue ? TenantUtil.DateTimeFromUtc(res.Value) as DateTime? : null;
        }

        public List<bool> IsReaded(Guid participant, List<ProjectEntity> entities)
        {
            //collect unreaded entity by participant
            var nested = new SqlQuery("projects_comments c")
                            .Select("c.target_uniq_id", "max(c.create_on) max_date")
                            .Where(Exp.In("c.target_uniq_id", entities.ConvertAll(e => e.UniqID)))
                            .Where("c.tenant_id", Tenant)
                            .GroupBy(1);

            var query = new SqlQuery().From(nested, "s")
                            .LeftOuterJoin("projects_review_entity_info", Exp.EqColumns("entity_uniqID", "target_uniq_id") & Exp.Eq("user_id", participant) & Exp.Eq("projects_review_entity_info.tenant_id", Tenant))
                            .Where(new SqlExp("entity_review is null") | new SqlExp("entity_review < max_date"))
                            .Select("target_uniq_id");

            var unreaded_entities = DbManager
                    .ExecuteList(query)
                    .ConvertAll(entity => Convert.ToString(entity[0]));

            return entities.ConvertAll(entity => !unreaded_entities.Contains(entity.UniqID));
        }

        public void Read(Guid participant, string uniqueId, DateTime when)
        {
            DbManager.ExecuteNonQuery(
                new SqlInsert("projects_review_entity_info", true)
                .InColumnValue("user_id", participant.ToString())
                .InColumnValue("entity_uniqID", uniqueId)
                .InColumnValue("entity_review", TenantUtil.DateTimeToUtc(when))
                .InColumnValue("tenant_id", Tenant));
        }
    }
}
