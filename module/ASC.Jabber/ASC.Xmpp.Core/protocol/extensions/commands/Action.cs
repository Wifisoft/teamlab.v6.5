// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Action.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace ASC.Xmpp.Core.protocol.extensions.commands
{
    /*
      <xs:attribute name='action' use='optional'>
        <xs:simpleType>
          <xs:restriction base='xs:NCName'>
            <xs:enumeration value='cancel'/>
            <xs:enumeration value='complete'/>
            <xs:enumeration value='execute'/>
            <xs:enumeration value='next'/>
            <xs:enumeration value='prev'/>
          </xs:restriction>
        </xs:simpleType>
      </xs:attribute>
    */

    public enum Action
    {
        NONE = -1,
        next = 1,
        prev = 2,
        complete = 4,
        execute = 8,
        cancel = 16
    }
}