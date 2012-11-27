using System;
using ASC.Core.Caching;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

namespace ASC.Web.Projects.Classes
{
    public class SecurityAdapter : IFileSecurity
    {
        private readonly ASC.Projects.Core.DataInterfaces.IProjectDao dao;
        private readonly int projectId;

        private Project project;
        private readonly TrustInterval interval = new TrustInterval();
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

        private Project Project
        {
            get
            {
                if (interval.Expired)
                {
                    project = dao.GetById(projectId);
                    interval.Start(timeout);
                }
                return project;
            }
        }


        public SecurityAdapter(ASC.Projects.Core.DataInterfaces.IDaoFactory factory, int projectId)
        {
            this.dao = factory.GetProjectDao();
            this.projectId = projectId;
        }


        public bool CanRead(FileEntry file, Guid userId)
        {
            return Can(file, userId, SecurityAction.Read);
        }

        public bool CanCreate(FileEntry file, Guid userId)
        {
            return Can(file, userId, SecurityAction.Create);
        }

        public bool CanDelete(FileEntry file, Guid userId)
        {
            return Can(file, userId, SecurityAction.Delete);
        }

        public bool CanEdit(FileEntry file, Guid userId)
        {
            return Can(file, userId, SecurityAction.Edit);
        }

        private bool Can(FileEntry file, Guid userId, SecurityAction action)
        {
            if (!ProjectSecurity.CanReadFiles(Project)) return false;

            if (IsAdmin(userId)) return true;
            if (file == null || Project == null) return false;
            if (file is Folder && ((Folder)file).FolderType == FolderType.DEFAULT && file.CreateBy == userId) return true;
            if (file is File && file.CreateBy == userId) return true;

            switch (action)
            {
                case SecurityAction.Read: return Project.Private ? dao.IsInTeam(Project.ID, userId) : true;
                case SecurityAction.Create: return dao.IsInTeam(Project.ID, userId);
                case SecurityAction.Edit: return Project.Responsible == userId;
                case SecurityAction.Delete: return Project.Responsible == userId;
                default: return false;
            }
        }


        private static bool IsAdmin(Guid userId)
        {
            return ProjectSecurity.IsAdministrator(userId);
        }


        private enum SecurityAction
        {
            Read,
            Create,
            Edit,
            Delete,
        };
    }
}
