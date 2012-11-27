using System;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Studio.Controls.Dashboard.Dao;

namespace ASC.Web.Studio.Controls.Dashboard
{
    public enum CheckEmptyContentResult
    { 
        Empty,
        NotEmpty,
        Unknown
    }
    public interface ICheckEmptyContent
    {
        CheckEmptyContentResult IsEmpty();
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class WidgetPositionAttribute : Attribute
    {
        public Point Position
        {
            get;
            private set;
        }

        public WidgetPositionAttribute(int x, int y)
        {
            Position = new Point(x, y);
        }
    }


    public class Widget : WebControl, ICheckEmptyContent
    {
        public Type SettingsProviderType { get; set; }

        public Guid WidgetID { get; private set; }

        public string Name { get; set; }

        public string ImageURL { get; set; }

        public string WidgetURL { get; set; }

        public string Description { get; set; }

        public bool DisableHorizontalMotion { get; set; }

        public bool Removable { get; set; }

        public Point Position { get; set; }

        private bool usePositionAttribute;

        private string _jsObjName;

        internal WidgetState WidgetState { get; set; }



        public bool UsePositionAttribute
        {
            get { return usePositionAttribute; }
            set
            {
                usePositionAttribute = value;
                if (usePositionAttribute && 0 < Controls.Count)
                {
                    var widgetControl = Controls[0];
                    var posAttr = Attribute.GetCustomAttribute(widgetControl.GetType(), typeof(WidgetPositionAttribute)) as WidgetPositionAttribute;
                    if (posAttr != null) Position = posAttr.Position;
                }
            }
        }

        public Widget(Guid widgetID, Control widgetControl, string name, string description)
        {
            WidgetID = widgetID;
            Name = name;
            Description = description;
            Controls.Add(widgetControl);
        }

        internal void InitWidgetState(WidgetState widgetState, string jsObjName)
        {
            WidgetState = widgetState;
            Position = new Point(widgetState.X, widgetState.Y);
            _jsObjName = jsObjName;
            ID = widgetState.ContainerID + "_" + WidgetID;
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            var sb = new StringBuilder()
                .AppendFormat("<li id='widgetBox_{0}' name='{1}' class='studioWidgetBox'>", ID, WidgetID)
                .Append("<div class='studioWidgetHeader'>")
                .AppendFormat("<input type='hidden' id='widgetDHM_{0}' value='{1}' />", ID, this.DisableHorizontalMotion ? "1" : "0")
                .Append("<table style='width:100%;' cellpadding='0' cellspacing='0'>")
                .Append("<tr>")
                .Append("<td style='cursor:move;' id=\"widgetHandle_" + this.ID + "\">");

            if (!string.IsNullOrEmpty(ImageURL))
            {
                sb.AppendFormat("<img style='border:0px; margin-right:8px;' align='absmiddle'  alt='' src='{0}' />", ImageURL);
            }
            if (!string.IsNullOrEmpty(WidgetURL))
            {
                sb.AppendFormat("<a href='{0}'>{1}</a>", WidgetURL, Name.HtmlEncode());
            }
            else
            {
                sb.AppendFormat("<span>{0}</span>", Name.HtmlEncode());
            }
            sb.Append("</td></tr></table></div>");

            bool isCollapse = false;
            sb.AppendFormat("<div style='{0} {1}' id='widgetDescription_{2}'>{3}</div>", isCollapse ? "" : "display:none;", String.IsNullOrEmpty(Description) ? "" : "margin:5px;", ID, HttpUtility.HtmlEncode(Description));
            sb.AppendFormat("<div  style='overflow:hidden; padding:0px; width:100%; {0}' id='widgetContent_{1}'>", isCollapse ? "display:none;" : "", ID);
            sb.Append("<div class='studioWidgetContent'>");
            writer.Write(sb.ToString());
        }
        public override void RenderEndTag(HtmlTextWriter writer)
        {
            writer.Write("</div></div></li>");
        }

        #region ICheckEmptyContent Members

        public CheckEmptyContentResult IsEmpty()
        {
            if (Controls.Count > 0)
            {
                var widgetControl = Controls[0];
                if (widgetControl is ICheckEmptyContent)
                    return ((ICheckEmptyContent)widgetControl).IsEmpty();

                return CheckEmptyContentResult.Unknown;
            }
            return CheckEmptyContentResult.Empty;
        }

        #endregion
    }
}
