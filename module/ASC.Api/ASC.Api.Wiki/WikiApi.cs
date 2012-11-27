using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using ASC.Api.Attributes;
using ASC.Api.Exceptions;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Api.Utils;
using ASC.Api.Wiki.Wrappers;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Data;
using File = ASC.Web.UserControls.Wiki.Data.File;

namespace ASC.Api.Wiki
{
    /// <summary>
    /// Wiki access
    /// </summary>
    public class WikiApi : IApiEntryPoint
    {
        public string Name
        {
            get { return "wiki"; }
        }

        private readonly ApiContext _apiContext;
        private readonly WikiEngine _engine;       

        public WikiApi(ApiContext apiContext)
        {
            _apiContext = apiContext;
            _engine = new WikiEngine();
        }

        /// <summary>
        /// Creates a new wiki page with the page name and content specified in the request
        /// </summary>
        /// <short>Create page</short>
        /// <param name="name">page name</param>
        /// <param name="body">page content</param>
        /// <returns>page info</returns>
        [Create("")]
        public PageWrapper CreatePage(string name, string body)
        {
            return new PageWrapper(_engine.CreatePage(new Page {PageName = name, Body = body}));
        }

        /// <summary>
        /// Returns the list of all pages in wiki or pages in wiki category specified in the request
        /// </summary>
        /// <short>Pages</short>
        /// <section>Pages</section>
        /// <param name="category">category name</param>
        /// <returns></returns>
        [Read("")]
        public IEnumerable<PageWrapper> GetPages(string category)
        {
            return category == null 
                ? _engine.GetPages().ConvertAll(x => new PageWrapper(x)) 
                : _engine.GetPages(category).ConvertAll(x => new PageWrapper(x));
        }

        /// <summary>
        /// Return the detailed information about the wiki page with the name and version specified in the request
        /// </summary>
        /// <short>Page</short>
        /// <section>Pages</section>
        /// <param name="name">page name</param>
        /// <param name="version">page version</param>
        /// <returns>page info</returns>
        [Read("{name}")]
        public PageWrapper GetPage(string name, int? version)
        {
            return version != null ? new PageWrapper(_engine.GetPage(name, (int)version)) : new PageWrapper(_engine.GetPage(name));
        }
        
        /// <summary>
		  /// Returns the list of history changes for the wiki page with the name specified in the request
        /// </summary>
        /// <short>History</short>
        /// <section>Pages</section>
        /// <param name="page">page name</param>
        /// <returns>list of pages</returns>
        [Read("{page}/story")]
        public IEnumerable<PageWrapper> GetHistory(string page)
        {
            return _engine.GetPageHistory(page).ConvertAll(x => new PageWrapper(x));
        }

        /// <summary>
        /// Returns the list of wiki pages with the name matching the search query specified in the request
        /// </summary>
        /// <short>Search</short>
        /// <section>Pages</section>
        /// <param name="name">part of the page name</param>
        /// <returns>list of pages</returns>
        [Read("search/byname")]
        public IEnumerable<PageWrapper> SearchPages(string name)
        {
            return _engine.SearchPagesByName(name).ConvertAll(x => new PageWrapper(x));
        }

        /// <summary>
		  /// Returns the list of wiki pages with the content matching the search query specified in the request
		  /// </summary>
        /// <short>Search</short>
        /// <section>Pages</section>
        /// <param name="content">part of the page content</param>
        /// <returns>list of pages</returns>
        [Read("search/bycontent")]
        public IEnumerable<PageWrapper> SearchPagesByContent(string content)
        {
            return _engine.SearchPagesByContent(content).ConvertAll(x => new PageWrapper(x));
        }

        /// <summary>
        /// Updates the wiki page with the name and content specified in the request
        /// </summary>
        /// <short>Update page</short>
        /// <section>Pages</section>
        /// <param name="name">page name</param>
        /// <param name="body">page content</param>
        /// <returns>page info</returns>
        [Update("{name}")]
        public PageWrapper UpdatePage(string name, string body)
        {
            return new PageWrapper(_engine.SavePage(new Page {PageName = name, Body = body}));
        }

