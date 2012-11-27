// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="DataEncoder.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Text;
using System.Web;

namespace ASC.Licensing.Utils
{
    internal static class DataEncoder
    {
        public static string ToString(byte[] bytes)
        {
            return HttpServerUtility.UrlTokenEncode(bytes);
        }

        public static byte[] FromString(string token)
        {
            return HttpServerUtility.UrlTokenDecode(token);
        }

        public static string ToHexString(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length*2);
            for (int index = 0; index < bytes.Length; index++)
            {
                builder.Append(bytes[index].ToString("X2"));
            }
            return builder.ToString();
        }

        public static byte[] FromHexString(string s)
        {
            var buffer = new byte[s.Length/2];
            for (int i = 0; i < s.Length; i+=2)
            {
                buffer[i/2] = byte.Parse(s.Substring(i, 2), NumberStyles.HexNumber);
            }
            return buffer;
        }

    }
}