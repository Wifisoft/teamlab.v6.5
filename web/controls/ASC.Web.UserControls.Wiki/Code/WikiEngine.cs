using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Api.Exceptions;
using ASC.Core;
using ASC.Core.Common.Notify;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Controls;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Handlers;
using ASC.Web.UserControls.Wiki.Resources;
using ASC.Web.UserControls.Wiki.UC;
using File = ASC.Web.UserControls.Wiki.Data.File;

namespace ASC.Web.UserControls.Wiki
{
    public class WikiEngine
    {
        private static readonly string connectionStringName = WikiSection.Section.DB.ConnectionStringName;

        #region Categories

        public List<Category> GetCategories()
        {
            using (var dao = GetCategoryDAO())
            {
                return dao.GetCategories();
            }
        }

        public List<Category> GetCategories(string pagename)
        {
            using (var dao = GetCategoryDAO())
            {
                return dao.GetCategories(pagename, false);
            }
        }

        public List<Category> GetCategoriesRemovedWithPage(string pagename)
        {
            using (var dao = GetCategoryDAO())
            {
                return dao.GetCategories(pagename, true);
            }
        }

        public Category SaveCategory(Category category)
        {
            if (String.IsNullOrEmpty(category.CategoryName)) throw new ArgumentException(@"category name cannot be empty", "category");
            if (String.IsNullOrEmpty(category.PageName)) throw new ArgumentException(@"page name cannot be empty", "category");
            using (var dao = GetCategoryDAO())
            {
                var saved = dao.SaveCategory(category);

                if(saved != null) NotifyCategoryAdded(category.CategoryName, category.PageName);

                return saved;
            }
        }

        public void RemoveCategories(string pagename)
        {
            using (var dao = GetCategoryDAO())
            {
                dao.RemoveCategories(pagename);
            }
        }

