using System.Text;

namespace ASC.Web.Controls
{
	public class ViewSwitcherLinkItem : ViewSwitcherBaseItem
	{
        private string _linkCssClass;

        public string LinkCssClass
        {
            get
            {
                if (string.IsNullOrEmpty(this._linkCssClass))
                    return "linkAction";
                return _linkCssClass;
            }
            set
            {
                _linkCssClass = value;
            }
        }

        public bool ActiveItemIsLink
        {
            get; set;
        }

		public override string GetLink()
		{
			StringBuilder sb = new StringBuilder();
            if (!ActiveItemIsLink)
            {
                if (!IsSelected)
                {
                    sb.AppendFormat("<a href=\"{0}\" class='{1}'>{2}</a>", SortUrl, LinkCssClass, SortLabel);
                }
                else
                {
                    sb.Append(SortLabel);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_linkCssClass))
                    sb.AppendFormat("<a href=\"{0}\" class='{1}'>{2}</a>", SortUrl, LinkCssClass, SortLabel);
                else
                {
                    if (!IsSelected)
                        sb.AppendFormat("<a href=\"{0}\" class='{1}'>{2}</a>", SortUrl, LinkCssClass, SortLabel);
                    else
                        sb.AppendFormat("<a href=\"{0}\" class='{1}' style='font-weight:bold;'>{2}</a>", SortUrl, LinkCssClass, SortLabel);
                }
                
            }
			return sb.ToString();
		}

        public ViewSwitcherLinkItem()
        {
            ActiveItemIsLink = false;
        }
	}
}
