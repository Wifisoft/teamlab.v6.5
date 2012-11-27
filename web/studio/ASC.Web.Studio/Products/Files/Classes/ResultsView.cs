using System;
using System.Collections.Generic;
using System.Web.UI;
using ASC.Files.Core;
using ASC.Web.Controls;
using ASC.Web.Core.ModuleManagement.Common;

namespace ASC.Web.Files.Classes
{
    public sealed class ResultsView : ItemSearchControl
    {

        private string GetFolderCssClass(object value)
        {
            if (value == null)
                return String.Empty;

            if ((bool) value)
                return "thumb-folder";
            return String.Empty;
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write("<link href=" + PathProvider.GetFileStaticRelativePath("common.css") + " type=\"text/css\" rel=\"stylesheet\" />");

            writer.Write("<script type=\"text/javascript\" language=\"javascript\" src=" + PathProvider.GetFileStaticRelativePath("common.js") + "></script>");

            foreach (var srGroup in Items.GetRange(0, (MaxCount < Items.Count) ? MaxCount : Items.Count))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, String.Format("document {0} clearFix", srGroup.Additional.ContainsKey("IsFolder") ? GetFolderCssClass(srGroup.Additional["IsFolder"]) : String.Empty));
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "icon");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "body");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, srGroup.URL);

                writer.AddAttribute(HtmlTextWriterAttribute.Title, srGroup.Name);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "linkHeaderMedium");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(HtmlUtility.SearchTextHighlight(Text, srGroup.Name.HtmlEncode(), false));
                writer.RenderEndTag();

                writer.WriteBreak();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "textSmallDescribe");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (srGroup.Additional.ContainsKey("Author"))
                {
                    writer.Write("{0} {1}", Resources.FilesCommonResource.Author, srGroup.Additional["Author"]);
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "separator");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write("|");
                writer.RenderEndTag();

                writer.Write("{0} {1}", Resources.FilesCommonResource.TitleUploaded, srGroup.Date);

                if (srGroup.Additional.ContainsKey("Size"))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "separator");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write("|");
                    writer.RenderEndTag();
                    writer.Write("{0} {1}", Resources.FilesCommonResource.Size, srGroup.Additional["Size"]);
                }
                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "adv");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(HtmlUtility.GetText(Search.FolderPathBuilder((List<Folder>) srGroup.Additional["Container"]), 80));
                writer.RenderEndTag();

                writer.RenderEndTag();
            }

            var str = @"
<script type='text/javascript'>
    jq(function() {
        jq('div.searchResults div.document').each(function() {
            var ftClass;
            if (jq(this).hasClass('thumb-folder')) {
                ftClass = ASC.Files.Utility.getFolderCssClass();
            } else {
                var title = jq(this).find('a.linkHeaderMedium').text().trim();
                ftClass = ASC.Files.Utility.getCssClassByFileTitle(title);
            }
            jq(this).find('div.icon').addClass(ftClass);
        });
    });
</script>";
            writer.Write(str);
        }
    }
}