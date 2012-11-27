using System;
using System.Web;
using System.Web.UI;
using ASC.Projects.Core.Domain;
using ASC.Web.Controls;
using ASC.Web.Core.ModuleManagement.Common;

namespace ASC.Web.Projects.Classes
{
    public sealed class ResultsView : ItemSearchControl
    {
        protected override void RenderContents(HtmlTextWriter writer)
        {
            foreach (var srGroup in Items.GetRange(0, (MaxCount < Items.Count) ? MaxCount : Items.Count))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "clearFix projItem");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "img");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Src, srGroup.Additional["imageRef"].ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Title, srGroup.Additional["Hint"].ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, srGroup.Additional["Hint"].ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "body");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, srGroup.URL);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "linkHeaderMedium");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(HtmlUtility.SearchTextHighlight(Text, srGroup.Name.HtmlEncode(), false));
                writer.RenderEndTag();

                writer.WriteBreak();



                if ((EntityType) (Enum.Parse(typeof (EntityType), (srGroup.Additional["Type"]).ToString())) == EntityType.Project)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "textBigDescribe");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(CheckEmptyValue(HtmlUtility.SearchTextHighlight("", HttpUtility.HtmlEncode(HtmlUtility.GetText(srGroup.Description, 100)))));
                    writer.RenderEndTag();

                }
                else
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "textBigDescribe");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(Resources.ProjectsCommonResource.InProject);
                    writer.RenderEndTag();
                    writer.Write("&nbsp;");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(srGroup.Additional["ProjectName"].ToString());
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();

                if (srGroup.Date.HasValue)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "date");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.Write(srGroup.Date.Value.ToShortDateString());
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            }
        }
    }
}