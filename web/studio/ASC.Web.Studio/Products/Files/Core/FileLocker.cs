using System;
using System.Collections.Generic;
using System.Security;
using System.Web.Configuration;
using ASC.Web.Files.Resources;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Files.Core
{
    public class FileLocker
    {
        private static readonly Dictionary<string, FileLocker> NowEditing = new Dictionary<string, FileLocker>();
        private static readonly TimeSpan EditTimeout;

        private readonly DateTime _date;
        private readonly Guid _editingBy;

        static FileLocker()
        {
            EditTimeout = TimeSpan.FromMilliseconds(Convert.ToInt32(WebConfigurationManager.AppSettings["files.docservice.edit-timeout"] ?? "6000"));
        }

        private FileLocker()
        {
            _date = DateTime.UtcNow;
            _editingBy = SecurityContext.CurrentAccount.ID;
        }

        public static void Add(object fileId)
        {
            lock (NowEditing)
            {
                if (IsLocked(fileId) && !NowEditing[fileId.ToString()]._editingBy.Equals(SecurityContext.CurrentAccount.ID))
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

                NowEditing[fileId.ToString()] = new FileLocker();
            }
        }

        public static void Remove(object fileId)
        {
            lock (NowEditing)
                NowEditing.Remove(fileId.ToString());
        }

        public static bool IsLocked(object fileId)
        {
            lock (NowEditing)
            {
                if (NowEditing.ContainsKey(fileId.ToString()))
                {
                    if ((DateTime.UtcNow - NowEditing[fileId.ToString()]._date).Duration() > EditTimeout)
                    {
                        Remove(fileId);
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static Guid GetLockedBy(object fileId)
        {
            lock (NowEditing)
                return IsLocked(fileId) ? NowEditing[fileId.ToString()]._editingBy : Guid.Empty;
        }
    }
}