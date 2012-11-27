using System;
using System.Collections.Generic;
using ASC.Core;

namespace ASC.Web.Studio.Core.SMS
{
    public sealed class StudioSmsKeyStorage
    {
        private static readonly StudioSmsKeyStorage _instance = new StudioSmsKeyStorage();

        private Dictionary<string, string> List { get; set; }

        public StudioSmsKeyStorage()
        {
            List = new Dictionary<string, string>();
        }

        public static StudioSmsKeyStorage Instance
        {
            get { return _instance; }
        }
        
        /// <summary>
        /// Puts a new record to the storage or updates existing if phone number already exists
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="validationKey"></param>
        public void CreateRecord(string phoneNumber, string validationKey)
        {
            lock (List)
            {
                if (List.ContainsKey(BuildKey(phoneNumber)))
                    List[BuildKey(phoneNumber)] = validationKey;
                else
                {
                    List.Add(BuildKey(phoneNumber), validationKey);
                }
            }
        }
        /// <summary>
        /// Returns validation key by phone number if it exists
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public string GetKey(string phoneNumber)
        {
            string value;
            lock (List)
            {
                List.TryGetValue(BuildKey(phoneNumber), out value);
            }
            return value;
        }

        /// <summary>
        /// Deletes record from storage
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public bool DeleteRecord(string phoneNumber)
        {
            lock (List)
            {
                return List.Remove(BuildKey(phoneNumber));
            }
        }

        private string BuildKey(string phoneNumber)
        {
            return String.Concat(phoneNumber, CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }
    }
}
