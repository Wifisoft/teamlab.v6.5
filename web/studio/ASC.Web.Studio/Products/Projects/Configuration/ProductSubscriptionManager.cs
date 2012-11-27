using System;
using System.Collections.Generic;
using ASC.Core;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Web.Core.Subscriptions;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;

namespace ASC.Web.Projects.Configuration
{
    public class ProductSubscriptionManager : IProductSubscriptionManager
    {
        private readonly Guid _newCommentForMessage = new Guid("{04EAEAB5-948D-4491-8CB9-493CDD37B52A}");
        private readonly Guid _newCommentForTask = new Guid("{3E732AFC-B786-48a0-A88D-F7C43A45942E}");
        private readonly Guid _newCommentForMilestone = new Guid("{4A3A6D35-575C-40da-B1F8-7362D20EA858}");

        private readonly Guid _taskClosed = new Guid("{6C0ED420-DDF1-41ff-8D40-93D3BF213FA3}");
        private readonly Guid _responsibleForTask = new Guid("{85502DA1-E681-4fc0-98A0-08BA4E58D435}");
        private readonly Guid _responsibleForMilestone = new Guid("{BEC3DFF6-FA2B-447a-A3D3-1A35477C9288}");
        private readonly Guid _responsibleForProject = new Guid("{E838ECA9-DC2E-4a06-A786-28958C9EEE2C}");
        private readonly Guid _inviteToProject = new Guid("{6C950DA7-C69C-4ed2-935F-E2E55B0318DF}");
        private readonly Guid _removeFromProject = new Guid("{F25BA11D-68EF-44cc-803B-EE96080FA87B}");
        private readonly Guid _projectEditRequest = new Guid("{E8B78A07-574A-4b79-AD92-89A6807C0FE1}");
        private readonly Guid _projectCreateRequest = new Guid("{C8935BFE-BB8F-4bd5-9493-0E02AC83A8F6}");

        private static readonly Dictionary<Int32, Guid> BindingProjectID = new Dictionary<Int32, Guid>();
        private static readonly Dictionary<Int32, SubscriptionGroup> BindingProjectToGroup = new Dictionary<Int32, SubscriptionGroup>();


        private int GetProjectIDByGroupID(Guid moduleOrGroupID)
        {
            var result = 0;
            if (moduleOrGroupID != Guid.Empty)
            {
                foreach (var item in BindingProjectID)
                {
                    if (item.Value == moduleOrGroupID) result = item.Key;
                }
            }
            return result;
        }

        private List<SubscriptionObject> GetNewCommentForMessageObjects(Guid moduleOrGroupID)
        {
            var result = new List<SubscriptionObject>();
            var objects = GetSubscriptions(NotifyConstants.Event_NewCommentForMessage);
            var subscriptionType = GetSubscriptionTypes().Find(item => item.ID == _newCommentForMessage);
            var filterProjectID = GetProjectIDByGroupID(moduleOrGroupID);

            foreach (var item in objects)
            {
                if (item == null) continue;

                var messageID = int.Parse(item.Split(new[] {'_'})[1]);
                var projectID = int.Parse(item.Split(new[] {'_'})[2]);
                if (filterProjectID > 0 && projectID != filterProjectID) continue;

                var message = Global.EngineFactory.GetMessageEngine().GetByID(messageID);
                if (message != null && message.Project != null)
                {
                    result.Add(new SubscriptionObject
                                   {
                                       ID = item,
                                       Name = message.Title,
                                       URL = String.Concat(PathProvider.BaseAbsolutePath, String.Format("messages.aspx?prjID={0}&id={1}", message.Project.ID, message.ID)),
                                       SubscriptionGroup = BindingProjectToGroup[projectID],
                                       SubscriptionType = subscriptionType
                                   });
                }
            }

            return result;
        }

        private List<SubscriptionObject> GetNewCommentForTaskObjects(Guid moduleOrGroupID)
        {
            var result = new List<SubscriptionObject>();
            var objects = GetSubscriptions(NotifyConstants.Event_NewCommentForTask);
            var subscriptionType = GetSubscriptionTypes().Find(item => item.ID == _newCommentForTask);
            var filterProjectID = GetProjectIDByGroupID(moduleOrGroupID);

            foreach (var item in objects)
            {
                if (item == null) continue;

                var taskID = int.Parse(item.Split(new[] {'_'})[1]);
                var projectID = int.Parse(item.Split(new[] {'_'})[2]);
                if (filterProjectID > 0 && projectID != filterProjectID) continue;

                var task = Global.EngineFactory.GetTaskEngine().GetByID(taskID);
                if (task != null && task.Project != null)
                {
                    result.Add(new SubscriptionObject
                                   {
                                       ID = item,
                                       Name = task.Title,
                                       URL = String.Concat(PathProvider.BaseAbsolutePath, String.Format("tasks.aspx?prjID={0}&id={1}", task.Project.ID, task.ID)),
                                       SubscriptionGroup = BindingProjectToGroup[projectID],
                                       SubscriptionType = subscriptionType
                                   });
                }
            }
            return result;
        }

