#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Documents;
using ASC.Api.Exceptions;
using ASC.Api.Projects.Wrappers;
using ASC.Api.Utils;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;
using ASC.Web.Studio.Utility;
using ProjectActivityFilter = ASC.Projects.Core.Domain.ProjectActivityFilter;

#endregion

namespace ASC.Api.Projects
{
    public partial class ProjectApi
    {
        #region projects

		 ///<summary>
		 ///Returns the list of all the portal projects with base information about them
		 ///</summary>
		 ///<short>
		 ///Projects
		 ///</short>
		 /// <category>Projects</category>
        ///<returns>List of projects</returns>
        [Read("")]
        public IEnumerable<ProjectWrapper> GetAllProjects()
        {
            return EngineFactory.GetProjectEngine().GetAll().Select(x => new ProjectWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all the portal projects filtered using project title, status or participant ID and 'Followed' status specified in the request
		  ///</summary>
		  ///<short>
		  ///Projects
		  ///</short>
		  /// <category>Projects</category>
        ///<param name="tag" optional="true">Project tag</param>
		///<param name="status" optional="true">Project status</param>
        ///<param name="participant" optional="true">Participant GUID</param>
        ///<param name="manager" optional="true">Project manager GUID</param>
        ///<param name="follow" optional="true">My followed project</param>
        ///<param name="lastId">Last project ID</param>
        ///<returns>Projects list</returns>
        [Read(@"filter")]
        public IEnumerable<ProjectWrapperFull> GetProjectsByFilter(int tag, ProjectStatus? status, Guid participant, Guid manager, bool follow, int lastId)
        {
            var filter = new TaskFilter
            {
                ParticipantId = participant,
                UserId = manager,
                SortBy = _context.SortBy,
                SortOrder = !_context.SortDescending,
                SearchText = _context.FilterValue,
                TagId = tag,
                Follow = follow,
                LastId = lastId,
                Offset = _context.StartIndex,
                Max = _context.Count
            };

            if (status != null)
                filter.ProjectStatuses.Add((ProjectStatus)status);

            _context.SetDataFiltered();
            _context.SetDataPaginated();
            _context.SetDataSorted();

            return EngineFactory.GetProjectEngine().GetByFilter(filter).NotFoundIfNull().Select(r => new ProjectWrapperFull(r)).ToSmartList();
        }

		  ///<summary>
		  ///Creates a new project using all the necessary (title, description, responsible ID, etc) and some optional parameters specified in the request
		  ///</summary>
		  ///<short>
		  ///Create project
		  ///</short>
		  /// <category>Projects</category>
        ///<param name="title">title</param>
        ///<param name="description">description</param>
		  ///<param name="responsibleId">responsible ID</param>
        ///<param name="tags">tags</param>
        ///<param name="private">Is project private</param>
        ///<param name="participants" optional="true">Project participants</param>
        ///<param name="notify" optional="true">Notify project manager</param>
        ///<returns>Newly created project</returns>
        ///<exception cref="ArgumentException"></exception>
        [Create("")]
        public ProjectWrapper CreateProject(string title, string description, Guid responsibleId, string tags, bool @private, IEnumerable<Guid> participants, bool? notify)
        {
            if (responsibleId == Guid.Empty) throw new ArgumentException(@"responsible can't be empty", "responsibleId");
            if (string.IsNullOrEmpty(title)) throw new ArgumentException(@"title can't be empty", "title");

            ProjectSecurity.DemandCreateProject();

            var project = new Project
            {
                Title = title,
                Status = ProjectStatus.Open,
                Responsible = responsibleId,
                Description = description,
                Private = @private
            };

            EngineFactory.GetProjectEngine().SaveOrUpdate(project, true);
            EngineFactory.GetProjectEngine().AddToTeam(project, EngineFactory.GetParticipantEngine().GetByID(responsibleId), notify ?? true);
            EngineFactory.GetTagEngine().SetProjectTags(project.ID, tags);

            foreach (var participant in participants)
            {
                EngineFactory.GetProjectEngine().AddToTeam(project, EngineFactory.GetParticipantEngine().GetByID(participant), true);
            } 


            return new ProjectWrapper(project);
        }

        ///<summary>
        ///Updates the existing project information using all the parameters (project ID, title, description, responsible ID, etc) specified in the request
        ///</summary>
        ///<short>
        ///Update project
        ///</short>
        /// <category>Projects</category>
        ///<param name="id">project ID</param>
        ///<param name="title">title</param>
        ///<param name="description">description</param>
        ///<param name="responsibleId">responsible ID</param>
        ///<param name="tags">tags</param>
        ///<param name="private">Is project private</param>
        ///<param name="status">status. one of (Open|Closed)</param>
        ///<param name="notify">Notify project manager</param>
        ///<returns>Updated project</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"{id:[0-9]+}")]
        public ProjectWrapperFull UpdateProject(int id, string title, string description, Guid responsibleId, string tags, ProjectStatus status, bool @private, bool notify)
        {
            if (responsibleId == Guid.Empty) throw new ArgumentException(@"responsible can't be empty", "responsibleId");
            if (string.IsNullOrEmpty(title)) throw new ArgumentException(@"title can't be empty", "title");

            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            ProjectSecurity.DemandEdit(project);
            project.Title = Update.IfNotEmptyAndNotEquals(project.Title, title);
            /*if (project.Status != status)*/ project.StatusChangedOn = DateTime.Now;
            project.Status = status;
            project.Responsible = Update.IfNotEmptyAndNotEquals(project.Responsible, responsibleId);
            project.Description = Update.IfNotEmptyAndNotEquals(project.Description, description);
            project.Private = @private;

            EngineFactory.GetProjectEngine().SaveOrUpdate(project, notify);

            if (!EngineFactory.GetProjectEngine().IsInTeam(project.ID, project.Responsible))
                EngineFactory.GetProjectEngine().AddToTeam(project, EngineFactory.GetParticipantEngine().GetByID(project.Responsible), false);

            EngineFactory.GetTagEngine().SetProjectTags(project.ID, tags);

            return new ProjectWrapperFull(project,EngineFactory.GetFileEngine().GetRoot(id));
        }

        ///<summary>
        ///Updates the status of the project with the ID specified in the request
        ///</summary>
        ///<short>
        ///Update project status
        ///</short>
        /// <category>Projects</category>
		  ///<param name="id">project ID</param>
        ///<param name="status">status. one of (Open|Paused|Closed)</param>
        ///<returns>Updated project</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"{id:[0-9]+}/status")]
        public ProjectWrapperFull UpdateProject(int id, ProjectStatus status)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();
            ProjectSecurity.DemandEdit(project);
            /*if (project.Status != status)*/ project.StatusChangedOn = DateTime.Now;
            project.Status = status;
            EngineFactory.GetProjectEngine().SaveOrUpdate(project, false);
            return new ProjectWrapperFull(project, EngineFactory.GetFileEngine().GetRoot(id));
        }

		  ///<summary>
		  ///Deletes the project with the ID specified in the request from the portal
		  ///</summary>
		  ///<short>
		  ///Delete project
		  ///</short>
		  /// <category>Projects</category>
		  ///<param name="id">project ID</param>
        ///<returns>deleted project</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Delete(@"{id:[0-9]+}")]
        public ProjectWrapperFull DeleteProject(int id)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            ProjectSecurity.DemandEdit(project);
            EngineFactory.GetProjectEngine().Delete(id);
            return new ProjectWrapperFull(project, EngineFactory.GetFileEngine().GetRoot(id));
        }

