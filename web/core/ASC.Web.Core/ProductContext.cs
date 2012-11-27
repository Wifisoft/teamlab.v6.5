using ASC.Common.Security;
using ASC.Web.Core.Subscriptions;
using ASC.Web.Core.Users.Activity;

namespace ASC.Web.Core
{
    public class ProductContext : WebItemContext
    {
        public string MasterPageFile { get; set; }

        public string ProductHTMLOverview {get; set;}

        public IUserActivityControlLoader UserActivityControlLoader { get; set; }

        private IProductSubscriptionManager _sunscriptionManager;
        public new IProductSubscriptionManager SubscriptionManager
        {
            get
            {
                return _sunscriptionManager;

            }
            set
            {
                _sunscriptionManager = value;
                base.SubscriptionManager = value;
            }
        }

		public IWhatsNewHandler WhatsNewHandler { get; set; }
    }
    
}