        private List<SubscriptionObject> GetNewCommentForMilestoneObjects(Guid moduleOrGroupID)
        {
            var result = new List<SubscriptionObject>();
            var objects = GetSubscriptions(NotifyConstants.Event_NewCommentForMilestone);
            var subscriptionType = GetSubscriptionTypes().Find(item => item.ID == _newCommentForMilestone);
            var filterProjectID = GetProjectIDByGroupID(moduleOrGroupID);

            foreach (var item in objects)
            {
                if (item == null) continue;

                var milestoneID = int.Parse(item.Split(new[] {'_'})[1]);
                var projectID = int.Parse(item.Split(new[] {'_'})[2]);
                if (filterProjectID > 0 && projectID != filterProjectID) continue;

                var milestone = Global.EngineFactory.GetMilestoneEngine().GetByID(milestoneID);
                if (milestone != null && milestone.Project != null)
                {
                    result.Add(new SubscriptionObject
                                   {
                                       ID = item,
                                       Name = milestone.Title,
                                       URL = String.Concat(PathProvider.BaseAbsolutePath, String.Format("milestones.aspx?prjID={0}&id={1}", milestone.Project.ID, milestone.ID)),
                                       SubscriptionGroup = BindingProjectToGroup[projectID],
                                       SubscriptionType = subscriptionType
                                   });
                }
            }
            return result;
        }

        public List<SubscriptionObject> GetSubscriptionObjects()
        {
            var objects = new List<SubscriptionObject>();
            objects.AddRange(GetNewCommentForMessageObjects(Guid.Empty));
            objects.AddRange(GetNewCommentForTaskObjects(Guid.Empty));
            objects.AddRange(GetNewCommentForMilestoneObjects(Guid.Empty));
            return objects;
        }

        private List<SubscriptionObject> GetNewCommentForMilestoneObjects(Guid productID, Guid moduleOrGroupID, Guid typeID)
        {
            return GetNewCommentForMilestoneObjects(moduleOrGroupID);
        }

        private bool IsEmptyNewCommentForMilestoneSubscriptionType(Guid productID, Guid moduleOrGroupID, Guid typeID)
        {
            var objects = NotifySource.Instance.GetSubscriptionProvider().GetSubscriptions(
                NotifyConstants.Event_NewCommentForMilestone,
                new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), SecurityContext.CurrentAccount.Name));

