using System;
using ASC.Core;
using ASC.Web.Studio;

namespace ASC.Web.Community.PhotoManager
{
    public abstract class BasePage : MainPage
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(ASC.PhotoManager.Helpers.NHibernateHelper.PhotoManagerSessionFactoryConfigPath))
            //    ASC.PhotoManager.Helpers.NHibernateHelper.PhotoManagerSessionFactoryConfigPath = Page.MapPath("~\\Addons\\PhotoManager\\config\\NHibernate.cfg.xml");
            PageLoad();
        }

        protected string GetLimitedText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            return text.Length > ASC.PhotoManager.PhotoConst.MAX_TEXT_LENGTH ? text.Substring(0, ASC.PhotoManager.PhotoConst.MAX_TEXT_LENGTH) : text;
        }

        #endregion

        #region Properties

        public Guid currentUserID
        {
            get
            {
                return SecurityContext.CurrentAccount.ID;
            }
        }

        public string UserID
        {
            get
            {
                return Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_USER] != null ? Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_USER] : null;
            }
        }
        public string AlbumID
        {
            get
            {
                return Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_ALBUM] != null ? Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_ALBUM] : null;
            }
        }
        
        public string PhotoID
        {
            get
            {
                return Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_PHOTO] != null ? Request.QueryString[ASC.PhotoManager.PhotoConst.PARAM_PHOTO] : null;
            }
        }
        public int CountSmallPhoto
        {
            get
            {
                return ASC.PhotoManager.PhotoConst.countSmallPhoto;
            }
        }
        public int CountMediumPhoto
        {
            get
            {
                return ASC.PhotoManager.PhotoConst.countMediumPhoto;
            }
        }

        #endregion

       
        #region Methods
        
        protected abstract void PageLoad();

        protected int GetCountSmallPhoto(Guid userID)
        {
            return ASC.PhotoManager.PhotoConst.countSmallPhoto;
        }
        protected int GetCountMediumPhoto(Guid userID)
        {
            return ASC.PhotoManager.PhotoConst.countMediumPhoto;
        }

        #endregion
    }
}

