#region Import

using System;
using ASC.Files.Core;
using ASC.Web.Files.Api;

#endregion

namespace ASC.CRM.Core.Dao
{

    public class FileDao : AbstractDao
    {

        #region Constructor

        public FileDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {
        }

        #endregion

        #region Public Methods

        public File GetFile(int id, int version)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                var file = 0 < version ? dao.GetFile(id, version) : dao.GetFile(id);
                return file;
            }
        }

        public void DeleteFile(int id, int version)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                dao.DeleteFile(id);
            }
        }

        public object GetRoot()
        {
            return FilesIntegration.RegisterBunch("crm", "crm_common", "");
        }

        public File SaveFile(File file, System.IO.Stream stream)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                return dao.SaveFile(file, stream);
            }
        }

        #endregion

    }
}