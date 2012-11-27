// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Affiliation.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
        <affiliation node='node1' jid='francisco@denmark.lit' affiliation='owner'/>
    */

    public class Affiliation : Element
    {
        #region << Constructors >>

        public Affiliation()
        {
            TagName = "affiliation";
            Namespace = Uri.PUBSUB;
        }

        public Affiliation(Jid jid, AffiliationType affiliation)
        {
            Jid = jid;
            AffiliationType = affiliation;
        }

        public Affiliation(string node, Jid jid, AffiliationType affiliation) : this(jid, affiliation)
        {
            Node = node;
        }

        #endregion

        public Jid Jid
        {
            get
            {
                if (HasAttribute("jid"))
                    return new Jid(GetAttribute("jid"));
                else
                    return null;
            }
            set
            {
                if (value != null)
                    SetAttribute("jid", value.ToString());
            }
        }

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public AffiliationType AffiliationType
        {
            get { return (AffiliationType) GetAttributeEnum("affiliation", typeof (AffiliationType)); }
            set { SetAttribute("affiliation", value.ToString()); }
        }
    }
}