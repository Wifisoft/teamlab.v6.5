using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ASC.Blogs.Core.Domain
{
	public class Settings
    {
        public virtual Guid ID { get; set; }
        public virtual Guid UserID { get; set; }
        public virtual byte[] Data { get; set; }

        public override bool Equals(object obj)
        {
            Settings settings = obj as Settings;

            if (settings != null
                && settings.ID.Equals(this.ID)
                && settings.UserID.Equals(this.UserID))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode() | this.UserID.GetHashCode();
        }
    }
}
