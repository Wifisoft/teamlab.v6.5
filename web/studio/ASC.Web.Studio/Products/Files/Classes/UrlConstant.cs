namespace ASC.Web.Files.Classes
{
    public static class UrlConstant
    {
        public const string Action = "action"; //CommonLinkUtility
        public const string Version = "version"; //CommonLinkUtility
        public const string FileId = "fileID"; //CommonLinkUtility

        public const string Search = "search";
        public const string FolderId = "folderID";
        public const string FileTitle = "fileTitle";
        public const string DownloadTitle = "download";
        public const string Error = "error";
        public const string AuthKey = "asc_auth_key";
        public const string ActivityType = "activityType";
        public const string FileUri = "fileUri";
        public const string New = "new";
        public const string ProjectId = "prjID";
        public const string DocUrlKey = "doc";
        public const string OutType = "outputtype";
        public const string Template = "template";

        public const string ParamsUpload = "?action=upload&folderID={0}&fileTitle={1}";

        public const string ParamsSave = "?action=save&fileID={0}&version={1}&fileUri={2}";
    }
}