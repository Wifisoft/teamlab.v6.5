#region usings

using System;
using System.Globalization;
using System.Net.Mime;

#endregion

namespace ASC.Api.Utils
{
    public static class StringUtils
    {
        public static string TrimExtension(string path, int startIndex)
        {
            int index = path.LastIndexOf('.');
            if (index != -1 && index > startIndex)
            {
                path = path.Remove(index);
            }
            return path;
        }

        public static string GetExtension(string path)
        {
            int index = path.LastIndexOf('.');
            return index != -1 ? path.Substring(index) : string.Empty;
        }

        public static bool IsContentType(string mediaType, string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && new ContentType(contentType).MediaType == mediaType;
        }

        public static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
                return s;
            string str = char.ToLower(s[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            if (s.Length > 1)
                str = str + s.Substring(1);
            return str;
        }

        public static string FirstPart(this string value, char separator)
        {
            int index;
            if (!string.IsNullOrEmpty(value) && (index=value.IndexOf(separator))!=-1)
            {
                return value.Substring(0, index);
            }
            return value;
        }
    }
}