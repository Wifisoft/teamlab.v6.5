using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ASC.Common.Data;
using ASC.Blogs.Core.Domain;

namespace ASC.Blogs.Core.Data
{
    static class RowMappers
    {

        public static Blog ToBlog(IDataRecord row)
        {
            return new Blog()
            {
                BlogID = row.Get<int>("id"),
                Name = row.Get<string>("name"),
                UserID = row.Get<Guid>("user_id"),
                GroupID = row.Get<Guid>("group_id"),
            };

        }

        public static string ToString(IDataRecord row)
        {
            return row.Get<string>("name");
        }

        public static Tag ToTag(IDataRecord row)
        {
            return new Tag()
            {
                Content = row.Get<string>("name"),
                PostId = row.Get<Guid>("post_id"),
            };
        }

        public static Comment ToComment(IDataRecord row)
        {
            return ToComment(row, string.Empty);
        }

        public static Comment ToComment(IDataRecord row, string alias)
        {
            return new Comment()
            {
                ID = row.Get<Guid>("id"),
                PostId = row.Get<Guid>("post_id"),
                Content = row.Get<string>("content"),
                UserID = row.Get<Guid>("created_by"),
                Datetime = ASC.Core.Tenants.TenantUtil.DateTimeFromUtc(row.Get<DateTime>("created_when")),
                ParentId = row.Get<Guid>("parent_id"),
                Inactive = row.Get<int>("inactive") > 0
            };
        }

        public static Post ToPost(IDataRecord row)
        {
            return new Post()
            {
                ID = row.Get<Guid>("id"),
                Title = row.Get<string>("title"),
                UserID = row.Get<Guid>("created_by"),
                Datetime = ASC.Core.Tenants.TenantUtil.DateTimeFromUtc(row.Get<DateTime>("created_when")),
                BlogId = row.Get<int>("blog_id"),

                Content = row.Get<string>("content")

            };
        }
    }
}