            foreach (var item in objects)
            {
                if (item == null) continue;

                var milestoneID = int.Parse(item.Split(new[] {'_'})[1]);
                var projectID = int.Parse(item.Split(new[] {'_'})[2]);

                if (!Global.EngineFactory.GetMilestoneEngine().IsExists(milestoneID)) continue;
                if (!Global.EngineFactory.GetProjectEngine().IsExists(projectID)) continue;
                if (!BindingProjectID.ContainsKey(projectID)) continue;

                var localGroupID = BindingProjectID[projectID];
                if (localGroupID != moduleOrGroupID) continue;

                return false;
            }
            return true;
        }

        private List<SubscriptionObject> GetNewCommentForMessageObjects(Guid productID, Guid moduleOrGroupID, Guid typeID)
        {
            return GetNewCommentForMessageObjects(moduleOrGroupID);
        }

        private bool IsEmptyNewCommentForMessageSubscriptionType(Guid productID, Guid moduleOrGroupID, Guid typeID)
        {
            var objects = NotifySource.Instance.GetSubscriptionProvider().GetSubscriptions(
                NotifyConstants.Event_NewCommentForMessage,
                new DirectRecipient(SecurityContext.CurrentAccount.ID.ToString(), SecurityContext.CurrentAccount.Name));

            foreach (var item in objects)
            {
                if (item == null) continue;

                var messageID = int.Parse(item.Split(new[] {'_'})[1]);
                var projectID = int.Parse(item.Split(new[] {'_'})[2]);

                if (!Global.EngineFactory.GetMessageEngine().IsExists(messageID)) continue;
                if (!Global.EngineFactory.GetProjectEngine().IsExists(projectID)) continue;
                if (!BindingProjectID.ContainsKey(projectID)) continue;

                var localGroupID = BindingProjectID[projectID];
                if (localGroupID != moduleOrGroupID) continue;

                return false;
            }
            return true;
        }

        private List<SubscriptionObject> GetNewCommentForTaskObjects(Guid productID, Guid moduleOrGroupID, Guid typeID)
        {
            return GetNewCommentForTaskObjects(moduleOrGroupID);
        }

        private bool IsEmptyNewCommentForTaskSubscriptionType(Guid productID, Guid moduleOrGroupID, Guid typeID)
        {
            var objects = NotifySource.Instance.GetSubscriptionProvider().GetSubscriptions(
                NotifyConstants.Event_NewCommentForTask,
                NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()));

            foreach (var item in objects)
            {
                if (item == null) continue;

                var taskID = int.Parse(item.Split(new[] {'_'})[1]);
                var projectID = int.Parse(item.Split(new[] {'_'})[2]);

                if (!Global.EngineFactory.GetProjectEngine().IsExists(projectID)) continue;
                if (!BindingProjectID.ContainsKey(projectID)) continue;
                if (!Global.EngineFactory.GetTaskEngine().IsExists(taskID)) continue;

                var localGroupID = BindingProjectID[projectID];
                if (localGroupID != moduleOrGroupID) continue;

                return false;
            }

            return true;
        }


        public List<SubscriptionType> GetSubscriptionTypes()
        {
            return new List<SubscriptionType>
                       {
                           new SubscriptionType
                               {
                                   ID = _newCommentForMilestone,
                                   Name = ProjectsCommonResource.NewCommentForMilestone,
                                   NotifyAction = NotifyConstants.Event_NewCommentForMilestone,
                                   Single = false,
                                   GetSubscriptionObjects = GetNewCommentForMilestoneObjects,
                                   IsEmptySubscriptionType = IsEmptyNewCommentForMilestoneSubscriptionType
                               },
                           new SubscriptionType
                               {
                                   ID = _newCommentForMessage,
                                   Name = ProjectsCommonResource.NewCommentForMessage,
                                   NotifyAction = NotifyConstants.Event_NewCommentForMessage,
                                   Single = false,
                                   GetSubscriptionObjects = GetNewCommentForMessageObjects,
                                   IsEmptySubscriptionType = IsEmptyNewCommentForMessageSubscriptionType
                               },
                           new SubscriptionType
                               {
                                   ID = _newCommentForTask,
                                   Name = ProjectsCommonResource.NewCommentForTask,
                                   NotifyAction = NotifyConstants.Event_NewCommentForTask,
                                   Single = false,
                                   GetSubscriptionObjects = GetNewCommentForTaskObjects,
                                   IsEmptySubscriptionType = IsEmptyNewCommentForTaskSubscriptionType
                               },
                           new SubscriptionType
                               {
                                   ID = _taskClosed,
                                   Name = "Close task",
                                   NotifyAction = NotifyConstants.Event_TaskClosed,
                                   Single = false
                               },
                           new SubscriptionType
                               {
                                   ID = _responsibleForTask,
                                   Name = "Responsible for task",
                                   NotifyAction = NotifyConstants.Event_ResponsibleForTask,
                                   Single = false
                               },
                           new SubscriptionType
                               {
                                   ID = _responsibleForProject,
                                   Name = "Responsible for project",
                                   NotifyAction = NotifyConstants.Event_ResponsibleForProject,
                                   Single = false
                               },
                           new SubscriptionType
                               {
                                   ID = _responsibleForMilestone,
                                   Name = "Deadline milestone",
                                   NotifyAction = NotifyConstants.Event_MilestoneDeadline,
                                   Single = false
                               },

                           new SubscriptionType
                               {
                                   ID = _inviteToProject,
                                   Name = "Invite to project",
                                   NotifyAction = NotifyConstants.Event_InviteToProject,
                                   Single = false
                               },
                           new SubscriptionType
                               {
                                   ID = _removeFromProject,
                                   Name = "Remove from project",
                                   NotifyAction = NotifyConstants.Event_RemoveFromProject,
                                   Single = false
                               }
                       };
        }

        public ISubscriptionProvider SubscriptionProvider
        {
            get { return NotifySource.Instance.GetSubscriptionProvider(); }
        }

        public GroupByType GroupByType
        {
            get { return GroupByType.Groups; }
        }

        public List<SubscriptionGroup> GetSubscriptionGroups()
        {
            var preparateData = new List<string>();

            preparateData.AddRange(GetSubscriptions(NotifyConstants.Event_NewCommentForMessage));
            preparateData.AddRange(GetSubscriptions(NotifyConstants.Event_NewCommentForTask));
            preparateData.AddRange(GetSubscriptions(NotifyConstants.Event_NewCommentForMilestone));

            var projects = Global.EngineFactory.GetProjectEngine().GetAll();

            foreach (var item in preparateData)
            {
                if (item == null) continue;

                var projectID = int.Parse(item.Split(new[] {'_'})[2]);
                var project = projects.Find(p => p.ID == projectID);
                if (project == null) continue;

                if (!BindingProjectToGroup.ContainsKey(projectID))
                {
                    var generatedProjectID = Guid.NewGuid();
                    BindingProjectID.Add(projectID, generatedProjectID);
                    BindingProjectToGroup.Add(projectID, new SubscriptionGroup {ID = generatedProjectID, Name = project.HtmlTitle});
                }
            }
            return new List<SubscriptionGroup>(BindingProjectToGroup.Values);
        }

        private string[] GetSubscriptions(INotifyAction action)
        {
            return NotifySource.Instance.GetSubscriptionProvider()
                .GetSubscriptions(action, NotifySource.Instance.GetRecipientsProvider().GetRecipient(SecurityContext.CurrentAccount.ID.ToString()));
        }
    }
}