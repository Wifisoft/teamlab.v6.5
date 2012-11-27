#region Import

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Collections;
using ASC.Common.Utils;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.Controls;
using ASC.Web.Core.Utility.Skins;
using AjaxPro;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM;

#endregion

namespace ASC.CRM.Core.Dao
{

    public class CachedListItem : ListItemDao
    {

        #region Members

        private readonly HttpRequestDictionary<ListItem> _listItemCache = new HttpRequestDictionary<ListItem>("crm_list_item");

        #endregion

        #region Constructor

        public CachedListItem(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {


        }

        #endregion

        #region Members

        public override void ChangeColor(int id, string newColor)
        {
            ResetCache(id);

            base.ChangeColor(id, newColor);
        }

        public override void DeleteItem(ListType listType, int itemID)
        {
            ResetCache(itemID);

            base.DeleteItem(listType, itemID);
        }

        public override void ChangePicture(int id, string newPicture)
        {
            ResetCache(id);

            base.ChangePicture(id, newPicture);
        }

        public override void EditItem(ListType listType, ListItem enumItem)
        {
            ResetCache(enumItem.ID);

            base.EditItem(listType, enumItem);
        }

        public override void ReorderItems(ListType listType, string[] titles)
        {
            _listItemCache.Clear();

            base.ReorderItems(listType, titles);
        }

        public override ListItem GetByID(int id)
        {
            return _listItemCache.Get(id.ToString(), () => GetByIDBase(id));
        }

        private ListItem GetByIDBase(int id)
        {
            return base.GetByID(id);
        }

        private void ResetCache(int id)
        {
            _listItemCache.Reset(id.ToString());
        }

        #endregion


    }

    public class ListItemDao : AbstractDao
    {
        #region Constructor

        public ListItemDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {


        }

        #endregion



        public bool IsExist(ListType listType, String title)
        {

            return DbManager.ExecuteScalar<int>(Query("crm_list_item").SelectCount()
                                                .Where(Exp.Eq("list_type", (int)listType) & Exp.Eq("title", title))) > 0;

        }

        public bool IsExist(int id)
        {
            return DbManager.ExecuteScalar<int>(Query("crm_list_item").SelectCount().Where(Exp.Eq("id", id))) > 0;
        }

        public List<ListItem> GetItems(ListType listType)
        {
            SqlQuery sqlQuery = GetListItemSqlQuery(Exp.Eq("list_type", (int)listType))
                               .OrderBy("sort_order", true);

            return DbManager.ExecuteList(sqlQuery)
                  .ConvertAll(row => ToListItem(row));
        }

        public ListItem GetSystemListItem(int id)
        {
            switch (id)
            {
                case (int)HistoryCategorySystem.TaskClosed:
                    return new ListItem
                               {
                                   ID = -1,
                                   Title = HistoryCategorySystem.TaskClosed.ToLocalizedString(),
                                   AdditionalParams = "event_category_close.png"
                               };
                case (int)HistoryCategorySystem.FilesUpload:
                    return new ListItem
                               {
                                   ID = -2,
                                   Title = HistoryCategorySystem.FilesUpload.ToLocalizedString(),
                                   AdditionalParams = "32-attach.png"
                               };
                default:
                    return null;
            }

        }

        public virtual ListItem GetByID(int id)
        {
            if (id < 0) return GetSystemListItem(id);

            var result = DbManager.ExecuteList(GetListItemSqlQuery(Exp.Eq("id", id))).ConvertAll(row => ToListItem(row));

            return result.Count > 0 ? result[0] : null;
        }

        public virtual List<ListItem> GetItems(int[] id)
        {
            var sqlResult = DbManager.ExecuteList(GetListItemSqlQuery(Exp.In("id", id))).ConvertAll(row => ToListItem(row));

            var systemItem = id.Where(item => item < 0).Select(x => GetSystemListItem(x));

            return systemItem.Any() ? sqlResult.Union(systemItem).ToList() : sqlResult;
        }

        public virtual List<ListItem> GetAll()
        {
            return DbManager.ExecuteList(GetListItemSqlQuery(null)).ConvertAll(row => ToListItem(row));
        }




        public virtual void ChangeColor(int id, String newColor)
        {

            DbManager.ExecuteNonQuery(Update("crm_list_item")
                                      .Set("color", newColor)
                                      .Where(Exp.Eq("id", id)));

        }

