using System;
using System.Text;

namespace ASC.Web.Core.Utility.Skins
{
    public static class JsSkinHash
    {
        public static string GetJs()
        {
            StringBuilder js = new StringBuilder();
            js.AppendFormat(@"var SkinManager = new function() {{{0}", Environment.NewLine);
            js.AppendFormat(@"    this.images = new Array();{0}", Environment.NewLine);
            js.AppendFormat(@"    this.images[""""] = ""{1}"";{0}", Environment.NewLine, WebImageSupplier.GetImageFolderAbsoluteWebPath());
            foreach (IWebItem item in WebItemManager.Instance.GetItems(ASC.Web.Core.WebZones.WebZoneType.All, ItemAvailableState.All))
            {
                js.AppendFormat(@"    this.images[""{2}""] = ""{1}"";{0}", Environment.NewLine, WebImageSupplier.GetImageFolderAbsoluteWebPath(item.ID), item.ID.ToString().ToLowerInvariant());
            }

            js.AppendFormat(@"    this.GetImage = function(imageName, moduleID) {{{0}", Environment.NewLine);
            js.AppendFormat(@"        var module = """";{0}", Environment.NewLine);
            js.AppendFormat(@"        if (moduleID != 'undefined' && moduleID != undefined && moduleID != null && moduleID) {{{0}", Environment.NewLine);
            js.AppendFormat(@"            module = moduleID.toLowerCase();{0}", Environment.NewLine);
            js.AppendFormat(@"        }}{0}", Environment.NewLine);
            js.AppendFormat(@"        return this.images[module]+'/' + imageName;{0}", Environment.NewLine);
            js.AppendFormat(@"    }};{0}", Environment.NewLine);
            js.AppendFormat(@"}};");

            return js.ToString();
        }
    }
}