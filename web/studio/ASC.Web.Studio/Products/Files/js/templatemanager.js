window.ASC.Files.TemplateManager = (function() {
    var isInit = false,
        templatesDirPath = "",
        tempatesHandlerPath = "",
        xslTemplates = { },
        supportedCustomEvents = {
            CreateNewFile: "createnewfile",
            CreateFolder: "createfolder",

            CheckEditing: "checkediting",

            GetTreeSubFolders: "gettreesubfolders",
            GetFolderInfo: "getfolderinfo",
            GetFolderItems: "getfolderitems",

            GetSharedInfo: "getsharedinfo",
            SetAceObject: "setaceobject",
            UnSubscribeMe: "unsubscribeme",
            GetShortenLink: "getshortenlink",

            MarkAsRead: "markasread",

            FolderRename: "folderrename",
            FileRename: "filerename",

            DeleteItem: "deleteitem",
            EmptyTrash: "emptytrash",

            GetFileHistory: "getfilehistory",
            ReplaceVersion: "replaceversion",
            SetCurrentVersion: "setcurrentversion",

            IsZohoAuthentificated: "iszohoauthentificated",
            GetImportData: "getImportData",
            ExecImportData: "execImportData",

            Download: "download",
            GetTasksStatuses: "getTasksStatuses",
            TerminateTasks: "terminatetasks",

            MoveFilesCheck: "movefilescheck",
            MoveItems: "moveitems",

            GetSiblingsFile: "getsiblingsfile",

            SaveThirdParty: "savethirdparty",
            DeleteThirdParty: "deletethirdparty"
        },
        xslTemplatesName = {
            getFileHistory: "getfilehistory",
            getFoldersTree: "getfolderstree",
            getFolderInfo: "getfolderinfo",
            getFolderItems: "getfolderitems",
            getFolderItem: "getfolderitem",
            getImportData: "getimportdata",
            getTooltip: "gettooltip"
        };

    var init = function(templatesHandler, templatesDir) {
        if (isInit === false) {
            isInit = true;

            templatesDirPath = templatesDir;
            tempatesHandlerPath = templatesHandler;
        }
    };

    var getTemplate = function(name) {
        if (typeof name !== "string" || name.length === 0) {
            return undefined;
        }
        if (xslTemplates.hasOwnProperty(name)) {
            return xslTemplates[name];
        }
        var xslTemplate = ASC.Controls.XSLTManager.loadXML(tempatesHandlerPath + "?id=" + templatesDirPath + "&name=" + name);
        if (xslTemplate && typeof xslTemplate === "object") {
            xslTemplates[name] = xslTemplate;
            return xslTemplates[name];
        }
        return undefined;
    };

    return {
        events: supportedCustomEvents,
        templates: xslTemplatesName,

        getTemplate: getTemplate,

        init: init
    };
})();