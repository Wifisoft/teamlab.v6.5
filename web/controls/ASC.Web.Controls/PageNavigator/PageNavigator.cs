using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;

namespace ASC.Web.Controls
{
    [Themeable(true)]
    [ToolboxData("<{0}:PageNavigator runat=server></{0}:PageNavigator>")]
    public class PageNavigator : WebControl, ICloneable
    {
        private string _navigatorID = Guid.NewGuid().ToString().Replace('-', '_');
        private string _jsObjName;

        public string Splitter { get; set; }

        public string SplitterCSSClass { get; set; }

        public int CurrentPageNumber { get; set; }

        public int EntryCountOnPage { get; set; }

        public int EntryCount { get; set; }

        public bool VisibleOnePage { get; set; }

        public int VisiblePageCount { get; set; }

        public string PageUrl { get; set; }

        public string ParamName { get; set; }

        public string BoxCSSClass { get; set; }

        public string NavigationLinkCSSClass { get; set; }

        public string CurrentPositionCSSClass { get; set; }

        public string PrevButtonCSSClass { get; set; }

        public string NextButtonCSSClass { get; set; }

        public bool AutoDetectCurrentPage { get; set; }

        public PageNavigator()
        {
            ParamName = "p";
            NavigationLinkCSSClass = "";
            BoxCSSClass = "";
            CurrentPositionCSSClass = "";
            VisiblePageCount = 10;
            VisibleOnePage = false;
            AutoDetectCurrentPage = false;
            Splitter = "...";
            SplitterCSSClass = "";
            EntryCountOnPage = 1;
            PageUrl = "";
        }

        private int _page_amount;
        private int _start_page;
        private int _end_page;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _jsObjName = String.IsNullOrEmpty(ID) ? "pageNavigator" + UniqueID.Replace('$', '_') : ID;

            if (HttpContext.Current != null && HttpContext.Current.Request != null && AutoDetectCurrentPage)
            {
                if (!String.IsNullOrEmpty(HttpContext.Current.Request[this.ParamName]))
                {
                    try
                    {
                        CurrentPageNumber = Convert.ToInt32(HttpContext.Current.Request[this.ParamName]);
                    }
                    catch { CurrentPageNumber = 0; }
                }
            }

            if (CurrentPageNumber <= 0)
                CurrentPageNumber = 1;


            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ASC_Controls_PageNavigator_Script"))
                Page.ClientScript.RegisterClientScriptInclude("ASC_Controls_PageNavigator_Script",
                                                              Page.ClientScript.GetWebResourceUrl(GetType(), "ASC.Web.Controls.PageNavigator.js.pagenavigator.js"));

            var scriptNav = String.Format(@"var {1} = new ASC.Controls.PageNavigator.initPrototype('{0}', '{1}', {2}, {3}, {4},'{5}','{6}', '{7}','{8}','{9}','{10}');",
                                            _navigatorID,
                                            _jsObjName,
                                            EntryCountOnPage,
                                            VisiblePageCount,
                                            CurrentPageNumber,
                                            Resources.PageNavigator.PreviousPage,
                                            Resources.PageNavigator.NextPage,
                                            NavigationLinkCSSClass,
                                            CurrentPositionCSSClass,
                                            PrevButtonCSSClass,
                                            NextButtonCSSClass
                                            );

            Page.ClientScript.RegisterStartupScript(typeof(string), "navigationControlMod", scriptNav, true);


            _page_amount = Convert.ToInt32(Math.Ceiling(EntryCount / (EntryCountOnPage * 1.0)));
            _start_page = CurrentPageNumber - 1 - VisiblePageCount / 2;

            if (_start_page + VisiblePageCount > _page_amount)
                _start_page = _page_amount - VisiblePageCount;

            if (_start_page < 0)
                _start_page = 0;

            _end_page = _start_page + VisiblePageCount;

            if (_end_page > _page_amount)
                _end_page = _page_amount;

            if ((_page_amount == 1 && VisibleOnePage) || _start_page >= _end_page || _end_page - _start_page <= 1)
                return;

            string spliter = "&";
            if (PageUrl.IndexOf("?") == -1)
            {
                spliter = "&";
            }
            string url = PageUrl;

            bool isFirst = (CurrentPageNumber == 1);
            bool isLast = (CurrentPageNumber == _page_amount);

            string prevURL = PageUrl + spliter + ParamName + "=" + (CurrentPageNumber - 1).ToString();
            string nextURL = PageUrl + spliter + ParamName + "=" + (CurrentPageNumber + 1).ToString();

