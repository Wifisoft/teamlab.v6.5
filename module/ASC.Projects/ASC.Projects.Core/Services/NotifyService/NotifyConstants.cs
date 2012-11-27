using ASC.Notify.Model;
using ASC.Notify.Patterns;

namespace ASC.Projects.Core.Services.NotifyService
{
    public static class NotifyConstants
    {
        public static INotifyAction Event_NewCommentForMessage = new NotifyAction("NewCommentForMessage", "new comment for message");
        public static INotifyAction Event_NewCommentForTask = new NotifyAction("NewCommentForTask", "new comment for task");
        public static INotifyAction Event_NewCommentForMilestone = new NotifyAction("NewCommentForMilestone", "new comment for milestone");
        public static INotifyAction Event_NewCommentForIssue = new NotifyAction("NewCommentForIssue", "new comment for issue");

        public static INotifyAction Event_TaskEdited = new NotifyAction("TaskEdited", "task edited");
        public static INotifyAction Event_TaskClosed = new NotifyAction("TaskClosed", "task closed");
        public static INotifyAction Event_TaskCreated = new NotifyAction("TaskCreated", "task created");
        public static INotifyAction Event_TaskDeleted = new NotifyAction("TaskDeleted", "task deleted");
        public static INotifyAction Event_TaskResumed = new NotifyAction("TaskResumed", "task resumed");
        public static INotifyAction Event_MilestoneDeadline = new NotifyAction("MilestoneDeadline", "milestone deadline");
        public static INotifyAction Event_ResponsibleForProject = new NotifyAction("ResponsibleForProject", "responsible for project");
        public static INotifyAction Event_ResponsibleForTask = new NotifyAction("ResponsibleForTask", "responsible for task");
        public static INotifyAction Event_ReminderAboutTask = new NotifyAction("ReminderAboutTask", "reminder about task");
        public static INotifyAction Event_ReminderAboutTaskDeadline = new NotifyAction("ReminderAboutTaskDeadline", "reminder about task deadline");
        public static INotifyAction Event_InviteToProject = new NotifyAction("InviteToProject", "invite to project");
        public static INotifyAction Event_RemoveFromProject = new NotifyAction("RemoveFromProject", "remove from project");

        public static INotifyAction Event_MilestoneCreated = new NotifyAction("MilestoneCreated", "milestone created");
        public static INotifyAction Event_MilestoneEdited = new NotifyAction("MilestoneEdited", "milestone edited");
        public static INotifyAction Event_MilestoneClosed = new NotifyAction("MilestoneClosed", "milestone closed");
        public static INotifyAction Event_ResponsibleForMilestone = new NotifyAction("ResponsibleForMilestone", "responsible for milestone");

        public static INotifyAction Event_MessageCreated = new NotifyAction("NewMessage", "message created");
        public static INotifyAction Event_MessageEdited = new NotifyAction("EditMessage", "message edited");

        public static INotifyAction Event_ImportData = new NotifyAction("ImportData", "import data");

        public static INotifyAction Event_ResponsibleForSubTask = new NotifyAction("ResponsibleForSubTask", "responsible for subtask");
        public static INotifyAction Event_SubTaskCreated = new NotifyAction("SubTaskCreated", "subtask created");
        public static INotifyAction Event_SubTaskResumed = new NotifyAction("SubTaskResumed", "subtask resumed");
        public static INotifyAction Event_SubTaskClosed = new NotifyAction("SubTaskClosed", "subtask closed");
        public static INotifyAction Event_SubTaskDeleted = new NotifyAction("SubTaskDeleted", "subtask deleted");

        public static ITag Tag_ProjectTitle = new Tag("ProjectTitle");
        public static ITag Tag_ProjectID = new Tag("ProjectID");
        public static ITag Tag_AdditionalData = new Tag("AdditionalData");
        public static ITag Tag_EntityID = new Tag("EntityID");
        public static ITag Tag_EntityTitle = new Tag("EntityTitle");
        public static ITag Tag_SubEntityTitle = new Tag("SubEntityTitle");
        public static ITag Tag_EventType = new Tag("EventType");
        public static ITag Tag_Responsible = new Tag("Responsible");
    }
}