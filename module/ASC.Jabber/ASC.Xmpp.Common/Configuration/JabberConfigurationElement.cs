using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace ASC.Xmpp.Common.Configuration
{
    public class JabberConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty(Schema.NAME, IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)this[Schema.NAME]; }
			set { this[Schema.NAME] = value; }
		}

		[ConfigurationProperty(Schema.TYPE, IsRequired = true)]
		public string TypeName
		{
			get { return (string)this[Schema.TYPE]; }
			set { this[Schema.TYPE] = value; }
		}

		[ConfigurationProperty(Schema.PROPERTIES, IsDefaultCollection = false)]
		public NameValueConfigurationCollection JabberProperties
		{
			get { return (NameValueConfigurationCollection)this[Schema.PROPERTIES]; }
			set { this[Schema.PROPERTIES] = value; }
		}


		public JabberConfigurationElement()
		{

		}

		public JabberConfigurationElement(string name, Type type)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			if (type == null) throw new ArgumentNullException("type");

			Name = name;
			TypeName = type.FullName;
		}

		public IDictionary<string, string> GetProperties()
		{
			var properties = new Dictionary<string, string>();
			foreach (NameValueConfigurationElement nameValuePair in JabberProperties)
			{
				properties.Add(nameValuePair.Name, nameValuePair.Value);
			}
			return properties;
		}

		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (elementName == Schema.PROPERTY)
			{
				JabberProperties.Add(new NameValueConfigurationElement(reader.GetAttribute(0), reader.GetAttribute(1)));
				return true;
			}
			return base.OnDeserializeUnrecognizedElement(elementName, reader);
		}
	}
}