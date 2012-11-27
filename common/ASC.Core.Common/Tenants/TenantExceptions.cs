using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantTooShortException : Exception
    {
        public TenantTooShortException(string message)
            : base(message)
        {
        }

        protected TenantTooShortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class TenantIncorrectCharsException : Exception
    {
        public TenantIncorrectCharsException(string message)
            : base(message)
        {
        }

        protected TenantIncorrectCharsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class TenantAlreadyExistsException : Exception
    {
        public IEnumerable<string> ExistsTenants
        {
            get;
            private set;
        }

        public TenantAlreadyExistsException(string message, IEnumerable<string> existsTenants)
            : base(message)
        {
            ExistsTenants = existsTenants ?? Enumerable.Empty<string>();
        }

        protected TenantAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}