        public NameValueCollection GetColors(ListType listType)
        {

            Exp where = Exp.Eq("list_type", (int)listType);

            var result = new NameValueCollection();

            DbManager.ExecuteList(Query("crm_list_item")
                                      .Select("id", "color")
                                      .Where(where))
                                  .ForEach(row => result.Add(row[0].ToString(), row[1].ToString()));

            return result;

        }

        public ListItem GetByTitle(ListType listType, String title)
        {

            var result = DbManager.ExecuteList(GetListItemSqlQuery(Exp.Eq("title", title) & Exp.Eq("list_type", (int)listType))).ConvertAll(row => ToListItem(row));

            return result.Count > 0 ? result[0] : null;
        }

        public int GetRelativeItemsCount(ListType listType, int id)
        {

            SqlQuery sqlQuery;


            switch (listType)
            {
                case ListType.ContactStatus:
                    sqlQuery = Query("crm_contact")
                              .Select("count(*)")
                              .Where(Exp.Eq("status_id", id));
                    break;
                case ListType.TaskCategory:
                    sqlQuery = Query("crm_task")
                             .Select("count(*)")
                             .Where(Exp.Eq("category_id", id));
                    break;
                case ListType.HistoryCategory:
                    sqlQuery = Query("crm_relationship_event")
                              .Select("count(*)")
                              .Where(Exp.Eq("category_id", id));

                    break;
                default:
                    throw new ArgumentException();
                  
            }

            return DbManager.ExecuteScalar<int>(sqlQuery);
        }

        public Dictionary<int, int> GetRelativeItemsCount(ListType listType)
        {
            var sqlQuery = Query("crm_list_item tbl_list_item")
              .Where(Exp.Eq("tbl_list_item.list_type", (int)listType))
              .Select("tbl_list_item.id")
              .OrderBy("tbl_list_item.sort_order", true)
              .GroupBy("tbl_list_item.id");

            switch (listType)
            {
                case ListType.ContactStatus:
                    sqlQuery.LeftOuterJoin("crm_contact tbl_crm_contact",
                                            Exp.EqColumns("tbl_list_item.id", "tbl_crm_contact.status_id")
                                             & Exp.EqColumns("tbl_list_item.tenant_id", "tbl_crm_contact.tenant_id"))
                                          .Select("count(tbl_crm_contact.status_id)");
                    break;
                case ListType.TaskCategory:
                    sqlQuery.LeftOuterJoin("crm_task tbl_crm_task",
                                            Exp.EqColumns("tbl_list_item.id", "tbl_crm_task.category_id")
                                              & Exp.EqColumns("tbl_list_item.tenant_id", "tbl_crm_task.tenant_id"))
                                           .Select("count(tbl_crm_task.category_id)");
                    break;
                case ListType.HistoryCategory:
                    sqlQuery.LeftOuterJoin("crm_relationship_event tbl_crm_relationship_event",
                                            Exp.EqColumns("tbl_list_item.id", "tbl_crm_relationship_event.category_id")
                                              & Exp.EqColumns("tbl_list_item.tenant_id", "tbl_crm_relationship_event.tenant_id"))
                                           .Select("count(tbl_crm_relationship_event.category_id)");

                    break;
                default:
                    throw new ArgumentException();
            }

            var queryResult = DbManager.ExecuteList(sqlQuery);

            return queryResult.ToDictionary(x => Convert.ToInt32(x[0]), y => Convert.ToInt32(y[1]));

        }

        public virtual int CreateItem(ListType listType, ListItem enumItem)
        {

            if (IsExist(listType, enumItem.Title))
                return GetByTitle(listType, enumItem.Title).ID;

            if (String.IsNullOrEmpty(enumItem.Title))
                throw new ArgumentException();

            if (listType == ListType.TaskCategory || listType == ListType.HistoryCategory)
                if (String.IsNullOrEmpty(enumItem.AdditionalParams))
                    throw new ArgumentException();
                else
                   enumItem.AdditionalParams = System.IO.Path.GetFileName(enumItem.AdditionalParams);
                
            if (listType == ListType.ContactStatus)
                if (String.IsNullOrEmpty(enumItem.Color))
                    throw new ArgumentException();

            var sortOrder = enumItem.SortOrder;

            if (sortOrder == 0)
                sortOrder = DbManager.ExecuteScalar<int>(Query("crm_list_item")
                                                        .Where(Exp.Eq("list_type", (int)listType))
                                                        .SelectMax("sort_order")) + 1;

            return DbManager.ExecuteScalar<int>(
                                              Insert("crm_list_item")
                                              .InColumnValue("id", 0)
                                              .InColumnValue("list_type", (int)listType)
                                              .InColumnValue("description", enumItem.Description)
                                              .InColumnValue("title", enumItem.Title)
                                              .InColumnValue("additional_params", enumItem.AdditionalParams)
                                              .InColumnValue("color", enumItem.Color)
                                              .InColumnValue("sort_order", sortOrder)
                                              .Identity(1, 0, true));
        }

