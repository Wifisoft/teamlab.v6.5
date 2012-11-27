using System;
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Studio.Core
{
    [Serializable]
    [DataContract]
    public sealed class StudioPasswordSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("aa93a4d1-012d-4ccd-895a-e094e809c840"); }
        }

        /// <summary>
        /// Minimal length password has
        /// </summary>
        [DataMember]
        public int MinLength { get; set; }

        /// <summary>
        /// Password must contains upper case
        /// </summary>
        [DataMember]
        public bool UpperCase { get; set; }

        /// <summary>
        /// Password must contains digits
        /// </summary>
        [DataMember]
        public bool Digits { get; set; }

        /// <summary>
        /// Password must contains special symbols
        /// </summary>
        [DataMember]
        public bool SpecSymbols { get; set; }

        public ISettings GetDefault()
        {
            return new StudioPasswordSettings { MinLength = 6, UpperCase = false, Digits = false, SpecSymbols = false };
        }
    }
}
