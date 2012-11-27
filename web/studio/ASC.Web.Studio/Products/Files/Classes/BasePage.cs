using System;
using ASC.Web.Studio;

namespace ASC.Web.Files
{
    public abstract class BasePage : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageLoad();
        }

        protected abstract void PageLoad();
    }
}