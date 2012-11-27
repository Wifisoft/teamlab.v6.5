using System;

namespace ASC.Web.Files.Services.WCFService
{
    internal class UriTemplates
    {

        #region Folder Template

        public const String XMLCreateFolder = "folders/create?parentId={parentId}&title={title}";
        public const String XMLPostSubFolders = "folders/subfolders?parentId={parentId}";
        public const String XMLRenameFolder = "folders/rename?folderId={folderId}&title={title}";
        public const String XMLGetFolder = "folders/info?folderId={folderId}";

        public const String XMLPostFolderItems = "folders?parentId={parentId}&from={from}&count={count}&filter={filter}&subjectID={subjectID}&search={searchText}&compactView={compactView}";

        #endregion


        #region File Template

        public const String XMLCreateNewFile = "folders/files/createfile?parentId={parentId}&title={fileTitle}";
        public const String XMLRenameFile = "folders/files/rename?fileId={fileId}&title={title}";
        public const String XMLGetFileHistory = "folders/files/history?fileId={fileId}";
        public const String JSONPostDeleteItems = "folders/files?action=delete";
        public const String JSONPostGetSiblingsFile = "folders/files/siblings?fileId={fileId}&filter={filter}&subjectID={subjectID}&search={searchText}";
        public const String XMLUpdateToVersion = "folders/files/updateToVersion?fileId={fileId}&version={version}";
        public const String JSONPostCheckMoveFiles = "folders/files/moveOrCopyFilesCheck?destFolderId={destFolderId}";
        public const String XMLLastFileVersion = "folders/files/lastversion?fileId={fileId}";

        #endregion


        #region Utils Template

        public const String XMLPostGetImportDocs = "import?source={source}";
        public const String JSONPostExecImportDocs = "import/exec?login={login}&password={password}&token={token}&source={source}&tofolder={parentId}&ignoreCoincidenceFiles={ignoreCoincidenceFiles}";
        public const String JSONGetTasksStatuses = "tasks/statuses";
        public const String JSONPostBulkDownload = "bulkdownload";
        public const String JSONTerminateTasks = "tasks?terminate={import}";
        public const String JSONTrackEditFile = "trackeditfile?fileID={fileId}&docKeyForTrack={docKeyForTrack}&doc={shareLink}&isFinish={isFinish}";
        public const String JSONPostCheckEditing = "checkediting";
        public const String JSONPostMoveItems = "moveorcopy?destFolderId={destFolderId}&ow={overwriteFiles}&ic={isCopyOperation}";
        public const String JSONEmptyTrash = "emptytrash";
        public const String JSONGetShortenLink = "shorten?fileId={fileId}&longUrl={longUrl}&doc={docKey}";

        // ???
        public const String JSONGetCanEdit = "canedit?fileId={fileId}&doc={shareLink}";

        #endregion


        #region Ace Tempate

        public const String JSONGetSharedInfo = "sharedinfo?objectId={objectId}";
        public const String JSONPostSetAceObject = "setaceobject?objectId={objectId}&notify={notify}&message={message}";
        public const String JSONPostRemoveAce = "removeace";
        public const String JSONPostMarkAsRead = "markasread";

        #endregion


        #region ThirdParty

        public const String XMLPostSaveThirdParty = "thirdparty/save";
        public const String JSONDeleteThirdParty = "thirdparty/delete?folderId={folderId}";

        #endregion

    }
}