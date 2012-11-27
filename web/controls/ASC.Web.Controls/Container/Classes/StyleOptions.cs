using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Web.Controls
{
    public class StyleOptions
    {
        public string ContainerCssClass { get; set; }
        public string ContainerStyle { get; set; }

        public string PopupContainerCssClass { get; set; }

        public string HeadCssClass { get; set; }
        public string HeadStyle { get; set; }

        public string BodyCssClass { get; set; }
        public string BodyStyle { get; set; }

        public bool IsPopup { get; set; }

        public string BreadCrumbLinkCssClass { get; set; }
        public string BreadCrumbLinkStyle { get; set; }

        public string BreadCrumbSpiterCssClass { get; set; }
        public string BreadCrumbSpiterStyle { get; set; }

        public string BreadCrumbSpiter { get; set; }
        public string OnCancelButtonClick { get; set; }

        public bool ShowLastLevel { get; set; }

        public string HeaderBreadCrumbCaption { get; set; }

        public string InfoMessageText { get; set; }
        public InfoType InfoType { get; set; }
    }
}
