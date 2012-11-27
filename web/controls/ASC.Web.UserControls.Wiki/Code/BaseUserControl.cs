using System;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Handlers;

namespace ASC.Web.UserControls.Wiki
{
    public class VersionEventArgs : EventArgs
    {
        public Guid UserID { get; set; }
        public DateTime Date { get; set; }
        public int Version { get; set; }
    }


    public class BaseUserControl : System.Web.UI.UserControl
    {
        public static readonly string[] reservedPrefixes = new string[] { Constants.WikiCategoryKeyCaption, Constants.WikiInternalKeyCaption };

        private string _mainWikiClassName = "wiki";
        public delegate void PageEmptyHamdler(object sender, EventArgs e);
        public event PageEmptyHamdler PageEmpty;

        public delegate void PublishVersionInfoHandler(object sender, VersionEventArgs e);
        public event PublishVersionInfoHandler PublishVersionInfo;

        public delegate void WikiPageLoadedHandler(bool isNew, IWikiObjectOwner owner);
        public event WikiPageLoadedHandler WikiPageLoaded;

        protected WikiEngine Wiki
        {
            get { return new WikiEngine(); }
        }
        

        public void RiseWikiPageLoaded(IWikiObjectOwner owner)
        {
            RiseWikiPageLoaded(false, owner);
        }
        
        public void RiseWikiPageLoaded(bool isNew, IWikiObjectOwner owner)
        {
            if(WikiPageLoaded != null)
            {
                WikiPageLoaded(isNew, owner);
            }
        }

        public string MainWikiClassName
        {
            get { return _mainWikiClassName; }
            set{ _mainWikiClassName = value; }
        }

        
        protected void RisePageEmptyEvent()
        {
            if (PageEmpty != null)
            {
                PageEmpty(this, new EventArgs());
            }
        }

        protected string PathFromFCKEditor
        {
            get { return WikiSection.Section.FckeditorInfo.PathFrom.ToLower(); }
        }

        protected string BaseFCKRelPath
        {
            get { return WikiSection.Section.FckeditorInfo.BaseRelPath; }
        }

        protected void RisePublishVersionInfo(IVersioned container)
        {
            if (!this.Visible)
                return;

            if (PublishVersionInfo != null)
            {
                PublishVersionInfo(this, new VersionEventArgs()
                {
                    UserID = container.UserID,
                    Date = container.Date,
                    Version = container.Version
                });
            }
        }


        public int TenantId
        {
            get
            {
                if (ViewState["TenantId"] == null)
                    return 0;
                try
                {
                    return Convert.ToInt32(ViewState["TenantId"]);
                }
                catch (System.Exception) { }

                return 0;
            }
            set
            {
                ViewState["TenantId"] = value;
            }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!Page.ClientScript.IsClientScriptBlockRegistered("DefaultWikiCss"))
            {
                string script = string.Format("<link rel='stylesheet' text='text/css' href='{0}' />",
                    Page.ClientScript.GetWebResourceUrl(typeof(BaseUserControl), "ASC.Web.UserControls.Wiki.Css.main.css"));

                Page.ClientScript.RegisterClientScriptInclude("DefaultWikiJs", Page.ClientScript.GetWebResourceUrl(typeof(BaseUserControl), "ASC.Web.UserControls.Wiki.Js.EditPage.js"));
                Page.ClientScript.RegisterClientScriptBlock(typeof(BaseUserControl), "DefaultWikiCss", script);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }


        private string _imageHandlerUrlFormat = WikiSection.Section.ImageHangler.UrlFormat;
        public string ImageHandlerUrlFormat
        {
            get { return _imageHandlerUrlFormat; }
            set { _imageHandlerUrlFormat = value; }
        }
    }
}