          ///<summary>
          ///Subscribe or unsubscribe to notifications about the actions performed with the project with the ID specified in the request
          ///</summary>
          ///<short>
          ///Following/Unfollowing to project
          ///</short>
          /// <category>Projects</category>
          ///<param name="projectId">Project Id</param>
        /// <returns>Project</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"{projectid:[0-9]+}/follow")]
        public ProjectWrapper FollowToProject(int projectId)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectId).NotFoundIfNull();

            ProjectSecurity.DemandAuthentication();

            var participantEngine = EngineFactory.GetParticipantEngine();
            if (participantEngine.GetFollowingProjects(CurrentUserId).Contains(projectId))
            {
                participantEngine.RemoveFromFollowingProjects(projectId, CurrentUserId);
            }
            else
            {
                participantEngine.AddToFollowingProjects(projectId, CurrentUserId);
            }
            return new ProjectWrapper(project);
        }

		  ///<summary>
		  ///Returns the list of all projects with the status specified in the request
		  ///</summary>
		  ///<short>
		  ///Project by status
		  ///</short>
		  /// <category>Projects</category>
        ///<param name="status">"open" or "closed"</param>
        ///<returns>List of projects</returns>
        [Read("{status:(open|closed)}")]
        public IEnumerable<ProjectWrapper> GetProjects(ProjectStatus status)
        {
            return EngineFactory.GetProjectEngine().GetAll(status, 0).Select(x => new ProjectWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the detailed information about the project with ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Project by ID
		  ///</short>
		  /// <category>Projects</category>
		  ///<param name="id">Project ID</param>
        ///<returns>Project</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{id:[0-9]+}")]
        public ProjectWrapperFull GetProject(int id)
        {
            return new ProjectWrapperFull(EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull(), EngineFactory.GetFileEngine().GetRoot(id));
        }

		  ///<summary>
		  ///Returns the detailed list of all files and folders for the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Project files by project ID
		  ///</short>
		  /// <category>Projects</category>
		  ///<param name="id">Project ID</param>
        ///<returns>Project files</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{id:[0-9]+}/files")]
        public FolderContentWrapper GetProjectFiles(int id)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            if (ProjectSecurity.CanReadFiles(project))
                return _documentsApi.GetFolder(EngineFactory.GetFileEngine().GetRoot(id).ToString());

            throw new SecurityException("Access to files is denied");
        }

		  ///<summary>
		  ///Returns the search results for the project containing the words/phrases matching the query specified in the request
		  ///</summary>
		  ///<short>
		  ///Search project
		  ///</short>
		  /// <category>Projects</category>
		  ///<param name="id">project ID</param>
        ///<param name="query">search query</param>
        ///<returns>list of results</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{id:[0-9]+}/@search/{query}")]
        public IEnumerable<SearchWrapper> SearchProject(int id, string query)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(id)) throw new ItemNotFoundException();
            return EngineFactory.GetSearchEngine().Search(query, id).Select(x => new SearchWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all projects matching the query specified in the request
		  ///</summary>
		  ///<short>
		  ///Search all projects
		  ///</short>
		  /// <category>Projects</category>
        ///<param name="query">search query</param>
        ///<returns>list of results</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"@search/{query}")]
        public IEnumerable<SearchWrapper> SearchProjects(string query)
        {
            return EngineFactory.GetSearchEngine().Search(query, -1).Select(x => new SearchWrapper(x)).ToSmartList();
        }

        ///<summary>
        ///Creates a new milestone using the parameters (project ID, milestone title, deadline, etc) specified in the request
        ///</summary>
        ///<short>
        ///Add Milestone
        ///</short>
        /// <category>Projects</category>
        ///<param name="id">project ID</param>
        ///<param name="title">milestone title</param>
        ///<param name="deadline">milestone deadline</param>
        ///<param name="isKey">is milestone key?</param>
        ///<param name="isNotify">Remind me 48 hours before the due date</param>
        ///<param name="description">milestone description</param>
        ///<param name="responsible">milestone responsible</param>
        ///<param name="notifyResponsible">notify responsible</param>
        ///<returns>created milestone</returns>
        ///<exception cref="ArgumentNullException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"{id:[0-9]+}/milestone")]
        public MilestoneWrapper AddProjectMilestone(int id, string title, ApiDateTime deadline, bool isKey, bool isNotify, string description, Guid responsible, bool notifyResponsible)
        {
            if (title == null) throw new ArgumentNullException("title");
            if (deadline == DateTime.MinValue) throw new ArgumentNullException("deadline");
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            ProjectSecurity.DemandCreateMilestone(project);

            var milestone = new Milestone
            {
                Description = description ?? "",
                Project = project,
                Title = title.Trim(),
                DeadLine = deadline,
                IsKey = isKey,
                Status = MilestoneStatus.Open,
                IsNotify = isNotify,
                Responsible = responsible
            };
            EngineFactory.GetMilestoneEngine().SaveOrUpdate(milestone, notifyResponsible);
            return new MilestoneWrapper(milestone);
        }

		  ///<summary>
		  ///Returns the list of all the milestones within the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Get Milestones by project ID
		  ///</short>
		  /// <category>Projects</category>
		  ///<param name="id">project ID</param>
        ///<returns>list of milestones</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{id:[0-9]+}/milestone")]
        public IEnumerable<MilestoneWrapper> GetProjectMilestones(int id)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();
            
            //NOTE: move to engine
            if (!ProjectSecurity.CanReadMilestones(project)) throw ProjectSecurity.CreateSecurityException();

            var milestones = EngineFactory.GetMilestoneEngine().GetByProject(id);

            return milestones.Select(x => new MilestoneWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all the milestones with the selected status within the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Get Milestones by project ID and milestone status
		  ///</short>
		  /// <category>Projects</category>
		  ///<param name="id">project ID</param>
        ///<param name="status">Milestone Status</param>
        ///<returns>list of milestones</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{id:[0-9]+}/milestone/{status:(open|closed|late|disable)}")]
        public IEnumerable<MilestoneWrapper> GetProjectMilestones(int id, MilestoneStatus status)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            if (!ProjectSecurity.CanReadMilestones(project)) throw ProjectSecurity.CreateSecurityException();

            var milestones = EngineFactory.GetMilestoneEngine().GetByStatus(id, status);

            return milestones.Select(x => new MilestoneWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Updates the tags for the project with the selected project ID with the tags specified in the request
		  ///</summary>
		  ///<short>
		  ///Update project tags
		  ///</short>
		  /// <category>Projects</category>
		  ///<param name="id">project ID</param>
        ///<param name="tags">tags</param>
        ///<returns>project</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"{id:[0-9]+}/tag")]
        public ProjectWrapperFull UpdateProjectTags(int id, string tags)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(id).NotFoundIfNull();

            ProjectSecurity.DemandEdit(project);

            EngineFactory.GetTagEngine().SetProjectTags(id, tags);
            return new ProjectWrapperFull(project, EngineFactory.GetFileEngine().GetRoot(id));
        }

		  ///<summary>
		  ///Returns the detailed information about the time spent on the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Project time spent
		  ///</short>
		  /// <category>Projects</category>
		  ///<param name="id">project ID</param>
        ///<returns>List of time spent</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{id:[0-9]+}/time")]
        public IEnumerable<TimeWrapper> GetProjectTime(int id)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(id)) throw new ItemNotFoundException();
            return EngineFactory.GetTimeTrackingEngine().GetByProject(id).Select(x => new TimeWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all projects in which the current user participates
		  ///</summary>
		  ///<short>
		  ///Participated projects
		  ///</short>
		  /// <category>Projects</category>
        ///<returns>List of projects</returns>
        [Read(@"@self")]
        public IEnumerable<ProjectWrapper> GetMyProjects()
        {
            return EngineFactory
                .GetProjectEngine()
                .GetByParticipant(CurrentUserId)
                .Select(x => new ProjectWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all projects which the current user follows
		  ///</summary>
		  ///<short>
		  ///Followed projects
		  ///</short>
		  /// <category>Projects</category>
        ///<returns>List of projects</returns>
        [Read(@"@follow")]
        public IEnumerable<ProjectWrapper> GetFollowProjects()
        {
            return EngineFactory
                .GetProjectEngine()
                .GetFollowing(CurrentUserId)
                .Select(x => new ProjectWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all users participating in the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Project team
		  ///</short>
		  /// <category>Team</category>
		  /// <param name="projectid">ID of project</param>
        ///<returns>List of team members</returns>
        [Read(@"{projectid:[0-9]+}/team")]
        public IEnumerable<ParticipantWrapper> GetProjectTeam(int projectid)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();

            return EngineFactory.GetProjectEngine().GetTeam(projectid).Where(r => r.UserInfo != Core.Users.Constants.LostUser)
                .Select(x => new ParticipantWrapper(x))
                .OrderBy(r=> r.DisplayName).ToSmartList();
        }

		  ///<summary>
		  ///Adds the user with the ID specified in the request to the selected project team
		  ///</summary>
		  ///<short>
		  ///Add to team
		  ///</short>
		  /// <category>Team</category>
		  ///<param name="projectid">Project ID</param>
		  ///<param name="userId">User ID to add</param>
        ///<returns>List of team members</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"{projectid:[0-9]+}/team")]
        public IEnumerable<ParticipantWrapper> AddToProjectTeam(int projectid, Guid userId)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();

            ProjectSecurity.DemandEditTeam(project);

            EngineFactory.GetProjectEngine().AddToTeam(project, EngineFactory.GetParticipantEngine().GetByID(userId), true);
            
            return GetProjectTeam(projectid);
        }

		  ///<summary>
		  ///Sets the security rights for the user or users with the IDs specified in the request within the selected project
		  ///</summary>
		  ///<short>
		  ///Set team security
		  ///</short>
		  /// <category>Team</category>
		  ///<param name="projectid">Project ID</param>
		  ///<param name="userId">User ID to set</param>
        ///<param name="security">security rights</param>
        ///<param name="visible">visible</param>
        ///<returns>List of team members</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"{projectid:[0-9]+}/team/security")]
        public IEnumerable<ParticipantWrapper> SetProjectTeamSecurity(int projectid, Guid userId, ProjectTeamSecurity security, bool visible)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();
            if (!EngineFactory.GetProjectEngine().IsInTeam(projectid,userId))
            {
                throw new ArgumentOutOfRangeException("userId","Not a project memeber");
            }
            ProjectSecurity.DemandEditTeam(project);

            EngineFactory.GetProjectEngine().SetTeamSecurity(project, EngineFactory.GetParticipantEngine().GetByID(userId), security, visible);
            return GetProjectTeam(projectid);
        }

		  ///<summary>
		  ///Removes the user with the ID specified in the request from the selected project team
		  ///</summary>
		  ///<short>
		  ///Remove from team
		  ///</short>
		  /// <category>Team</category>
		  ///<param name="projectid">Project ID</param>
		  ///<param name="userId">User ID to add</param>
        ///<returns>List of team members</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Delete(@"{projectid:[0-9]+}/team")]
        public IEnumerable<ParticipantWrapper> DeleteFromProjectTeam(int projectid, Guid userId)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();

            ProjectSecurity.DemandEditTeam(project);
            EngineFactory.GetProjectEngine().RemoveFromTeam(project, EngineFactory.GetParticipantEngine().GetByID(userId), true);
            return GetProjectTeam(projectid);
        }

          ///<summary>
          /// Updates the project team with the users IDs specified in the request
          ///</summary>
          ///<short>
          ///Updates project team
          ///</short>
          /// <category>Team</category>
          ///<param name="projectId">Project ID</param>
          ///<param name="participants">Users IDs to update team</param>
          ///<param name="notify">Notify project team</param>
        ///<returns>string with the number of project participants</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"{projectid:[0-9]+}/team")]
        public IEnumerable<ParticipantWrapper> UpdateProjectTeam(int projectId, IEnumerable<Guid> participants, bool notify)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectId).NotFoundIfNull();

            ProjectSecurity.DemandEditTeam(project);
            EngineFactory.GetProjectEngine().UpdateTeam(project, participants, notify);

            return GetProjectTeam(projectId);
        }

	      ///<summary>
	      ///Returns the list of all the tasks within the project with the ID specified in the request
	      ///</summary>
	      ///<short>
	      ///Tasks
	      ///</short>
	      /// <category>Tasks</category>
	      ///<param name="projectid">Project ID</param>
        ///<returns></returns>
        ///<exception cref="ItemNotFoundException">List of tasks</exception>
        [Read(@"{projectid:[0-9]+}/task")]
        public IEnumerable<TaskWrapper> GetProjectTasks(int projectid)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();

            return EngineFactory
                .GetTaskEngine().GetByProject(projectid, TaskStatus.Open, Guid.Empty)
                .Select(x => new TaskWrapper(x)).ToSmartList();
        }

        ///<summary>
        ///Returns the list with the detailed information about all the message matching the filter parameters specified in the request
        ///</summary>
        ///<short>
        /// Get message by Filter
        ///</short>
        /// <category>Discussions</category>
        ///<param name="projectid" optional="true"> Project ID</param>
        ///<param name="tag" optional="true">Project Tag</param>
        ///<param name="departament" optional="true">Departament GUID</param>
        ///<param name="participant" optional="true">Participant GUID</param>
        ///<param name="createdStart" optional="true">Minimum value of message creation date</param>
        ///<param name="createdStop" optional="true">Maximum value of message creation date</param>
        ///<param name="lastId">Last message ID</param>
        ///<param name="myProjects">Messages in my projects</param>
        ///<param name="follow">Followed messages</param>
        ///<returns>List of Messages</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"message/filter")]
        public IEnumerable<MessageWrapper> GetMessageByFilter(int projectid, int tag, Guid departament, Guid participant, 
                                                              ApiDateTime createdStart, ApiDateTime createdStop, int lastId, 
                                                              bool myProjects, bool follow)
        {
            var taskFilter = new TaskFilter
            {
                DepartmentId = departament,
                UserId = participant,
                FromDate = createdStart,
                ToDate = createdStop,
                SortBy = _context.SortBy,
                SortOrder = !_context.SortDescending,
                SearchText = _context.FilterValue,
                TagId = tag,
                Offset = _context.StartIndex,
                Max = _context.Count,
                MyProjects = myProjects,
                LastId = lastId,
                Follow = follow
            };

            if (projectid != 0)
                taskFilter.ProjectIds.Add(projectid);

            _context.SetDataPaginated();
            _context.SetDataFiltered();
            _context.SetDataSorted();

            return EngineFactory.GetMessageEngine().GetByFilter(taskFilter).NotFoundIfNull().Select(r => new MessageWrapper(r)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all the messages in the discussions within the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Messages
		  ///</short>
		  /// <category>Discussions</category>
		  ///<param name="projectid">Project ID</param>
        ///<returns>List of messages</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{projectid:[0-9]+}/message")]
        public IEnumerable<MessageWrapper> GetProjectMessages(int projectid)
        {
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();
            
            if (!ProjectSecurity.CanReadMessages(project)) throw ProjectSecurity.CreateSecurityException();

            return EngineFactory.GetMessageEngine().GetByProject(projectid).Select(x => new MessageWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Adds a message to the selected discussion within the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Add message
		  ///</short>
		  /// <category>Discussions</category>
		  ///<param name="projectid">project ID</param>
          ///<param name="title">discussion title</param>
          ///<param name="content">text of message</param>
		  ///<param name="participants">IDs (GUIDs) of users separated with ','</param>
          ///<param name="notify">notify participants</param>
        ///<returns></returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"{projectid:[0-9]+}/message")]
        public MessageWrapper AddProjectMessage(int projectid, string title, string content, string participants, bool? notify)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException(@"title can't be empty", "title");
            if (string.IsNullOrEmpty(content)) throw new ArgumentException(@"description can't be empty", "content");

            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();

            ProjectSecurity.DemandCreateMessage(project);

            var newMessage = new Message
            {
                Content = content,
                Title = title,
                Project = project,
            };

            EngineFactory.GetMessageEngine().SaveOrUpdate(newMessage, notify ?? true, ToGuidList(participants), null);
            return new MessageWrapper(newMessage);
        }

		  ///<summary>
		  ///Updates the selected message in the discussion within the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Update message
		  ///</short>
		  /// <category>Discussions</category>
		  ///<param name="messageid">message ID</param>
          ///<param name="projectid">project ID</param>
          ///<param name="title">discussion title</param>
          ///<param name="content">text of message</param>
		  ///<param name="participants">IDs (GUIDs) of users separated with ','</param>
          ///<param name="notify">notify participants</param>
        ///<returns></returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"message/{messageid:[0-9]+}")]
        public MessageWrapper UpdateProjectMessage(int messageid, int projectid, string title, string content, string participants, bool? notify)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException(@"title can't be empty", "title");
            if (string.IsNullOrEmpty(content)) throw new ArgumentException(@"description can't be empty", "content");

            var message = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();
            var project = EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull();

            ProjectSecurity.DemandEdit(message);

		    message.Project = Update.IfNotEmptyAndNotEquals(message.Project, project);
            message.Content = Update.IfNotEmptyAndNotEquals(message.Content, content);
            message.Title = Update.IfNotEmptyAndNotEquals(message.Title, title);
            EngineFactory.GetMessageEngine().SaveOrUpdate(message, notify ?? true, ToGuidList(participants), null);
            return new MessageWrapper(message);
        }

		  ///<summary>
		  ///Deletes the message with the ID specified in the request from a project discussion
		  ///</summary>
		  ///<short>
		  ///Delete message
		  ///</short>
		  /// <category>Discussions</category>
		  ///<param name="messageid">message ID</param>
        ///<returns></returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Delete(@"message/{messageid:[0-9]+}")]
        public MessageWrapper DeleteProjectMessage(int messageid)
        {
            var message = EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull();

            ProjectSecurity.DemandEdit(message);
            EngineFactory.GetMessageEngine().Delete(message);
            return new MessageWrapper(message);
        }

        private static IEnumerable<Guid> ToGuidList(string participants)
        {
            return !string.IsNullOrEmpty(participants) ?
                participants.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => new Guid(x))
                : new List<Guid>();
        }

		  ///<summary>
		  ///Returns the detailed information about the message with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Message
		  ///</short>
		  /// <category>Discussions</category>
		  ///<param name="messageid">Message ID</param>
        ///<returns>Message</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"message/{messageid:[0-9]+}")]
        public MessageWrapper GetProjectMessage(int messageid)
        {
            return new MessageWrapper(EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull());
        }

		  ///<summary>
		  ///Returns the detailed information about files attached to the message with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Message files
		  ///</short>
		  /// <category>Files</category>
		  ///<param name="messageid">Message ID</param>
        ///<returns> List of Message files</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"message/{messageid:[0-9]+}/files")]
        public IEnumerable<FileWrapper> GetMessageFiles(int messageid)
        {
            return
                EngineFactory.GetFileEngine().GetMessageFiles(
                    EngineFactory.GetMessageEngine().GetByID(messageid).NotFoundIfNull()).Select(x => new FileWrapper(x)).
                    ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of latest messages in the discussions within the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Latest messages
		  ///</short>
		  /// <category>Discussions</category>
        ///<returns>List of messages</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"message")]
        public IEnumerable<MessageWrapper> GetProjectRecentMessages()
        {
            var messages = EngineFactory.GetMessageEngine().GetMessages((int)_context.StartIndex, (int)_context.Count).Select(x => new MessageWrapper(x));
            _context.SetDataPaginated();
            return messages.ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of comments to the messages in the discussions within the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Message comments
		  ///</short>
		  /// <category>Comments</category>
		  ///<param name="messageid">Message ID</param>
        ///<returns>Comments for message</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"message/{messageid:[0-9]+}/comment")]
        public IEnumerable<CommentWrapper> GetProjectMessagesComments(int messageid)
        {
            if (!EngineFactory.GetMessageEngine().IsExists(messageid)) throw new ItemNotFoundException();
            return EngineFactory.GetCommentEngine().GetComments(EngineFactory.GetMessageEngine().GetByID(messageid)).Where(x => !x.Inactive).Select(
                    x => new CommentWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Adds a comment to the selected message in a discussion within the project with the content specified in the request. The parent comment ID can also be selected.
		  ///</summary>
		  ///<short>
          ///Add message comment
		  ///</short>
		  /// <category>Comments</category>
		  ///<param name="messageid">Message ID</param>
        ///<param name="content">Comment content</param>
		  ///<param name="parentId">Parrent comment ID</param>
        ///<returns></returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"message/{messageid:[0-9]+}/comment")]
        public CommentWrapper AddProjectMessagesComment(int messageid, string content, Guid parentId)
        {
            if (!EngineFactory.GetMessageEngine().IsExists(messageid)) throw new ItemNotFoundException();
            if (string.IsNullOrEmpty(content)) throw new ArgumentException(@"Comment text is empty", content);
            if (parentId != Guid.Empty && EngineFactory.GetCommentEngine().GetByID(parentId) == null) throw new ItemNotFoundException("parent comment not found");

            var comment = new Comment
            {
                Content = content,
                TargetUniqID = ProjectEntity.BuildUniqId<Message>(messageid),
                CreateBy = CurrentUserId,
                CreateOn = Core.Tenants.TenantUtil.DateTimeNow()
            };

            if (parentId != Guid.Empty)
                comment.Parent = parentId;

            EngineFactory.GetMessageEngine().SaveMessageComment(EngineFactory.GetMessageEngine().GetByID(messageid),
                                                                comment);
            return new CommentWrapper(comment);
        }

		  ///<summary>
		  ///Returns the list of events within the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Events
		  ///</short>
		  /// <category>Events</category>
		  ///<param name="projectid">project ID</param>
        ///<returns>List of events</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{projectid:[0-9]+}/event")]
        public IEnumerable<EventWrapper> GetProjectEvents(int projectid)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();

            return EngineFactory.GetEventEngine().GetByProject(projectid).Select(x => new EventWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the detailed information about the event with the ID specified in the request within the project
		  ///</summary>
		  ///<short>
		  ///Event
		  ///</short>
		  /// <category>Events</category>
		  ///<param name="eventid">Event ID</param>
        ///<returns>Event</returns>
        [Read(@"event/{eventid:[0-9]+}")]
        public EventWrapper GetProjectEvent(int eventid)
        {
            return new EventWrapper(EngineFactory.GetEventEngine().GetByID(eventid).NotFoundIfNull());
        }


		  ///<summary>
		  ///Returns the list of comments to the event with the ID specified in the request within the project
		  ///</summary>
		  ///<short>
		  ///Event comments
		  ///</short>
		  /// <category>Comments</category>
		  ///<param name="eventid">event ID</param>
        ///<returns>List of comments</returns>
        [Read(@"event/{eventid:[0-9]+}/comment")]
        public IEnumerable<CommentWrapper> GetProjectEventComments(int eventid)
        {
            return EngineFactory.GetCommentEngine().GetComments(EngineFactory.GetEventEngine().GetByID(eventid).NotFoundIfNull()).Where(x => !x.Inactive).Select(
                    x => new CommentWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Adds the task to the selected project with the parameters (responsible user ID, task description, deadline time, etc) specified in the request
		  ///</summary>
		  ///<short>
		  ///Add Task
		  ///</short>
		  /// <category>Tasks</category>
		  ///<param name="projectid">project ID</param>
		  ///<param name="responsible">responsible member ID</param>
        ///<param name="description">description</param>
        ///<param name="deadline">deadline time</param>
        ///<param name="priority">Low|Normal|High</param>
        ///<param name="title">title</param>
		  ///<param name="milestoneid">milestone ID</param>
        ///<param name="responsibles">list responsibles</param>
        ///<param name="notify">notify responsible</param>
        ///<returns>Created task</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"{projectid:[0-9]+}/task")]
        public TaskWrapper AddProjectTask(int projectid, Guid responsible, string description, ApiDateTime deadline, TaskPriority priority, string title, int milestoneid, IEnumerable<Guid> responsibles, bool notify)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException(@"title can't be empty", "title");

            ProjectSecurity.DemandCreateTask(EngineFactory.GetProjectEngine().GetByID(projectid).NotFoundIfNull());
            if (!EngineFactory.GetMilestoneEngine().IsExists(milestoneid) && milestoneid>0)
            {
                throw new ItemNotFoundException("Milestone not found");
            }
            var task = new Task
            {
                CreateBy = CurrentUserId,
                CreateOn = Core.Tenants.TenantUtil.DateTimeNow(),
                Deadline = deadline,
                Description = description ?? "",
                Responsible = responsible,
                Priority = priority,
                Status = TaskStatus.Open,
                Title = title,
                Project = EngineFactory.GetProjectEngine().GetByID(projectid),
                Milestone = milestoneid,
                Responsibles = new HashSet<Guid>(responsibles) { responsible }
            };
            EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, notify);
            return GetTask(task.ID);
        }

		  ///<summary>
		  ///Updates the selected task with the parameters (responsible user ID, task description, deadline time, etc) specified in the request
		  ///</summary>
		  ///<short>
		  ///Update Task
		  ///</short>
		  /// <category>Tasks</category>
		  ///<param name="taskid">task ID</param>
		  ///<param name="responsible">responsible member ID</param>
        ///<param name="description">description</param>
        ///<param name="deadline">deadline time</param>
        ///<param name="priority">priority</param>
        ///<param name="title">title</param>
		  ///<param name="milestoneid">milestone ID</param>
        ///<param name="responsibles">list responsibles</param>
        ///<param name="projectID">Project ID</param>
        ///<param name="notify">notify responsible</param>
        ///<returns>Updated task</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"task/{taskid:[0-9]+}")]
        public TaskWrapper UpdateProjectTask(int taskid, Guid responsible, string description, ApiDateTime deadline, TaskPriority priority, string title, int milestoneid, IEnumerable<Guid> responsibles, int? projectID, bool notify)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            if (string.IsNullOrEmpty(title)) throw new ArgumentException(@"title can't be empty", "title");

            var canWork = ProjectSecurity.CanWork(task);
            ProjectSecurity.DemandWork(task);

            if (!EngineFactory.GetMilestoneEngine().IsExists(milestoneid) && milestoneid > 0)
            {
                throw new ItemNotFoundException("Milestone not found");
            }

            task.Responsible = responsible;
            task.Responsibles = new HashSet<Guid>(responsibles){responsible};

            if (canWork > 1)
            {
                task.Deadline = Update.IfNotEquals(task.Deadline, deadline);
                task.Description = Update.IfNotEquals(task.Description, description);
                task.Priority = Update.IfNotEquals(task.Priority, priority);
                task.Title = Update.IfNotEmptyAndNotEquals(task.Title, title);
                task.Milestone = Update.IfNotEquals(task.Milestone, milestoneid);

                if (projectID.HasValue)
                {
                    var project = EngineFactory.GetProjectEngine().GetByID((int) projectID).NotFoundIfNull();
                    task.Project = project;
                }
            }

            EngineFactory.GetTaskEngine().SaveOrUpdate(task, null, notify);
            return GetTask(taskid);
        }

		  ///<summary>
		  ///Deletes the task with the ID specified in the request from the project
		  ///</summary>
		  ///<short>
		  ///Delete task
		  ///</short>
		  /// <category>Projects</category>
		  ///<param name="taskid">task ID</param>
          ///<returns>Deleted task</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Delete(@"task/{taskid:[0-9]+}")]
        public TaskWrapper DeleteTask(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            if (ProjectSecurity.CanWork(task) == 3)
                EngineFactory.GetTaskEngine().Delete(task);

		      return new TaskWrapper(task);
        }

		  ///<summary>
		  ///Returns the list of all tasks with the selected status in the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Tasks with status
		  ///</short>
		  /// <category>Tasks</category>
		  ///<param name="projectid">project ID</param>
        ///<param name="status">status of task. can be one of: notaccept|open|closed|disable|unclassified|notinmilestone</param>
        ///<returns>List of tasks</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{projectid:[0-9]+}/task/{status:(notaccept|open|closed|disable|unclassified|notinmilestone)}")]
        public IEnumerable<TaskWrapper> GetProjectTasks(int projectid, TaskStatus status)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();
            return EngineFactory
                .GetTaskEngine().GetByProject(projectid, status, Guid.Empty)
                .Select(x => new TaskWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all tasks in the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///All Tasks
		  ///</short>
		  /// <category>Tasks</category>
		  ///<param name="projectid">project ID</param>
        ///<returns>List of tasks</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{projectid:[0-9]+}/task/@all")]
        public IEnumerable<TaskWrapper> GetAllProjectTasks(int projectid)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();
            return EngineFactory
                .GetTaskEngine().GetByProject(projectid, null, Guid.Empty)
                .Select(x => new TaskWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all tasks for the current user with the selected status in the project with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///My tasks
		  ///</short>
		  /// <category>Tasks</category>
		  ///<param name="projectid">project ID</param>
        ///<param name="status">status of task. can be one of: notaccept|open|closed|disable|unclassified|notinmilestone</param>
        ///<returns>List of tasks</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{projectid:[0-9]+}/task/@self/{status:(notaccept|open|closed|disable|unclassified|notinmilestone)}")]
        public IEnumerable<TaskWrapper> GetProjectMyTasks(int projectid, TaskStatus status)
        {
            if (!EngineFactory.GetProjectEngine().IsExists(projectid)) throw new ItemNotFoundException();

            return EngineFactory
                .GetTaskEngine().GetByProject(projectid, status, CurrentUserId)
                .Select(x => new TaskWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list of all files within the entity (project, milestone, task) with the type and ID specified
		  ///</summary>
		  ///<short>
		  ///Entity files
		  ///</short>
		  /// <category>Files</category>
        ///<param name="entityType">Entity Type </param>
        ///<param name="entityID">Entity ID</param>
        ///<returns>Message</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"{entityID:[0-9]+}/entityfiles")]
        public IEnumerable<FileWrapper> GetEntityFiles(EntityType entityType, int entityID)
		  {
		    ProjectEntity entity = null;
            switch (entityType)
            {
                case EntityType.Message:
                    entity = EngineFactory.GetMessageEngine().GetByID(entityID).NotFoundIfNull();
                    break;

                case EntityType.Task:
                    var task = EngineFactory.GetTaskEngine().GetByID(entityID).NotFoundIfNull();
                    if (!ProjectSecurity.CanRead(task)) throw ProjectSecurity.CreateSecurityException();
                    entity = task;
                    break;
            }

            return EngineFactory.GetFileEngine().GetEntityFiles(entity).Select(x => new FileWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Uploads the selected files to the entity (project, milestone, task) with the type and ID specified
		  ///</summary>
		  ///<short>
		  /// Upload File to Entity
		  ///</short>
		  /// <category>Files</category>
        ///<param name="entityType">Entity Type </param>
        ///<param name="entityID">Entity ID</param>
		  ///<param name="files">file IDs</param>
        ///<returns>Uploaded Files</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"{entityID:[0-9]+}/entityfiles")]
        public IEnumerable<FileWrapper> UploadFilesToEntity(EntityType entityType, int entityID, IEnumerable<int> files)
        {

            switch (entityType)
            {
                case EntityType.Comment:
                    var comment = EngineFactory.GetMessageEngine().GetByID(entityID).NotFoundIfNull();
                    ProjectSecurity.DemandEdit(comment);
                    break;

                case EntityType.Task:
                    var task = EngineFactory.GetTaskEngine().GetByID(entityID).NotFoundIfNull();
                    ProjectSecurity.DemandEdit(task);
                    EngineFactory.GetTaskEngine().SubscribeToTask(task, CurrentUserId);
                    break;
            }

		      var filesEngine = files.Select(r => EngineFactory.GetFileEngine().GetFile(r, 1).NotFoundIfNull()).ToList();

            foreach (var file in filesEngine)
            {
                EngineFactory.GetFileEngine().AttachFileToEntity(entityType, entityID, file.ID);
            }

            return filesEngine.Select(x => new FileWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Detaches the selected file from the entity (project, milestone, task) with the type and ID specified
		  ///</summary>
		  ///<short>
		  /// Detach File from Entity
		  ///</short>
		  /// <category>Files</category>
        ///<param name="entityType">Entity Type </param>
        ///<param name="entityID">Entity ID</param>
		  ///<param name="fileid">file ID</param>
        ///<returns>Detached file</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Delete(@"{entityID:[0-9]+}/entityfiles")]
        public FileWrapper DetachFileFromEntity(EntityType entityType, int entityID, int fileid)
        {
            switch (entityType)
            {
                case EntityType.Comment:
                    var comment = EngineFactory.GetMessageEngine().GetByID(entityID).NotFoundIfNull();
                    ProjectSecurity.DemandEdit(comment);
                    break;

                case EntityType.Task:
                    var task = EngineFactory.GetTaskEngine().GetByID(entityID).NotFoundIfNull();
                    ProjectSecurity.DemandEdit(task);
                    break;
            }

            var file = EngineFactory.GetFileEngine().GetFile(fileid, 1).NotFoundIfNull();
            EngineFactory.GetFileEngine().DetachFileFromEntity(entityType, entityID, fileid);

		      return new FileWrapper(file);
        }

        #endregion

        #region activities

        ///<summary>
        ///Returns the list of all project activities matching the filter with the parameters specified in the request
        ///</summary>
        ///<short>
        ///Project Activities by filter
        ///</short>
        /// <category>Project Activities</category>
        ///<param name="projectId" optional="true">Project ID</param>
        ///<param name="periodStart" optional="true">Minimum value of activity date</param>
        ///<param name="periodStop" optional="true">Maximum value of activity date</param>
        ///<param name="user" optional="true">User GUID</param>
        ///<param name="entity" optional="true">Project Entity Type</param>
        ///<param name="lastId" optional="true">Last Id</param>
        ///<returns>List of Project Activities</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"activities/filter")]
        public IEnumerable<ProjectActivityWrapper> GetProjectActivitiesByFilter(int projectId, ApiDateTime periodStart, ApiDateTime periodStop, Guid user, EntityType? entity, int lastId)
        {
            var filter = new ProjectActivityFilter
                             {
                                 ProjectId = projectId,
                                 UserId = user,
                                 Type = entity.ToString(),
                                 From = periodStart,
                                 To = periodStop,
                                 SortBy = _context.SortBy,
                                 SortOrder = !_context.SortDescending,
                                 SearchText = _context.FilterValue,
                                 Offset = (int) _context.StartIndex,
                                 Max = (int) _context.Count,
                                 LastId = lastId
                             };

            _context.SetDataPaginated();
            _context.SetDataFiltered();
            _context.SetDataSorted();

            return EngineFactory.GetProjectActivityEngine().GetByFilter(filter).NotFoundIfNull().Select(a => new ProjectActivityWrapper(a)).ToSmartList();
        }

        #endregion
    }
}
