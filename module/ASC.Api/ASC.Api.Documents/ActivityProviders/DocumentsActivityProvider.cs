using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using ASC.Common.Data;
using ASC.Feed.Activity;
using ASC.Feed.ActivityProvider;
using ASC.Feed.Utils;
using ASC.Files.Core;
using ASC.Web.Files.Services.WCFService;

namespace ASC.Api.Documents.ActivityProviders
{
    /// <summary>
    /// </summary>
    public class DocumentsActivityProvider : IActivityProvider
    {
        private const string Documents = "documents";

        /// <summary>
        /// </summary>
        public string SourceName
        {
            get { return Documents; }
        }

        /// <summary>
        /// </summary>
        public DocumentsActivityProvider()
        {
            _fileStorage = new Service();
        }

        private readonly IFileStorageService _fileStorage;

        private IFileStorageService FileStorage
        {
            get
            {
                if (!DbRegistry.IsDatabaseRegistered(FileConstant.DatabaseId))
                {
                    DbRegistry.RegisterDatabase(FileConstant.DatabaseId,
                                                WebConfigurationManager.ConnectionStrings[FileConstant.DatabaseId]);
                }
                return _fileStorage;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="range"></param>
        /// <param name="relativeTo"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public IEnumerable<Activity> GetActivities(DateTimeRange range, object relativeTo, ActivityAction? actions)
        {
            var files = FileStorage.GetFileUpdates(range.From, range.To);
            var folders = FileStorage.GetFolderUpdates(range.From, range.To);

            var activities = new List<Activity>(files.Select(x => GetFileActivity(x, range)));
            activities.AddRange(folders.Select(x => GetFileActivity(x, range)));

            return activities;
        }

        private Activity GetFileActivity(FileEntry fileEntry, DateTimeRange range)
        {
            object wrapper;

            if (fileEntry is File)
                wrapper = new FileWrapper((File) fileEntry);
            else
                wrapper = new FolderWrapper((Folder) fileEntry);

            var activity = new Activity(Documents, wrapper)
            {
                Action = (range.In(fileEntry.CreateOn) ? ActivityAction.Created : ActivityAction.Undefined) | (range.In(fileEntry.ModifiedOn) ? ActivityAction.Updated : ActivityAction.Undefined) | (range.In(fileEntry.SharedToMeOn) ? ActivityAction.Shared : ActivityAction.Undefined),
                ItemType = "file",
            };

            if (range.In(fileEntry.CreateOn))
            {
                activity.CreatedBy = fileEntry.CreateBy;
                activity.When = fileEntry.CreateOn;
            }
            else if (range.In(fileEntry.ModifiedOn))
            {
                activity.CreatedBy = fileEntry.ModifiedBy;
                activity.When = fileEntry.ModifiedOn;
            }
            else if (range.In(fileEntry.SharedToMeOn))
            {
                activity.CreatedBy = new Guid(fileEntry.SharedToMeBy);
                activity.When = fileEntry.SharedToMeOn;
            }

            return activity;
        }
    }
}
