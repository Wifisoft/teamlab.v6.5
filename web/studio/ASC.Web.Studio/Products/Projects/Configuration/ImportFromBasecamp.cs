using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using ASC.Common.Threading.Workers;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Projects.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.Engine;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using BasecampRestAPI;
using log4net;
using Comment = ASC.Projects.Core.Domain.Comment;
using Milestone = ASC.Projects.Core.Domain.Milestone;
using Project = ASC.Projects.Core.Domain.Project;
using System.Security.Principal;
using ASC.Web.Files.Api;
using ASC.Web.Studio.Core;
using ASC.Common.Web;
using ASC.Web.Core.Users;

namespace ASC.Web.Projects.Configuration
{
    public class ImportQueue
    {
        private static readonly WorkerQueue<ImportFromBasecamp> Imports = new WorkerQueue<ImportFromBasecamp>(4, TimeSpan.FromMinutes(30));

        private static readonly List<ImportFromBasecamp> Completed = new List<ImportFromBasecamp>();

        static ImportQueue()
        {
            Imports.Start(DoImport);
        }

        public static int Add(string url, string token, bool processClosed, bool disableNotifications)
        {
            lock (Imports.SynchRoot)
            {
                if (Imports.GetItems().Count(x => x.Id == CoreContext.TenantManager.GetCurrentTenant().TenantId) > 0)
                    throw new DuplicateNameException("Import already running");

            }
            lock (Completed)
            {
                Completed.RemoveAll(x => x.Id == CoreContext.TenantManager.GetCurrentTenant().TenantId);
            }

            SecurityContext.DemandPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser);
            ProjectSecurity.DemandCreateProject();

            var importTask = new ImportFromBasecamp(url, token, HttpContext.Current, SecurityContext.CurrentAccount.ID, processClosed, disableNotifications, Global.EngineFactory);//NOTE: engine factory newly created!!!
            lock (Imports.SynchRoot)
            {
                Imports.Add(importTask);
            }
            return importTask.Id;
        }

        public static ImportStatus GetStatus()
        {
            ImportFromBasecamp importd = null;
            lock (Imports.SynchRoot)
            {
                importd =
                    Imports.GetItems().Where(x => x.Id == CoreContext.TenantManager.GetCurrentTenant().TenantId).
                        SingleOrDefault();
            }
            if (importd == null)
            {
                lock (Completed)
                {
                    //Maybe it's completed already
                    importd = Completed.Where(x => x.Id == CoreContext.TenantManager.GetCurrentTenant().TenantId).
                        FirstOrDefault();
                }
            }
            if (importd != null)
            {
                return importd.Status;
            }
            throw new KeyNotFoundException("Import not found");
        }

