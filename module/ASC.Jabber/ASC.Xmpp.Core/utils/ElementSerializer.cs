// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="ElementSerializer.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

#region using

using System.Text;
using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.utils
{
    public class ElementSerializer
    {
        public static string SerializeElement(Node element)
        {
            return element.ToString(Encoding.UTF8);
        }

        public static T DeSerializeElement<T>(string serialized) where T : class
        {
            var doc = new Document();
            doc.LoadXml(string.Format("<root>{0}</root>", serialized));
            return doc.RootElement.FirstChild as T;
        }
    }
}