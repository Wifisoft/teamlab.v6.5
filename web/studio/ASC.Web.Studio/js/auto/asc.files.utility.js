if (typeof ASC === 'undefined')
    ASC = {};
if (typeof ASC.Files === 'undefined')
    ASC.Files = (function() { return {}; })();
if (typeof ASC.Files.Utility === 'undefined')
    ASC.Files.Utility = {};

if (typeof ASC.Files.Utility.GetFileExtension === 'undefined') {
    ASC.Files.Utility.GetFileExtension = function(fileTitle) {
        if (typeof fileTitle == 'undefined' || fileTitle == null)
            return '';

        var posExt = fileTitle.lastIndexOf('.');
        return 0 <= posExt ? fileTitle.substring(posExt).trim().toLowerCase() : '';
    };
}

ASC.Files.Utility.FileExtensionLibrary = {
    ArchiveExts: [".zip", ".rar", ".ace", ".arc", ".arj", ".bh", ".cab", ".enc", ".gz", ".ha", ".jar", ".lha", ".lzh", ".pak", ".pk3", ".tar", ".tgz", ".uu", ".uue", ".xxe", ".z", ".zoo"],
    AviExts: [".avi"],
    CsvExts: [".csv"],
    DjvuExts: [".djvu"],
    DocExts: [".doc", ".docx"],
    DoctExts: [".doct"],
    EbookExts: [".epub", ".fb2"],
    FlvExts: [".flv", ".fla"],
    HtmlExts: [".html", ".htm", ".mht"],
    IafExts: [".iaf"],
    ImgExts: [".bmp", ".cod", ".gif", ".ief", ".jpe", ".jpeg", ".jpg", ".jfif", ".tiff", ".tif", ".cmx", ".ico", ".png", ".pnm", ".pbm", ".ppm", ".rgb", ".xbm", ".xpm", ".xwd"],
    M2tsExts: [".m2ts"],
    MkvExts: [".mkv"],
    MovExts: [".mov"],
    Mp4Exts: [".mp4"],
    MpgExts: [".mpg"],
    OdpExts: [".odp"],
    OdsExts: [".ods"],
    OdtExts: [".odt"],
    PdfExts: [".pdf"],
    PpsExts: [".pps", ".ppsx"],
    PptExts: [".ppt", ".pptx"],
    PpttExts: [".pptt"],
    RtfExts: [".rtf"],
    SoundExts: [".mp3", ".wav", ".pcm", ".aiff", ".3gp", ".flac", ".fla", ".cda"],
    SvgExts: [".svg"],
    SvgtExts: [".svgt"],
    TxtExts: [".txt"],
    DvdExts: [".vob"],
    XlsExts: [".xls", ".xlsx"],
    XlstExts: [".xlst"],
    XmlExts: [".xml"],
    XpsExts: [".xps"]
};

ASC.Files.Utility.getCssClassByFileTitle = function(fileTitle, compact) {
    var fileExt = ASC.Files.Utility.GetFileExtension(fileTitle);

    var ext = "file";

    if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.ArchiveExts) != -1) ext = "Archive";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.AviExts) != -1) ext = "Avi";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.CsvExts) != -1) ext = "Csv";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.DjvuExts) != -1) ext = "Djvu";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.DocExts) != -1) ext = "Doc";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.DoctExts) != -1) ext = "Doct";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.EbookExts) != -1) ext = "Ebook";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.FlvExts) != -1) ext = "Flv";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.HtmlExts) != -1) ext = "Html";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.IafExts) != -1) ext = "Iaf";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.ImgExts) != -1) ext = "Image";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.M2tsExts) != -1) ext = "M2ts";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.MkvExts) != -1) ext = "Mkv";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.MovExts) != -1) ext = "Mov";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.Mp4Exts) != -1) ext = "Mp4";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.MpgExts) != -1) ext = "Mpg";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.OdpExts) != -1) ext = "Odp";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.OdtExts) != -1) ext = "Odt";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.OdsExts) != -1) ext = "Ods";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.PdfExts) != -1) ext = "Pdf";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.PpsExts) != -1) ext = "Pps";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.PptExts) != -1) ext = "Ppt";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.PpttExts) != -1) ext = "Pptt";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.RtfExts) != -1) ext = "Rtf";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.SoundExts) != -1) ext = "Sound";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.SvgExts) != -1) ext = "Svg";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.SvgtExts) != -1) ext = "Svgt";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.TxtExts) != -1) ext = "Txt";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.DvdExts) != -1) ext = "Dvd";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.XlsExts) != -1) ext = "Xls";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.XlstExts) != -1) ext = "Xlst";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.XmlExts) != -1) ext = "Xml";
    else if (jq.inArray(fileExt, ASC.Files.Utility.FileExtensionLibrary.XpsExts) != -1) ext = "Xps";

    return "ftFile_" + (compact === true ? 21 : 32) + " ft_" + ext;
};

ASC.Files.Utility.getFolderCssClass = function(compact) {
    return "ftFolder_" + (compact === true ? 21 : 32);
};