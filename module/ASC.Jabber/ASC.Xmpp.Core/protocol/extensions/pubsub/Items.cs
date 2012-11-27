// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Items.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
        <xs:element name='items'>
            <xs:complexType>
              <xs:sequence>
                <xs:element ref='item' minOccurs='0' maxOccurs='unbounded'/>
              </xs:sequence>
              <xs:attribute name='max_items' type='xs:positiveInteger' use='optional'/>
              <xs:attribute name='node' type='xs:string' use='required'/>
              <xs:attribute name='subid' type='xs:string' use='optional'/>
            </xs:complexType>
        </xs:element>
     
        <iq type='get'
            from='francisco@denmark.lit/barracks'
            to='pubsub.shakespeare.lit'
            id='items1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub'>
            <items node='blogs/princely_musings'/>
          </pubsub>
        </iq>
    */

    public class Items : Publish
    {
        #region << Constructors >>

        public Items()
        {
            TagName = "items";
        }

        public Items(string node) : this()
        {
            Node = node;
        }

        public Items(string node, string subId) : this(node)
        {
            SubId = subId;
        }

        public Items(string node, string subId, int maxItems) : this(node, subId)
        {
            MaxItems = maxItems;
        }

        #endregion

        //public string Node
        //{
        //    get { return GetAttribute("node"); }
        //    set { SetAttribute("node", value); }
        //}

        public string SubId
        {
            get { return GetAttribute("subid"); }
            set { SetAttribute("subid", value); }
        }

        public int MaxItems
        {
            get { return GetAttributeInt("max_items"); }
            set { SetAttribute("max_items", value); }
        }
    }
}