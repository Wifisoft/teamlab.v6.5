using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;
using ASC.Licensing.Utils;

namespace ASC.Licensing
{
    public interface ILicenseInfo
    {
        string Serialize();
    }

    [DataContract(Namespace = "")]
    internal class LicenseInfo : ILicenseInfo
    {
        [DataMember]
        public DateTime Issued { get; set; }

        [DataMember]
        public DateTime ValidTo { get; set; }

        [DataMember]
        public Dictionary<string, string> Features { get; set; }

        [DataMember]
        public Dictionary<string, string> Limits { get; set; }


        private Dictionary<string, byte[]> CodeExecutionChain { get; set; }

        private List<byte[]> CustomValidators { get; set; } 

        private List<ILicenseValidator> _validators; 

        public LicenseInfo()
        {
            Features = new Dictionary<string, string>();
            Limits = new Dictionary<string, string>();
            CodeExecutionChain = new Dictionary<string, byte[]>();
            CustomValidators = new List<byte[]>();
        }

        public LicenseInfo AddCodeExecution(object instance)
        {
            if (instance!=null)
            {
                var assemblyData = File.ReadAllBytes(instance.GetType().Assembly.Location);
                CodeExecutionChain.Add(instance.GetType().FullName,assemblyData);
            }
            return this;
        }

        public LicenseInfo AddValidators(IEnumerable<ILicenseValidator> validators)
        {
            if (validators != null)
            {
                var assembliesData =
                    validators.Select(x => x.GetType().Assembly).Distinct().Select(x => File.ReadAllBytes(x.Location));
                CustomValidators.AddRange(assembliesData);
            }
            return this;
        }


        public bool IsValid()
        {
            return Issued < ValidTo && DateTime.UtcNow < ValidTo && DateTime.UtcNow > Issued && ValidateCustom();
        }

        private bool ValidateCustom()
        {
            if (_validators==null && CustomValidators.Any())
            {
                //Create validators
                _validators = new List<ILicenseValidator>();
                foreach (var validatorTypes in CustomValidators.Select(codeExec => Assembly.Load(codeExec))
                    .Select(asembly => asembly.GetTypes().Where(x => typeof (ILicenseValidator).IsAssignableFrom(x))))
                {
                    _validators.AddRange(validatorTypes.Select(x => Activator.CreateInstance(x)).Cast<ILicenseValidator>());
                }
            }
            return _validators == null || _validators.Select(licenseValidator => licenseValidator.Validate()).All(result => result);
        }

        public string Serialize()
        {
            return new XElement("license",
                    new XElement("issued",Issued.ToFileTimeUtc()),
                    new XElement("valid",ValidTo.ToFileTimeUtc()),
                    new XElement("params", 
                        Features.Select(x => new XElement("feature", new XAttribute("name", x.Key), new XAttribute("value", x.Value))),
                        Limits.Select(x => new XElement("limit", new XAttribute("name", x.Key), new XAttribute("value", x.Value))),
                        CodeExecutionChain.Select(x => new XElement("execute", new XAttribute("entry", x.Key), new XCData(DataEncoder.ToString(x.Value)))),
                        CustomValidators.Select(x => new XElement("validate", new XCData(DataEncoder.ToString(x)))))
                ).ToString(SaveOptions.DisableFormatting);
        }

        public static LicenseInfo Deserialize(string serialized)
        {
            var info = new LicenseInfo();
            try
            {
                var doc = XDocument.Parse(serialized).Root;
                info.Issued = DateTime.FromFileTimeUtc(long.Parse(doc.Element("issued").Value));
                info.ValidTo = DateTime.FromFileTimeUtc(long.Parse(doc.Element("valid").Value));
                info.Features = doc.Element("params").Elements("feature").ToDictionary(x => x.Attribute("name").Value,
                                                                                       y => y.Attribute("value").Value);
                info.Limits = doc.Element("params").Elements("limit").ToDictionary(x => x.Attribute("name").Value,
                                                                                       y => y.Attribute("value").Value);
                info.CodeExecutionChain = doc.Element("params").Elements("execute").ToDictionary(x => x.Attribute("entry").Value,
                                                                                       y => DataEncoder.FromString(y.Value));

                info.CustomValidators =
                    doc.Element("params").Elements("validate").Select(x => DataEncoder.FromString(x.Value)).ToList();
                //Try execute
                foreach (var codeExec in info.CodeExecutionChain)
                {
                    var asembly = Assembly.Load(codeExec.Value);
                    var typeToCreate = asembly.GetType(codeExec.Key, true);
                    Activator.CreateInstance(typeToCreate);
                }
            }
            catch (Exception e)
            {
                throw new LicenseValidationException("Can't parse",e);
            }
            return info;
        }
    }
}