// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Method.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using ASC.Xmpp.Core.protocol.extensions.compression;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.stream.feature.compression
{
    public class Method : Element
    {
        #region << Constructors >>

        public Method()
        {
            TagName = "method";
            Namespace = Uri.FEATURE_COMPRESS;
        }

        public Method(CompressionMethod method) : this()
        {
            Value = method.ToString();
        }

        #endregion

        /*
         *  <compression xmlns='http://jabber.org/features/compress'>
         *      <method>zlib</method>
         *  </compression>
         * 
         * <stream:features>
         *      <starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>
         *      <compression xmlns='http://jabber.org/features/compress'>
         *          <method>zlib</method>
         *          <method>lzw</method>
         *      </compression>
         * </stream:features>
         */

        public CompressionMethod CompressionMethod
        {
            get
            {
#if CF
				return (CompressionMethod) util.Enum.Parse(typeof(CompressionMethod), this.Value, true);
#else
                return (CompressionMethod) Enum.Parse(typeof (CompressionMethod), Value, true);
#endif
            }
            set { Value = value.ToString(); }
        }
    }
}