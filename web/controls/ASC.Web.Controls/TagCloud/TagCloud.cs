using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Controls
{
    [Themeable(true)]
    [ToolboxData("<{0}:TagCloudItem runat=server />")]
    #region TagCloudItem
    public class TagCloudItem
    {   
        public string TagName{get; set;}

        public string URL { get; set; }
        
        #region Rank
        private float m_rank;

        public float Rank
        {
            get { return (float)Math.Log(m_rank + 1, Math.E); }
            set { m_rank = value; }
        }

        #endregion       

        public string TagID{get; set;}
    }
    #endregion


    [ToolboxData("<{0}:TagCloud runat=server></{0}:TagCloud>")]
    [ParseChildren(true, "Items")]
    [PersistChildren(true)]
    public class TagCloud : WebControl
    {

        public TagCloud()
        {
            this.IsUseGradient = true;            
            this.Items = new List<TagCloudItem>(0);
            this.LowFontSizePercent = 90;
            this.HighFontSizePercent = 200;
                
        }

        #region ExecGradient
        private Color ExecGradient(Color c1, Color c2, int prc)
        {
            int R, G, B;
            if (c1.R == c2.R)
            {
                R = c1.R;
            }
            else if (c2.R > c1.R)
            {
                R = (byte)((c2.R - c1.R) * prc / 100) + c1.R;
            }
            else
            {
                R = (byte)((c1.R - c2.R) * (100 - prc) / 100) + c2.R;
            }

            if (c1.G == c2.G)
            {
                G = c1.G;
            }
            else if (c2.G > c1.G)
            {
                G = (byte)((c2.G - c1.G) * prc / 100) + c1.G;
            }
            else
            {
                G = (byte)((c1.G - c2.G) * (100 - prc) / 100) + c2.G;
            }

            if (c1.B == c2.B)
            {
                B = c1.B;
            }
            else if (c2.B > c1.B)
            {
                B = (byte)((c2.B - c1.B) * prc / 100) + c1.B;
            }
            else
            {

                B = (byte)((c1.B - c2.B) * (100 - prc) / 100) + c2.B;
            }
            return Color.FromArgb(R, G, B);
        }
        #endregion

        protected override void RenderContents(HtmlTextWriter output)
        {   
         
            output.Write("<div");
            if (!String.IsNullOrEmpty(this.BoxCSSClass))
                output.Write(" class=\"" + BoxCSSClass + "\"");
            output.Write(" style=\"width:" + this.Width + ";\"");
            output.Write(">");

            if (!String.IsNullOrEmpty(this.Caption))
            {
                output.Write("<div"+(!String.IsNullOrEmpty(this.CaptionCSSClass)?(" class='"+this.CaptionCSSClass+"'"):"")+" style='margin-bottom:5px; text-align:left;'>");
                output.Write(HttpUtility.HtmlEncode(this.Caption));
                output.Write("</div>");
            }

            float minRank = 0;
            float maxRank = 1;

            if (this.Items.Count > 0)
            {
                minRank = this.Items[0].Rank;
                maxRank = this.Items[0].Rank; 
            }
            foreach (TagCloudItem item in this.Items)
            {
                if (item.Rank < minRank)
                    minRank = item.Rank;
                if (item.Rank > maxRank)
                    maxRank = item.Rank;
            }
            float dif = maxRank - minRank;
            foreach (TagCloudItem item in this.Items)
            {
                float scale = 0.5f;
                if (dif != 0)
                    scale = (item.Rank - minRank) / dif;

                scale = (item.Rank - minRank) / dif;
                int size = (int)((this.HighFontSizePercent - this.LowFontSizePercent) * scale) + this.LowFontSizePercent;

                
                output.Write("<a ");
                if (!String.IsNullOrEmpty(LinkCSSClass))
                    output.Write("class=\"" + LinkCSSClass + "\" ");
                output.Write("style=\"font-size:" + size + "%;");

                if (this.IsUseGradient)
                {
                    Color c1 = System.Drawing.ColorTranslator.FromHtml(String.IsNullOrEmpty(this.LowLinkColor) ? "#50748f" : this.LowLinkColor);
                    Color c2 = System.Drawing.ColorTranslator.FromHtml(String.IsNullOrEmpty(this.HighLinkColor) ? "#1e66a4" : this.HighLinkColor);

                    Color color = ExecGradient(c1, c2, (int)scale * 100);
                    string htmlColor = System.Drawing.ColorTranslator.ToHtml(color);

                    output.Write("color:" + htmlColor + ";");
                }

                output.Write("\" href=\"" + item.URL + "\">" + HttpUtility.HtmlEncode(item.TagName) + "</a>");
                output.Write(" ");
            }
            output.Write("</div>");
        }

        public string LowLinkColor { get; set; }

        public string HighLinkColor { get; set; }

        public string LinkCSSClass{get; set;}

        public string BoxCSSClass { get; set; }

        public int LowFontSizePercent { get; set; }

        public int HighFontSizePercent { get; set; }       

        public bool IsUseGradient { get; set; }

        public string Caption { get; set; }

        public string CaptionCSSClass { get; set; }

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<TagCloudItem> Items { get; set; }
    }
}
