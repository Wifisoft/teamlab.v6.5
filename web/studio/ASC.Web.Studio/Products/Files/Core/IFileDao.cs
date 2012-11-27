using System;
using System.Collections.Generic;
using System.IO;

namespace ASC.Files.Core
{
    /// <summary>
    ///    Interface encapsulates access toFolderId files
    /// </summary>
    public interface IFileDao : IDisposable
    {

        /// <summary>
        ///     Receive file
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <returns></returns>
        File GetFile(object fileId);

        /// <summary>
        ///     Receive file
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <param name="fileVersion">file version</param>
        /// <returns></returns>
        File GetFile(object fileId, int fileVersion);

        /// <summary>
        ///     Receive file
        /// </summary>
        /// <param name="parentId">folder id</param>
        /// <param name="title">file name</param>
        /// <returns>
        ///   file
        /// </returns>
        File GetFile(object parentId, String title);

        /// <summary>
        ///  Returns all versions of the file
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        List<File> GetFileHistory(object fileId);

        /// <summary>
        ///     Gets the file (s) by ID (s)
        /// </summary>
        /// <param name="fileIds">id file</param>
        /// <returns></returns>
        List<File> GetFiles(object[] fileIds);

        /// <summary>
        /// Get stream of file
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Stream</returns>
        Stream GetFileStream(File file);

        /// <summary>
        ///  Saves / updates the version of the file
        ///  and save stream of file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileStream"> </param>
        /// <returns></returns>
        /// <remarks>
        /// Updates the file if:
        /// - The file comes with the given id
        /// - The file with that name in the folder / container exists
        ///
        /// Save in all other cases
        /// </remarks>
        File SaveFile(File file, Stream fileStream);

        /// <summary>
        ///   Deletes a file including all previous versions
        /// </summary>
        /// <param name="fileId">file id</param>
        void DeleteFile(object fileId);

        /// <summary>
        ///     Checks whether or not file
        /// </summary>
        /// <param name="title">file name</param>
        /// <param name="folderId">folder id</param>
        /// <returns>Returns true if the file exists, otherwise false</returns>
        bool IsExist(String title, object folderId);

        /// <summary>
        ///   Moves a file or set of files in a folder
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <param name="toFolderId">The ID of the destination folder</param>
        void MoveFile(object fileId, object toFolderId);

        /// <summary>
        ///  Copy the files in a folder
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <param name="toFolderId">The ID of the destination folder</param>
        File CopyFile(object fileId, object toFolderId);

        /// <summary>
        ///   Rename file
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <param name="newTitle">new name</param>
        object FileRename(object fileId, String newTitle);

        /// <summary>
        /// Check the need to use the trash before removing
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool UseTrashForRemove(File file);

        #region Only in TMFileDao

        /// <summary>
        /// Search the list of files containing text
        /// Only in TMFileDao
        /// </summary>
        /// <param name="text">search text</param>
        /// <param name="folderType">type of parent folder</param>
        /// <returns>list of files</returns>
        IEnumerable<File> Search(String text, FolderType folderType);

        /// <summary>
        /// Get files created or updated during time interval
        /// Only in TMFileDao
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        List<File> GetUpdates(DateTime from, DateTime to);

        /// <summary>
        /// Delete streama of file
        /// Only in TMFileDao
        /// </summary>
        /// <param name="fileId"></param>
        void DeleteFileStream(object fileId);

        /// <summary>
        /// Delete parent folder on storage
        /// Only in TMFileDao
        /// </summary>
        /// <param name="fileId"></param>
        void DeleteFolder(object fileId);

        /// <summary>
        ///   Checks whether file exists on storage
        /// </summary>
        /// <param name="file">file</param>
        /// <returns></returns>
        bool IsExistOnStorage(File file);

        #endregion
    }
}