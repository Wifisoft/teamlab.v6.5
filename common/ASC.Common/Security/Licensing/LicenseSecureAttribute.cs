using System;
using System.Security;
using System.Security.Permissions;

namespace ASC.Common.Security.Licensing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    [Serializable]
    public class LicenseSecureAttribute : CodeAccessSecurityAttribute
    {
        private string[] _features;


        public LicenseSecureAttribute(SecurityAction action)
            : base(action)
        {
        }

        public string Features
        {
            get { return _features != null ? string.Join(",", _features) : ""; }
            set
            {
                if (value != null)
                {
                    _features = value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    _features = null;
                }
            }
        }


        public override IPermission CreatePermission()
        {
            if (LicenseManager.ValidateLicense(_features))
                return null;
            throw new LicenseExpiredException();
        }
    }
}