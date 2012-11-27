using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects.Classes
{
    static class ReportTransformer
    {
        public static string Transform(IList<object[]> report, ReportType type, int subType, ReportViewType view)
        {
            var xml = new StringBuilder()
                .Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
                .Append("<reportResult>");
            foreach (var row in report)
            {
                xml.Append("<r ");
                for (int i = 0; i < row.Length; i++)
                {
                    xml.AppendFormat("c{0}=\"{1}\" ", i, ToString(row[i]));
                }
                xml.Append("/>");
            }
            xml.Append("</reportResult>");

            return Transform(xml.ToString(), type, subType, view);
        }

        public static string Transform(string xml, ReportType type, int subType, ReportViewType view)
        {
            if (view == ReportViewType.Xml)
            {
                return Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(xml)));
            }
            if (view == ReportViewType.EMail)
            {
                xml = Transform(xml, type, subType, ReportViewType.Html);
            }

            var xslt = GetXslTransform(type, subType, view);
            if (xslt == null) throw new InvalidOperationException("Xslt not found for type " + type + " and view " + view);

            using (var reader = XmlReader.Create(new StringReader(xml.ToString())))
            using (var writer = new StringWriter())
            using (var xmlwriter = XmlWriter.Create(writer, new XmlWriterSettings() { Encoding = Encoding.UTF8 }))
            {
                xslt.Transform(reader, GetXslParameters(type, view), writer);
                return writer.ToString();
            }
        }


        private static string ToString(object value)
        {
            if (value == null) return null;
            if (value is Enum) return ((Enum)value).ToString("d");
            if (value is DateTime) return ((DateTime)value).ToString("o");
            return value.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        private static XslCompiledTransform GetXslTransform(ReportType reportType, ReportViewType viewType)
        {
            return GetXslTransform(string.Format("{0}.{1}.xsl", reportType, viewType));
        }

        private static XslCompiledTransform GetXslTransform(ReportType reportType, int subType, ReportViewType viewType)
        {
            return GetXslTransform(string.Format("{0}_{1}.{2}.xsl", reportType, subType, viewType)) ??
                GetXslTransform(reportType, viewType) ??
                GetXslTransform(string.Format("{0}.xsl", viewType));
        }

        private static XslCompiledTransform GetXslTransform(string fileName)
        {
            //use const products\projects\data\templates for async not in http request
            fileName = Path.Combine(HttpRuntime.AppDomainAppPath, "products\\projects\\templates\\" + fileName).ToLower();
            if (File.Exists(fileName))
            {
                var transform = new XslCompiledTransform();
                transform.Load(fileName);
                return transform;
            }
            return null;
        }

        private static XsltArgumentList GetXslParameters(ReportType reportType, ReportViewType view)
        {
            var parameters = new XsltArgumentList();
            if (view == ReportViewType.EMail)
            {
                parameters.AddParam("p0", string.Empty, CommonLinkUtility.GetFullAbsolutePath("~/products/projects/templates.aspx"));
                parameters.AddParam("p1", string.Empty, ReportResource.ChangeSettings);
            }
            else if (view == ReportViewType.Csv)
            {
                var csvColumns = ReportHelper.GetCsvColumnsName(reportType);
                for (int i = 0; i < csvColumns.Length; i++)
                {
                    parameters.AddParam("p" + i, string.Empty, csvColumns[i]);
                }
            }
            else
            {
                var columns = ReportHelper.GetReportInfo(reportType).Columns;
                for (int i = 0; i < columns.Length; i++)
                {
                    parameters.AddParam("p" + i, string.Empty, columns[i]);
                }
            }
            return parameters;
        }
    }
}