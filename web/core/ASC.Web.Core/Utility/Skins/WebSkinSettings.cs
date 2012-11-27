using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using System.Xml;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Core.Utility.Skins
{
    [Serializable]
    [DataContract]
    public class WebSkin
    {
        public static List<WebSkin> Skins
        {
            get;
            private set;
        }

        public static WebSkin DefaultSkin
        {
            get;
            private set;
        }

        [DataMember(Name = "Id")]
        public string ID { get; set; }

        public string Name { get; set; }

        public string DisplayName
        {
            get
            {
                if (!String.IsNullOrEmpty(Name) && Name.IndexOf(";") >= 0)
                {
                    try
                    {
                        var typeName = Name.Split(';')[0];
                        var resKey = Name.Split(';')[1];

                        return (string)Type.GetType(typeName).GetProperty(resKey, BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
                    }
                    catch { };
                }
                return Name;
            }
        }

        public List<string> CSSFileNames { get; set; }

        public string BaseCSSFileName { get; set; }

        public string FolderName { get; set; }

        public string ASPTheme { get; set; }

        public int SortOrder { get; set; }

        public string BaseCSSFileAbsoluteWebPath
        {
            get
            {
                return GetAbsoluteWebPath("/skins/<theme_folder>/" + this.BaseCSSFileName.ToLower());
            }
        }


        static WebSkin()
        {
            Skins = new List<WebSkin>();
            if (HttpContext.Current != null)
            {
                var dir = HttpContext.Current.Server.MapPath("~/app_themes");
                if (Directory.Exists(dir))
                {
                    foreach (var d in Directory.GetDirectories(dir))
                    {
                        var s = MakeSkinFromDirectory(d);
                        if (s != null) Skins.Add(s);
                    }
                    Skins.Sort((x, y) => x.SortOrder.CompareTo(y.SortOrder));
                }
            }
            DefaultSkin = GetSkin("default") ?? new WebSkin { ID = "default", ASPTheme = "Default", BaseCSSFileName = "common_style.css", Name = "ASC.Web.Studio.PublicResources.SkinNameResource,ASC.Web.Studio;DefaultSkin", FolderName = "default" };
        }


        public WebSkin()
        {
            CSSFileNames = new List<string>();
        }

        public static WebSkin GetSkin(string id)
        {
            return Skins.Find(s => s.ID == id) ?? DefaultSkin;
        }

        public static WebSkin GetUserSkin()
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            var settings = SettingsManager.Instance.LoadSettingsFor<WebSkinSettings>(SecurityContext.CurrentAccount.ID);
            if (settings.IsDefault)
            {
                settings = SettingsManager.Instance.LoadSettings<WebSkinSettings>(tenant.TenantId);
            }
            return settings.WebSkin != null ? GetSkin(settings.WebSkin.ID) : DefaultSkin;
        }

        public string GetAbsoluteWebPath(string path)
        {
            var p = path.ToLower().Replace("<theme_folder>", FolderName.ToLower());
            if (WebPath.Exists(p)) return WebPath.GetPath(p);

            p = path.ToLower().Replace("<theme_folder>", DefaultSkin.FolderName.ToLower());
            return WebPath.GetPath(p);
        }

        public string GetCSSFileAbsoluteWebPath(string fileName)
        {
            return GetAbsoluteWebPath("/skins/<theme_folder>/" + fileName.ToLower());
        }


        internal string GetImageFolderAbsoluteWebPath(Guid partID)
        {
            if (HttpContext.Current == null) return string.Empty;

            var currentThemePath = GetPartImageFolderRel(partID);
            return GetAbsoluteWebPath(currentThemePath);
        }

        internal string GetImageAbsoluteWebPath(string fileName, Guid partID)
        {
            if (HttpContext.Current == null || string.IsNullOrEmpty(fileName)) return string.Empty;

            var filepath = GetPartImageFolderRel(partID) + "/" + fileName;
            return GetAbsoluteWebPath(filepath);
        }


        private static WebSkin MakeSkinFromDirectory(string dir)
        {
            if (File.Exists(dir + "\\skin.xml"))
            {
                var skin = new WebSkin();
                skin.FolderName = new DirectoryInfo(dir).Name;

                var doc = new XmlDocument();
                doc.Load(dir + "\\skin.xml");

                var list = doc.GetElementsByTagName("ID");
                if (list != null && list.Count > 0)
                    skin.ID = list[0].InnerText;

                list = doc.GetElementsByTagName("SortOrder");
                if (list != null && list.Count > 0)
                    skin.SortOrder = Convert.ToInt32(list[0].InnerText);

                list = doc.GetElementsByTagName("Name");
                if (list != null && list.Count > 0)
                    skin.Name = list[0].InnerText;

                list = doc.GetElementsByTagName("ASPTheme");
                if (list != null && list.Count > 0)
                    skin.ASPTheme = list[0].InnerText;

                list = doc.GetElementsByTagName("BaseCSSFileName");
                if (list != null && list.Count > 0)
                {
                    skin.BaseCSSFileName = list[0].InnerText;
                    skin.CSSFileNames.Add(skin.BaseCSSFileName);
                }

                list = doc.GetElementsByTagName("CSSFileName");
                if (list != null && list.Count > 0)
                {
                    foreach (XmlNode node in list)
                        skin.CSSFileNames.Add(node.InnerText);
                }
                return skin;
            }
            return null;
        }

        private string GetPartImageFolderRel(Guid partID)
        {
            string folderName = "/skins/<theme_folder>/images";
            string itemFolder = null;
            if (!Guid.Empty.Equals(partID))
            {
                var product = ProductManager.Instance[partID];
                if (product != null &&
                    product.Context != null &&
                   !String.IsNullOrEmpty(product.Context.ThemesFolderVirtualPath) &&
                   !String.IsNullOrEmpty(product.Context.ImageFolder))
                {

                    itemFolder = product.Context.ThemesFolderVirtualPath.TrimEnd('/') + "/<theme_folder>/" + product.Context.ImageFolder;
                }
                else if (product != null && product.Context != null && !String.IsNullOrEmpty(product.Context.ImageFolder))
                    itemFolder = "/skins/<theme_folder>/modules/" + product.Context.ImageFolder;


                if (itemFolder == null)
                {
                    var module = ProductManager.Instance.GetModuleByID(partID);
                    if (module != null &&
                       module.Context != null &&
                      !String.IsNullOrEmpty(module.Context.ThemesFolderVirtualPath) &&
                      !String.IsNullOrEmpty(module.Context.ImageFolder))
                    {
                        itemFolder = module.Context.ThemesFolderVirtualPath.TrimEnd('/') + "/<theme_folder>/" + module.Context.ImageFolder;
                    }
                    else if (module != null && module.Context != null && !String.IsNullOrEmpty(module.Context.ImageFolder))
                        itemFolder = "/skins/<theme_folder>/modules/" + module.Context.ImageFolder;
                }

                if (itemFolder == null)
                {
                    var addon = WebItemManager.Instance[partID] as IAddon;
                    if (addon != null &&
                       addon.Context != null &&
                      !String.IsNullOrEmpty(addon.Context.ThemesFolderVirtualPath) &&
                      !String.IsNullOrEmpty(addon.Context.ImageFolder))
                    {
                        itemFolder = addon.Context.ThemesFolderVirtualPath.TrimEnd('/') + "/<theme_folder>/" + addon.Context.ImageFolder;
                    }
                    else if (addon != null && addon.Context != null && !String.IsNullOrEmpty(addon.Context.ImageFolder))
                        itemFolder = "/skins/<theme_folder>/modules/" + addon.Context.ImageFolder;
                }

                folderName = itemFolder ?? folderName;
            }
            return folderName.TrimStart('~').ToLowerInvariant();
        }
    }

    [Serializable]
    [DataContract]
    public class WebSkinSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("b209fa64-0607-40ba-9c2d-173378ef3e70"); }
        }

        [DataMember(Name = "IsDefault")]
        public bool IsDefault
        {
            get;
            set;
        }

        [DataMember(Name = "Skin")]
        public WebSkin WebSkin { get; set; }


        public ISettings GetDefault()
        {
            return new WebSkinSettings() { WebSkin = new WebSkin { ID = "default" }, IsDefault = true };
        }
    }
}
