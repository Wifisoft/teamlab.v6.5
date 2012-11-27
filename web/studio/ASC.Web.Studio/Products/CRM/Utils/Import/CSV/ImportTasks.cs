#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Core.Tenants;
using ASC.Core.Users;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json.Linq;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Web.CRM.Classes
{
    public partial class ImportDataOperation
    {
        private void ImportTaskData()
        {
            using (var CSVFileStream = _dataStore.GetReadStream("temp", _CSVFileURI))
            using (CsvReader csv = ImportFromCSV.CreateCsvReaderInstance(CSVFileStream, _importSettings))
            {
                int currentIndex = 0;

                using (var contactDao = _daoFactory.GetContactDao())
                using (var listItemDao = _daoFactory.GetListItemDao())
                using (var taskDao = _daoFactory.GetTaskDao())
                {
                    var findedTasks = new List<Task>();
                    var taskCategories = listItemDao.GetItems(ListType.TaskCategory);

                    while (csv.ReadNextRecord())
                    {
                        _columns = csv.GetCurrentRowFields(false);

                        var obj = new Task();

                        obj.ID = currentIndex;

                        obj.Title = GetPropertyValue("title");

                        if (String.IsNullOrEmpty(obj.Title)) continue;

                        obj.Description = GetPropertyValue("description");

                        DateTime deadline;

                        if (DateTime.TryParse(GetPropertyValue("due_date"), out deadline))
                            obj.DeadLine = deadline;
                        else
                            obj.DeadLine = TenantUtil.DateTimeNow();

                        var responsible = ASC.Core.CoreContext.UserManager.Search(GetPropertyValue("responsible"), EmployeeStatus.All).FirstOrDefault();

                        if (responsible != null)
                            obj.ResponsibleID = responsible.ID;
                        else
                            obj.ResponsibleID = Constants.LostUser.ID;

                        var categoryTitle = GetPropertyValue("taskCategory");

                        if (!String.IsNullOrEmpty(categoryTitle))
                        {
                            var findedCategory = taskCategories.Find(item => String.Compare(item.Title, categoryTitle) == 0);

                            if (findedCategory == null)
                            {
                                obj.CategoryID = taskCategories[0].ID;
                            }
                            else
                                obj.CategoryID = findedCategory.ID;
                        }
                        else
                            obj.CategoryID = taskCategories[0].ID;

                        var contactName = GetPropertyValue("contact");

                        if (!String.IsNullOrEmpty(contactName))
                        {
                            var contacts = contactDao.GetContactsByName(contactName);

                            if (contacts.Count > 0)
                                obj.ContactID = contacts[0].ID;
                        }

                        obj.IsClosed = false;

                        var taskStatus = GetPropertyValue("status");
                        
                        if (!String.IsNullOrEmpty(taskStatus))
                        {
                            if (String.Compare(taskStatus, CRMTaskResource.TaskStatus_Closed, true) == 0)
                                obj.IsClosed = true;
                          
                        }

                        findedTasks.Add(obj);

                        if ((currentIndex + 1) > ImportFromCSV.MaxRoxCount) break;

                        currentIndex++;

                    }

                    Percentage = 50;

                    var newIDs = taskDao.SaveTaskList(findedTasks);

                    Percentage += 12.5;
                }

                Complete();

            }

        }

    }
}