using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Studio.Controls.Common
{
    /// <summary>
    /// Base side container
    /// </summary>   
    [ToolboxData("<{0}:SideContainer runat=server></{0}:SideContainer>")]
    public class SideContainer : PlaceHolder
    {
        [Category("Title"), PersistenceMode(PersistenceMode.Attribute)]
        public string Title { get; set; }

        [Category("Title"), PersistenceMode(PersistenceMode.Attribute)]
        public string ImageURL { get; set; }

        [Category("Style"), PersistenceMode(PersistenceMode.Attribute)]
        public string HeaderCSSClass { get; set; }

        [Category("Style"), PersistenceMode(PersistenceMode.Attribute)]
        public string BodyCSSClass { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            var sb = new StringBuilder();
            sb.Append("<div class='studioSideBox'>");

            sb.Append("<table cellspacing=0 cellpadding=0 style='width:100%;'>");
            sb.Append("<tr>");
            sb.Append("<td class='studioSideBoxTopLeftCorner'>&nbsp;</td>");
            sb.Append("<td class='studioSideBoxTopMargin'>&nbsp;</td>");
            sb.Append("<td class='studioSideBoxTopRightCorner'>&nbsp;</td>");
            sb.Append("</table>");

            sb.Append("<div class='studioSideBoxContent'>");

            //header
            sb.Append("<div class='" + (String.IsNullOrEmpty(HeaderCSSClass) ? "studioSideBoxHeader" : HeaderCSSClass) + "'>");
            if (!String.IsNullOrEmpty(ImageURL))
                sb.Append("<img alt='' style='margin-right:8px;' align='absmiddle' src='" + ImageURL + "'/>");

            sb.Append((Title ?? "").HtmlEncode());
            sb.Append("</div>");

            sb.Append("<div class='" + (String.IsNullOrEmpty(BodyCSSClass) ? "studioSideBoxBody" : BodyCSSClass) + "'>");

            writer.Write(sb.ToString());
            base.Render(writer);


            sb = new StringBuilder();
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("<table cellspacing=0 cellpadding=0 style='width:100%;'>");
            sb.Append("<tr>");
            sb.Append("<td class='studioSideBoxBottomLeftCorner'>&nbsp;</td>");
            sb.Append("<td class='studioSideBoxBottomMargin'>&nbsp;</td>");
            sb.Append("<td class='studioSideBoxBottomRightCorner'>&nbsp;</td>");
            sb.Append("</table>");

            sb.Append("</div>");
            writer.Write(sb.ToString());
        }
    }
}