        public virtual void EditItem(ListType listType, ListItem enumItem)
        {

            if (HaveRelativeItemsLink(listType, enumItem.ID))
                throw new ArgumentException();

            DbManager.ExecuteNonQuery(Update("crm_list_item")
                                      .Set("description", enumItem.Description)
                                      .Set("title", enumItem.Title)
                                      .Set("additional_params", enumItem.AdditionalParams)
                                      .Set("color", enumItem.Color)
                                      .Where(Exp.Eq("id", enumItem.ID)));


        }

        public virtual void ChangePicture(int id, String newPicture)
        {
            DbManager.ExecuteNonQuery(Update("crm_list_item")
                                  .Set("additional_params", newPicture)
                                  .Where(Exp.Eq("id", id)));
        }

        private bool HaveRelativeItemsLink(ListType listType, int itemID)
        {
            SqlQuery sqlQuery;

            switch (listType)
            {

                case ListType.ContactStatus:
                    sqlQuery = Query("crm_contact")
                               .Where(Exp.Eq("status_id", itemID));

                    break;
                case ListType.TaskCategory:
                    sqlQuery = Query("crm_task")
                              .Where(Exp.Eq("category_id", itemID));
                    break;
                case ListType.HistoryCategory:
                    sqlQuery = Query("crm_relationship_event")
                              .Where(Exp.Eq("category_id", itemID));
                    break;
                default:
                    throw new ArgumentException();
            }

            return DbManager.ExecuteScalar<int>(sqlQuery.SelectCount()) > 0;

        }

        public void ChangeRelativeItemsLink(ListType listType, int fromItemID, int toItemID)
        {


            if (!IsExist(fromItemID))
                throw new ArgumentException("", "toItemID");
           
            if (!HaveRelativeItemsLink(listType, fromItemID)) return;

            if (!IsExist(toItemID))
                throw new ArgumentException("", "toItemID");
           
            SqlUpdate sqlUpdate;
            
            switch (listType)
            {
                case ListType.ContactStatus:
                    sqlUpdate = Update("crm_contact")
                                .Set("status_id", toItemID)
                                .Where(Exp.Eq("status_id", fromItemID));

                    break;
                case ListType.TaskCategory:
                    sqlUpdate = Update("crm_task")
                               .Set("category_id", toItemID)       
                               .Where(Exp.Eq("category_id", fromItemID));
                    break;
                case ListType.HistoryCategory:
                    sqlUpdate = Update("crm_relationship_event")
                               .Set("category_id", toItemID)  
                               .Where(Exp.Eq("category_id", fromItemID));
                    break;
                default:
                    throw new ArgumentException();
            }
            
            DbManager.ExecuteNonQuery(sqlUpdate);
        }

        public virtual void DeleteItem(ListType listType, int itemID)
        {
             
            if (HaveRelativeItemsLink(listType, itemID))
                throw new ArgumentException();

            DbManager.ExecuteNonQuery(Delete("crm_list_item").Where(Exp.Eq("id", itemID) & Exp.Eq("list_type", (int)listType)));
        }

        public virtual void ReorderItems(ListType listType, String[] titles)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                for (int index = 0; index < titles.Length; index++)
                    DbManager.ExecuteNonQuery(Update("crm_list_item")
                                             .Set("sort_order", index)
                                             .Where(Exp.Eq("title", titles[index]) & Exp.Eq("list_type", (int)listType)));

                tx.Commit();
            }
        }

        private SqlQuery GetListItemSqlQuery(Exp where)
        {
            var result = Query("crm_list_item")
               .Select(
                   "id",
                   "title",
                   "description",
                   "color",
                   "sort_order",
                   "additional_params",
                   "list_type"
               );

            if (where != null)
                result.Where(where);

            return result;

        }

        public static ListItem ToListItem(object[] row)
        {
            return new ListItem
                       {
                           ID = Convert.ToInt32(row[0]),
                           Title = Convert.ToString(row[1]).HtmlEncode(),
                           Description = Convert.ToString(row[2]).HtmlEncode(),
                           Color = Convert.ToString(row[3]),
                           SortOrder = Convert.ToInt32(row[4]),
                           AdditionalParams = Convert.ToString(row[5])
                       };
        }
    }
}
