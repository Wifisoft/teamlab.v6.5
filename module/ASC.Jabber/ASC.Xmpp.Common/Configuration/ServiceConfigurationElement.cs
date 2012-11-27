using System;
using System.Configuration;

namespace ASC.Xmpp.Common.Configuration
{
    public class ServiceConfigurationElement : JabberConfigurationElement
	{
		[ConfigurationProperty(Schema.JID, IsRequired = true)]
		public string Jid
		{
			get { return (string)this[Schema.JID]; }
			set { this[Schema.JID] = value; }
		}

		[ConfigurationProperty(Schema.PARENT)]
		public string Parent
		{
			get { return (string)this[Schema.PARENT]; }
			set { this[Schema.PARENT] = value; }
		}

		
		public ServiceConfigurationElement()
		{
			
		}

		public ServiceConfigurationElement(string jid, string name, Type type)
			: base(name, type)
		{
			Jid = jid;
		}

		public ServiceConfigurationElement(string jid, string name, Type type, string parentJid)
			: this(jid, name, type)
		{
			Parent = parentJid;
		}

    }
}