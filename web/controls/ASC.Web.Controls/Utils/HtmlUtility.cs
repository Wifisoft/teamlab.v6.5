using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Users;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace ASC.Web.Controls
{
    public class HtmlUtility : HtmlUtil
    {
        private static RegexOptions mainOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
        private static Regex worder = new Regex(@"\S+", mainOptions);

        public static string GetPreview(string html, string replacmentHtml, Guid productID)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(string.Format("<html>{0}</html>", htmlTags.Replace(html, string.Empty)));
            var nodes = doc.DocumentNode.SelectNodes("//div[translate(@class,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='asccut']");
            if (nodes != null)
            {
                HtmlNode newNode;
                HtmlAttribute styleAttr;
                foreach (var node in nodes)
                {
                    newNode = doc.CreateElement("div");
                    styleAttr = doc.CreateAttribute("style");
                    styleAttr.Value = "display:inline;";
                    newNode.Attributes.Append(styleAttr);
                    newNode.InnerHtml = replacmentHtml ?? string.Empty;
                    node.ParentNode.ReplaceChild(newNode, node);
                }
            }
            ProcessCustomTags(doc, productID);
            return htmlTags.Replace(doc.DocumentNode.InnerHtml, string.Empty);
        }

        public static string GetFull(string html, Guid productID)
        {
            html = html ?? string.Empty;
            var doc = new HtmlDocument();
            doc.LoadHtml(string.Format("<html>{0}</html>", htmlTags.Replace(html, string.Empty)));
            var nodes = doc.DocumentNode.SelectNodes("//div[translate(@class,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='asccut']");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    node.Attributes.Remove("class");
                    var styleAttr = doc.CreateAttribute("style");
                    styleAttr.Value = "display:inline;";
                    node.Attributes.Append(styleAttr);
                }
            }

            ProcessCustomTags(doc, productID);
            return htmlTags.Replace(doc.DocumentNode.InnerHtml, string.Empty);
        }


        #region SearchTextHighlight


        /// <summary>
        /// The function highlight all words in htmlText by searchText.
        /// </summary>
        /// <param name="searchText">the space separated string</param>
        /// <param name="htmlText">html for highlight</param>
        /// <returns>highlighted html</returns>
        public static string SearchTextHighlight(string searchText, string htmlText)
        {
            return SearchTextHighlight(searchText, htmlText, Guid.Empty);
        }

        /// <summary>
        /// The function highlight all words in htmlText by searchText.
        /// </summary>
        /// <param name="searchText">the space separated string</param>
        /// <param name="htmlText">html for highlight</param>
        /// <param name="prepereHtml">an input html to be prepare (GetFull)</param>
        /// <returns>highlighted html</returns>
        public static string SearchTextHighlight(string searchText, string htmlText, Guid ProductId)
        {
            return SearchTextHighlight(searchText, htmlText, ProductId, true);
        }

        /// <summary>
        /// The function highlight all words in htmlText by searchText.
        /// </summary>
        /// <param name="searchText">the space separated string</param>
        /// <param name="htmlText">html for highlight</param>
        /// <param name="ProductId">current ProfuctId</param>
        /// <returns>highlighted html</returns>
        public static string SearchTextHighlight(string searchText, string htmlText, bool prepareHtml)
        {
            return SearchTextHighlight(searchText, htmlText, Guid.Empty, prepareHtml);
        }

        //searchTextHighlight

        /// <summary>
        /// The function highlight all words in htmlText by searchText.
        /// </summary>
        /// <param name="searchText">the space separated string</param>
        /// <param name="htmlText">html for highlight</param>
        /// <param name="ProductId">current ProfuctId</param>
        /// <param name="className">custom css class name</param>
        /// <returns>highlighted html</returns>
        public static string SearchTextHighlight(string searchText, string htmlText, Guid ProductId, string className)
        {
            return SearchTextHighlight(searchText, htmlText, ProductId, className, true);
        }

        /// <summary>
        /// The function highlight all words in htmlText by searchText.
        /// </summary>
        /// <param name="searchText">the space separated string</param>
        /// <param name="htmlText">html for highlight</param>
        /// <param name="ProductId">current ProfuctId</param>
        /// <param name="prepereHtml">an input html to be prepare (GetFull)</param>
        /// <returns>highlighted html</returns>
        public static string SearchTextHighlight(string searchText, string htmlText, Guid ProductId, bool prepareHtml)
        {
            return SearchTextHighlight(searchText, htmlText, ProductId, "searchTextHighlight", prepareHtml);
        }


        /// <summary>
        /// The function highlight all words in htmlText by searchText.
        /// </summary>
        /// <param name="searchText">the space separated string</param>
        /// <param name="htmlText">html for highlight</param>
        /// <param name="ProductId">current ProfuctId</param>
        /// <param name="className">custom css class name</param>
        /// <param name="prepereHtml">an input html to be prepare (GetFull)</param>
        /// <returns>highlighted html</returns>
        public static string SearchTextHighlight(string search, string html, Guid ProductId, string className, bool prepare)
        {
            if (string.IsNullOrEmpty(search) || string.IsNullOrEmpty(html)) return html;
            if (prepare) html = GetFull(html, ProductId);

            var regexpstr = worder.Matches(search).Cast<Match>().Select(m => m.Value).Distinct().Aggregate((r, n) => r + "|" + n);
            var wordsFinder = new Regex(Regex.Escape(regexpstr), mainOptions | RegexOptions.Multiline);
            return wordsFinder.Replace(html, m => string.Format("<span class='{0}'>{1}</span>", className, m.Value));
        }

        #endregion



        private static string EscapeHtml(string src)
        {
            //src = src.Replace("&lt;", "<");
            //src = src.Replace("&gt;", "<");
            src = HttpContext.Current.Server.HtmlDecode(src);
            src = src.Replace("&", "&amp;");
            src = src.Replace("<", "&lt;");
            src = src.Replace(">", "&gt;");
            return src;
        }

        private static string GetLanguageAttrValue(HtmlNode node)
        {
            HtmlAttribute attr = node.Attributes["lang"];
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                return attr.Value;
            }
            return string.Empty;
        }

        private static LangType GetLanguage(HtmlNode node)
        {
            LangType result = LangType.Unknown;

            switch (GetLanguageAttrValue(node).ToLower())
            {
                case "c":
                    result = LangType.C;
                    break;
                case "cpp":
                case "c++":
                    result = LangType.CPP;
                    break;
                case "csharp":
                case "c#":
                case "cs":
                    result = LangType.CS;
                    break;
                case "asp":
                    result = LangType.Asp;
                    break;
                case "html":
                case "htm":
                    result = LangType.Html;
                    break;
                case "xml":
                    result = LangType.Xml;
                    break;
                case "js":
                case "jsript":
                case "javascript":
                    result = LangType.JS;
                    break;
                case "sql":
                case "tsql":
                    result = LangType.TSql;
                    break;
                case "vb":
                case "vbnet":
                    result = LangType.VB;
                    break;


            }

            return result;
        }

        private static void ProcessCustomTags(HtmlDocument doc, Guid productID)
        {
            ProcessAscUserTag(doc, productID);
            //ProcessAscDocumentTag(doc, productID);
            ProcessCodeTags(doc, productID);
            ProcessExternalLinks(doc, productID);
            ProcessScriptTag(doc, productID);
            ProcessMaliciousAttributes(doc, productID);
            ProcessZoomImages(doc, productID);
        }

        private static readonly List<string> BlockedAttrs = new List<string>() 
        { 
            "onload", 
            "onunload", 
            "onclick", 
            "ondblclick", 
            "onmousedown", 
            "onmouseup", 
            "onmouseover", 
            "onmousemove", 
            "onmouseout", 
            "onfocus", 
            "onblur", 
            "onkeypress", 
            "onkeydown", 
            "onkeyup",
            "onsubmit", 
            "onreset", 
            "onselect", 
            "onchange"
        };

        private static void ProcessMaliciousAttributes(HtmlDocument doc, Guid productID)
        {
            foreach (var node in doc.DocumentNode.SelectNodes("//*"))
            {
                var toRemove = node.Attributes.Cast<HtmlAttribute>()
                    .Where(htmlAttribute => BlockedAttrs.Contains(htmlAttribute.Name.ToLowerInvariant()) 
                                            || 
                                            htmlAttribute.Value.StartsWith("javascript",StringComparison.OrdinalIgnoreCase)
                                            ||
                                            htmlAttribute.Value.StartsWith("data", StringComparison.OrdinalIgnoreCase)
                                            ||
                                            htmlAttribute.Value.StartsWith("vbscript", StringComparison.OrdinalIgnoreCase)
                                            ).ToList();
                foreach (var htmlAttribute in toRemove)
                {
                    node.Attributes.Remove(htmlAttribute);
                }
            }
        }

        private static Regex rxNumeric = new Regex(@"^[0-9]+$", mainOptions);
        private static void ProcessZoomImages(HtmlDocument doc, Guid productID)
        {
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//img[@_zoom]");
            HtmlNode hrefNode;
            HtmlAttribute borderAttribute, hrefAttribute, srcAttribute, zoomAttribute;

            string imgSrc = string.Empty;
            if (nodes == null)
                return;
            foreach (HtmlNode node in nodes)
            {
                srcAttribute = node.Attributes["src"];
                if (srcAttribute == null || string.IsNullOrEmpty(srcAttribute.Value))
                    continue;

                zoomAttribute = node.Attributes["_zoom"];
                if (zoomAttribute == null || string.IsNullOrEmpty(zoomAttribute.Value))
                    continue;
                borderAttribute = node.Attributes["border"];

                if (borderAttribute == null)
                {
                    borderAttribute = doc.CreateAttribute("border");
                    node.Attributes.Append(borderAttribute);
                }
                borderAttribute.Value = "0";

                imgSrc = srcAttribute.Value;

                if (!rxNumeric.IsMatch(zoomAttribute.Value))
                {
                    imgSrc = zoomAttribute.Value;
                }

                if (node.ParentNode != null)
                {
                    hrefNode = doc.CreateElement("a");

                    hrefAttribute = doc.CreateAttribute("href");
                    hrefAttribute.Value = imgSrc;
                    hrefNode.Attributes.Append(hrefAttribute);

                    hrefAttribute = doc.CreateAttribute("class");
                    hrefAttribute.Value = "fancyzoom";
                    hrefNode.Attributes.Append(hrefAttribute);

                    /*
                    hrefAttribute = doc.CreateAttribute("onclick");
                                        hrefAttribute.Value = string.Format(@"javascript:if(typeof(popimgFckup) == 'function')popimgFckup('{0}');", srcAttribute.Value);
                                        hrefNode.Attributes.Append(hrefAttribute);*/


                    node.ParentNode.ReplaceChild(hrefNode, node);
                    hrefNode.AppendChild(node);
                }
            }

        }

        private static void ProcessScriptTag(HtmlDocument doc, Guid productID)
        {
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//script");

            if (nodes == null || nodes.Count == 0)
                return;

            foreach (HtmlNode node in nodes)
            {
                if (node.ParentNode != null)
                    node.ParentNode.RemoveChild(node);
            }
        }

        private static void ProcessAscUserTag(HtmlDocument doc, Guid productID)
        {
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@__ascuser]");
            HtmlAttribute styleAttr;
            if (nodes == null || nodes.Count == 0)
                return;
            foreach (HtmlNode node in nodes)
            {
                Guid userId = new Guid(node.Attributes["__ascuser"].Value);
                node.Attributes.RemoveAll();
                styleAttr = doc.CreateAttribute("style");
                styleAttr.Value = "display:inline;";
                node.Attributes.Append(styleAttr);
                node.InnerHtml = CoreContext.UserManager.GetUsers(userId).RenderProfileLinkBase(productID);
            }
        }



        private static void ProcessCodeTags(HtmlDocument doc, Guid productID)
        {
            HtmlTextNode textNode;
            HtmlNodeCollection scripts = doc.DocumentNode.SelectNodes("//code");
            if (scripts != null)
            {
                foreach (HtmlNode node in scripts)
                {
                    textNode = doc.CreateTextNode(Highlight.HighlightToHTML(node.InnerHtml, GetLanguage(node), true).Replace(@"class=""codestyle""", string.Format(@"class=""codestyle"" _wikiCodeStyle=""{0}""", GetLanguageAttrValue(node))));
                    node.ParentNode.ReplaceChild(textNode, node);
                }
            }
        }

        private static void ProcessExternalLinks(HtmlDocument doc, Guid productID)
        {
            HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//a");
            if (links == null)
                return;
            HttpContext con = HttpContext.Current;
            string internalHost = con.Request.GetUrlRewriter().Host;
            if ((con.Request.GetUrlRewriter().Port != 80 && con.Request.GetUrlRewriter().Scheme.Equals("http", StringComparison.InvariantCultureIgnoreCase))
                || (con.Request.GetUrlRewriter().Port != 443 && con.Request.GetUrlRewriter().Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase)))
            {
                internalHost = string.Format(@"^{2}:\/\/{0}:{1}", internalHost, con.Request.GetUrlRewriter().Port, con.Request.GetUrlRewriter().Scheme);
            }
            else
            {
                internalHost = string.Format(@"^{2}:\/\/{0}(:{1})?", internalHost, con.Request.GetUrlRewriter().Port, con.Request.GetUrlRewriter().Scheme);
            }

            Regex rxInternalHost = new Regex(internalHost, RegexOptions.Compiled | RegexOptions.CultureInvariant);

            foreach (HtmlNode node in links)
            {
                ProcessExternalLink(node, rxInternalHost);
            }
        }

        private static void ProcessExternalLink(HtmlNode node, Regex rxIntLink)
        {

            HtmlAttribute attrHref = node.Attributes["href"];
            HtmlAttribute attrTarg;
            if (attrHref == null)
                return;
            if (!rxIntLink.IsMatch(attrHref.Value))
            {
                attrTarg = node.Attributes["target"];
                if (attrTarg == null)
                {
                    attrTarg = node.OwnerDocument.CreateAttribute("target");
                    node.Attributes.Append(attrTarg);
                }

                attrTarg.Value = "_blank";
            }

        }
    }
}
