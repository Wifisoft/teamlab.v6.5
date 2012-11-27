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
using System.Linq.Expressions;
using ASC.CRM.Core.Entities;
using ASC.Api.Collections;
using ASC.Web.CRM.Classes;
using Newtonsoft.Json.Linq;
using Contact = ASC.CRM.Core.Entities.Contact;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {

        /// <summary>
        ///  Returns the list of all tags associated with the entity with the ID and type specified in the request
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <short>Get entity tags</short> 
        /// <category>Tags</category>
        /// <returns>
        ///   Tag
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [Read("{entityType:(contact|opportunity|case)}/tag/{entityid:[0-9]+}")]
        public IEnumerable<String> GetEntityTags(String entityType, int entityid)
        {
            if (String.IsNullOrEmpty(entityType) || entityid <= 0)
                throw new ArgumentException();

            return DaoFactory.GetTagDao().GetEntityTags(ToEntityType(entityType), entityid);
        }

        /// <summary>
        ///    Returns the list of all tags for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <short>Get all contact tags</short> 
        /// <category>Tags</category>
        /// <returns>
        ///   List of contact tags
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [Read(@"contact/{contactid:[0-9]+}/tag")]
        public IEnumerable<String> GetContactTags(int contactid)
        {
            if (contactid <= 0)
                throw new ArgumentException();

            return DaoFactory.GetTagDao().GetEntityTags(EntityType.Contact, contactid).ToItemList();
        }

        /// <summary>
        ///  Creates the tag for the selected entity with the tag name specified in the request
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="tagName">Tag name</param>
        /// <short>Create tag</short> 
        /// <category>Tags</category>
        /// <returns>
        ///   Tag
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [Create("{entityType:(contact|opportunity|case)}/tag")]
        public String CreateTag(String entityType, String tagName)
        {
            if (String.IsNullOrEmpty(tagName))
                throw new ArgumentException();

            DaoFactory.GetTagDao().AddTag(ToEntityType(entityType), tagName);

            return tagName;
        }


        /// <summary>
        ///  Returns the list of all tags associated with the entity type specified in the request
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <short>Get tags for entity type</short> 
        /// <category>Tags</category>
        /// <returns>
        ///   Tag
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [Read("{entityType:(contact|opportunity|case)}/tag")]
        public IEnumerable<String> GetAllTags(String entityType)
        {
            if (String.IsNullOrEmpty(entityType))
                throw new ArgumentException();

            return DaoFactory.GetTagDao().GetAllTags(ToEntityType(entityType));
        }


        /// <summary>
        ///    Adds a group of tags to the entity with the ID specified in the request
        /// </summary>
        /// <short>Add tag group to entity</short> 
        /// <category>Tags</category>
        /// <param name="entityType">Tag type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="tagName">Tag name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Tag
        /// </returns> 
        [Create(@"{entityType:(contact|opportunity|case)}/taglist")]
        public String AddTagToBatch(String entityType, IEnumerable<int> entityid, String tagName)
        {
            if (entityid == null || !entityid.Any())
                throw new ArgumentException();

            foreach (var entityID in entityid)
                AddTagTo(entityType, entityID, tagName);

            return tagName;
        }

        /// <summary>
        ///  Deletes all the unused tags from the entities with the type specified in the request
        /// </summary>
        /// <short>Delete unused tags</short> 
        /// <category>Tags</category>
        /// <param name="entityType">Entity type</param>
        /// <returns>Tags</returns>
        [Delete("{entityType:(contact|opportunity|case)}/tag/unused")]
        public IEnumerable<String> DeleteUnusedTag(String entityType)
        {
            var entityTypeObj = ToEntityType(entityType);

            var result = DaoFactory.GetTagDao().GetUnusedTags(entityTypeObj);

            DaoFactory.GetTagDao().DeleteUnusedTags(ToEntityType(entityType));

            return new List<String>(result);
        }



        /// <summary>
        ///  Adds the selected tag to the entity with the type and ID specified in the request
        /// </summary>
        /// <short>Add tag</short> 
        /// <category>Tags</category>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="tagName">Tag name</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Tag
        /// </returns> 
        [Create(@"{entityType:(contact|opportunity|case)}/{entityid:[0-9]+}/tag")]
        public String AddTagTo(String entityType, int entityid, String tagName)
        {
            if (entityid <= 0 || String.IsNullOrEmpty(tagName))
                throw new ArgumentException();

            DaoFactory.GetTagDao().AddTagToEntity(ToEntityType(entityType), entityid, tagName);

            return tagName;
        }



        /// <summary>
        ///   Deletes the selected tag from the entity with the type specified in the request
        /// </summary>
        /// <short>Delete tag</short> 
        /// <param name="entityType">Entity type</param>
        /// <param name="tagName">Tag name</param>
        /// <category>Tags</category>
        /// <exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Tag
        /// </returns>
        [Delete("{entityType:(contact|opportunity|case)}/tag")]
        public String DeleteTag(String entityType, String tagName)
        {
            if (String.IsNullOrEmpty(entityType) || String.IsNullOrEmpty(tagName))
                throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);

            if (!DaoFactory.GetTagDao().IsExist(entityTypeObj, tagName))
                throw new ItemNotFoundException();

            DaoFactory.GetTagDao().DeleteTag(entityTypeObj, tagName);

            return tagName;
        }

    }
}
