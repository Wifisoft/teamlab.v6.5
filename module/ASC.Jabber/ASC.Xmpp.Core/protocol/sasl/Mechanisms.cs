// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Mechanisms.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using ASC.Xmpp.Core.utils.Xml.Dom;

//	<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//		<mechanism>DIGEST-MD5</mechanism>
//		<mechanism>PLAIN</mechanism>
//	</mechanisms>

namespace ASC.Xmpp.Core.protocol.sasl
{
    /// <summary>
    ///   Summary description for Mechanisms.
    /// </summary>
    public class Mechanisms : Element
    {
        public Mechanisms()
        {
            TagName = "mechanisms";
            Namespace = Uri.SASL;
        }

        public Mechanism[] GetMechanisms()
        {
            ElementList elements = SelectElements("mechanism");

            var items = new Mechanism[elements.Count];
            int i = 0;
            foreach (Element e in elements)
            {
                items[i] = (Mechanism) e;
                i++;
            }
            return items;
        }

        public bool SupportsMechanism(MechanismType type)
        {
            foreach (Mechanism m in GetMechanisms())
            {
                if (m.MechanismType == type)
                    return true;
            }
            return false;
        }
    }
}