using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Documents;
using ASC.Api.Exceptions;
using ASC.Api.Projects.Wrappers;
using ASC.Api.Utils;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects
{
    public partial class ProjectApi
    {
        #region tasks

		 ///<summary>
		 ///Returns the list with the detailed information about all tasks for the current user
		 ///</summary>
		 ///<short>
		 ///My tasks
		 ///</short>
		 /// <category>Tasks</category>
        ///<returns>List of tasks</returns>
        [Read(@"task/@self")]
        public IEnumerable<TaskWrapper> GetMyTasks()
        {
            return EngineFactory
                .GetTaskEngine().GetByResponsible(CurrentUserId)
                .Select(x => new TaskWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Returns the list with the detailed information about the tasks for the current user with the status specified in the request
		  ///</summary>
		  ///<short>
		  ///My tasks by status
		  ///</short>
		  /// <category>Tasks</category>
        ///<param name="status">status of task/ one of notaccept|open|closed|disable|unclassified|notinmilestone</param>
        ///<returns>List of tasks</returns>
        [Read(@"task/@self/{status:(notaccept|open|closed|disable|unclassified|notinmilestone)}")]
        public IEnumerable<TaskWrapper> GetMyTasks(TaskStatus status)
        {
            return EngineFactory
                .GetTaskEngine().GetByResponsible(CurrentUserId, status)
                .Select(x => new TaskWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  /// Returns the detailed information about the task with the ID specified in the request
		  ///</summary>
		  ///<short>
		  /// Get Task
		  ///</short>
		  /// <category>Tasks</category>
        ///<param name="taskid">task id</param>
        ///<returns>Task</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"task/{taskid:[0-9]+}")]
        public TaskWrapper GetTask(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            if (task.Milestone == 0) return new TaskWrapper(task);

            var milestone = EngineFactory.GetMilestoneEngine().GetByID(task.Milestone).NotFoundIfNull();
            return new TaskWrapper(task, milestone);
        }

        ///<summary>
        ///Returns the list with the detailed information about all the tasks matching the filter parameters specified in the request
        ///</summary>
        ///<short>
        /// Get Task by Filter
        ///</short>
        /// <category>Tasks</category>
        ///<param name="projectid" optional="true"> Project Id</param>
        ///<param name="tag" optional="true">Project Tag</param>
        ///<param name="status" optional="true">Task Status</param>
        ///<param name="departament" optional="true">Departament GUID</param>
        ///<param name="participant" optional="true">Participant GUID</param>
        ///<param name="milestone" optional="true">Milestone ID</param>
        ///<param name="deadlineStart" optional="true">Minimum value of task deadline</param>
        ///<param name="deadlineStop" optional="true">Maximum value of task deadline</param>
        ///<param name="lastId">Last task id</param>
        ///<param name="myProjects">Tasks in My Projects</param>
        ///<param name="myMilestones">Tasks in My Milestones</param>
        ///<param name="follow">Followed tasks</param>
        ///<returns>List of Tasks</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"task/filter")]
        public IEnumerable<TaskWrapper> GetTaskByFilter(int projectid, bool myProjects, int? milestone, bool myMilestones, int tag, 
                                                        TaskStatus? status, bool follow, Guid departament, Guid? participant, 
                                                        ApiDateTime deadlineStart, ApiDateTime deadlineStop, int lastId)
        {
            var taskFilter = new TaskFilter
                                 {
                                     DepartmentId = departament,
                                     ParticipantId = participant,
                                     Milestone = milestone,
                                     FromDate = deadlineStart,
                                     ToDate = deadlineStop,
                                     SortBy = _context.SortBy,
                                     SortOrder = !_context.SortDescending,
                                     SearchText = _context.FilterValue,
                                     TagId = tag,
                                     Offset = _context.StartIndex,
                                     Max = _context.Count,
                                     LastId = lastId,
                                     MyProjects = myProjects,
                                     MyMilestones = myMilestones,
                                     Follow = follow
                                 };

            if (projectid != 0)
                taskFilter.ProjectIds.Add(projectid);

            if (status != null)
                taskFilter.TaskStatuses.Add((TaskStatus) status);


            _context.SetDataPaginated();
            _context.SetDataFiltered();
            _context.SetDataSorted();

            return EngineFactory.GetTaskEngine().GetByFilter(taskFilter).NotFoundIfNull().Select(r => new TaskWrapper(r)).ToSmartList();
        }

		  ///<summary>
		  /// Returns the list of all files attached to the task with the ID specified in the request
		  ///</summary>
		  ///<short>
		  /// Get Task files
		  ///</short>
		  /// <category>Files</category>
        ///<param name="taskid">task id</param>
        ///<returns>List files</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"task/{taskid:[0-9]+}/files")]
        public IEnumerable<FileWrapper> GetTaskFiles(int taskid)
        {
            return
                EngineFactory.GetFileEngine().GetTaskFiles(
                    EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull()).Select(x => new FileWrapper(x)).
                    ToSmartList();
        }

		  ///<summary>
		  /// Uploads the file specified in the request to the selected task
		  ///</summary>
		  ///<short>
		  /// Upload File to Task
		  ///</short>
		  /// <category>Files</category>
        ///<param name="taskid">task id</param>
        ///<param name="files">files id</param>
        ///<returns>List of tasks</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"task/{taskid:[0-9]+}/files")]
        public TaskWrapper UploadFilesToTask(int taskid, IEnumerable<int> files)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            foreach (var fileid in files)
            {
                EngineFactory.GetFileEngine().GetFile(fileid, 1).NotFoundIfNull();
                EngineFactory.GetFileEngine().AttachFileToTask(taskid, fileid);
            }

            return new TaskWrapper(task);
        }

		  ///<summary>
		  /// Detaches the selected file from the task with the ID specified in the request
		  ///</summary>
		  ///<short>
		  /// Detach File from Task
		  ///</short>
		  /// <category>Files</category>
        ///<param name="taskid">task id</param>
        ///<param name="fileid">file id</param>
        ///<returns>Task</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Delete(@"task/{taskid:[0-9]+}/files")]
        public TaskWrapper DetachFileFromTask(int taskid, int fileid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            EngineFactory.GetFileEngine().GetFile(fileid, 1).NotFoundIfNull();
            EngineFactory.GetFileEngine().DetachFileFromTask(taskid, fileid);

            return new TaskWrapper(task);
        }

        ///<summary>
        ///Returns the list with the detailed information about all the timespend matching the filter parameters specified in the request
        ///</summary>
        ///<short>
        /// Get timespend by Filter
        ///</short>
        /// <category>timespend</category>
        ///<param name="projectid" optional="true"> Project Id</param>
        ///<param name="tag" optional="true">Project Tag</param>
        ///<param name="departament" optional="true">Departament GUID</param>
        ///<param name="participant" optional="true">Participant GUID</param>
        ///<param name="createStart" optional="true">Minimum value of create time</param>
        ///<param name="createStop" optional="true">Maximum value of create time</param>
        ///<param name="lastId">Last timespend id</param>
        ///<returns>List of Timespends</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"time/filter")]
        public IEnumerable<TimeWrapper> GetTaskTimeByFilter(int projectid, int tag, Guid departament, Guid participant, ApiDateTime createStart, ApiDateTime createStop, int lastId)
        {
            var taskFilter = new TaskFilter
            {
                DepartmentId = departament,
                UserId = participant,
                FromDate = createStart,
                ToDate = createStop,
                SortBy = _context.SortBy,
                SortOrder = !_context.SortDescending,
                SearchText = _context.FilterValue,
                TagId = tag,
                Offset = _context.StartIndex,
                Max = _context.Count,
                LastId = lastId
            };

            if (projectid != 0)
                taskFilter.ProjectIds.Add(projectid);

            _context.SetDataPaginated();
            _context.SetDataFiltered();
            _context.SetDataSorted();

            return EngineFactory.GetTimeTrackingEngine().GetByFilter(taskFilter).NotFoundIfNull().Select(r => new TimeWrapper(r)).ToSmartList();
        }

		  ///<summary>
		  /// Returns the time spent on the task with the ID specified in the request
		  ///</summary>
		  ///<short>
		  /// Get Time Spent
		  ///</short>
		  /// <category>Time</category>
        ///<param name="taskid">id of task</param>
        ///<returns></returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"task/{taskid:[0-9]+}/time")]
        public IEnumerable<TimeWrapper> GetTaskTime(int taskid)
        {
            if (!EngineFactory.GetTaskEngine().IsExists(taskid)) throw new ItemNotFoundException();
            return EngineFactory.GetTimeTrackingEngine().GetByTask(taskid).NotFoundIfNull().Select(x => new TimeWrapper(x)).ToSmartList();
        }

		  ///<summary>
		  ///Adds the time to the selected task with the time parameters specified in the request
		  ///</summary>
		  ///<short>
		  ///Add Task time
		  ///</short>
		  /// <category>Time</category>
        ///<param name="taskid">id of task</param>
        ///<param name="note">note</param>
        ///<param name="date">date when</param>
        ///<param name="personId">person that spends time</param>
        ///<param name="hours">hours spent</param>
        ///<param name="projectId">project id</param>
        ///<returns>created time</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"task/{taskid:[0-9]+}/time")]
        public TimeWrapper AddTaskTime(int taskid, string note, DateTime date, Guid personId, float hours, int projectId)
        {
            if (date == DateTime.MinValue) throw new ArgumentException("date can't be empty");
            if (personId == Guid.Empty) throw new ArgumentException("person can't be empty");

		    var task = EngineFactory.GetTaskEngine().GetByID(taskid);

            if (task == null) throw new ItemNotFoundException();

            if (!EngineFactory.GetProjectEngine().IsExists(projectId)) throw new ItemNotFoundException("project");

            var ts = new TimeSpend
            {
                Date = date.Date,
                Person = personId,
                Hours = hours,
                Note = note,
                Task = task
            };

            EngineFactory.GetTimeTrackingEngine().SaveOrUpdate(ts);

            return new TimeWrapper(ts);
        }

		  ///<summary>
		  ///Updates the time for the selected task with the time parameters specified in the request
		  ///</summary>
		  ///<short>
		  ///Update Task time
		  ///</short>
		  /// <category>Time</category>
        ///<param name="timeid">id of time spend</param>
        ///<param name="note">note</param>
        ///<param name="date">date when</param>
        ///<param name="personId">person that spends time</param>
        ///<param name="hours">hours spent</param>
        ///<returns>created time</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"time/{timeid:[0-9]+}")]
        public TimeWrapper UpdateTime(int timeid, string note, DateTime date, Guid personId, float hours)
        {
            if (date == DateTime.MinValue) throw new ArgumentException("date can't be empty");
            if (personId == Guid.Empty) throw new ArgumentException("person can't be empty");

            var time = EngineFactory.GetTimeTrackingEngine().GetByID(timeid).NotFoundIfNull();

            time.Date = date.Date;
            time.Person = personId;
            time.Hours = hours;
            time.Note = note;

            EngineFactory.GetTimeTrackingEngine().SaveOrUpdate(time);
            return new TimeWrapper(time);
        }

		  ///<summary>
		  ///Deletes the time from the task with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Delete Time spent
		  ///</short>
		  /// <category>Time</category>
        ///<param name="timeid">time spend id</param>
        ///<returns></returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Delete(@"time/{timeid:[0-9]+}")]
        public TimeWrapper DeleteTaskTime(int timeid)
        {
            var time = EngineFactory.GetTimeTrackingEngine().GetByID(timeid).NotFoundIfNull();

            EngineFactory.GetTimeTrackingEngine().Delete(timeid);
            return new TimeWrapper(time);
        }

		  ///<summary>
		  /// Updates the status of the task with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Update task status
		  ///</short>
		  /// <category>Tasks</category>
        ///<param name="taskid">task id</param>
        ///<param name="status">status of task. can be one of: notaccept|open|closed|disable|unclassified|notinmilestone</param>
        ///<returns>Updated Task</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"task/{taskid:[0-9]+}/status")]
        public TaskWrapper UpdateTask(int taskid, TaskStatus status)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
		    ProjectSecurity.DemandEdit(task);
		    EngineFactory.GetTaskEngine().ChangeStatus(task, status);

            return GetTask(taskid);
        }

		  ///<summary>
		  ///Returns the list of the comments for the task with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Task comments
		  ///</short>
		  /// <category>Comments</category>
        ///<param name="taskid">task id</param>
        ///<returns>List of comments</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"task/{taskid:[0-9]+}/comment")]
        public IEnumerable<CommentWrapper> GetTaskComments(int taskid)
        {
            return
                EngineFactory.GetCommentEngine().GetComments(EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull()).Where(x => !x.Inactive).Select(
                    x => new CommentWrapper(x)).ToSmartList();
        }


		  ///<summary>
		  ///Adds the comments for the selected task with the comment text and parent comment ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Add Task comment
		  ///</short>
		  /// <category>Comments</category>
        ///<param name="taskid">task id</param>
        ///<param name="content">comment text</param>
        ///<param name="parentid">parent comment id</param>
        ///<returns>List of comments</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"task/{taskid:[0-9]+}/comment")]
        public CommentWrapper AddTaskComments(int taskid, string content, Guid parentid)
        {
            if (string.IsNullOrEmpty(content)) throw new ArgumentException(@"Comment text is empty", content);
            if (parentid != Guid.Empty && EngineFactory.GetCommentEngine().GetByID(parentid) == null) throw new ItemNotFoundException("parent comment not found");

            var comment = new Comment
            {
                Content = content,
                TargetUniqID = ProjectEntity.BuildUniqId<Task>(taskid),
                CreateBy = CurrentUserId,
                CreateOn = Core.Tenants.TenantUtil.DateTimeNow()
            };

            if (parentid != Guid.Empty)
                comment.Parent = parentid;

            EngineFactory.GetTaskEngine().SaveOrUpdateTaskComment(EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull(),comment);
            return new CommentWrapper(comment);
        }

		  ///<summary>
		  ///Notify the responsible for the task with the ID specified in the request about the task
		  ///</summary>
		  ///<short>
		  ///Notify Task Responsible
		  ///</short>
		  /// <category>Tasks</category>
        /// <returns>Task</returns>
        ///<param name="taskid">task id</param>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"task/{taskid:[0-9]+}/notify")]
        public TaskWrapper NotifyTaskResponsible(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            ProjectSecurity.DemandEdit(task);

            EngineFactory.GetTaskEngine().NotifyResponsible(task);

		    return new TaskWrapper(task);
        }

		  ///<summary>
		  ///Subscribe to notifications about the actions performed with the task with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Subscribe to task action
		  ///</short>
		  /// <category>Tasks</category>
          /// <returns>Task</returns>
        ///<param name="taskid">task id</param>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"task/{taskid:[0-9]+}/subscribe")]
        public TaskWrapper SubscribeToTask(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            ProjectSecurity.DemandAuthentication();

            EngineFactory.GetTaskEngine().FollowTask(task);

            return new TaskWrapper(task);
        }

		  ///<summary>
		  ///Checks subscription to notifications about the actions performed with the task with the ID specified in the request
		  ///</summary>
		  ///<short>
		  ///Check subscription to task action
		  ///</short>
		  /// <category>Tasks</category>
        ///<param name="taskid">task id</param>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"task/{taskid:[0-9]+}/subscribe")]
        public bool IsSubscribeToTask(int taskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            ProjectSecurity.DemandAuthentication();

            return  EngineFactory.GetTaskEngine().IsSubscribedToTask(task);
        }

        #endregion

        #region subtasks

        ///<summary>
        /// Creates the subtask with the selected title and responsible within the parent task specified in the request
        ///</summary>
        ///<short>
        /// Create subtask
        ///</short>
        /// <category>Tasks</category>
        ///<param name="taskid">parent task id</param>
        ///<param name="responsible">subtask responsible</param>
        ///<param name="title">subtask title</param>
        ///<returns>Task</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Create(@"task/{taskid:[0-9]+}")]
        public SubtaskWrapper AddSubtask(int taskid, Guid responsible, string title)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException(@"title can't be empty", "title");
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();

            if (task.Status == TaskStatus.Closed) throw new ArgumentException(@"task can't be closed");

            var subtask = new Subtask
            {
                Responsible = responsible,
                Task = task.ID,
                Status = TaskStatus.Open,
                Title = title
            };

            subtask = EngineFactory.GetSubtaskEngine().SaveOrUpdate(subtask, task);

            return new SubtaskWrapper(subtask, task);
        }

        ///<summary>
        /// Updates the subtask with the selected title and responsible with the subtask ID specified in the request
        ///</summary>
        ///<short>
        /// Update subtask
        ///</short>
        /// <category>Tasks</category>
        ///<param name="taskid">task id</param>
        ///<param name="subtaskid">subtask id</param>
        ///<param name="responsible">subtask responsible</param>
        ///<param name="title">subtask title</param>
        ///<returns>Task</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"task/{taskid:[0-9]+}/{subtaskid:[0-9]+}")]
        public SubtaskWrapper UpdateSubtask(int taskid, int subtaskid, Guid responsible, string title)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException(@"title can't be empty", "title");
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            var subtask = task.SubTasks.Find(r => r.ID == subtaskid).NotFoundIfNull();

            subtask.Responsible = responsible;
            subtask.Title = Update.IfNotEmptyAndNotEquals(subtask.Title, title);

            subtask = EngineFactory.GetSubtaskEngine().SaveOrUpdate(subtask, task);

            return new SubtaskWrapper(subtask, task);
        }

        ///<summary>
        /// Deletes the selected subtask from the parent task with the ID specified in the request
        ///</summary>
        ///<short>
        /// Delete subtask
        ///</short>
        ///<category>Tasks</category>
        ///<param name="taskid">task id</param>
        ///<param name="subtaskid">subtask id</param>
        ///<exception cref="ItemNotFoundException"></exception>
        [Delete(@"task/{taskid:[0-9]+}/{subtaskid:[0-9]+}")]
        public SubtaskWrapper DeleteSubtask(int taskid, int subtaskid)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            var subtask = task.SubTasks.Find(r => r.ID == subtaskid).NotFoundIfNull();

            EngineFactory.GetSubtaskEngine().Delete(subtask, task);

            return new SubtaskWrapper(subtask, task);

        }

        ///<summary>
        /// Updates the selected subtask status in the parent task with the ID specified in the request
        ///</summary>
        ///<short>
        ///Update subtask status
        ///</short>
        /// <category>Tasks</category>
        ///<param name="taskid">task id</param>
        ///<param name="subtaskid">subtask id</param>
        ///<param name="status">status of task. can be one of: open|closed|disable|unclassified</param>
        ///<returns>Updated Task</returns>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"task/{taskid:[0-9]+}/{subtaskid:[0-9]+}/status")]
        public SubtaskWrapper UpdateSubtask(int taskid, int subtaskid, TaskStatus status)
        {
            var task = EngineFactory.GetTaskEngine().GetByID(taskid).NotFoundIfNull();
            var subtask = task.SubTasks.Find(r => r.ID == subtaskid).NotFoundIfNull();

            ProjectSecurity.DemandEdit(task, subtask);
            EngineFactory.GetSubtaskEngine().ChangeStatus(task, subtask, status);

            return new SubtaskWrapper(subtask, task);
        }

        #endregion
    }
}