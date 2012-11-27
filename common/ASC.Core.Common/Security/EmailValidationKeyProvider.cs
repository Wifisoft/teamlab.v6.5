using System;
using System.Text;
using ASC.Core;
using log4net;

namespace ASC.Security.Cryptography
{
    public class EmailValidationKeyProvider
    {
        public enum ValidationResult
        {
            Ok,
            Invalid,
            Expired
        }

        private static readonly ILog log = LogManager.GetLogger("ASC.KeyValidation.EmailSignature");
        private static readonly DateTime _from = new DateTime(2010, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        private static byte[] _MachineConstant;


        public static string GetEmailKey(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email");

            email = FormatEmail(email);

            var ms = (long)(DateTime.UtcNow - _from).TotalMilliseconds;
            var hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));
            return string.Format("{0}.{1}", ms, DoStringFromBytes(hash));
        }

        private static string FormatEmail(string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            try
            {
                return string.Format("{0}|{1}", email.ToLowerInvariant(), CoreContext.TenantManager.GetCurrentTenant().TenantId);
            }
            catch (Exception e)
            {
                log.Fatal("Failed to format tenant specific email", e);
                return email.ToLowerInvariant();
            }
        }

        public static ValidationResult ValidateEmailKey(string email, string key)
        {
            return ValidateEmailKey(email, key, TimeSpan.MaxValue);
        }

        public static ValidationResult ValidateEmailKey(string email, string key, TimeSpan validInterval)
        {
            log.DebugFormat("validating '{0}' with key:{1} interval:{2}",email,key,validInterval);
            var result = ValidateEmailKeyInternal(email, key, validInterval);
            log.DebugFormat("validation result:{3}, source: '{0}' with key:{1} interval:{2}", email, key, validInterval,result);
            return result;
        }


        private static ValidationResult ValidateEmailKeyInternal(string email, string key, TimeSpan validInterval)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email");
            if (key == null) throw new ArgumentNullException("key");

            email = FormatEmail(email);
            var parts = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return ValidationResult.Invalid;

            long ms = 0;
            if (!Int64.TryParse(parts[0], out ms)) return ValidationResult.Invalid;

            var hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));
            var key2 = DoStringFromBytes(hash);
            var key2_good = String.Compare(parts[1], key2, StringComparison.InvariantCultureIgnoreCase) == 0;
            if (!key2_good) return ValidationResult.Invalid;
            var ms_current = (long)(DateTime.UtcNow - _from).TotalMilliseconds;
            return validInterval >= TimeSpan.FromMilliseconds(ms_current - ms)?ValidationResult.Ok : ValidationResult.Expired;
        }

        public static byte[] GetMachineConstant()
        {
            if (_MachineConstant == null)
                _MachineConstant = GetMachineConstantInternal();
            return _MachineConstant;
        }

        internal static string DoStringFromBytes(byte[] data)
        {
            string str = Convert.ToBase64String(data);
            str = str.Replace("=", "").Replace("+", "").Replace("/", "").Replace("\\", "");
            return str.ToUpperInvariant();
        }

        internal static byte[] GetMashineHashedData(byte[] salt, byte[] data)
        {
            var alldata = new byte[salt.Length + data.Length];
            Array.Copy(data, alldata, data.Length);
            Array.Copy(salt, 0, alldata, data.Length, salt.Length);
            return Hasher.Hash(alldata, HashAlg.MD5);
        }

        internal static byte[] GetMachineConstantInternal()
        {
            return MachinePseudoKeys.GetMachineConstant();
        }
    }
}