        private static void DoImport(ImportFromBasecamp obj)
        {
            try
            {
                obj.StartImport();
                obj.Status.Completed = true;
                NotifyClient.Instance.SendAboutImportComplite(obj.InitiatorId);
            }
            catch (Exception e)
            {
                obj.Status.LogError(SettingsResource.ImportFailed, e);
                obj.Status.Error = e.GetType().ToString();
                obj.LogError("generic error", e);
            }
            finally
            {
                obj.Status.CompletedAt = DateTime.Now;
                lock (Completed)
                {
                    Completed.Add(obj);
                }
            }
        }
    }

    public class ImportStatusLogEntry
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public Exception Ex { get; set; }

        public ImportStatusLogEntry(Exception e)
        {
            Time = DateTime.UtcNow;
            Ex = e;
        }
    }

    public class ImportStatus
    {
        public string Url { get; set; }
        public string Token { get; set; }
        private double _userProgress;
        public double UserProgress
        {
            get { return Math.Min(Math.Max(_userProgress, 0), 100); }
            set { _userProgress = value; }
        }

        private double _projectProgress;
        public double ProjectProgress
        {
            get { return Math.Min(Math.Max(_projectProgress, 0), 100); ; }
            set { _projectProgress = value; }
        }

        private double _fileProgress;
        public double FileProgress
        {
            get { return Math.Min(Math.Max(_fileProgress, 0), 100); ; }
            set { _fileProgress = value; }
        }

        public string Error { get; set; }

        public bool Completed
        {
            get { return FileProgress == 100 && ProjectProgress == 100 && UserProgress == 100 && string.IsNullOrEmpty(Error); }
            set
            {
                FileProgress = 100;
                UserProgress = 100;
                ProjectProgress = 100;
                Error = null;
            }
        }

        public DateTime CompletedAt { get; set; }

        public List<ImportStatusLogEntry> Log { get; set; }

        public ImportStatus(string url, string token)
        {
            Url = url;
            Token = token;
            Log = new List<ImportStatusLogEntry>();
        }

        public void LogInfo(string message)
        {
            Log.Add(new ImportStatusLogEntry(null) { Message = HttpUtility.HtmlEncode(message), Type = "info" });
        }
        public void LogError(string message, Exception e)
        {
            Log.Add(new ImportStatusLogEntry(e) { Message = HttpUtility.HtmlEncode(message), Type = "error" });
        }
        public void LogWarn(string message)
        {
            Log.Add(new ImportStatusLogEntry(null) { Message = HttpUtility.HtmlEncode(message), Type = "warn" });
        }
    }

    public class ImportFromBasecamp
    {
        #region Members

        private List<UserIDWrapper> NewUsersID;
        private List<ProjectIDWrapper> NewProjectsID;
        private List<MilestoneIDWrapper> NewMilestonesID;
        private List<MessageIDWrapper> NewMessagesID;
        private List<TaskIDWrapper> NewTasksID;
        private List<FileCategoryIDWrapper> NewFileCategoriesID;
        private List<FileIDWrapper> NewFilesID;
        private List<IAttachment> Attachments;
        private readonly string _url;
        private readonly string _token;
        private HttpContext Context;
        private readonly Guid _initiatorId;
        private readonly bool _withClosed;
        private readonly bool _disableNotifications;
        private readonly EngineFactory _engineFactory;
        public int Id;
        public ImportStatus Status { get; set; }
        private ILog _log;
        IPrincipal _principal;

        public Guid InitiatorId
        {
            get { return _initiatorId; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ImportFromBasecamp)) return false;
            return Equals((ImportFromBasecamp)obj);
        }

        public ImportFromBasecamp(string url, string token, HttpContext context, Guid initiatorId, bool withClosed,bool disableNotifications, EngineFactory engineFactory)
        {
            NewUsersID = new List<UserIDWrapper>();
            NewProjectsID = new List<ProjectIDWrapper>();
            NewMilestonesID = new List<MilestoneIDWrapper>();
            NewMessagesID = new List<MessageIDWrapper>();
            NewTasksID = new List<TaskIDWrapper>();
            NewFileCategoriesID = new List<FileCategoryIDWrapper>();
            NewFilesID = new List<FileIDWrapper>();
            Attachments = new List<IAttachment>();


            _url = PrepUrl(url).ToString().TrimEnd('/');
            _token = token;
            Context = context;
            _initiatorId = initiatorId;
            _withClosed = withClosed;
            _disableNotifications = disableNotifications;
            _engineFactory = engineFactory;
            _engineFactory.DisableNotifications = disableNotifications;
            Status = new ImportStatus(_url, _token);
            Id = CoreContext.TenantManager.GetCurrentTenant().TenantId;
            _log = LogManager.GetLogger("ASC.Project.BasecampImport");
            _principal = Thread.CurrentPrincipal;
        }

        private static Uri PrepUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                if (!(url.StartsWith(Uri.UriSchemeHttp) || url.StartsWith(Uri.UriSchemeHttps)))
                {
                    //Add and try again
                    url = Uri.UriSchemeHttps + "://" + url;
                    return PrepUrl(url);
                }
                return new Uri(url, UriKind.Absolute);
            }
            return uri;
        }

        private void LogStatus(string message)
        {
            _log.DebugFormat("in {0}; {1} {2}", _initiatorId, _url, message);
        }
        internal void LogError(string message, Exception e)
        {
            _log.Error(string.Format("in {0}; {1} {2}", _initiatorId, _url, message), e);
        }

        public void StartImport()
        {
            LogStatus("started");

            CoreContext.TenantManager.SetCurrentTenant(Id);
            Thread.CurrentPrincipal = _principal;

            Status.LogInfo(SettingsResource.ImportStarted);
            var basecampManager = BaseCamp.GetInstance(_url, _token, "X");
            LogStatus("import users");
            SaveUsers(basecampManager);
            LogStatus("import projects");
            SaveProjects(basecampManager);
            LogStatus("import files");
            SaveFiles(basecampManager, Attachments);
            Status.LogInfo(SettingsResource.ImportCompleted);
        }

        #region Save Functions

        private void SaveUsers(BaseCamp basecampManager)
        {
            var employees = basecampManager.People;
            var step = 100.0 / employees.Count();
            foreach (var person in employees.Where(x => _withClosed ? true : !x.Deleted))
            {
                try
                {

                    Status.UserProgress += step;
                    Guid userID = FindUserByEmail(person.EmailAddress);

                    if (userID.Equals(Guid.Empty))
                    {
                        UserInfo userInfo = new UserInfo()
                        {
                            Email = person.EmailAddress,
                            FirstName = person.FirstName,
                            LastName = person.LastName,
                            Title = person.Title,
                            Status = person.Deleted ? EmployeeStatus.Terminated : EmployeeStatus.Active,
                        };

                        if (!string.IsNullOrEmpty(person.PhoneNumberMobile))
                            userInfo.AddSocialContact(SocialContactsManager.ContactType_mobphone, person.PhoneNumberMobile);
                        if (!string.IsNullOrEmpty(person.PhoneNumberHome))
                            userInfo.AddSocialContact(SocialContactsManager.ContactType_phone, person.PhoneNumberHome);
                        if (!string.IsNullOrEmpty(person.PhoneNumberOffice))
                            userInfo.AddSocialContact(SocialContactsManager.ContactType_phone, person.PhoneNumberOffice);
                        if (!string.IsNullOrEmpty(person.PhoneNumberFax))
                            userInfo.AddSocialContact(SocialContactsManager.ContactType_phone, person.PhoneNumberFax);
                        if (!string.IsNullOrEmpty(person.ImHandle))
                            switch (person.ImService)
                            {
                                case "MSN":
                                    userInfo.AddSocialContact(SocialContactsManager.ContactType_msn, person.ImHandle);
                                    break;
                                case "ICQ":
                                    userInfo.AddSocialContact(SocialContactsManager.ContactType_icq, person.ImHandle);
                                    break;
                                case "Yahoo":
                                    userInfo.AddSocialContact(SocialContactsManager.ContactType_yahoo, person.ImHandle);
                                    break;
                                case "Jabber":
                                    userInfo.AddSocialContact(SocialContactsManager.ContactType_jabber, person.ImHandle);
                                    break;
                                case "Skype":
                                    userInfo.AddSocialContact(SocialContactsManager.ContactType_skype, person.ImHandle);
                                    break;
                                case "Google":
                                    userInfo.AddSocialContact(SocialContactsManager.ContactType_gmail, person.ImHandle);
                                    break;
                            }

                        var newUserInfo = UserManagerWrapper.AddUser(userInfo, UserManagerWrapper.GeneratePassword(),false,!_disableNotifications);
                        if (person.Administrator)
                            CoreContext.UserManager.AddUserIntoGroup(newUserInfo.ID, ASC.Core.Users.Constants.GroupAdmin.ID);
                        NewUsersID.Add(new UserIDWrapper() { inBasecamp = person.ID, inProjects = newUserInfo.ID });

                        //save user avatar
                        const string emptyAvatar = "http://asset1.37img.com/global/missing/avatar.png?r=3";//TODO:?!!! Wtf??!!
                        if (person.AvatarUrl != emptyAvatar)
                            UserPhotoManager.SaveOrUpdatePhoto(newUserInfo.ID, StreamFile(person.AvatarUrl));
                    }
                    else
                    {
                        NewUsersID.Add(new UserIDWrapper() { inBasecamp = person.ID, inProjects = userID });
                    }
                }
                catch (Exception e)
                {
                    Status.LogError(string.Format(SettingsResource.FailedToSaveUser, person.EmailAddress), e);
                    LogError(string.Format("user '{0}' failed", person.EmailAddress), e);
                    NewUsersID.RemoveAll(x => x.inBasecamp == person.ID);
                }
            }
        }

        private void SaveProjects(BaseCamp basecampManager)
        {
            var projects = basecampManager.Projects;
            var step = 50.0 / projects.Count();
            var projectEngine = _engineFactory.GetProjectEngine();
            var participantEngine = _engineFactory.GetParticipantEngine();
            foreach (var project in projects.Where(x => _withClosed ? true : x.Status == "active"))
            {
                try
                {
                    Status.LogInfo(string.Format(SettingsResource.ImportProjectStarted, project.Name));
                    Status.ProjectProgress += step;
                    var newProject = new Project()
                                             {
                                                 Status = project.Status == "active" ? ProjectStatus.Open : ProjectStatus.Closed,
                                                 Title = ReplaceLineSeparator(project.Name),
                                                 Description = project.Description,
                                                 Responsible = _initiatorId,
                                                 Private = true
                                             };

                    projectEngine.SaveOrUpdate(newProject, true);
                    Participant prt = participantEngine.GetByID(newProject.Responsible);
                    projectEngine.AddToTeam(newProject, prt, true);

                    foreach (var wrapper in
                        project.People.SelectMany(user => NewUsersID.Where(wrapper => user.ID == wrapper.inBasecamp)))
                    {
                        prt = participantEngine.GetByID(wrapper.inProjects);
                        projectEngine.AddToTeam(newProject, prt, true);
                        
                        //check permission
                        var user = project.People.ToList().Find(p => p.ID == wrapper.inBasecamp);
                        if (user!=null)
                        {
                            if (!user.HasAccessToNewProjects)
                            {
                                switch (user.CanPost)
                                {
                                    case 1:
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Messages, true);
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Files, true);
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Tasks, false);
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Milestone, false);
                                        break;
                                    case 2:
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Messages, true);
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Files, true);
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Tasks, true);
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Milestone, false);
                                        break;
                                    case 3:
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Messages, true);
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Files, true);
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Tasks, true);
                                        projectEngine.SetTeamSecurity(newProject, prt, ProjectTeamSecurity.Milestone, true);
                                        break;
                                }
                            }
                        }
                    }
                    NewProjectsID.Add(new ProjectIDWrapper() { inBasecamp = project.ID, inProjects = newProject.ID });
                }
                catch (Exception e)
                {
                    Status.LogError(string.Format(SettingsResource.FailedToSaveProject, project.Name), e);
                    LogError(string.Format("project '{0}' failed", project.Name), e);
                    NewProjectsID.RemoveAll(x => x.inBasecamp == project.ID);
                }

            }

            //Select only suceeded projects
            var projectsToProcess = projects.Where(x => NewProjectsID.Count(y => y.inBasecamp == x.ID) > 0).ToList();
            step = 50.0 / projectsToProcess.Count;
            foreach (var project in projectsToProcess)
            {
                Status.LogInfo(string.Format(SettingsResource.ImportProjectDataStarted, project.Name));

                Status.ProjectProgress += step;
                SaveMilestones(project.Milestones);

                var messages = project.RecentMessages;
                foreach (var message in messages)
                {
                    SaveMessages(message);
                }

                var todoLists = project.ToDoLists;
                foreach (var todoList in todoLists)
                {
                    SaveTasks(todoList);
                }
                SaveFileCategories(project.Categories);
                SaveTimeSpends(project.TimeEntries);
                Attachments.AddRange(project.Attachments);

            }
        }

        private void SaveMilestones(IMilestone[] milestones)
        {
            var projectEngine = _engineFactory.GetProjectEngine();
            var mileStoneEngine = _engineFactory.GetMilestoneEngine();
            foreach (var milestone in milestones.Where(x => _withClosed ? true : !x.Completed))
            {
                try
                {
                    Milestone newMilestone = new Milestone()
                    {
                        Title = ReplaceLineSeparator(milestone.Title),
                        DeadLine = milestone.Deadline,
                        Status = milestone.Completed ? MilestoneStatus.Closed : MilestoneStatus.Open,
                        Project = projectEngine.GetByID(FindProject(milestone.ProjectID)),
                        IsKey = false,
                        IsNotify = false
                    };
                    newMilestone = mileStoneEngine.SaveOrUpdate(newMilestone, false, true);
                    NewMilestonesID.Add(new MilestoneIDWrapper() { inBasecamp = milestone.ID, inProjects = newMilestone.ID });
                }
                catch (Exception e)
                {
                    Status.LogError(string.Format(SettingsResource.FailedToSaveMilestone, milestone.Title), e);
                    LogError(string.Format("milestone '{0}' failed", milestone.Title), e);
                    NewMilestonesID.RemoveAll(x => x.inBasecamp == milestone.ID);
                }
            }
        }

        private void SaveMessages(IPost message)
        {
            var projectEngine = _engineFactory.GetProjectEngine();
            var messageEngine = _engineFactory.GetMessageEngine();
            try
            {
                Message newMessage = new Message()
                {
                    Title = ReplaceLineSeparator(message.Title),
                    Content = message.Body,
                    Project = projectEngine.GetByID(FindProject(message.ProjectID)),
                    CreateOn = message.PostedOn.ToUniversalTime(),
                    CreateBy = FindUser(message.AuthorID)
                };

                newMessage = messageEngine.SaveOrUpdate(newMessage, true, new[] { newMessage.CreateBy }, null, true);
                NewMessagesID.Add(new MessageIDWrapper() { inBasecamp = message.ID, inProjects = newMessage.ID });
                SaveMessageComments(message.RecentComments, message.ID);
            }
            catch (Exception e)
            {
                Status.LogError(string.Format(SettingsResource.FailedToSaveMessage, message.Title), e);
                LogError(string.Format("message '{0}' failed", message.Title), e);
                NewMessagesID.RemoveAll(x => x.inBasecamp == message.ID);
            }
        }

        private void SaveTasks(IToDoList todoList)
        {
            var projectEngine = _engineFactory.GetProjectEngine();
            var taskEngine = _engineFactory.GetTaskEngine();
            foreach (var task in todoList.Items.Where(x => _withClosed ? true : !x.Completed))
            {
                try
                {

                    var newTask = new Task()
                    {
                        Title = ReplaceLineSeparator(task.Content),
                        Status = task.Completed ? TaskStatus.Closed : TaskStatus.Open,
                        Project = projectEngine.GetByID(FindProject(todoList.ProjectID)),
                        CreateOn = task.CreatedOn.ToUniversalTime(),
                        CreateBy = FindUser(task.CreatorID),
                        Responsibles = new HashSet<Guid>(),
                        Description = string.Empty
                    };
                    if ("Person".Equals(task.ResponsiblePartyType,StringComparison.OrdinalIgnoreCase) )
                    {
                        var user = FindUser(task.ResponsiblePartyID);
                        newTask.Responsible = user;
                        newTask.Responsibles.Add(user);
                    }

                    if (todoList.MilestoneID != -1)
                    {
                        var foundMilestone = FindMilestone(todoList.MilestoneID);
                        if (foundMilestone != -1)
                        {
                            newTask.Milestone = foundMilestone;
                        }
                    }

                    //if (newTask.Responsible.Equals(Guid.Empty)) newTask.Status = TaskStatus.Unclassified;
                    //if (newTask.Responsibles.Count == 0) newTask.Status = TaskStatus.Unclassified;

                    newTask = taskEngine.SaveOrUpdate(newTask, null, true, true);
                    NewTasksID.Add(new TaskIDWrapper { inBasecamp = task.ID, inProjects = newTask.ID });
                    SaveTaskComments(task.RecentComments, task.ID);
                }
                catch (Exception e)
                {
                    Status.LogError(string.Format(SettingsResource.FailedToSaveTask, task.Content), e);
                    LogError(string.Format("task '{0}' failed", task.Content), e);
                    NewTasksID.RemoveAll(x => x.inBasecamp == task.ID);
                }
            }
        }

        private void SaveMessageComments(IComment[] comments, int messageid)
        {
            var commentEngine = _engineFactory.GetCommentEngine();
            foreach (var comment in comments)
            {
                try
                {

                    Comment newComment = new Comment()
                    {
                        CreateBy = FindUser(comment.AuthorID),
                        Content = comment.Body,
                        CreateOn = comment.CreatedAt.ToUniversalTime(),
                        TargetUniqID = ProjectEntity.BuildUniqId<Message>(FindMessage(messageid))
                    };
                    commentEngine.SaveOrUpdate(newComment);
                }
                catch (Exception e)
                {
                    Status.LogError(string.Format(SettingsResource.FailedToSaveComment, comment.ID), e);
                    LogError(string.Format("comment '{0}' failed", comment.ID), e);
                }
            }
        }

        private void SaveTaskComments(IComment[] comments, int taskid)
        {
            var commentEngine = _engineFactory.GetCommentEngine();
            foreach (var comment in comments)
            {
                try
                {
                    Comment newComment = new Comment()
                    {
                        CreateBy = FindUser(comment.AuthorID),
                        Content = comment.Body,
                        CreateOn = comment.CreatedAt.ToUniversalTime(),
                        TargetUniqID = ProjectEntity.BuildUniqId<Task>(FindTask(taskid))
                    };
                    commentEngine.SaveOrUpdate(newComment);
                }
                catch (Exception e)
                {
                    Status.LogError(string.Format(SettingsResource.FailedToSaveComment, comment.ID), e);
                    LogError(string.Format("comment '{0}' failed", comment.ID), e);
                }
            }
        }

        private void SaveFileCategories(ICategory[] categories)
        {
            foreach (var category in categories)
            {
                if (category.Type == CategoryType.Attachment)
                {
                    try
                    {
                        var folder = new ASC.Files.Core.Folder
                        {
                            ParentFolderID = FileEngine2.GetRoot(FindProject(category.ProjectID)),
                            Title = category.Name
                        };
                        folder = FileEngine2.SaveFolder(folder);
                        NewFileCategoriesID.Add(new FileCategoryIDWrapper { inBasecamp = category.ID, inProjects = folder.ID });
                    }
                    catch (Exception e)
                    {
                        Status.LogError(string.Format(SettingsResource.FailedToSaveFileCategory, category.Name), e);
                        LogError(string.Format("file category '{0}' failed", category.Name), e);
                        NewFileCategoriesID.RemoveAll(x => x.inBasecamp == category.ID);
                    }
                }
            }
        }

        private void SaveTimeSpends(ITimeEntry[] timeEntries)
        {
            TimeTrackingEngine timeTrackingEngine = _engineFactory.GetTimeTrackingEngine();
            foreach (var timeEntry in timeEntries)
            {
                try
                {
                    TimeSpend newTimeSpend = new TimeSpend()
                                                 {
                                                     Hours = (float)timeEntry.Hours,
                                                     Date = timeEntry.Date,
                                                     Note = timeEntry.Description,
                                                     Task = new Task { ID = timeEntry.ToDoItemID != -1 ? FindTask(timeEntry.ToDoItemID) : 0, Project = new Project { ID = FindProject(timeEntry.ProjectID) } },
                                                     Person = FindUser(timeEntry.PersonID)
                                                 };

                    timeTrackingEngine.SaveOrUpdate(newTimeSpend, true);
                }
                catch (Exception e)
                {
                    Status.LogError(string.Format(SettingsResource.FailedToSaveTimeSpend, timeEntry.ID), e);
                    LogError(string.Format("time spend '{0}' failed", timeEntry.ID), e);
                }
            }
        }

        private void SaveFiles(BaseCamp basecampManeger, IEnumerable<IAttachment> attachments)
        {
            var step = 100.0 / attachments.Count();

            Status.LogInfo(string.Format(SettingsResource.ImportFileStarted, attachments.Count()));

            //select last version
            foreach (var attachment in attachments.GroupBy(a => a.Collection).Select(g => g.OrderByDescending(a => a.Vers).FirstOrDefault()))
            {
                Status.FileProgress += step;
                try
                {
                    HttpWebRequest HttpWReq = basecampManeger.Service.GetRequest(attachment.DownloadUrl);
                    using (HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse())
                    {
                        if (attachment.ByteSize > SetupInfo.MaxUploadSize) break;

                        var file = new ASC.Files.Core.File();
                        file.FolderID = attachment.CategoryID != -1 ? FindFileCategory(attachment.CategoryID) : FileEngine2.GetRoot(FindProject(attachment.ProjectID));
                        file.Title = attachment.Name;
                        file.ContentLength = attachment.ByteSize;
                        file.ContentType = MimeMapping.GetMimeMapping(attachment.Name);
                        file.CreateBy = FindUser(attachment.AuthorID);
                        file.CreateOn = attachment.CreatedOn.ToUniversalTime();
                        if (file.Title.LastIndexOf('\\') != -1) file.Title = file.Title.Substring(file.Title.LastIndexOf('\\') + 1);

                        file = FileEngine2.SaveFile(file, HttpWResp.GetResponseStream());

                        if ("Post".Equals(attachment.OwnerType,StringComparison.OrdinalIgnoreCase) )
                        {
                            try
                            {
                                var messageId = FindMessage(attachment.OwnerID);
                                FileEngine2.AttachFileToMessage(messageId, file.ID);//It's not critical 
                            }
                            catch (Exception e)
                            {
                                LogError(string.Format("not critical. attaching file '{0}' to message  failed", attachment.Name), e);
                            }
                        }

                        NewFilesID.Add(new FileIDWrapper
                        {
                            inBasecamp = attachment.ID,
                            inProjects = file.ID,
                            version = attachment.Vers,
                            collection = attachment.Collection
                        });
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        Status.LogError(string.Format(SettingsResource.FailedToSaveFile, attachment.Name), e);
                        LogError(string.Format("file '{0}' failed", attachment.Name), e);
                        NewFilesID.RemoveAll(x => x.inBasecamp == attachment.ID && x.version == attachment.Vers);
                    }
                    catch (Exception ex)
                    {
                        LogError(string.Format("file remove after error failed"), ex);
                    }
                }
            }
        }

        private byte[] StreamFile(string filepath)
        {
            WebClient urlGrabber = new WebClient();
            byte[] data = urlGrabber.DownloadData(filepath);
            return data;
        }

        private static string ReplaceLineSeparator(string value)
        {
            var listChar = value.ToCharArray().ToList();
            listChar.RemoveAll(r => char.GetUnicodeCategory(r) == UnicodeCategory.LineSeparator);
            return new string(listChar.ToArray());
        }

        #endregion

        #region Search matches

        private Guid FindUserByEmail(string email)
        {
            foreach (var user in CoreContext.UserManager.GetUsers(EmployeeStatus.All))
            {
                if (String.Equals(user.Email, email, StringComparison.InvariantCultureIgnoreCase)
                  && !String.IsNullOrEmpty(email))
                    return user.ID;
            }
            return Guid.Empty;
        }

        private Guid FindUser(int userID)
        {
            foreach (UserIDWrapper record in NewUsersID)
            {
                if (record.inBasecamp == userID)
                    return record.inProjects;
            }
            return Guid.Empty;
        }

        private int FindProject(int projectID)
        {
            foreach (ProjectIDWrapper record in NewProjectsID)
            {
                if (record.inBasecamp == projectID)
                    return record.inProjects;
            }
            throw new ArgumentException(string.Format("basecamp project not found {0}", projectID), "projectID");
        }

        private int FindMilestone(int milestoneID)
        {
            foreach (MilestoneIDWrapper record in NewMilestonesID)
            {
                if (record.inBasecamp == milestoneID)
                    return record.inProjects;
            }
            return -1;
        }

        private int FindMessage(int messageID)
        {
            foreach (MessageIDWrapper record in NewMessagesID)
            {
                if (record.inBasecamp == messageID)
                    return record.inProjects;
            }
            throw new ArgumentException(string.Format("basecamp message not found {0}", messageID), "messageID");
        }

        private int FindTask(int taskID)
        {
            foreach (TaskIDWrapper record in NewTasksID)
            {
                if (record.inBasecamp == taskID)
                    return record.inProjects;
            }
            throw new ArgumentException(string.Format("basecamp task not found {0}", taskID), "taskID");
        }

        private object FindFileCategory(int fileCategoryID)
        {
            foreach (FileCategoryIDWrapper record in NewFileCategoriesID)
            {
                if (record.inBasecamp == fileCategoryID)
                    return record.inProjects;
            }
            return 0;
        }

        private object FindFile(int collection)
        {
            foreach (FileIDWrapper record in NewFilesID)
            {
                if (record.collection == collection && record.version == 1)
                    return record.inProjects;
            }
            return -1;
        }

        #endregion

        #region Wrappers

        private class UserIDWrapper
        {
            public int inBasecamp;
            public Guid inProjects;
        }

        private class ProjectIDWrapper
        {
            public int inBasecamp;
            public int inProjects;
        }

        private class MilestoneIDWrapper
        {
            public int inBasecamp;
            public int inProjects;
        }

        private class MessageIDWrapper
        {
            public int inBasecamp;
            public int inProjects;
        }

        private class TaskIDWrapper
        {
            public int inBasecamp;
            public int inProjects;
        }

        private class FileCategoryIDWrapper
        {
            public int inBasecamp;
            public object inProjects;
        }

        private class FileIDWrapper
        {
            public int inBasecamp;
            public int collection;
            public int version;
            public object inProjects;
        }

        #endregion

        public bool Equals(ImportFromBasecamp other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }



}
