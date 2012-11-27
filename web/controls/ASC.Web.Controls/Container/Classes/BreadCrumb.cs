
namespace ASC.Web.Controls
{
    public class BreadCrumb
    {
        public string Caption
        {
            get;
            set;
        }

        public string NavigationUrl
        {
            get;
            set;
        }


        public BreadCrumb()
        {

        }

        public BreadCrumb(string caption)
        {
            Caption = caption;
        }

        public BreadCrumb(string caption, string navigationUrl)
            : this(caption)
        {
            NavigationUrl = navigationUrl;
        }
    }
}
