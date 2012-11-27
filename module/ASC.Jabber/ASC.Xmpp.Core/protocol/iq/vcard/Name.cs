// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Name.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.vcard
{
    /// <summary>
    /// </summary>
    public class Name : Element
    {
        #region << Constructors >>

        public Name()
        {
            TagName = "N";
            Namespace = Uri.VCARD;
        }

        public Name(string family, string given, string middle) : this()
        {
            Family = family;
            Given = given;
            Middle = middle;
        }

        #endregion

        // <N>
        //	<FAMILY>Saint-Andre<FAMILY>
        //	<GIVEN>Peter</GIVEN>
        //	<MIDDLE/>
        // </N>

        public string Family
        {
            get { return GetTag("FAMILY"); }
            set { SetTag("FAMILY", value); }
        }

        public string Given
        {
            get { return GetTag("GIVEN"); }
            set { SetTag("GIVEN", value); }
        }

        public string Middle
        {
            get { return GetTag("MIDDLE"); }
            set { SetTag("MIDDLE", value); }
        }
    }
}