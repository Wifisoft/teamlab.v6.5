using System;
using System.Runtime.Serialization;

namespace ASC.Licensing
{
    [Serializable]
    public class LicenseException : Exception
    {
        public LicenseException()
        {
        }

        public LicenseException(string message)
            : base(message)
        {
        }

        public LicenseException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected LicenseException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public sealed class LicenseValidationException : LicenseException
    {
        public LicenseValidationException()
        {
        }

        public LicenseValidationException(string message)
            : base(message)
        {
        }

        public LicenseValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private LicenseValidationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public sealed  class LicenseNotFoundException : LicenseException
    {
        public LicenseNotFoundException()
        {
        }

        public LicenseNotFoundException(string message)
            : base(message)
        {
        }

        public LicenseNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private LicenseNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public sealed class LicenseCertificateException : LicenseException
    {
        public LicenseCertificateException()
        {
        }

        public LicenseCertificateException(string message)
            : base(message)
        {
        }

        public LicenseCertificateException(string message, Exception inner)
            : base(message, inner)
        {
        }

        private LicenseCertificateException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public sealed  class LicenseFeatureNotFoundException : LicenseException
    {
        public string Feature { get; set; }

        public LicenseFeatureNotFoundException(string feature):this(string.Empty,feature)
        {
        }

        public LicenseFeatureNotFoundException(string message, string feature) : base(string.Format("'{0}' feture is not found. {1}",feature,message))
        {
            Feature = feature;
        }

        public LicenseFeatureNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        private LicenseFeatureNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }


}