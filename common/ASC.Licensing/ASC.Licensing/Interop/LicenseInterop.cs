// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="LicenseInterop.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace ASC.Licensing.Interop
{
    [ComVisible(true)]
    public interface ILicenseInterop
    {
        bool IsValid();
        string GetFeature(string featureName);
        string GetLimit(string limitName);
    }

    [ComVisible(true)]
    public class LicenseInterop : ILicenseInterop
    {
        public bool IsValid()
        {
            return License.Current.IsValid();
        }

        public string GetFeature(string featureName)
        {
            return License.Current.GetFeature(featureName);
        }

        public string GetLimit(string limitName)
        {
            return License.Current.GetLimit(limitName);
        }

    }
}