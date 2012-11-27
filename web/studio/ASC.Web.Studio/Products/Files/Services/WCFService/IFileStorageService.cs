using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using ASC.Files.Core;
using ASC.Web.Files.Import;
using File = ASC.Files.Core.File;

namespace ASC.Web.Files.Services.WCFService
{
    [ServiceContract]
    public interface IFileStorageService
    {
        #region Folder Manager

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.XMLPostSubFolders, Method = "POST")]
        ItemList<Folder> GetFolders(String parentId, OrderBy orderBy);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLGetFolder)]
        Folder GetFolder(String folderId);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLCreateFolder)]
        Folder CreateNewFolder(String title, String parentId);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLRenameFolder)]
        Folder FolderRename(String folderId, String title);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.XMLPostFolderItems, Method = "POST")]
        DataWrapper GetFolderItems(String parentId, String from, String count, String filter, OrderBy orderBy, String subjectID, String searchText, bool compactView);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostCheckMoveFiles, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemDictionary<String, String> MoveOrCopyFilesCheck(ItemList<String> items, String destFolderId);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostMoveItems, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> MoveOrCopyItems(ItemList<String> items, String destFolderId, String overwriteFiles, String isCopyOperation);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostDeleteItems, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> DeleteItems(ItemList<String> items);

        IEnumerable<Folder> GetFolderUpdates(DateTime from, DateTime to);

        #endregion


        #region File Manager

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLLastFileVersion)]
        File GetLastFileVersion(String fileId);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLCreateNewFile)]
        File CreateNewFile(String parentId, String fileTitle);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLRenameFile)]
        File FileRename(String fileId, String title);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLUpdateToVersion)]
        File UpdateToVersion(String fileId, String version);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.XMLGetFileHistory)]
        ItemList<File> GetFileHistory(String fileId);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostGetSiblingsFile, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemDictionary<String, String> GetSiblingsFile(String fileId, String filter, OrderBy orderBy, String subjectID, String searchText);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONTrackEditFile, ResponseFormat = WebMessageFormat.Json)]
        void TrackEditFile(String fileId, String docKeyForTrack, String shareLink, bool isFinish);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostCheckEditing, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemDictionary<String, String> CheckEditing(ItemList<String> filesId);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONGetCanEdit, Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        Boolean CanEdit(String fileId, String shareLink);

        IEnumerable<File> GetFileUpdates(DateTime from, DateTime to);

        #endregion


        #region Utils

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostBulkDownload, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> BulkDownload(ItemList<String> items);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetTasksStatuses, ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> GetTasksStatuses();

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONEmptyTrash, ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> EmptyTrash();

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONTerminateTasks, ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> TerminateTasks(bool import);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetShortenLink, ResponseFormat = WebMessageFormat.Json)]
        String GetShortenLink(String fileId, String longUrl, String docKey);

        #endregion


        #region Import

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.XMLPostGetImportDocs, Method = "POST")]
        ItemList<DataToImport> GetImportDocs(String source, AuthData authData);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostExecImportDocs, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> ExecImportDocs(String login, String password, String token, String source, String parentId, String ignoreCoincidenceFiles, List<DataToImport> dataToImport);

        #endregion


        #region Ace Manager

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONGetSharedInfo, Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        ItemList<AceWrapper> GetSharedInfo(String objectId);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostSetAceObject, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        bool SetAceObject(ItemList<AceWrapper> aceWrappers, String objectId, bool notify, String message);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostRemoveAce, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        Int32 RemoveAce(ItemList<String> items);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostMarkAsRead, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        int MarkAsRead(ItemList<String> items);

        #endregion

        #region ThirdParty

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.XMLPostSaveThirdParty, Method = "POST")]
        Folder SaveThirdParty(ThirdPartyParams thirdPartyParams);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONDeleteThirdParty, ResponseFormat = WebMessageFormat.Json)]
        void DeleteThirdParty(String folderId);

        #endregion

    }
}