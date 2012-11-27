﻿#region Import

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Collections;
using ASC.Api.Employee;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Web.CRM.Services.NotifyService;
using EnumExtension = ASC.Web.CRM.Classes.EnumExtension;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {

        /// <summary>
        ///  Returns the detailed information about the task with the ID specified in the request
        /// </summary>
        /// <param name="taskid">Task ID</param>
        /// <returns>Task</returns>
        /// <short>Get task by ID</short> 
        /// <category>Tasks</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"task/{taskid:[0-9]+}")]
        public TaskWrapper GetTaskByID(int taskid)
        {
            if (taskid <= 0)
                throw new ArgumentException();

            var task = DaoFactory.GetTaskDao().GetByID(taskid);

            if (task == null)
                throw new ItemNotFoundException();


            return ToTaskWrapper(task);
        }

        /// <summary>
        ///   Returns the list of tasks matching the creteria specified in the request
        /// </summary>
        /// <param optional="true" name="responsibleid">Task responsible</param>
        /// <param optional="true" name="categoryid">Task category ID</param>
        /// <param optional="true" name="isClosed">Show open or closed tasks only</param>
        /// <param optional="true" name="fromDate">Earliest task due date</param>
        /// <param optional="true" name="toDate">Latest task due date</param>
        /// <param name="entityType" remark="Allowed values: opportunity, contact or case">Related entity type</param>
        /// <param name="entityid">Related entity ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <short>Get task list</short> 
        /// <category>Tasks</category>
        /// <returns>
        ///   Task list
        /// </returns>
        [Read(@"task/filter")]
        public IEnumerable<TaskWrapper> GetAllTasks(Guid responsibleid,
                                                   int categoryid,
                                                   bool? isClosed,
                                                   ApiDateTime fromDate,
                                                   ApiDateTime toDate,
                                                   String entityType,
                                                   int entityid)
        {

            TaskSortedByType taskSortedByType;

            if (!String.IsNullOrEmpty(entityType) && !(
                String.Compare(entityType, "contact", true) == 0 ||
                String.Compare(entityType, "opportunity", true) == 0 ||
                String.Compare(entityType, "case", true) == 0))
                throw new ArgumentException();


            IEnumerable<TaskWrapper> result;

            OrderBy taskOrderBy;

            if (ASC.Web.CRM.Classes.EnumExtension.TryParse(_context.SortBy, true, out taskSortedByType))
                taskOrderBy = new OrderBy(taskSortedByType, !_context.SortDescending);
            else if (String.IsNullOrEmpty(_context.SortBy))
                taskOrderBy = new OrderBy(TaskSortedByType.DeadLine, true);
            else
                taskOrderBy = null;

            if (taskOrderBy != null)
            {
                result = ToTaskListWrapper(DaoFactory.GetTaskDao().GetTasks(_context.FilterValue,
                                                          responsibleid,
                                                          categoryid,
                                                          isClosed,
                                                          fromDate,
                                                          toDate,
                                                          ToEntityType(entityType),
                                                          entityid,
                                                          (int)_context.StartIndex,
                                                          (int)_context.Count,
                                                          taskOrderBy));
                _context.SetDataPaginated();
                _context.SetDataFiltered();
                _context.SetDataSorted();
            }
            else
                result = ToTaskListWrapper(DaoFactory.GetTaskDao().GetTasks(_context.FilterValue,
                                                          responsibleid,
                                                          categoryid,
                                                          isClosed,
                                                          fromDate,
                                                          toDate,
                                                          ToEntityType(entityType),
                                                          entityid,
                                                          0,
                                                          0, null));


            return result.ToSmartList();
        }

        /// <summary>
        ///   Open anew the task with the ID specified in the request
        /// </summary>
        /// <short>Resume task</short> 
        /// <category>Tasks</category>
        /// <param name="taskid">Task ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Task
        /// </returns>
        [Update("task/{taskid:[0-9]+}/reopen")]
        public TaskWrapper ReOpenTask(int taskid)
        {
            if (taskid <= 0)
                throw new ArgumentException();

            DaoFactory.GetTaskDao().OpenTask(taskid);

            return ToTaskWrapper(DaoFactory.GetTaskDao().GetByID(taskid));

        }

        /// <summary>
        ///   Close the task with the ID specified in the request
        /// </summary>
        /// <short>Close task</short> 
        /// <category>Tasks</category>
        /// <param name="taskid">Task ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Task
        /// </returns>
        [Update("task/{taskid:[0-9]+}/close")]
        public TaskWrapper CloseTask(int taskid)
        {
            if (taskid <= 0)
                throw new ArgumentException();

            DaoFactory.GetTaskDao().CloseTask(taskid);

            return ToTaskWrapper(DaoFactory.GetTaskDao().GetByID(taskid));

        }

        /// <summary>
        ///   Delete the task with the ID specified in the request
        /// </summary>
        /// <short>Delete task</short> 
        /// <category>Tasks</category>
        /// <param name="taskid">Task ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///  Deleted task
        /// </returns>
        [Delete("task/{taskid:[0-9]+}")]
        public TaskWrapper DeleteTask(int taskid)
        {
            if (taskid <= 0)
                throw new ArgumentException();

            var taskObj = DaoFactory.GetTaskDao().GetByID(taskid);

            if (taskObj == null)
                throw new ItemNotFoundException();
          
            var taskWrapper = ToTaskWrapper(taskObj);

            DaoFactory.GetTaskDao().DeleteTask(taskid);

            return taskWrapper;
        }

        /// <summary>
        ///  Creates the task with the parameters (title, description, due date, etc.) specified in the request
        /// </summary>
        /// <param name="title">Task title</param>
        /// <param optional="true"  name="description">Task description</param>
        /// <param name="deadline">Task due date</param>
        /// <param name="responsibleId">Task responsible ID</param>
        /// <param name="categoryId">Task category ID</param>
        /// <param optional="true"  name="contactId">Contact ID</param>
        /// <param optional="true"  name="entityType" remark="Allowed values: opportunity or case">Related entity type</param>
        /// <param optional="true"  name="entityId">Related entity ID</param>
	    /// <param optional="true"  name="isNotify">Notify the responsible about the task</param>
        /// <exception cref="ArgumentException"></exception>
        /// <short>Create task</short> 
        /// <category>Tasks</category>
        /// <returns>Task</returns>
        [Create("task")]
        public TaskWrapper CreateTask(
            String title,
            String description,
            ApiDateTime deadline,
            Guid responsibleId,
            int categoryId,
            int contactId,
            String entityType,
            int entityId,
            bool isNotify
           )
        {

            if (!String.IsNullOrEmpty(entityType) && !(String.Compare(entityType, "opportunity", true) == 0 ||
                String.Compare(entityType, "case", true) == 0))
                throw new ArgumentException();
           
            var task = new Task
                           {
                               Title = title,
                               Description = description,
                               ResponsibleID = responsibleId,
                               CategoryID = categoryId,
                               DeadLine = deadline,
                               ContactID = contactId,
                               EntityType = ToEntityType(entityType),
                               EntityID = entityId,
                               IsClosed = false
                           };

            task.ID = DaoFactory.GetTaskDao().SaveOrUpdateTask(task);

            task.DeadLine = deadline;

            if (isNotify)
                NotifyClient.Instance.SendAboutResponsibleByTask(task, null);
            
            return ToTaskWrapper(task);
        }


        /// <summary>
		  ///  Returns the list of all tasks with the upcoming due dates for the contacts with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID list</param>
        /// <short>Get contact upcoming tasks</short> 
        /// <category>Tasks</category>
        /// <returns>
        ///   Association of contact ID and task
        /// </returns>
        [Create(@"contact/task/near")]
        public ItemDictionary<int, TaskWrapper> GetNearestTask(IEnumerable<int> contactid)
        {
            var sqlResult = DaoFactory.GetTaskDao().GetNearestTask(contactid.ToArray());
            var result = new ItemDictionary<int, TaskWrapper>();

            foreach (var item in sqlResult)
                result.Add(item.Key, ToTaskWrapper(item.Value));

            return result;
        }

        /// <summary>
        ///   Updates the selected task with the parameters (title, description, due date, etc.) specified in the request
        /// </summary>
        /// <param name="taskid">Task ID</param>
        /// <param name="title">Task title</param>
        /// <param name="description">Task description</param>
        /// <param name="deadline">Task due date</param>
        /// <param name="responsibleid">Task responsible ID</param>
        /// <param name="categoryid">Task category ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <param name="entityType" remark="Allowed values: opportunity or case">Related entity type</param>
        /// <param name="entityid">Related entity ID</param>
        /// <param name="isNotify">Notify or not</param>
        /// <short> Updates the selected task</short> 
        /// <category>Tasks</category>
        /// <returns>Task</returns>
        [Update("task/{taskid:[0-9]+}")]
        public TaskWrapper UpdateTask(
            int taskid,
            String title,
            String description,
            ApiDateTime deadline,
            Guid responsibleid,
            int categoryid,
            int contactid,
            String entityType,
            int entityid,
            bool isNotify)
        {

            if (!String.IsNullOrEmpty(entityType) && !(String.Compare(entityType, "opportunity", true) == 0 ||
                String.Compare(entityType, "case", true) == 0))
                throw new ArgumentException();

            var task = new Task
                           {
                               ID = taskid,
                               Title = title,
                               Description = description,
                               DeadLine = deadline,
                               ResponsibleID = responsibleid,
                               CategoryID = categoryid,
                               ContactID = contactid,
                               EntityID = entityid,
                               EntityType = ToEntityType(entityType)
                           };



            task.ID =  DaoFactory.GetTaskDao().SaveOrUpdateTask(task);
            
            if (isNotify)
                NotifyClient.Instance.SendAboutResponsibleByTask(task, null);

            return ToTaskWrapper(task);
        }

        private Task FromTaskWrapper(TaskWrapper taskWrapper)
        {
            var task = new Task
                           {
                               ContactID = taskWrapper.Contact.ID,
                               Title = taskWrapper.Title,
                               Description = taskWrapper.Description,
                               DeadLine = taskWrapper.DeadLine,
                               ResponsibleID = taskWrapper.Responsible.Id,
                               IsClosed = taskWrapper.IsClosed,
                               CategoryID = taskWrapper.Category.ID,
                               EntityType = ToEntityType(taskWrapper.Entity.EntityType),
                               EntityID = taskWrapper.Entity.EntityId
                           };

            return task;
        }

        private IEnumerable<TaskWrapper> ToTaskListWrapper(IEnumerable<Task> itemList)
        {
            var result = new List<TaskWrapper>();

            var contactIDs = new List<int>();
            var taskIDs = new List<int>();
            var categoryIDs = new List<int>();
            var entityWrappersIDs = new Dictionary<EntityType, List<int>>();

            foreach (var item in itemList)
            {
                taskIDs.Add(item.ID);

                if (!categoryIDs.Contains(item.CategoryID))
                    categoryIDs.Add(item.CategoryID);

                if (item.ContactID > 0 && !contactIDs.Contains(item.ContactID))
                    contactIDs.Add(item.ContactID);

                if (item.EntityID > 0)
                {
                    if (item.EntityType != EntityType.Opportunity && item.EntityType != EntityType.Case) continue;

                    if (!entityWrappersIDs.ContainsKey(item.EntityType))
                        entityWrappersIDs.Add(item.EntityType, new List<int> {
                            item.EntityID
                        });
                    else if (!entityWrappersIDs[item.EntityType].Contains(item.EntityID))
                        entityWrappersIDs[item.EntityType].Add(item.EntityID);
                }
            }

            var entityWrappers = new Dictionary<String, EntityWrapper>();

            foreach (EntityType entityType in entityWrappersIDs.Keys)
            {

                switch (entityType)
                {
                    case EntityType.Opportunity:
                        DaoFactory.GetDealDao().GetByID(entityWrappersIDs[entityType].Distinct().ToArray())
                        .ForEach(item =>
                        {

                            if (item == null) return;

                            entityWrappers.Add(
                                String.Format("{0}_{1}", (int)entityType, item.ID),
                                new EntityWrapper
                                {
                                    EntityId = item.ID,
                                    EntityTitle = item.Title,
                                    EntityType = "opportunity"
                                });
                        });
                        break;
                    case EntityType.Case:
                        DaoFactory.GetCasesDao().GetByID(entityWrappersIDs[entityType].ToArray())
                              .ForEach(item =>
                              {

                                  if (item == null) return;

                                  entityWrappers.Add(
                                      String.Format("{0}_{1}", (int)entityType, item.ID),
                                      new EntityWrapper
                                      {
                                          EntityId = item.ID,
                                          EntityTitle = item.Title,
                                          EntityType = "case"
                                      });
                              });
                        break;
                    default:
                        break;
                }
            }


            var categories = DaoFactory.GetListItemDao().GetItems(categoryIDs.ToArray()).ToDictionary(x => x.ID, x => ToTaskCategory(x));

            var contacts = DaoFactory.GetContactDao().GetContacts(contactIDs.ToArray()).ToDictionary(item => item.ID,
                                                                               item =>
                                                                               ToContactBaseWrapper(item));

            foreach (var item in itemList)
            {
                var taskWrapper = new TaskWrapper(item);

                taskWrapper.CanEdit = CRMSecurity.CanEdit(item);

                if (contacts.ContainsKey(item.ContactID))
                    taskWrapper.Contact = contacts[item.ContactID];

                if (item.EntityID > 0)
                {

                    var entityStrKey = String.Format("{0}_{1}", (int)item.EntityType, item.EntityID);

                    if (entityWrappers.ContainsKey(entityStrKey))
                        taskWrapper.Entity = entityWrappers[entityStrKey];

                }

                if (categories.ContainsKey(item.CategoryID))
                    taskWrapper.Category = categories[item.CategoryID];

                result.Add(taskWrapper);

            }

            return result;

        }


        private TaskWrapper ToTaskWrapper(Task task)
        {
            var result = new TaskWrapper(task);

            if (task.CategoryID > 0)
                result.Category = GetTaskCategoryByID(task.CategoryID);

            if (task.ContactID > 0)
                result.Contact = ToContactBaseWrapper(DaoFactory.GetContactDao().GetByID(task.ContactID));

            if (task.EntityID > 0)
                result.Entity = ToEntityWrapper(task.EntityType, task.EntityID);

            result.CanEdit = CRMSecurity.CanEdit(task);

            return result;
        }
    }
}
