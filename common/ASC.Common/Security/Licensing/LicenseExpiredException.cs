using System.Web;

namespace ASC.Common.Security.Licensing
{
    public class LicenseExpiredException : HttpException
    {
        private readonly string[] _reasons;

        public LicenseExpiredException() : this(new string[] {})
        {
        }

        public LicenseExpiredException(string[] reasons) : base(402, "License invalid")
        {
            _reasons = reasons;
        }

        public string[] Reasons
        {
            get { return _reasons; }
        }
    }
}