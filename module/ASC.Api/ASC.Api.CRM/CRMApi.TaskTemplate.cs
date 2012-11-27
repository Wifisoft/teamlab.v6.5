#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Employee;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {

        /// <summary>
        ///   Создать контейнер шаблонов задач
        /// </summary>
        /// <param name="entityType">Type</param>
        /// <param name="title">Title</param>
        /// <short>Создать контейнер шаблонов задач</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Контейнер шаблонов задач
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [Create(@"{entityType:(contact|person|company|opportunity|case)}/tasktemplatecontainer")]
        public TaskTemplateContainerWrapper CreateTaskTemplateContainer(String entityType, String title)
        {
            if (String.IsNullOrEmpty(title))
                throw new ArgumentException();

            var taskTemplateContainer = new TaskTemplateContainer
                                            {
                                                EntityType = ToEntityType(entityType),
                                                Title = title
                                            };

            taskTemplateContainer.ID = DaoFactory.GetTaskTemplateContainerDao().SaveOrUpdate(taskTemplateContainer);

            return ToTaskTemplateContainerWrapper(taskTemplateContainer);

        }

        /// <summary>
        ///    Получить список контейнеров шаблонов задач
        /// </summary>
        /// <param name="entityType">Type</param>
        /// <short>Получить список контейнеров шаблонов задач</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Список контейнеров шаблонов задач
        /// </returns>
        [Read(@"{entityType:(contact|person|company|opportunity|case)}/tasktemplatecontainer")]
        public IEnumerable<TaskTemplateContainerWrapper> GetTaskTemplateContainers(String entityType)
        {
            return ToTaskListTemplateContainerWrapper(DaoFactory.GetTaskTemplateContainerDao().GetItems(ToEntityType(entityType)));

        }

        /// <summary>
        ///   Удалить контейнер шаблонов задач
        /// </summary>
        /// <param name="containerid">ID контейнера шаблонов задач</param>
        /// <short>Удалить контейнер шаблонов задач</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///    Удаленный контейнер шаблонов задач
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Delete(@"tasktemplatecontainer/{containerid:[0-9]+}")]
        public TaskTemplateContainerWrapper DeleteTaskTemplateContainer(int containerid)
        {
            if (containerid <= 0)
                throw new ArgumentException();

            var result = ToTaskTemplateContainerWrapper(DaoFactory.GetTaskTemplateContainerDao().GetByID(containerid));

            if (result == null)
                throw new ItemNotFoundException();

            DaoFactory.GetTaskTemplateContainerDao().Delete(containerid);

            return result;
        }

        /// <summary>
        ///   Обновить контейнер шаблонов задач
        /// </summary>
        /// <param name="containerid">ID контейнера шаблонов задач</param>
        /// <param name="title">Название</param>
        /// <short>Обновить контейнер шаблонов задач</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Контейнер шаблонов задач
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"tasktemplatecontainer/{containerid:[0-9]+}")]
        public TaskTemplateContainerWrapper UpdateTaskTemplateContainer(int containerid, String title)
        {
            if (containerid <= 0 || String.IsNullOrEmpty(title))
                throw new ArgumentException();

            var result = DaoFactory.GetTaskTemplateContainerDao().GetByID(containerid);

            if (result == null)
                throw new ItemNotFoundException();

            result.Title = title;

            DaoFactory.GetTaskTemplateContainerDao().SaveOrUpdate(result);

            return ToTaskTemplateContainerWrapper(result);
        }

        /// <summary>
        ///   Получить контейнер шаблонов задач по id
        /// </summary>
        /// <param name="containerid">ID контейнера шаблонов задач</param>
        /// <short>Получить контейнер шаблонов задач по id</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Контейнер шаблонов задач
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"tasktemplatecontainer/{containerid:[0-9]+}")]
        public TaskTemplateContainerWrapper GetTaskTemplateContainerByID(int containerid)
        {
            if (containerid <= 0)
                throw new ArgumentException();

            var item = DaoFactory.GetTaskTemplateContainerDao().GetByID(containerid);

            if (item == null)
                throw new ItemNotFoundException();

            return ToTaskTemplateContainerWrapper(item);
        }

        /// <summary>
        ///   Получить список шаблонов задач по id контейнера
        /// </summary>
        /// <param name="containerid">ID контейнера шаблонов задач</param>
        /// <short> Получить список шаблонов задач по id контейнера</short> 
        /// <category>Task Templates</category>
        /// <returns>
        ///     Список шаблонов задач
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"tasktemplatecontainer/{containerid:[0-9]+}/tasktemplate")]
        public IEnumerable<TaskTemplateWrapper> GetTaskTemplates(int containerid)
        {
            if (containerid <= 0)
                throw new ArgumentException();

            var container = DaoFactory.GetTaskTemplateContainerDao().GetByID(containerid);

            if (container == null)
                throw new ItemNotFoundException();
            
            return DaoFactory.GetTaskTemplateDao().GetList(containerid).ConvertAll(item => ToTaskTemplateWrapper(item));

        }

        /// <summary>
        ///   Создать шаблон задачи
        /// </summary>
        /// <param name="containerid">id контейнера шаблона задач</param>
        /// <param name="title">Название</param>
        /// <param name="description">Описание</param>
        /// <param name="responsibleid">id ответственного</param>
        /// <param name="categoryid">id категории</param>
        /// <param name="isNotify">Оповещать ответственного или нет</param>
        /// <param name="offsetTicks">Смещение в тиках</param>
        /// <param name="deadLineIsFixed"></param>
        /// <short>Создать шаблон задачи</short> 
        /// <category>Task Templates</category>
        /// <returns>Шаблон задачи</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Create(@"tasktemplatecontainer/{containerid:[0-9]+}/tasktemplate")]
        public TaskTemplateWrapper CreateTaskTemplate(
            int containerid,
            String title,
            String description,
            Guid responsibleid,
            int categoryid,
            bool isNotify,
            long offsetTicks,
            bool deadLineIsFixed
            )
        {
            if (containerid <= 0 || String.IsNullOrEmpty(title) || categoryid <= 0)
                throw new ArgumentException();

            var container = DaoFactory.GetTaskTemplateContainerDao().GetByID(containerid);

            if (container == null)
                throw new ItemNotFoundException();

            var item = new TaskTemplate
                           {
                               CategoryID = categoryid,
                               ContainerID = containerid,
                               DeadLineIsFixed = deadLineIsFixed,
                               Description = description,
                               isNotify = isNotify,
                               ResponsibleID = responsibleid,
                               Title = title,
                               Offset = TimeSpan.FromTicks(offsetTicks)
                           };

            item.ID = DaoFactory.GetTaskTemplateDao().SaveOrUpdate(item);

            return ToTaskTemplateWrapper(item);

        }


        /// <summary>
        ///   Обновить шаблон задачи
        /// </summary>
        /// <param name="id">id шаблона задачи</param>
        /// <param name="containerid">id контейнера шаблонов задач</param>
        /// <param name="title">Название</param>
        /// <param name="description">Описание</param>
        /// <param name="responsibleid">id ответственного</param>
        /// <param name="categoryid">id категории</param>
        /// <param name="isNotify">Оповещать ответственного или нет</param>
        /// <param name="offsetTicks">Смещение в тиках</param>
        /// <param name="deadLineIsFixed"></param>
        /// <short>Обновить шаблон задачи</short> 
        /// <category>Task Templates</category>
        /// <returns>Шаблон задачи</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update(@"tasktemplatecontainer/{containerid:[0-9]+}/tasktemplate")]
        public TaskTemplateWrapper UpdateTaskTemplate(
            int id,
            int containerid,
            String title,
            String description,
            Guid responsibleid,
            int categoryid,
            bool isNotify,
            long offsetTicks,
            bool deadLineIsFixed
            )
        {

            if (containerid <= 0 || String.IsNullOrEmpty(title) || categoryid <= 0)
                throw new ArgumentException();

            var updatingItem = DaoFactory.GetTaskTemplateDao().GetByID(id);

            if (updatingItem == null)
                throw new ItemNotFoundException();

            var container = DaoFactory.GetTaskTemplateContainerDao().GetByID(containerid);

            if (container == null)
                throw new ItemNotFoundException();


            var item = new TaskTemplate
            {
                CategoryID = categoryid,
                ContainerID = containerid,
                DeadLineIsFixed = deadLineIsFixed,
                Description = description,
                isNotify = isNotify,
                ResponsibleID = responsibleid,
                Title = title,
                ID = id,
                Offset = TimeSpan.FromTicks(offsetTicks)
            };

            item.ID = DaoFactory.GetTaskTemplateDao().SaveOrUpdate(item);

            return ToTaskTemplateWrapper(item);
        }


        /// <summary>
        ///   Удалить шаблон задачи
        /// </summary>
        /// <param name="id">id шаблона задачи</param>
        /// <short>Удалить шаблон задачи</short> 
        /// <category>Task Templates</category>
        /// <returns>Шаблон задачи</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Delete(@"tasktemplatecontainer/tasktemplate/{id:[0-9]+}")]
        public TaskTemplateWrapper DeleteTaskTemplate(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var taskTemplate = DaoFactory.GetTaskTemplateDao().GetByID(id);
            
            if (taskTemplate == null)
                throw new ItemNotFoundException();
            

            var result = ToTaskTemplateWrapper(taskTemplate);

            DaoFactory.GetTaskTemplateDao().Delete(id);

            return result;

        }

        /// <summary>
        ///   Получить шаблон задачи по id
        /// </summary>
        /// <param name="id">id шаблона задачи</param>
        /// <short>Получить шаблон задачи по id</short> 
        /// <category>Task Templates</category>
        /// <returns>Шаблон задачи</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Read(@"tasktemplatecontainer/tasktemplate/{id:[0-9]+}")]
        public TaskTemplateWrapper GetTaskTemplateByID(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var taskTemplate = DaoFactory.GetTaskTemplateDao().GetByID(id);

            if (taskTemplate == null)
                throw new ItemNotFoundException();

            return ToTaskTemplateWrapper(taskTemplate);
        }

        protected TaskTemplateWrapper ToTaskTemplateWrapper(TaskTemplate taskTemplate)
        {
            return new TaskTemplateWrapper
                       {
                           Category = GetTaskCategoryByID(taskTemplate.CategoryID),
                           ContainerID = taskTemplate.ContainerID,
                           DeadLineIsFixed = taskTemplate.DeadLineIsFixed,
                           Description = taskTemplate.Description,
                           ID = taskTemplate.ID,
                           isNotify = taskTemplate.isNotify,
                           Title = taskTemplate.Title,
                           OffsetTicks = taskTemplate.Offset.Ticks,
                           Responsible = EmployeeWraper.Get(taskTemplate.ResponsibleID)
                       };
        }

        protected IEnumerable<TaskTemplateContainerWrapper> ToTaskListTemplateContainerWrapper(IEnumerable<TaskTemplateContainer> items)
        {

            var result = new List<TaskTemplateContainerWrapper>();

            var taskTemplateDictionary = DaoFactory.GetTaskTemplateDao().GetAll()
                              .GroupBy(item => item.ContainerID)
                              .ToDictionary(x => x.Key, y => y.Select(p => ToTaskTemplateWrapper(p)));

            foreach (var item in items)
            {

                var taskTemplateContainer = new TaskTemplateContainerWrapper()
                                                {
                                                    Title = item.Title,
                                                    EntityType = item.EntityType.ToString(),
                                                    ID = item.ID
                                                };
              //  ToTaskTemplateContainerWrapper(item);


                if (taskTemplateDictionary.ContainsKey(taskTemplateContainer.ID))
                    taskTemplateContainer.Items = taskTemplateDictionary[taskTemplateContainer.ID];

                result.Add(taskTemplateContainer);
            }

            return result;
        }

        protected TaskTemplateContainerWrapper ToTaskTemplateContainerWrapper(TaskTemplateContainer item)
        {
            return ToTaskListTemplateContainerWrapper(new List<TaskTemplateContainer>()
                                                          {
                                                              item
                                                          }).FirstOrDefault();
        }
    }
}
