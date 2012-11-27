// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Item.cs">
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
      <xs:element name='item'>
        <xs:complexType>
          <xs:sequence minOccurs='0'>
            <xs:any namespace='##other'/>
          </xs:sequence>
          <xs:attribute name='id' type='xs:string' use='optional'/>
        </xs:complexType>
      </xs:element>
    */

    public class Item : Element
    {
        public Item()
        {
            TagName = "item";
            Namespace = Uri.PUBSUB;
        }

        public Item(string id) : this()
        {
            Id = id;
        }

        /// <summary>
        ///   The optional id
        /// </summary>
        public string Id
        {
            get { return GetAttribute("id"); }
            set { SetAttribute("id", value); }
        }
    }
}