using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Drawing;
using ASC.Web.Controls.CodeFormat;

namespace ASC.Web.Controls
{
    public class Highlight
    {
        public static string HighlightToHTML(string source, LangType type)
        {
            return HighlightToHTML(source, type, false);
        }
        public static string HighlightToHTML(string source, LangType type, bool CustomProtectionTags)
        {
            SourceFormat sf = null;
            
            switch(type)
            {
                case LangType.C:
                case LangType.CPP:
                    sf = new CppFormat();
                    break;
                case LangType.CS:
                    sf = new CSharpFormat();
                    break;
                case LangType.Html:
                case LangType.Xml:
                case LangType.Asp:
                    sf = new HtmlFormat();
                    break;
                case LangType.JS:
                    sf = new JavaScriptFormat();
                    break;
                case LangType.Msh:
                    sf = new MshFormat();
                    break;
                case LangType.TSql:
                    sf = new TsqlFormat();
                    break;
                case LangType.VB:
                    sf = new VisualBasicFormat();
                    break;
            }

            if (sf == null)
                return source;
            sf.CustomProtectedTags = CustomProtectionTags;
            return sf.FormatCode(source);
        }

       
    }
}
