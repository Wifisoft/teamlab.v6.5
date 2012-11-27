using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace ASC.Data.Storage.Configuration
{
    public class HandlerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty(Schema.NAME, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) this[Schema.NAME]; }
            set { this[Schema.NAME] = value; }
        }

        [ConfigurationProperty(Schema.TYPE, IsRequired = true)]
        public string TypeName
        {
            get { return (string) this[Schema.TYPE]; }
            set { this[Schema.TYPE] = value; }
        }

        [ConfigurationProperty(Schema.PROPERTIES, IsDefaultCollection = false)]
        public NameValueConfigurationCollection HandlerProperties
        {
            get { return (NameValueConfigurationCollection) this[Schema.PROPERTIES]; }
            set { this[Schema.PROPERTIES] = value; }
        }

        public Type Type
        {
            get { return Type.GetType(TypeName, true); }
        }

        public IDictionary<string, string> GetProperties()
        {
            var properties = new Dictionary<string, string>();
            foreach (NameValueConfigurationElement nameValuePair in HandlerProperties)
            {
                properties.Add(nameValuePair.Name, nameValuePair.Value);
            }
            return properties;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            if (elementName == Schema.PROPERTY)
            {
                HandlerProperties.Add(new NameValueConfigurationElement(reader.GetAttribute(0), reader.GetAttribute(1)));
                return true;
            }
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}