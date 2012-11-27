using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using ASC.Common.Data;
using ASC.Common.Data.Sql;

namespace ASC.Web.Studio.Utility
{
    public enum FileType
    {
        Unknown = 0,
        Archive = 1,
        Video = 2,
        Audio = 3,
        Image = 4,
        Spreadsheet = 5,
        Presentation = 6,
        Document = 7
    }

    public static class FileUtility
    {
        private static readonly IEqualityComparer<string> Comparer = StringComparer.CurrentCultureIgnoreCase;

        static FileUtility()
        {
            EnableHtml5 = (WebConfigurationManager.AppSettings["files.docservice.html5"] ?? "false") == "true"
                          && !string.IsNullOrEmpty(CommonLinkUtility.DocServiceApiUrl);

            if (!string.IsNullOrEmpty(
                WebConfigurationManager.AppSettings["files.docservice.url.doceditor"]
                + WebConfigurationManager.AppSettings["files.docservice.url.spreditor"]
                + WebConfigurationManager.AppSettings["files.docservice.url.presenteditor"]
                + WebConfigurationManager.AppSettings["files.docservice.url.draweditor"]))
            {
                ExtsWebEdited = (WebConfigurationManager.AppSettings["files.docservice.edited-docs"] ?? "").Split(new char[] {'|', ','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            if (!string.IsNullOrEmpty(
                WebConfigurationManager.AppSettings["files.docservice.url.docviewer"]
                + WebConfigurationManager.AppSettings["files.docservice.url.sprviewer"]
                + WebConfigurationManager.AppSettings["files.docservice.url.presentviewer"]
                + WebConfigurationManager.AppSettings["files.docservice.url.drawviewer"]))
            {
                ExtsWebPreviewed = (WebConfigurationManager.AppSettings["files.docservice.viewed-docs"] ?? "").Split(new char[] {'|', ','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            ExtsImagePreviewed = (WebConfigurationManager.AppSettings["files.viewed-images"] ?? "").Split(new char[] {'|', ','}, StringSplitOptions.RemoveEmptyEntries).ToList();


            #region convert extensions

            const string databaseId = "files";
            const string tableTitle = "files_converts";

            var dbManager = new DbManager(databaseId);
            var sqlQuery = new SqlQuery(tableTitle).Select("input", "output");

            var list = dbManager.ExecuteList(sqlQuery);

            list.ForEach(item =>
                             {
                                 var input = item[0] as string;
                                 var output = item[1] as string;
                                 if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(output))
                                     return;
                                 input = input.ToLower().Trim();
                                 output = output.ToLower().Trim();
                                 if (!ExtConvertible.ContainsKey(input))
                                     ExtConvertible[input] = new List<string>();
                                 ExtConvertible[input].Add(output);
                             });

            #endregion
        }

        #region method

        public static string GetFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            var startIndex = fileName.LastIndexOf('\\');
            fileName = startIndex > 0 ? fileName.Substring(startIndex + 1) : fileName;
            return fileName.Substring(0, fileName.Length - GetFileExtension(fileName).Length);
        }

        public static string GetFileExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            var pos = fileName.LastIndexOf('.');
            return 0 <= pos ? fileName.Substring(pos).Trim().ToLower() : string.Empty;
        }

        public static FileType GetFileTypeByFileName(string name)
        {
            return GetFileTypeByExtention(GetFileExtension(name));
        }

        public static FileType GetFileTypeByExtention(string ext)
        {
            ext = ext.ToLower();

            if (ArchiveExts.Contains(ext)) return FileType.Archive;
            if (VideoExts.Contains(ext)) return FileType.Video;
            if (AudioExts.Contains(ext)) return FileType.Audio;
            if (ImageExts.Contains(ext)) return FileType.Image;
            if (SpreadsheetExts.Contains(ext)) return FileType.Spreadsheet;
            if (PresentationExts.Contains(ext)) return FileType.Presentation;
            if (DocumentExts.Contains(ext)) return FileType.Document;

            return FileType.Unknown;
        }

        public static string GetFileUtilityJScript()
        {
            var serializer = new JavaScriptSerializer();
            var str = new System.Text.StringBuilder();
            str.Append("<script type='text/javascript' language='javascript'>");
            str.Append("if (typeof ASC === 'undefined') ASC = { }; if (typeof ASC.Files === 'undefined') ASC.Files = (function() { return { }; })(); if (typeof ASC.Files.Utility === 'undefined') ASC.Files.Utility = { };");

            str.AppendFormat(
                @"
ASC.Files.Utility.CanWebEditBrowser = jq.browser.msie && jq.browser.versionCorrect >= 9 || jq.browser.safari && jq.browser.versionCorrect >= 5 || jq.browser.mozilla && jq.browser.versionCorrect >= 4 || jq.browser.chrome && jq.browser.versionCorrect >= 7 || jq.browser.opera && jq.browser.versionCorrect >= 10.5;
ASC.Files.Utility.GetFileExtension = function(fileTitle){{ if (typeof fileTitle == 'undefined' || fileTitle == null) return ''; fileTitle = fileTitle.trim(); var posExt = fileTitle.lastIndexOf('.'); return 0 <= posExt ? fileTitle.substring(posExt).trim().toLowerCase() : ''; }};

ASC.Files.Utility.GetFileViewUrl = function(fileId, fileVersion){{ var url = '{1}'.format(encodeURIComponent(fileId)); if (fileVersion) return url + '&{0}=' + fileVersion; return url;}};
ASC.Files.Utility.GetFileDownloadUrl = function(fileId, fileVersion){{ var url = '{2}'.format(encodeURIComponent(fileId)); if (fileVersion) return url + '&{0}=' + fileVersion; return url;}};

ASC.Files.Utility.GetFileWebViewerUrl = function(fileId, fileVersion){{ var url = '{3}'.format(encodeURIComponent(fileId)); if (fileVersion) return url + '&{0}=' + fileVersion; return url;}};
ASC.Files.Utility.GetFileWebEditorUrl = function(fileId){{return '{4}'.format(encodeURIComponent(fileId));}}

ASC.Files.Utility.CanImageView = function(fileTitle) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {5}) != -1; }};
ASC.Files.Utility.CanWebView = function(fileTitle) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {6}) != -1; }};
ASC.Files.Utility.CanWebEdit = function(fileTitle, withoutMobileDetect) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {7}) != -1 && (!jq.browser.mobile || withoutMobileDetect === true) && ({16} !== true || ASC.Files.Utility.CanWebEditBrowser || !ASC.Files.Utility.FileIsDocument(fileTitle)); }};

