using System;
using System.Configuration;
using System.Text.RegularExpressions;
using ASC.Notify.Patterns;
using log4net;

namespace ASC.Core.Common.Notify
{
    /// <summary>
    /// Class that generates 'mail to' addresses to create new TeamLab entities from post client 
    /// </summary>
    public static class ReplyToTagProvider
    {
        private static ILog _log = LogManager.GetLogger("ASC.Notify.Autoreply");
        private static readonly Regex EntityType = new Regex(@"blog|forum.topic|event|photo|file|wiki|bookmark|project\.milestone|project\.task|project\.message");
        private static readonly Regex FolderType = new Regex(@"common|shared|my|\d+");

        private const string TagName = "replyto";

        /// <summary>
        /// Creates 'replyto' tag that can be used to comment some TeamLab entity
        /// </summary>
        /// <param name="entity">Name of entity e.g. 'blog', 'project.task', etc.</param>
        /// <param name="entityId">Uniq id of the entity</param>
        /// <returns>New TeamLab tag</returns>
        public static TagValue Comment(string entity, string entityId)
        {
            return Comment(entity, entityId, null);
        }

        /// <summary>
        /// Creates 'replyto' tag that can be used to comment some TeamLab entity
        /// </summary>
        /// <param name="entity">Name of entity e.g. 'blog', 'project.task', etc.</param>
        /// <param name="entityId">Uniq id of the entity</param>
        /// <param name="parentId">Comment's parent comment id</param>
        /// <returns>New TeamLab tag</returns>
        public static TagValue Comment(string entity, string entityId, string parentId)
        {
            if (String.IsNullOrEmpty(entity) || !EntityType.Match(entity).Success) throw new ArgumentException(@"Not supported entity type", entity);
            if (String.IsNullOrEmpty(entityId)) throw new ArgumentException(@"Entity Id is null or empty", entityId);

            string pId = parentId != Guid.Empty.ToString() && parentId != null ? parentId : String.Empty;
            _log.DebugFormat("creating replyto comment:{0}_{1}_{2}",entity,entityId,pId);
            return new TagValue(TagName, String.Format("reply_{0}_{1}_{2}@{3}", entity, entityId, pId, AutoreplyDomain));
        }

        /// <summary>
        /// Creates 'replyto' tag that can be used to create TeamLab blog post
        /// </summary>
        /// <returns>New TeamLab tag</returns>
        public static TagValue Blog()
        {
            return new TagValue(TagName, String.Format("blog@{0}", AutoreplyDomain));
        }

        /// <summary>
        /// Creates 'replyto' tag that can be used to create TeamLab event
        /// </summary>
        /// <returns>New TeamLab tag</returns>
        public static TagValue Event()
        {
            return new TagValue(TagName, String.Format("event@{0}", AutoreplyDomain));
        }

        /// <summary>
        /// Creates 'replyto' tag that can be used to create TeamLab project message
        /// </summary>
        /// <param name="projectId">Id of the project to create message</param>
        /// <returns>New TeamLab tag</returns>
        public static TagValue Message(int projectId)
        {
            return new TagValue(TagName, String.Format("message_{0}@{1}", projectId, AutoreplyDomain));
        }

        /// <summary>
        /// Creates 'replyto' tag that can be used to create TeamLab project task
        /// </summary>
        /// <param name="projectId">Id of the project to create task</param>
        /// <returns>New TeamLab tag</returns>
        public static TagValue Task(int projectId)
        {
            return new TagValue(TagName, String.Format("task_{0}@{1}", projectId, AutoreplyDomain));
        }

        /// <summary>
        /// Creates 'replyto' tag that can be used to upload files to TeamLab
        /// </summary>
        /// <param name="folderIdType"></param>
        /// <returns></returns>
        public static TagValue File(string folderIdType)
        {
            if (String.IsNullOrEmpty(folderIdType) || !FolderType.Match(folderIdType).Success) throw new ArgumentException(@"Not supported folder id type", folderIdType);
            return new TagValue(TagName, String.Format("file_{0}@{1}", folderIdType, AutoreplyDomain));
        }

        private static string AutoreplyDomain
        {
            get
            {
                var tenant = CoreContext.TenantManager.GetCurrentTenant();
                var autoreplyhost = ConfigurationManager.AppSettings["core.notify.autoreplyhost"];
                var result = string.IsNullOrEmpty(autoreplyhost) ? tenant.MappedDomain : string.Format(autoreplyhost, tenant.TenantAlias);
                _log.DebugFormat("autoreply host: {0}", result);
                return result;
            }
        }
    }
}
