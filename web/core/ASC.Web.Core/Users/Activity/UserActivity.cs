using System;
using System.Reflection;
using ASC.Common.Utils;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Core.Users.Activity
{    
	public class UserActivity : ICloneable
    {
        public virtual long ID { get; set; }   

        public virtual Guid UserID { get; set; }

        public virtual string ContentID { get; set; }

        public virtual int ActionType { get; set; }

        public virtual string ActionText { get; set; }

        public virtual string URL { get; set; }

        public virtual string Title { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual int BusinessValue { get; set; }  

        public virtual Guid ProductID { get; set; }

        public virtual Guid ModuleID { get; set; }

        public virtual string ContainerID { get; set; }

        public virtual string AdditionalData { get; set; }

        public virtual ImageOptions ImageOptions { get; set; }

        public virtual int TenantID { get; set; }

        public bool SecurityFiltered { get; set; }

	    public virtual string HtmlPreview
	    {
	        get;
	        set;
	    }

        public virtual string SecurityId
        {
            get;
            set;
        }

	    #region ICloneable Members

        public object Clone()
        {
            return new UserActivity()
            {
                ID = this.ID,
                ContentID = this.ContentID,
                ActionText = this.ActionText,
                ActionType = this.ActionType,
                AdditionalData = this.AdditionalData,
                BusinessValue = this.BusinessValue,
                ContainerID = this.ContainerID,
                Date = this.Date,
                ModuleID = this.ModuleID,
                ProductID = this.ProductID,
                Title = this.Title,
                URL = this.URL,
                UserID = this.UserID,
                ImageOptions = this.ImageOptions,
                TenantID =this.TenantID,
                HtmlPreview = this.HtmlPreview,
                SecurityId = this.SecurityId
            };
        }

        #endregion
    }

    public class UserActivityEventArgs : EventArgs 
    {
        public UserActivity UserActivity { get; set; }
    }

    public interface IUserActivityPublisher
    {   
        event EventHandler<UserActivityEventArgs> DoUserActivity;
    }
}
