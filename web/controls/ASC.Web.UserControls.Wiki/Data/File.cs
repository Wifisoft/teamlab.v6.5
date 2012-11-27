using System;
using ASC.Core.Tenants;

namespace ASC.Web.UserControls.Wiki.Data
{
    public class File : IVersioned
    {
        public int Tenant { get; set; }
        public Guid UserID { get; set; }
        public int Version { get; set; }
        public DateTime Date { get; set; }

        public Guid OwnerID { get; set; }
        public object GetObjectId()
        {
            return FileName;
        }

        private string _FileName = string.Empty;
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value.Replace("[", "(").Replace("]", ")"); }
        }               
        public string UploadFileName { get; set; }
        public string FileLocation { get; set; }
        public int FileSize { get; set; }


        public File()
        {
            UploadFileName = FileLocation = string.Empty;
            Date = TenantUtil.DateTimeNow();
        }      
    }
}