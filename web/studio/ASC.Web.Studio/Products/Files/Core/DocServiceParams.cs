using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Services.WCFService;

namespace ASC.Files.Core
{
    [DataContract(Name = "service_params", Namespace = "")]
    public class DocServiceParams
    {
        private static Dictionary<string, Dictionary<string, string>> _docTemplates;

        [DataContract(Name = "recent", Namespace = "")]
        public class RecentDocument
        {
            public object ID;
            [DataMember(Name = "title")] public string Title;
            [DataMember(Name = "url")] public string Uri;
            [DataMember(Name = "folder")] public string FolderPath;
        }

        [DataContract(Name = "sharingsettings", Namespace = "")]
        public class Aces
        {
            [DataMember(Name = "user")] public string User;
            [DataMember(Name = "permissions")] public string Permissions;
        }

        [DataMember(Name = "file")] public File File;

        [DataMember(Name = "filetype")] public string FileType;
        [DataMember(Name = "outputtype")] public string OutputType;

        [DataMember(Name = "url")] public string FileUri;
        [DataMember(Name = "folderurl")] public string FolderUrl;
        [DataMember(Name = "filepath")] public string FilePath;

        [DataMember(Name = "key")] public string Key;
        [DataMember(Name = "vkey")] public string Vkey;
        [DataMember(Name = "buttons")] public string Buttons;
        [DataMember(Name = "lang")] public string Lang;
        [DataMember(Name = "type")] public string Type;
        [DataMember(Name = "documentType")] public string DocumentType;
        [DataMember(Name = "mode")] public string Mode;

        [DataMember(Name = "sharingsettings")] public ItemList<Aces> SharingSettings;

        [DataMember(Name = "templates")]
        public ItemDictionary<string, string> DocTemplates
        {
            get
            {
                if (_docTemplates == null)
                    _docTemplates = new Dictionary<string, Dictionary<string, string>>();
                if (!_docTemplates.ContainsKey(Lang))
                    _docTemplates.Add(Lang, DocumentUtils.GetDocumentTemplates());

                return new ItemDictionary<string, string>(_docTemplates[Lang]);
            }
        }

        [DataMember(Name = "recent")] public ItemList<RecentDocument> Recent;
    }
}