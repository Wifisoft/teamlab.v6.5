using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Files.Core;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Files.Configuration
{
    public class SearchHandler : BaseSearchHandlerEx
    {
        public override Guid ProductID
        {
            get { return ProductEntryPoint.ID; }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions {ImageFileName = "common_search_icon.png"}; }
        }

        public override Guid ModuleID
        {
            get { return new Guid("e67be73d-f9ae-4ce1-8fec-1880cb518cb4"); }
        }

        public override string SearchName
        {
            get { return FilesUCResource.Search; }
        }

        public override string AbsoluteSearchURL { get; set; }

        public override string PlaceVirtualPath
        {
            get { return PathProvider.BaseVirtualPath; }
        }

        public override SearchResultItem[] Search(string text)
        {
            var security = Global.GetFilesSecurity();
            using (var folderDao = Global.DaoFactory.GetFolderDao())
            using (var fileDao = Global.DaoFactory.GetFileDao())
            {
                var files = fileDao.Search(text, FolderType.USER | FolderType.COMMON)
                    .Where(security.CanRead)
                    .Select(r => new SearchResultItem
                                     {
                                         Name = r.Title ?? string.Empty,
                                         Description = string.Empty,
                                         URL = FileUtility.ExtsWebPreviewed.Contains(FileUtility.GetFileExtension(r.Title), StringComparer.CurrentCultureIgnoreCase)
                                                   ? CommonLinkUtility.GetFileWebViewerUrl(r.ID)
                                                   : r.ViewUrl,
                                         Date = r.ModifiedOn,
                                         Additional = new Dictionary<string, object>
                                                          {
                                                              {"Author", r.CreateByString.HtmlEncode()},
                                                              {"Container", folderDao.GetParentFolders(r.FolderID)},
                                                              {"IsFolder", false},
                                                              {"Size", FileSizeComment.FilesSizeToString(r.ContentLength)}
                                                          }
                                     });

                var folders = folderDao.Search(text, FolderType.USER | FolderType.COMMON)
                    .Where(security.CanRead)
                    .Select(f => new SearchResultItem
                                     {
                                         Name = f.Title ?? string.Empty,
                                         Description = String.Empty,
                                         URL = PathProvider.GetFolderUrl(f),
                                         Date = f.ModifiedOn,
                                         Additional = new Dictionary<string, object>
                                                          {
                                                              {"Author", f.CreateByString.HtmlEncode()},
                                                              {"Container", folderDao.GetParentFolders(f.ID)},
                                                              {"IsFolder", true}
                                                          }
                                     });

                return folders.Concat(files).ToArray();
            }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }

    }
}