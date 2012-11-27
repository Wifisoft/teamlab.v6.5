// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Compress.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.compression
{
    // <compress xmlns="http://jabber.org/protocol/compress">
    //      <method>zlib</method>
    // </compress>

    public class Compress : Element
    {
        #region << Constructors >>

        public Compress()
        {
            TagName = "compress";
            Namespace = Uri.COMPRESS;
        }

        /// <summary>
        ///   Constructor with a given method/algorithm for Stream compression
        /// </summary>
        /// <param name="method"> method/algorithm used to compressing the stream </param>
        public Compress(CompressionMethod method) : this()
        {
            Method = method;
        }

        #endregion

        /// <summary>
        ///   method/algorithm used to compressing the stream
        /// </summary>
        public CompressionMethod Method
        {
            set
            {
                if (value != CompressionMethod.Unknown)
                    SetTag("method", value.ToString());
            }
            get { return (CompressionMethod) GetTagEnum("method", typeof (CompressionMethod)); }
        }
    }
}