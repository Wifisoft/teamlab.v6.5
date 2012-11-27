// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Organization.cs">
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
    public class Organization : Element
    {
        #region << Constructors >>

        public Organization()
        {
            TagName = "ORG";
            Namespace = Uri.VCARD;
        }

        public Organization(string name, string unit) : this()
        {
            Name = name;
            Unit = unit;
        }

        #endregion

        // <ORG>
        //	<ORGNAME>Jabber Software Foundation</ORGNAME>
        //	<ORGUNIT/>
        // </ORG>

        public string Name
        {
            get { return GetTag("ORGNAME"); }
            set { SetTag("ORGNAME", value); }
        }

        public string Unit
        {
            get { return GetTag("ORGUNIT"); }
            set { SetTag("ORGUNIT", value); }
        }
    }
}