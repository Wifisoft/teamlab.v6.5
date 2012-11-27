using System;
using System.Web.UI.WebControls;

namespace ASC.Web.Controls
{
    public class Tab : Panel
    {
        internal delegate void TabSelectedSetHandler(string id);
        internal event TabSelectedSetHandler TabSelectedSected;

        public string ImageUrl
        {
            get
            {
                if(ViewState["ImageUrl"] == null)
                {
                    return string.Empty;
                }
                return ViewState["ImageUrl"].ToString();
            }
            set
            {
                ViewState["ImageUrl"] = value;
            }
        }

        public string Name
        {
            get
            {
                if (ViewState["Name"] == null)
                {
                    return string.Empty;
                }
                return ViewState["Name"].ToString();
            }
            set
            {
                ViewState["Name"] = value;
            }
        }

        public bool Selected
        {
            get
            {

                return Convert.ToBoolean(ViewState["Selected"]);
            }
            set
            {
                ViewState["Selected"] = value;
                if(TabSelectedSected != null)
                {
                    TabSelectedSected(this.ID);
                }
            }
        }

        public bool ClientSideSwitch
        {
            get
            {
                return Convert.ToBoolean(ViewState["ClientSideSwitch"]);
            }
            set
            {
                ViewState["ClientSideSwitch"] = value;
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (ClientSideSwitch)
            {
                if (!Selected)
                {
                    this.Attributes.Add("style", string.Format("{0} {1}", this.Attributes["style"], "display:none;"));
                }

            }
            else
            {
                if (!Selected)
                {
                    return;
                }
            }

            base.Render(writer);
        }
    }
}
