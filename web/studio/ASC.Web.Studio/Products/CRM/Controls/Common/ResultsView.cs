#region Import

using System;
using System.Web.UI;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.CRM.Resources;
using ASC.Web.Controls;

#endregion

namespace ASC.Web.CRM.Controls.Common
{
    public sealed class ResultsView : ItemSearchControl
    {
    
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute("class", "tableBase");
            writer.AddAttribute("cellspacing", "0");
            writer.AddAttribute("cellpadding", "7");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);


            foreach (var searchItemResult in Items.GetRange(0, (MaxCount < Items.Count) ? MaxCount : Items.Count))
            {

                var relativeInfo = searchItemResult.Additional["relativeInfo"].ToString();

                if (String.IsNullOrEmpty(relativeInfo))
                    relativeInfo = searchItemResult.Description.HtmlEncode();
                else
                    relativeInfo = String.Format("<span class='textBigDescribe'>{0}</span> {1}", CRMCommonResource.RelativeTo,
                                                 relativeInfo.HtmlEncode());
                
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                writer.AddStyleAttribute("white-space", "nowrap");
                writer.AddAttribute("class", "borderBase img");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                writer.AddAttribute("title",searchItemResult.Additional["typeInfo"].ToString());
                writer.AddAttribute("alt", searchItemResult.Additional["typeInfo"].ToString());
                writer.AddAttribute("src", searchItemResult.Additional["imageRef"].ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.AddStyleAttribute("width", "100%");
                writer.AddAttribute("class", "borderBase");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                writer.AddAttribute(HtmlTextWriterAttribute.Href, searchItemResult.URL);
                writer.AddAttribute("class", "title linkHeaderMedium");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(HtmlUtility.SearchTextHighlight(Text, searchItemResult.Name.HtmlEncode(), false));
                writer.RenderEndTag();

                if (!String.IsNullOrEmpty(relativeInfo))
                {
                    writer.WriteBreak();
                    writer.Write(relativeInfo);
                }

                writer.RenderEndTag();

             
                writer.AddAttribute("class", "borderBase");
                writer.AddStyleAttribute("white-space", "nowrap");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                writer.AddAttribute("class", "textBigDescribe");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(searchItemResult.Date.Value.ToShortDateString());
                writer.RenderEndTag();


                writer.RenderEndTag();

                writer.RenderEndTag();

            }

            writer.RenderEndTag();
            writer.RenderEndTag();

        }
    }
}