using System;
using System.Web.UI;
using ASC.Web.Controls;
using ASC.Web.Core.ModuleManagement.Common;

namespace ASC.Web.Studio.Controls.Common
{
    public sealed class CommonResultsView : ItemSearchControl
    {
        protected override void RenderContents(HtmlTextWriter writer)
        {
            foreach (var srGroup in Items.GetRange(0, (MaxCount < Items.Count) ? MaxCount : Items.Count))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "clearFix universalitem");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (srGroup.Additional!= null && srGroup.Additional.ContainsKey("imageRef"))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "img");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Src, srGroup.Additional["imageRef"].ToString());
                    writer.AddAttribute(HtmlTextWriterAttribute.Title, srGroup.Additional["Hint"].ToString());
                    writer.AddAttribute(HtmlTextWriterAttribute.Alt, srGroup.Additional["Hint"].ToString());
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                    writer.RenderEndTag();

                    writer.RenderEndTag();
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Class, String.IsNullOrEmpty(srGroup.Description) ? "widebody" : "body");
                
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, srGroup.URL);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "linkHeaderMedium");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(HtmlUtility.SearchTextHighlight(Text, srGroup.Name.HtmlEncode(), false));
                writer.RenderEndTag();

                if (!String.IsNullOrEmpty(srGroup.Description))
                {
                    writer.WriteBreak();

                    if (!string.IsNullOrEmpty(SpanClass))
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, SpanClass);
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(CheckEmptyValue(srGroup.Description.HtmlEncode()));
                    writer.RenderEndTag();

                }

                writer.RenderEndTag();

                if (srGroup.Date.HasValue)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "date");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Style, "height:100%");
                    writer.RenderBeginTag(HtmlTextWriterTag.Table);
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);

                    writer.Write(srGroup.Date.Value.ToShortDateString());

                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderEndTag();

                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            }
        }
    }
}