        public List<string> UpdateCategoriesByPageContent(Page page)
        {
            var RegExCategorySearch = string.Format(@"\[\[{0}:([^\|^\]]+)(\|[^]]+)*\]\]", Constants.WikiCategoryKeyCaption);          
            var result = new List<string>();

            using (var dao = GetCategoryDAO())
            {
                var catReg = new Regex(RegExCategorySearch, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                var oldCategories = dao.GetCategories(page.PageName, false);
                dao.RemoveCategories(page.PageName);
                string categoryName;
                string bodyWikiOnly = HtmlWikiUtil.rexNoSection.Replace(page.Body, string.Empty);
                foreach (Match m in catReg.Matches(bodyWikiOnly))
                {
                    categoryName = m.Groups[1].Value.Trim();
                    if (categoryName.Length > 240) categoryName = categoryName.Substring(0, 240).Trim();
                    categoryName = PageNameUtil.NormalizeNameCase(PageNameUtil.NormalizeNameCase(categoryName));
                    if (!oldCategories.Exists(oc => oc.CategoryName.Equals(categoryName, StringComparison.InvariantCulture)))
                    {
                        result.Add(categoryName);
                    }
                    dao.SaveCategory(new Category { PageName = page.PageName, CategoryName = categoryName });
                }
            }

            return result;
        }

        #endregion

        
        #region Comments

        public List<Comment> GetComments(string pagename)
        {
            using (var dao = GetCommentDAO())
            {
                return dao.GetComments(pagename);
            }
        }

        public Comment GetComment(Guid id)
        {
            using (var dao = GetCommentDAO())
            {
                return dao.GetComment(id);
            }
        }

        public Comment SaveComment(Comment comment)
        {
            using (var dao = GetCommentDAO())
            {
                return dao.SaveComment(comment);
            }
        }

        public Comment CreateComment(Comment comment)
        {
            if (String.IsNullOrEmpty(comment.PageName)) throw new ArgumentException(@"page name cannot be empty", "comment");
            if (String.IsNullOrEmpty(comment.Body)) throw new ArgumentException(@"comment content cannot be empty", "comment");
            if (comment.ParentId != Guid.Empty)
            {
                var parent = GetComment(comment.ParentId);
                if (parent == null) throw new ArgumentException(@"comment parent was not found", "comment");
            }

            comment.UserId = SecurityContext.CurrentAccount.ID;
            comment.Date = DateTime.UtcNow;

            var page = GetPage(comment.PageName);
            //WikiActivityPublisher.AddPageComment(page, comment);
            NotifyCommentCreated(page, comment);

            return SaveComment(comment);
        }

        public Comment UpdateComment(Comment comment)
        {
            var toUpdate = GetComment(comment.Id);

            if(toUpdate == null) throw new ItemNotFoundException("comment not found");
            if (String.IsNullOrEmpty(comment.Body)) throw new ArgumentException(@"comment content cannot be empty", "comment");

            toUpdate.Body = comment.Body;
            toUpdate.UserId = SecurityContext.CurrentAccount.ID;
            toUpdate.Date = DateTime.UtcNow;

            var page = GetPage(comment.PageName);
            WikiActivityPublisher.EditPageComment(page, comment);

            return SaveComment(comment);
        }

        /// <summary>
        /// Removes comment from database
        /// </summary>
        /// <param name="id">comment id</param>
        public void RemoveComment(Guid id)
        {
            using (var dao = GetCommentDAO())
            {
                dao.RemoveComment(id);
            }
        }

        /// <summary>
        /// Updates comment by setting inactive flag to 1
        /// </summary>
        /// <param name="id">comment id</param>
        public void DeleteComment(Guid id)
        {
            var comment = GetComment(id);
            var page = GetPage(comment.PageName);

            comment.Inactive = true;
            comment.Date = DateTime.UtcNow;
            comment.UserId = SecurityContext.CurrentAccount.ID;

            SaveComment(comment);
            WikiActivityPublisher.DeletePageComment(page, comment);
        }
        #endregion

        
        #region Files

        public List<File> GetFiles()
        {
            using (var dao = GetFileDAO())
            {
                return dao.GetFiles(null);
            }
        }

        public File GetFile(string name)
        {
            using (var dao = GetFileDAO())
            {
                return dao.GetFiles(new [] { name })
                    .FirstOrDefault();
            }
        }

        public List<File> FindFiles(string startwith)
        {
            using (var dao = GetFileDAO())
            {
                return dao.FindFiles(startwith);
            }
        }

        public File SaveFile(File file)
        {
            using (var dao = GetFileDAO())
            {
                var saved = dao.SaveFile(file);

                if (saved != null) WikiActivityPublisher.AddFile(file);

                return saved;
            }
        }

        public File CreateOrUpdateFile(File file)
        {
            return CreateOrUpdateFile(file, null);
        }

        public File CreateOrUpdateFile(File file, Stream stream)
        {
            if (String.IsNullOrEmpty(file.FileName)) throw new ArgumentException(@"name of file cannot be empty", "file");

            file.UserID = SecurityContext.CurrentAccount.ID;
            file.Date = DateTime.UtcNow;
            file.Version++;
            file.FileLocation = GetFileLocation(file.FileName);

            if (stream != null)
            {
                var store = StorageFactory.GetStorage(CoreContext.TenantManager.GetCurrentTenant().TenantId.ToString(), "wiki");
                store.Save(string.Empty, file.FileLocation, stream, file.FileName);
            }

            //WikiActivityPublisher.AddFile(file);

            return SaveFile(file);
        }

        public void RemoveFile(string name)
        {
            using (var dao = GetFileDAO())
            {
                dao.RemoveFile(name);
            }
        }

        public static string GetFileLocation(string name)
        {
            var letter = (byte) name[0];

            var second = letter.ToString("x");
            var first = second.Substring(0, 1);

            var location = Path.Combine(first, second);
            location = Path.Combine(location, name);

            return location;
        }
        #endregion

        
        #region Pages

        public List<Page> GetPages()
        {
            using (var dao = GetPageDAO())
            {
                return RemoveSpecialPages(dao.GetPages());
            }
        }

        public List<Page> GetPages(string category)
        {
            using (var dao = GetPageDAO())
            {
                if (String.IsNullOrEmpty(category)) throw new ArgumentException(@"category cannot be empty", "category");
                return RemoveSpecialPages(dao.GetPages(category));
            }
        }

        public List<Page> GetPages(Guid userID)
        {
            using (var dao = GetPageDAO())
            {
                return RemoveSpecialPages(dao.GetPages(userID));
            }
        }

        public List<Page> GetRecentEditedPages(int maxCount)
        {
            using (var dao = GetPageDAO())
            {
                return RemoveSpecialPages(dao.GetRecentEditedPages(maxCount));
            }
        }

        public List<Page> GetNewPages(int maxCount)
        {
            using (var dao = GetPageDAO())
            {
                return RemoveSpecialPages(dao.GetNewPages(maxCount));
            }
        }

        public int GetPagesCount(Guid userID)
        {
            using (var dao = GetPageDAO())
            {
                return dao.GetPagesCount(userID);
            }
        }

        public List<string> GetPagesAndFiles(string body)
        {
            var mainOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
            var rgxLinks = new Regex(@"\[([\s\S]*?)\]", mainOptions);

            var pages = new List<string>();
            var files = new List<string>();
            
            string sLink;
            LinkType lType;
            foreach (Match m in rgxLinks.Matches(body))
            {
                lType = CheckTheLink(m.Groups[1].Value, out sLink);
                if (lType == LinkType.Page && !pages.Exists(s => s.Equals(sLink)))
                    pages.Add(sLink);
                else if (lType == LinkType.File && !files.Exists(s => s.Equals(sLink)))
                    files.Add(sLink);
            }

            var result = new List<string>();

            if (pages.Count > 0)
            {
                using (var dao = GetPageDAO())
                {
                    foreach (var page in dao.GetPages(pages))
                    {
                        result.Add(page.PageName);
                    }
                }
            }
            if (files.Count > 0)
            {
                using (var dao = GetFileDAO())
                {
                    foreach (var file in dao.GetFiles(files))
                    {
                        result.Add(file.FileName);
                    }
                }                
            }

            return result;
        }

        public List<Page> SearchPagesByStartName(string startName)
        {
            using (var dao = GetPageDAO())
            {
                return String.IsNullOrEmpty(startName)
                    ? new List<Page>()
                    : RemoveSpecialPages(dao.SearchPagesByName(startName, true));
            }
        }

        public List<Page> SearchPagesByName(string name)
        {
            using (var dao = GetPageDAO())
            {
                return String.IsNullOrEmpty(name)
                    ? new List<Page>() 
                    : RemoveSpecialPages(dao.SearchPagesByName(name, false));
            }
        }

        public List<Page> SearchPagesByContent(string content)
        {
            using (var dao = GetPageDAO())
            {
                return String.IsNullOrEmpty(content)
                ? new List<Page>()
                : dao.SearchPagesByContent(content);
            }
        }
      
        public Page GetPage(string pageName)
        {
            using (var dao = GetPageDAO())
            {
                var page = dao.GetPage(pageName, 0);
                if (page == null)
                {
                    if (pageName == string.Empty)
                    {
                        page = new Page()
                        {
                            PageName = string.Empty,
                            Body = WikiUCResource.MainPage_DefaultBody
                        };
                    }
                    else if (pageName == WikiUCResource.HelpPageCaption)
                    {
                        page = new Page()
                        {
                            PageName = WikiUCResource.HelpPageCaption,
                            Body = WikiUCResource.HelpPage_DefaultBody
                        };
                    }
                }
                return page;
            }
        }

        public Page GetPage(string pageName, int version)
        {
            if (String.IsNullOrEmpty(pageName)) throw new ArgumentException(@"page name cannot be empty", "pageName");
            using (var dao = GetPageDAO())
            {
                return dao.GetPage(pageName, version);
            }
        }

        public List<Page> GetPageHistory(string pageName)
        {
            using (var dao = GetPageDAO())
            {
                //NOTE: Page can be empty string!
                //if (String.IsNullOrEmpty(pageName)) throw new ArgumentException(@"page name cannot be empty", "pageName");
                return dao.GetPageHistory(pageName);
            }
        }

        public int GetPageMaxVersion(string pageName)
        {
            using (var dao = GetPageDAO())
            {
                return dao.GetPageMaxVersion(pageName);
            }
        }

        public Page SavePage(Page page)
        {
            using (var dao = GetPageDAO())
            {
                var saved = dao.SavePage(page);

                if (saved != null)
                {
                    if(saved.Version == 1) NotifyPageCreated(saved);
                    else NotifyPageEdited(saved);
                }

                return saved;
            }  
        }

        public Page CreatePage(Page page)
        {
            if (String.IsNullOrEmpty(page.PageName)) throw new ArgumentException(@"page name cannot be empty", "page");
            if (String.IsNullOrEmpty(page.Body)) throw new ArgumentException(@"page content cannot be empty", "page");

            page.UserID = SecurityContext.CurrentAccount.ID;
            page.Version = 1;
            page.Date = DateTime.UtcNow;

            page = SavePage(page);
            UpdateCategoriesByPageContent(page);

            //NotifyPageCreated(page);

            return page;
        }

        public Page UpdatePage(Page page)
        {
            var toUpdate = GetPage(page.PageName);

            if (toUpdate == null) throw new ItemNotFoundException("page not found");
            if(String.IsNullOrEmpty(page.Body)) throw new ArgumentException(@"page content cannot be empty", "page");

            toUpdate.UserID = SecurityContext.CurrentAccount.ID;
            toUpdate.Body = page.Body;
            toUpdate.Version++;
            toUpdate.Date = DateTime.UtcNow;

            toUpdate = SavePage(toUpdate);
            UpdateCategoriesByPageContent(toUpdate);

            //NotifyPageEdited(toUpdate);

            return toUpdate;
        }

        public void RemovePage(string pageName)
        {
            if (String.IsNullOrEmpty(pageName)) throw new ArgumentException(@"page name cannot be empty", "pageName");
            using (var dao = GetPageDAO())
            {
                dao.RevomePage(pageName);
            }
        }


        private List<Page> RemoveSpecialPages(List<Page> pages)
        {
            return pages
                .Where(p => !BaseUserControl.reservedPrefixes.Any(pref => p.PageName.StartsWith(pref + ":", StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
        }

        #endregion

        #region Notification

        private void NotifyPageCreated(Page page)
        {
            WikiNotifyClient.SendNoticeAsync(
                SecurityContext.CurrentAccount.ID.ToString(), 
                Constants.NewPage,
                null,
                null,
                GetNotifyTags(page.PageName));
            WikiActivityPublisher.AddPage(page);
        }

        private void NotifyPageEdited(Page page)
        {
            WikiNotifyClient.SendNoticeAsync(
                SecurityContext.CurrentAccount.ID.ToString(),
                Constants.EditPage,
                PageNameUtil.Encode(page.PageName),
                null,
                GetNotifyTags(page.PageName, "edit wiki page", null));
            WikiActivityPublisher.EditPage(page);
        }

        private void NotifyCategoryAdded(string category, string page)
        {
            WikiNotifyClient.SendNoticeAsync(
                SecurityContext.CurrentAccount.ID.ToString(),
                Constants.AddPageToCat,
                category,
                null,
                GetCategoryNotifyTags(category, page));
        }

        private void NotifyCommentCreated(Page page, Comment comment)
        {
            WikiNotifyClient.SendNoticeAsync(
                SecurityContext.CurrentAccount.ID.ToString(),
                Constants.EditPage,
                PageNameUtil.Encode(page.PageName),
                null,
                GetNotifyTags(page.PageName, "new wiki page comment", comment));
            WikiActivityPublisher.AddPageComment(page, comment);
        }

        private ITagValue[] GetNotifyTags(string pageName)
        {
            return GetNotifyTags(pageName, null, null);
        }

        private ITagValue[] GetNotifyTags(string pageName, string patternType, Comment comment)
        {
            var page = GetPage(pageName);
            if (page == null) return null;

            var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            var defPage = VirtualPathUtility.ToAbsolute(WikiManager.ViewVirtualPath);

            var tags = new List<ITagValue>
                {
                    new TagValue(Constants.TagPageName, String.IsNullOrEmpty(page.PageName) ? WikiResource.MainWikiCaption : page.PageName),
                    new TagValue(Constants.TagURL, CommonLinkUtility.GetFullAbsolutePath(ActionHelper.GetViewPagePath(defPage, page.PageName))),
                    new TagValue(Constants.TagUserName, user.DisplayUserName()),
                    new TagValue(Constants.TagUserURL, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(user.ID, CommonLinkUtility.GetProductID()))),
                    new TagValue(Constants.TagDate, TenantUtil.DateTimeNow()),
                    new TagValue(Constants.TagPostPreview, HtmlUtility.GetText(EditPage.ConvertWikiToHtml(page.PageName, page.Body, defPage, WikiSection.Section.ImageHangler.UrlFormat, CoreContext.TenantManager.GetCurrentTenant().TenantId), 120)),
                    
                };
            if (comment != null && !string.IsNullOrEmpty(pageName))
                ReplyToTagProvider.Comment("wiki", pageName, comment.Id.ToString());

            if (!string.IsNullOrEmpty(patternType))
            {
                tags.Add(new TagValue(Constants.TagChangePageType, patternType));
            }
            if (comment != null)
            {
                tags.Add(new TagValue(Constants.TagCommentBody, comment.Body));
            }

            return tags.ToArray();
        }

        private ITagValue[] GetCategoryNotifyTags(string objectId, string pageName)
        {
            var page = GetPage(pageName);
            if (page == null) return null;

            var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            var defPageHref = VirtualPathUtility.ToAbsolute(WikiManager.ViewVirtualPath);
            
            var tags = new List<ITagValue>
            {
                new TagValue(Constants.TagPageName, page.PageName),
                new TagValue(Constants.TagURL, CommonLinkUtility.GetFullAbsolutePath(ActionHelper.GetViewPagePath(defPageHref, page.PageName))),
                new TagValue(Constants.TagUserName, user.DisplayUserName()),
                new TagValue(Constants.TagUserURL, CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetUserProfile(user.ID, CommonLinkUtility.GetProductID()))),
                new TagValue(Constants.TagDate, TenantUtil.DateTimeNow()),
                new TagValue(Constants.TagPostPreview, HtmlUtility.GetText(EditPage.ConvertWikiToHtml(page.PageName, page.Body, defPageHref, WikiSection.Section.ImageHangler.UrlFormat, CoreContext.TenantManager.GetCurrentTenant().TenantId), 120)),
                new TagValue(Constants.TagCatName, objectId),
                ReplyToTagProvider.Comment("wiki", pageName)
            };

            return tags.ToArray();
        }

        #endregion

        private CategoryDao GetCategoryDAO()
        {
            return new CategoryDao(connectionStringName, CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }

        private CommentDao GetCommentDAO()
        {
            return new CommentDao(connectionStringName, CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }

        private FileDAO GetFileDAO()
        {
            return new FileDAO(connectionStringName, CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }

        private PageDAO GetPageDAO()
        {
            return new PageDAO(connectionStringName, CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }


        private LinkType CheckTheLink(string str, out string sLink)
        {
            sLink = string.Empty;

            if (string.IsNullOrEmpty(str))
                return LinkType.None;

            if (str[0] == '[')
            {
                sLink = str.Trim("[]".ToCharArray()).Split('|')[0].Trim();
            }
            else if (str.StartsWith("image:", StringComparison.InvariantCultureIgnoreCase) || str.StartsWith("file:", StringComparison.InvariantCultureIgnoreCase))
            {
                sLink = str.Split('|')[0].Trim();
            }
            sLink = sLink.Split('#')[0].Trim(); //Trim anchors
            if (string.IsNullOrEmpty(str))
                return LinkType.None;

            if (sLink.Contains(":"))
            {
                if ((sLink.StartsWith("image:", StringComparison.InvariantCultureIgnoreCase) ||
                                        sLink.StartsWith("file:", StringComparison.InvariantCultureIgnoreCase)))
                {
                    sLink = sLink.Split(':')[1];
                    return LinkType.File;
                }
                else
                {
                    if (HtmlWikiUtil.IsSpetialExists(sLink))
                    {
                        sLink = string.Empty;
                        return LinkType.None;
                    }
                }
            }

            return LinkType.Page;
        }

        private enum LinkType
        {
            None = 0,
            Page,
            File
        }
    }
}