ASC.Files.Utility.FileIsArchive = function(fileTitle) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {8}) != -1; }};
ASC.Files.Utility.FileIsVideo = function(fileTitle) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {9}) != -1; }};
ASC.Files.Utility.FileIsAudio = function(fileTitle) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {10}) != -1; }};
ASC.Files.Utility.FileIsImage = function(fileTitle) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {11}) != -1; }};
ASC.Files.Utility.FileIsSpreadsheet = function(fileTitle) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {12}) != -1; }};
ASC.Files.Utility.FileIsPresentation = function(fileTitle) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {13}) != -1; }};
ASC.Files.Utility.FileIsDocument = function(fileTitle) {{ return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), {14}) != -1; }};

ASC.Files.Utility.GetConvertFormats = function(fileTitle) {{ var convertTable = []; eval('convertTable=' + '{15}'); var curExt = ASC.Files.Utility.GetFileExtension(fileTitle); return eval('convertTable' + curExt); }};
",
                CommonLinkUtility.Version,
                CommonLinkUtility.FileViewUrlString,
                CommonLinkUtility.FileDownloadUrlString,
                CommonLinkUtility.FileWebViewerUrlString,
                CommonLinkUtility.FileWebEditorUrlString,
                serializer.Serialize(ExtsImagePreviewed),
                serializer.Serialize(ExtsWebPreviewed),
                serializer.Serialize(ExtsWebEdited),
                serializer.Serialize(ArchiveExts),
                serializer.Serialize(VideoExts),
                serializer.Serialize(AudioExts),
                serializer.Serialize(ImageExts),
                serializer.Serialize(SpreadsheetExts),
                serializer.Serialize(PresentationExts),
                serializer.Serialize(DocumentExts),
                serializer.Serialize(ExtConvertible),
                EnableHtml5.ToString().ToLower()
                );

            str.Append("</script>");

            return str.ToString();
        }

        public static bool UsingHtml5(string fileTitle)
        {
            return UsingHtml5(fileTitle, true);
        }

        public static bool UsingHtml5(string fileTitle, bool forEdit)
        {
            var fileExt = GetFileExtension(fileTitle);
            return
                EnableHtml5
                && (DocumentExts.Contains(fileExt, Comparer)
                    || (!forEdit
                        && (SpreadsheetExts.Contains(fileExt, Comparer) || PresentationExts.Contains(fileExt, Comparer))));
        }

        #endregion

        #region member

        public static bool EnableHtml5 { get; private set; }

        public static readonly Dictionary<string, List<string>> ExtConvertible = new Dictionary<string, List<string>>();

        public static readonly List<string> ExtsImagePreviewed = new List<string>();

        public static readonly List<string> ExtsWebPreviewed = new List<string>();

        public static readonly List<string> ExtsWebEdited = new List<string>();

        public static readonly List<string> ArchiveExts = new List<string>
                                                              {
                                                                  ".zip", ".rar", ".ace", ".arc", ".arj",
                                                                  ".bh", ".cab", ".enc", ".gz", ".ha",
                                                                  ".jar", ".lha", ".lzh", ".pak", ".pk3",
                                                                  ".tar", ".tgz", ".uu", ".uue", ".xxe",
                                                                  ".z", ".zoo"
                                                              };

        public static readonly List<string> VideoExts = new List<string>
                                                            {
                                                                ".avi", ".mpg", ".mkv", ".mp4",
                                                                ".mov", ".3gp", ".vob", ".m2ts"
                                                            };

        public static readonly List<string> AudioExts = new List<string>
                                                            {
                                                                ".wav", ".mp3", ".wma", ".ogg"
                                                            };

        public static readonly List<string> ImageExts = new List<string>
                                                            {
                                                                ".bmp", ".cod", ".gif", ".ief", ".jpe", ".jpeg", ".jpg",
                                                                ".jfif", ".tiff", ".tif", ".cmx", ".ico", ".pnm", ".pbm",
                                                                ".png", ".ppm", ".rgb", ".svg", ".xbm", ".xpm", ".xwd",
                                                                ".svgt"                                                                
                                                            };

        public static readonly List<string> SpreadsheetExts = new List<string>
                                                                  {
                                                                      ".xls", ".xlsx",
                                                                      ".ods", ".csv",
                                                                      ".xlst"                                                                      
                                                                  };

        public static readonly List<string> PresentationExts = new List<string>
                                                                   {
                                                                       ".pps",
                                                                       ".ppsx",
                                                                       ".ppt",
                                                                       ".pptx",
                                                                       ".odp",
                                                                       ".pptt"                                                                       
                                                                   };

        public static readonly List<string> DocumentExts = new List<string>
                                                               {
                                                                   ".docx", ".doc", ".odt", ".rtf", ".txt",
                                                                   ".html", ".htm", ".mht", ".pdf", ".djvu",
                                                                   ".fb2", ".epub", ".xps",
                                                                   ".doct"
                                                               };

        #endregion

    }
}