        /// <summary>
        /// Deletes the wiki page with the name specified in the request
        /// </summary>
        /// <short>Delete page</short>
        /// <section>Pages</section>
        /// <param name="name">page name</param>
        [Delete("{name}")]
        public void DeletePage(string name)
        {
            _engine.RemovePage(name);
        }

        /// <summary>
        /// Creates the comment to the selected wiki page with the content specified in the request
        /// </summary>
        /// <short>Create comment</short>
        /// <section>Comments</section>
        /// <param name="page">page name</param>
        /// <param name="content">comment content</param>
        /// <param name="parentId">comment parent id</param>
        /// <returns>comment info</returns>
        [Create("{page}/comment")]
        public CommentWrapper CreateComment(string page, string content, string parentId)
        {
            var parentIdGuid = String.IsNullOrEmpty(parentId) ? Guid.Empty : new Guid(parentId);
            return new CommentWrapper(_engine.CreateComment(new Comment {PageName = page, Body = content, ParentId = parentIdGuid}));
        }

        /// <summary>
		  /// Returns the list of all comments to the wiki page with the name specified in the request
        /// </summary>
        /// <short>All comments</short>
        /// <section>Comments</section>
        /// <param name="page">page name</param>
        /// <returns>list of comments</returns>
        [Read("{page}/comment")]
        public List<CommentWrapper> GetComments(string page)
        {
            return _engine.GetComments(page).ConvertAll(x => new CommentWrapper(x));
        }

        /// <summary>
		  /// Updates the comment to the selected wiki page with the comment content specified in the request
        /// </summary>
        /// <short>Update comment</short>
        /// <section>Comments</section>
        /// <param name="id">comment id</param>
        /// <param name="body">comment content</param>
        /// <returns>comment info</returns>
        [Update("comment/{id}")]
        public CommentWrapper UpdateComment(string id, string body)
        {
            return new CommentWrapper(_engine.UpdateComment(new Comment {Id = new Guid(id), Body = body}));
        }

        /// <summary>
        /// Deletes the comment with the ID specified in the request from the selected wiki page
        /// </summary>
        /// <short>Delete comment</short>
        /// <section>Comment</section>
        /// <param name="id">comment id</param>
        [Delete("comment/{id}")]
        public void DeleteComment(string id)
        {
            _engine.RemoveComment(new Guid(id));
        }

        /// <summary>
        /// Uploads the selected files to the wiki 'Files' section
        /// </summary>
        /// <short>Upload files</short>
        /// <param name="files">list of files to upload</param>
        /// <returns>list of files</returns>
        [Create("file")]
        public IEnumerable<FileWrapper> UploadFiles(IEnumerable<System.Web.HttpPostedFileBase> files)
        {
            if (files == null || !files.Any()) throw new ArgumentNullException("files");

            return files.Select(file => new FileWrapper(_engine.CreateOrUpdateFile(new File{FileName = file.FileName, FileSize = file.ContentLength, UploadFileName = file.FileName}, file.InputStream)));
        }

        /// <summary>
        /// Returns the detailed file info about the file with the specified name in the wiki 'Files' section
        /// </summary>
        /// <short>File</short>
        /// <section>Files</section>
        /// <param name="name">file name</param>
        /// <returns>file info</returns>
        [Read("file/{name}")]
        public FileWrapper GetFile(string name)
        {
            return new FileWrapper(_engine.GetFile(name));
        }

        /// <summary>
        /// Deletes the files with the specified name from the wiki 'Files' section
        /// </summary>
        /// <short>Delete file</short>
        /// <param name="name">file name</param>
        [Delete("file/{name}")]
        public void DeleteFile(string name)
        {
            _engine.RemoveFile(name);
        }
    }
}
