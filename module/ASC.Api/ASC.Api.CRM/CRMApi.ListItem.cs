#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Collections;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {
        /// <summary>
        ///   Creates an opportunity stage with the parameters (title, description, success probability, etc.) specified in the request
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="color">Color</param>
        /// <param name="successProbability">Success probability</param>
        /// <param name="stageType" remark="Allowed values: 0 (Open), 1 (ClosedAndWon),2 (ClosedAndLost)">Stage type</param>
        /// <short>Create opportunity stage</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Opportunity stage
        /// </returns>
        [Create(@"opportunity/stage")]
        public DealMilestoneWrapper CreateDealMilestone(
            String title,
            String description,
            String color,
            int successProbability,
            DealMilestoneStatus stageType)
        {

            if (String.IsNullOrEmpty(title))
                throw new ArgumentException();

            if (successProbability < 0)
                successProbability = 0;

            var dealMilestone = new DealMilestone
                                    {
                                        Title = title,
                                        Color = color,
                                        Description = description,
                                        Probability = successProbability,
                                        Status = stageType
                                    };

            dealMilestone.ID = DaoFactory.GetDealMilestoneDao().Create(dealMilestone);

            return ToDealMilestoneWrapper(dealMilestone);

        }

        /// <summary>
        ///    Updates the selected opportunity stage with the parameters (title, description, success probability, etc.) specified in the request
        /// </summary>
        /// <param name="id">Opportunity stage id</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="color">Color</param>
        /// <param name="successProbability">Success probability</param>
        /// <param name="stageType" remark="Allowed values: Open, ClosedAndWon, ClosedAndLost">Stage type</param>
        /// <short>Updates the selected opportunity stage</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Opportunity stage
        /// </returns>
        [Update(@"opportunity/stage/{id:[0-9]+}")]
        public DealMilestoneWrapper UpdateDealMilestone(
            int id,
            String title,
            String description,
            String color,
            int successProbability,
            DealMilestoneStatus stageType)
        {

            if (id <= 0)
                throw new ArgumentException();

            if (successProbability < 0)
                successProbability = 0;

            var curDealMilestone = DaoFactory.GetDealMilestoneDao().GetByID(id);

            if (curDealMilestone == null)
                throw new ItemNotFoundException();

            var dealMilestone = new DealMilestone
            {
                Title = title,
                Color = color,
                Description = description,
                Probability = successProbability,
                Status = stageType,
                ID = id
            };

            DaoFactory.GetDealMilestoneDao().Edit(dealMilestone);

            return ToDealMilestoneWrapper(dealMilestone);

        }
     

        /// <summary>
        ///   Creates a new history category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Creates a new history category</short> 
        /// <category>History</category>
        ///<returns>History category</returns>
        ///<exception cref="ArgumentException"></exception>
        [Create(@"history/category")]
        public HistoryCategoryWrapper CreateHistoryCategory(String title,
                                               String description,
                                               String imageName,
                                               int sortOrder)
        {

            if (String.IsNullOrEmpty(title))
                throw new ArgumentException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName
            };

            listItem.ID = DaoFactory.GetListItemDao().CreateItem(ListType.HistoryCategory, listItem);

            return ToHistoryCategoryWrapper(listItem);
        }


        /// <summary>
        ///   Updates the selected history category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">History category id</param>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Updates the selected history category</short> 
        ///<category>History</category>
        ///<returns>History category</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"history/category/{id:[0-9]+}")]
        public HistoryCategoryWrapper UpdateHistoryCategory(
           int id, 
           String title,
           String description,
           String imageName,
           int sortOrder)
        {

            if (String.IsNullOrEmpty(title))
                throw new ArgumentException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName,
                ID = id
            };
            
            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().EditItem(ListType.HistoryCategory, listItem);

            return ToHistoryCategoryWrapper(listItem);
        }


        /// <summary>
        ///   Deletes the selected history category with the ID specified in the request
        /// </summary>
        /// <short>Delete the selected history category</short> 
        /// <category>History</category>
        /// <param name="id">History category ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <returns>History category</returns>
        [Delete(@"history/category/{id:[0-9]+}")]
        public HistoryCategoryWrapper DeleteHistoryCategory(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(id);

            if (listItem == null)
                throw new ItemNotFoundException();

            var result = ToHistoryCategoryWrapper(listItem);

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().DeleteItem(ListType.HistoryCategory, id);

            return result;
        }


       

        /// <summary>
        ///   Creates a new task category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Create task category</short> 
        ///<category>Tasks</category>
        ///<returns>Task category</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<returns>
        ///    Task category
        ///</returns>
        [Create(@"task/category")]
        public TaskCategory CreateTaskCategory(String title,
                                               String description,
                                               String imageName,
                                               int sortOrder)
        {

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName
            };

            listItem.ID = DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory, listItem);

            return ToTaskCategory(listItem);
        }

        /// <summary>
        ///   Updates the selected task category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">Task category id</param>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Updates the selected task category</short> 
        ///<category>Tasks</category>
        ///<returns>Task category</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        ///<returns>
        ///    Task category
        ///</returns>
        [Update(@"task/category/{id:[0-9]+}")]
        public TaskCategory UpdateTaskCategory(
           int id,
           String title,
           String description,
           String imageName,
           int sortOrder)
        {

            if (id <= 0)
                throw new ArgumentException();

            var curTaskCategory = DaoFactory.GetListItemDao().GetByID(id);

            if (curTaskCategory == null)
                throw new ItemNotFoundException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName,
                ID = id 
            };

            CRMSecurity.DemandEdit(listItem);


            DaoFactory.GetListItemDao().EditItem(ListType.TaskCategory, listItem);

            return ToTaskCategory(listItem);
        }

        /// <summary>
        ///   Creates a new contact type with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="color">Color</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact type</returns>
        /// <short>Create contact type</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Contact type
        /// </returns>
        [Create(@"contact/type")]
        public ContactType CreateContactType(String title,
                                             String description,
                                             String color,
                                             int sortOrder)
        {
            var listItem = new ListItem
                               {
                                   Title = title,
                                   Description = description,
                                   Color = color,
                                   SortOrder = sortOrder
                               };

            listItem.ID = DaoFactory.GetListItemDao().CreateItem(ListType.ContactStatus, listItem);

            return ToContactType(listItem);
        }


        /// <summary>
        ///   Updates the selected contact type with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">Contact type id</param>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="color">Color</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact type</returns>
        /// <short>Updates the selected contact type</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <returns>
        ///    Contact type
        /// </returns>
        [Update(@"contact/type/{id:[0-9]+}")]
        public ContactType UpdateContactType(
            int id, 
            String title,
            String description,
            String color,
            int sortOrder)
        {
            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                Color = color,
                SortOrder = sortOrder
            };

            
            CRMSecurity.DemandEdit(listItem);

            listItem.ID = DaoFactory.GetListItemDao().CreateItem(ListType.ContactStatus, listItem);

            return ToContactType(listItem);
        }




        /// <summary>
        ///   Returns the type of the contact for the ID specified in the request
        /// </summary>
        /// <param name="contactTypeid">Contact type ID</param>
        /// <returns>Contact type</returns>
        /// <short>Gets contact type</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"contact/type/{contactTypeid:[0-9]+}")]
        public ContactType GetContactTypeByID(int contactTypeid)
        {
            if (contactTypeid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(contactTypeid);

            if (listItem == null)
                throw new ItemNotFoundException();

            return ToContactType(listItem);
        }

        /// <summary>
        ///  Returns the stage of the opportunity with the ID specified in the request
        /// </summary>
        /// <param name="stageid">Opportunity stage ID</param>
        /// <returns>Opportunity stage</returns>
        /// <short>Get opportunity stage</short> 
        /// <category>Opportunities</category>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="ArgumentException"></exception>
        [Read(@"opportunity/stage/{stageid:[0-9]+}")]
        public DealMilestoneWrapper GetDealMilestoneByID(int stageid)
        {
            if (stageid <= 0)
                throw new ArgumentException();

            var dealMilestone = DaoFactory.GetDealMilestoneDao().GetByID(stageid);

            if (dealMilestone == null)
                throw new ItemNotFoundException();



            return ToDealMilestoneWrapper(dealMilestone);
        }

        /// <summary>
        ///    Returns the category of the task with the ID specified in the request
        /// </summary>
        /// <param name="categoryid">Task category ID</param>
        /// <returns>Task category</returns>
        /// <short>Get task category</short> 
        /// <category>Tasks</category>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="ArgumentException"></exception>
        [Read(@"task/category/{categoryid:[0-9]+}")]
        public TaskCategory GetTaskCategoryByID(int categoryid)
        {
            if (categoryid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(categoryid);

            if (listItem == null)
                throw new ItemNotFoundException();

            return ToTaskCategory(listItem);
        }

        /// <summary>
        ///    Returns the list of all history categories available on the portal
        /// </summary>
        /// <short>Get all history categories</short> 
        /// <category>History</category>
        /// <returns>
        ///    List of all history categories
        /// </returns>
        [Read(@"history/category")]
        public IEnumerable<HistoryCategoryWrapper> GetHistoryCategoryWrapper()
        {
            var result = DaoFactory.GetListItemDao().GetItems(ListType.HistoryCategory).ConvertAll(item => new HistoryCategoryWrapper(item));

            var relativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.HistoryCategory);

            result.ForEach(x =>
            {

                if (relativeItemsCount.ContainsKey(x.ID))
                    x.RelativeItemsCount = relativeItemsCount[x.ID];
            });
            
            return result;
        }


        /// <summary>
        ///    Returns the list of all task categories available on the portal
        /// </summary>
        /// <short>Get all task categories</short> 
        /// <category>Tasks</category>
        /// <returns>
        ///    List of all task categories
        /// </returns>
        [Read(@"task/category")]
        public IEnumerable<TaskCategory> GetTaskCategories()
        {

            var result =
                DaoFactory.GetListItemDao().GetItems(ListType.TaskCategory).ConvertAll(item => new TaskCategory(item));

            var relativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.TaskCategory);

            result.ForEach(x=>
                               {

                                   if (relativeItemsCount.ContainsKey(x.ID))
                                       x.RelativeItemsCount = relativeItemsCount[x.ID];
                               });



            return result;
        }

        /// <summary>
        ///    Returns the list of all contact types available on the portal
        /// </summary>
        /// <short>Get all contact types</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    List of all contact types
        /// </returns>
        [Read(@"contact/type")]
        public IEnumerable<ContactType> GetContactTypes()
        {

            var result =
                DaoFactory.GetListItemDao().GetItems(ListType.ContactStatus).ConvertAll(item => new ContactType(item));

            var relativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactStatus);

            result.ForEach(x=>
                               {

                                   if (relativeItemsCount.ContainsKey(x.ID))
                                       x.RelativeItemsCount = relativeItemsCount[x.ID];
                               });



            return result;

        }

        /// <summary>
        ///    Returns the list of all opportunity stages available on the portal
        /// </summary>
        /// <short>Get all opportunity stages</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///   List of all opportunity stages
        /// </returns>
        [Read(@"opportunity/stage")]
        public IEnumerable<DealMilestoneWrapper> GetDealMilestones()
        {
            var result = DaoFactory.GetDealMilestoneDao().GetAll().ConvertAll(item => new DealMilestoneWrapper(item));

            var relativeItemsCount = DaoFactory.GetDealMilestoneDao().GetRelativeItemsCount();

            result.ForEach(x =>
            {

                if (relativeItemsCount.ContainsKey(x.ID))
                    x.RelativeItemsCount = relativeItemsCount[x.ID];
            });

            return result;
        }
        
        /// <summary>
        ///   Deletes the task category with the ID specified in the request
        /// </summary>
        /// <short>Delete task category</short> 
        /// <category>Tasks</category>
        /// <param name="categoryid">Task category ID</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="SecurityException"></exception>
        [Delete(@"task/category/{categoryid:[0-9]+}")]
        public TaskCategory DeleteTaskCategory(int categoryid)
        {
            if (categoryid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(categoryid);

            if (listItem == null)
                throw new ItemNotFoundException();

           
            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().DeleteItem(ListType.TaskCategory, categoryid);

            return ToTaskCategory(listItem);
        }

        /// <summary>
        ///   Deletes the contact type with the ID specified in the request
        /// </summary>
        /// <short>Delete contact type</short> 
        /// <category>Contacts</category>
        /// <param name="contactTypeid">Contact type ID</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="SecurityException"></exception>
        /// <returns>
        ///  Contact type
        /// </returns>
        [Delete(@"contact/type/{contactTypeid:[0-9]+}")]
        public ContactType DeleteContactType(int contactTypeid)
        {
            if (contactTypeid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(contactTypeid);

            if (listItem == null)
                throw new ItemNotFoundException();

            var contactType = ToContactType(listItem);

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().DeleteItem(ListType.ContactStatus, contactTypeid);

            return contactType;
        }

        /// <summary>
        ///   Deletes the opportunity stage with the ID specified in the request
        /// </summary>
        /// <short>Delete opportunity stage</short> 
        /// <category>Opportunities</category>
        /// <param name="id">Opportunity stage ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Opportunity stage
        /// </returns>
        [Delete(@"opportunity/stage/{id:[0-9]+}")]
        public DealMilestoneWrapper DeleteMilestone(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var dealMilestone = DaoFactory.GetDealMilestoneDao().GetByID(id);

            if (dealMilestone == null)
                throw new ItemNotFoundException();

            var result = ToDealMilestoneWrapper(dealMilestone);

            DaoFactory.GetDealMilestoneDao().Delete(id);

            return result;

        }

        public ContactType ToContactType(ListItem listItem)
        {
            var result = new ContactType(listItem)
            {
                RelativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactStatus, listItem.ID)

            };

            return result;
        }

        public HistoryCategoryWrapper ToHistoryCategoryWrapper(ListItem listItem)
        {
            var result = new HistoryCategoryWrapper(listItem)
            {
                                 RelativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.HistoryCategory,listItem.ID)

                             };

            return result;
        }
        
        public TaskCategory ToTaskCategory(ListItem listItem)
        {

            var result = new TaskCategory(listItem)
                             {
                                 RelativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.TaskCategory,listItem.ID)

                             };

            return result;
        }

        private DealMilestoneWrapper ToDealMilestoneWrapper(DealMilestone dealMilestone)
        {
            var result = new DealMilestoneWrapper(dealMilestone)
                             {
                                 RelativeItemsCount = DaoFactory.GetDealMilestoneDao().GetRelativeItemsCount(dealMilestone.ID)
                             };

            return result;
        }

    }
}