            string script = @"document.onkeydown = function(e)
                            {
                                var code;
                                if (!e) var e = window.event;
                                if (e.keyCode) code = e.keyCode;
                                else if (e.which) code = e.which;" +

                                ((!isFirst) ?
                                @"if ((code == 37) && (e.ctrlKey == true))
                                {
                                    window.open('" + prevURL + @"','_self');
                                }" : "") +

                                ((!isLast) ?
                                @"if ((code == 39) && (e.ctrlKey == true))
                                {
                                    window.open('" + nextURL + @"','_self');
                                }" : "") +
                            @"}; ";

            Page.ClientScript.RegisterClientScriptBlock(typeof(string), "navigationControlScript", script, true);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();

            if (_page_amount == 1 && VisibleOnePage)
            {
                sb.Append("<div" + (String.IsNullOrEmpty(BoxCSSClass) ? "" : (" class='" + BoxCSSClass + "'")) + ">");
                sb.Append("<span" + (String.IsNullOrEmpty(CurrentPositionCSSClass) ? "" : (" class='" + CurrentPositionCSSClass + "'")) + ">");
                sb.Append(1);
                sb.Append("</span>");

                sb.Append("</div>");

                writer.Write(sb.ToString());
                return;
            }

            if (_start_page >= _end_page || _end_page - _start_page <= 1)
                return;

            sb.Append("<div" + (String.IsNullOrEmpty(BoxCSSClass) ? "" : (" class='" + BoxCSSClass + "'")) + ">");

            string spliter = "&";
            if (PageUrl.IndexOf("?") == -1)
            {
                spliter = "&";
            }
            string url = PageUrl;

            if (CurrentPageNumber != 1)
            {
                url = PageUrl + spliter + ParamName + "=" + Convert.ToString(CurrentPageNumber - 1);
                sb.Append("<a" + (String.IsNullOrEmpty(PrevButtonCSSClass) ? "" : (" class='" + PrevButtonCSSClass + "'")) + " href=\"" + url + "\">" + Resources.PageNavigator.PreviousPage + "</a>");
            }


            for (int i = _start_page; i < _end_page && _end_page - _start_page > 1; i++)
            {
                if (i == _start_page && i != 0)
                {
                    url = PageUrl + spliter + ParamName + "=" + Convert.ToString(1);
                    sb.Append("<a" + (String.IsNullOrEmpty(NavigationLinkCSSClass) ? "" : (" class='" + NavigationLinkCSSClass + "'")) + " href=\"" + url + "\">1</a>");
                    if (i != 1)
                        sb.Append("<span" + (String.IsNullOrEmpty(SplitterCSSClass) ? "" : (" class='" + SplitterCSSClass + "'")) + ">" + Splitter + "</span>");
                }
                if ((CurrentPageNumber - 1) == i)
                {
                    sb.Append("<span" + (String.IsNullOrEmpty(CurrentPositionCSSClass) ? "" : (" class='" + CurrentPositionCSSClass + "'")) + ">");
                    sb.Append(CurrentPageNumber);
                    sb.Append("</span>");
                }
                else
                {
                    url = PageUrl + spliter + ParamName + "=" + Convert.ToString((i + 1));
                    sb.Append("<a" + (String.IsNullOrEmpty(NavigationLinkCSSClass) ? "" : (" class='" + NavigationLinkCSSClass + "'")) + " href=\"" + url + "\">" + (i + 1) + "</a>");
                }
                if (i == _end_page - 1 && i != _page_amount - 1)
                {
                    url = PageUrl + spliter + ParamName + "=" + Convert.ToString((_page_amount));
                    if (i != _page_amount - 2)
                        sb.Append("<span" + (String.IsNullOrEmpty(SplitterCSSClass) ? "" : (" class='" + SplitterCSSClass + "'")) + ">" + Splitter + "</span>");
                    sb.Append("<a" + (String.IsNullOrEmpty(NavigationLinkCSSClass) ? "" : (" class='" + NavigationLinkCSSClass + "'")) + " href=\"" + url + "\">" + _page_amount + "</a>");
                }
            }

            if (CurrentPageNumber != _page_amount && _page_amount != 1)
            {
                url = PageUrl + spliter + ParamName + "=" + Convert.ToString(CurrentPageNumber + 1);
                sb.Append("<a" + (String.IsNullOrEmpty(NextButtonCSSClass) ? "" : (" class='" + NextButtonCSSClass + "'")) + " href=\"" + url + "\">" + Resources.PageNavigator.NextPage + "</a>");
            }

            sb.Append("</div>");
            writer.Write(sb.ToString());
        }

        #region ICloneable Members

        public object Clone()
        {
            return new PageNavigator()
            {
                CurrentPageNumber = this.CurrentPageNumber,
                EntryCountOnPage = this.EntryCountOnPage,
                EntryCount = this.EntryCount,
                VisibleOnePage = this.VisibleOnePage,
                VisiblePageCount = this.VisiblePageCount,
                PageUrl = this.PageUrl,
                ParamName = this.ParamName,
                BoxCSSClass = this.BoxCSSClass,
                NavigationLinkCSSClass = this.NavigationLinkCSSClass,
                CurrentPositionCSSClass = this.CurrentPositionCSSClass
            };
        }

        #endregion
    }
}