using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Api.Projects.Wrappers;
using ASC.Feed.Activity;
using ASC.Feed.ActivityProvider;
using ASC.Feed.Utils;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

namespace ASC.Api.Projects.ActivityProviders
{
    public class ProjectsActivityProvider : ProjectApiBase, IActivityProvider
    {
        private const string Projects = "projects";

        public string SourceName
        {
            get { return Projects; }
        }

        public IEnumerable<Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions)
        {
            var milestones = EngineFactory.GetMilestoneEngine().GetUpdates(range.From, range.To);
            var projects = EngineFactory.GetProjectEngine().GetUpdates(range.From, range.To);
            var tasks = EngineFactory.GetTaskEngine().GetUpdates(range.From, range.To);
            var comments = EngineFactory.GetCommentEngine().GetUpdates(range.From, range.To);
            var messages = EngineFactory.GetMessageEngine().GetUpdates(range.From, range.To);
            var participants = EngineFactory.GetProjectEngine().GetTeamUpdates(range.From, range.To);
            var timeTracking = EngineFactory.GetTimeTrackingEngine().GetUpdates(range.From, range.To);
            var subtasks = EngineFactory.GetTaskEngine().GetSubtaskUpdates(range.From, range.To);

            var activities = new List<Activity>(milestones
                .Select(x => new Activity(Projects, new MilestoneWrapper(x), (range.In(x.CreateOn) ? x.CreateBy : x.LastModifiedBy), (range.In(x.CreateOn) ? x.CreateOn : x.LastModifiedOn),
                    (range.In(x.CreateOn) ? ActivityAction.Created : ActivityAction.Undefined) | (range.In(x.LastModifiedOn) ? ActivityAction.Updated : ActivityAction.Undefined) | (range.In(x.StatusChangedOn) && x.Status == MilestoneStatus.Closed ? ActivityAction.Closed : ActivityAction.Opened))
                    { ItemType = "milestone"}));
            activities.AddRange(projects
                .Select(x => new Activity(Projects, new ProjectWrapper(x), (range.In(x.CreateOn) ? x.CreateBy : x.LastModifiedBy), (range.In(x.CreateOn) ? x.CreateOn : x.LastModifiedOn),
                    (range.In(x.CreateOn) ? ActivityAction.Created : ActivityAction.Undefined) | (range.In(x.LastModifiedOn) ? ActivityAction.Updated : ActivityAction.Undefined) | (range.In(x.StatusChangedOn) && x.Status == ProjectStatus.Closed ? ActivityAction.Closed : ActivityAction.Opened)) 
                    { ItemType = "project" }));
            activities.AddRange(tasks
                .Select(x => new Activity(Projects, new TaskWrapper(x), (range.In(x.CreateOn) ? x.CreateBy : x.LastModifiedBy), (range.In(x.CreateOn) ? x.CreateOn : x.LastModifiedOn),
                    (range.In(x.CreateOn) ? ActivityAction.Created : ActivityAction.Undefined) | (range.In(x.LastModifiedOn) ? ActivityAction.Updated : ActivityAction.Undefined) | (range.In(x.StatusChangedOn) && x.Status == TaskStatus.Closed ? ActivityAction.Closed : ActivityAction.Opened))
                    { ItemType = "task"}));
            activities.AddRange(messages
                .Select(x=>new Activity(Projects, new MessageWrapper(x), range.In(x.CreateOn) ? x.CreateBy : x.LastModifiedBy, range.In(x.CreateOn) ? x.CreateOn : x.LastModifiedOn,
                    (range.In(x.CreateOn) ? ActivityAction.Created : ActivityAction.Undefined) | (range.In(x.LastModifiedOn) ? ActivityAction.Updated : ActivityAction.Undefined))
                    { ItemType = "message"}));
            activities.AddRange(comments
                .Select(x=>new Activity(Projects,new { Comment = new CommentWrapper(x.Comment), CommentedType = x.CommentedType, CommentedTitle = x.CommentedTitle, CommentedId = x.CommentedId }, x.Comment.CreateBy, x.Comment.CreateOn,
                    (range.In(x.Comment.CreateOn) ? ActivityAction.Created : ActivityAction.Undefined) | (x.Comment.Inactive ? ActivityAction.Deleted : ActivityAction.Undefined) | (x.Comment.Parent == Guid.Empty ? ActivityAction.Commented : ActivityAction.Reply))
                    { ItemType = "comment"}));
            activities.AddRange(participants
                .Select(x => new Activity(Projects, new ParticipantFullWrapper(x), x.ID, range.In(x.Created) ? x.Created : x.Updated,
                    (range.In(x.Created) ? ActivityAction.Created : ActivityAction.Undefined) | (range.In(x.Updated) ? ActivityAction.Updated : ActivityAction.Undefined) | (x.Removed ? ActivityAction.Deleted : ActivityAction.Undefined)) 
                    { ItemType = "participant" }));
            activities.AddRange(timeTracking
                .Select(x => new Activity(Projects, new TimeWrapper(x), x.Person, x.Date, ActivityAction.Updated)
                    {ItemType = "time"}));
            foreach (var task in subtasks)
            {
                var group = task.SubTasks;
                task.SubTasks = null;
                activities.AddRange(group
                    .Select(x => new Activity(Projects, new {Task = new TaskWrapper(task), Subtask = new SubtaskWrapper(x, task)}, range.In(x.CreateOn) ? x.CreateBy : x.LastModifiedBy, range.In(x.CreateOn) ? x.CreateOn : x.LastModifiedOn,
                        (range.In(x.CreateOn) ? ActivityAction.Created : ActivityAction.Undefined) | (range.In(x.LastModifiedOn) ? ActivityAction.Updated : ActivityAction.Undefined) | (range.In(x.StatusChangedOn) && x.Status == TaskStatus.Open ? ActivityAction.Opened : ActivityAction.Closed))
                        {ItemType = "subtask"}));
            }

            return activities;
        }
    }
}
