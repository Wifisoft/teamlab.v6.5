using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Web.Controls
{
    public class StyleOptions
    {
        public string ContainerCssClass { get; set; }
        public string ContainerStyle { get; set; }

        public string HeadCssClass { get; set; }
        public string HeadStyle { get; set; }

        public string BodyCssClass { get; set; }
        public string BodyStyle { get; set; }

        public string BaseCssClass { get; set; }
        public string BaseStyle { get; set; }

        public bool IsPopup { get; set; }
    }
}
