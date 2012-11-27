using System;
using System.Collections.Generic;

namespace ASC.Files.Core
{
    public interface IFolderDao : IDisposable
    {

        /// <summary>
        ///     Get folder by id.
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <returns>folder</returns>
        Folder GetFolder(object folderId);

        /// <summary>
        ///     Returns the folder with the given name and id of the root
        /// </summary>
        /// <param name="title"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Folder GetFolder(String title, object parentId);

        /// <summary>
        ///    Gets the root folder
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <returns>root folder</returns>
        Folder GetRootFolder(object folderId);

        /// <summary>
        ///    Gets the root folder
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <returns>root folder</returns>
        Folder GetRootFolderByFile(object fileId);

        /// <summary>
        ///     Get a list of folders in current folder.
        /// </summary>
        List<Folder> GetFolders(object parentId);

        /// <summary>
        /// Get a list of folders.
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="orderBy"></param>
        /// <param name="filterType"></param>
        /// <param name="subjectID"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        List<Folder> GetFolders(object parentId, OrderBy orderBy, FilterType filterType, Guid subjectID, string searchText);

        /// <summary>
        /// Gets the folder (s) by ID (s)
        /// </summary>
        /// <param name="folderIds"></param>
        /// <returns></returns>
        List<Folder> GetFolders(object[] folderIds);

        /// <summary>
        ///     Get folder, contains folder with id
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <returns></returns>
        List<Folder> GetParentFolders(object folderId);

        /// <summary>
        ///     save or update folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        object SaveFolder(Folder folder);

        /// <summary>
        ///     delete folder
        /// </summary>
        /// <param name="folderId">folder id</param>
        void DeleteFolder(object folderId);

        /// <summary>
        ///  move folder
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <param name="toRootFolderId">destination folder id</param>
        void MoveFolder(object folderId, object toRootFolderId);

        /// <summary>
        ///     copy folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="toRootFolderId"></param>
        /// <returns> 
        /// </returns>
        object CopyFolder(object folderId, object toRootFolderId);

        /// <summary>
        /// Validate the transfer operation directory to another directory.
        /// </summary>
        /// <param name="folderIds"></param>
        /// <param name="to"></param>
        /// <returns>
        /// Returns pair of file ID, file name, in which the same name.
        /// </returns>
        IDictionary<object, string> CanMoveOrCopy(object[] folderIds, object to);

        /// <summary>
        ///     Rename folder
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <param name="newTitle">new name</param>
        object RenameFolder(object folderId, String newTitle);

        /// <summary>
        ///     Get files in folder
        /// </summary>
        /// <param name="parentId">folder id</param>
        /// <param name="orderBy"></param>
        /// <param name="subjectID"></param>
        /// <param name="filterType">filterType type</param>
        /// <param name="searchText"> </param>
        /// <returns>list of files</returns>
        /// <remarks>
        ///    Return only the latest versions of files of a folder
        /// </remarks>
        List<File> GetFiles(object parentId, OrderBy orderBy, FilterType filterType, Guid subjectID, string searchText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="withSubfolders"></param>
        /// <returns></returns>
        List<object> GetFiles(object parentId, bool withSubfolders);

        /// <summary>
        ///    Gets the number of files and folders to the container in your
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <param name="withSubfoldes"> </param>
        /// <returns></returns>
        int GetItemsCount(object folderId, bool withSubfoldes);

        /// <summary>
        /// Check the need to use the trash before removing
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        bool UseTrashForRemove(Folder folder);

        /// <summary>
        /// Check the need to use recursion for operations
        /// </summary>
        /// <param name="folderId"> </param>
        /// <param name="toRootFolderId"> </param>
        /// <returns></returns>
        bool UseRecursiveOperation(object folderId, object toRootFolderId);

        #region Only for TMFolderDao

        /// <summary>
        /// Search the list of folders containing text in title
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="text"></param>
        /// <param name="folderType"></param>
        /// <returns></returns>
        IEnumerable<Folder> Search(string text, FolderType folderType);

        /// <summary>
        /// Get folders created or updated during time interval
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        List<Folder> GetUpdates(DateTime from, DateTime to);

        /// <summary>
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="module"></param>
        /// <param name="bunch"></param>
        /// <param name="data"></param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        object GetFolderID(string module, string bunch, string data, bool createIfNotExists);

        /// <summary>
        ///  Returns id folder "Shared Documents"
        /// Only in TMFolderDao
        /// </summary>
        /// <returns></returns>
        object GetFolderIDCommon(bool createIfNotExists);

        /// <summary>
        ///  Returns id folder "My Documents"
        /// Only in TMFolderDao
        /// </summary>
        /// <returns></returns>
        object GetFolderIDUser(bool createIfNotExists);

        /// <summary>
        /// Returns id folder "Shared with me"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        object GetFolderIDShare(bool createIfNotExists);

        /// <summary>
        /// Returns id folder "Trash"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        object GetFolderIDTrash(bool createIfNotExists);

        #endregion
    }
}