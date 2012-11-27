using System;
using System.Collections.Generic;
using System.Web;

namespace ASC.Web.UserControls.Wiki.Handlers
{

    public class WikiPageHandler : IHttpModule
    {

        #region IHttpModule Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(Application_BeginRequest);
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            string wikiView = WikiSection.Section.MainPage.WikiView;
            string wikiEdit = WikiSection.Section.MainPage.WikiEdit;

            HttpApplication app = (HttpApplication)sender;
            string path = app.Request.RawUrl;
            if (path.Contains(wikiView))
            {
                path = path.Substring(path.IndexOf(wikiView) + wikiView.Length).TrimStart('/').Split('?')[0].Trim();
                if(string.IsNullOrEmpty(path))
                {
                    path = WikiSection.Section.MainPage.Url.Split('?')[0];
                }
                else
                {
                    path = string.Format(WikiSection.Section.MainPage.Url, path);
                }

                app.Context.RewritePath(path);
            }
            else if (path.Contains(wikiEdit))
            {
                path = path.Substring(path.IndexOf(wikiEdit) + wikiEdit.Length).TrimStart('/').Trim();
                path = path.Replace("?", "&");
                if(path[0] == '&')
                {
                    path = string.Format("?{0}", path.Substring(1));
                    path = string.Format("{0}{1}", WikiSection.Section.MainPage.Url.Split('?')[0], path);

                }
                else
                {
                    path = string.Format(WikiSection.Section.MainPage.Url, path);
                }
                
                app.Context.RewritePath(path);
            }
            
        }

        #endregion
    }
}
