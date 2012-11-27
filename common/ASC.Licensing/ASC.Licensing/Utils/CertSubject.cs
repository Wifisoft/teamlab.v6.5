// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Subject.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ASC.Licensing.Utils
{
    internal class CertSubject
    {
        public enum KnownField
        {
            Country=0,
            State=1,
            Location=2,
            Organization=3,
            OrganizationUnit=4,
            CanonicalName=5
        }

        private CertSubject()
        { }

        private static readonly Regex Parser = new Regex("(?'name'[A-Z]*)=(?'value'[^,]*)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly string[] NameMap = new[]{"C","ST","L","O","OU","CN"};

        private readonly Dictionary<string,string> _params = new Dictionary<string, string>(); 

        public string Get(KnownField field)
        {
            return Get(NameMap[(int) field]);
        }

        public string Get(string key)
        {
            string value;
            _params.TryGetValue(key, out value);
            return value;
        }

        public static CertSubject Parse(string subject)
        {
            var certSubject = new CertSubject();
            if (!string.IsNullOrEmpty(subject))
            {
                var mathes = Parser.Matches(subject);
                foreach (Match match in mathes)
                {
                    certSubject._params[match.Groups["name"].Value.Trim()] = match.Groups["value"].Value.Trim();
                }
            }

            return certSubject;
        }
    }
}