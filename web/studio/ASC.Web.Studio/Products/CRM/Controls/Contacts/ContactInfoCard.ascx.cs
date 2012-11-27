#region Import

using System;
using AjaxPro;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.Controls;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Web.CRM.Controls.Contacts
{
    public partial class ContactInfoCard : BaseUserControl
    {
        #region Properties

        public static string Location
        {
            get
            {
                return PathProvider.GetFileStaticRelativePath("Contacts/ContactInfoCard.ascx");
            }
        }

        public int ContactID { get; set; }

        protected Contact Target { get; set; }

        protected bool CanAccess { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Target = Global.DaoFactory.GetContactDao().GetByID(ContactID);
            CanAccess = CRMSecurity.CanAccessTo(Target);
        }

        #endregion

        #region Methods

        protected String RenderContactInfo(ContactInfo contactInfo)
        {

            switch (contactInfo.InfoType)
            {
                case ContactInfoType.Email:
                    return String.Format("<a class='crm-email linkMedium' href='mailto:{0}'>{0}</a><span class='textMediumDescribe'> ({1})</span>", contactInfo.Data.HtmlEncode(), contactInfo.CategoryToString());
                case ContactInfoType.Phone:
                    return String.Format("<div class='crm-phone'>{0}<span class='textMediumDescribe'> ({1})</span></div>", contactInfo.Data.HtmlEncode(), contactInfo.CategoryToString());
                //case ContactInfoType.Website:
                //    return String.Format("<a class='crm-website' href='${0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.Skype:
                //    return String.Format("<div class='crm-skype'>{0}<span class='textSmallDescribe'> ({1})</span></div>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.Twitter:
                //    return String.Format("<a class='crm-twitter' href='http://twitter.com/{0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.LinkedIn:
                //    return String.Format("<a class='crm-linkedin' href='{0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.Facebook:
                //    return String.Format("<a class='crm-facebook' href='http://facebook.com/{0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.LiveJournal:
                //    return String.Format("<a class='rm-livejournal' href='{0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.MySpace:
                //    return String.Format("<a class='crm-myspace' href='{0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.GMail:
                //    return String.Format("<a class='crm-gmail' href='{0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.Blogger:
                //    return String.Format("<a class='crm-blogger' href='{0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.Yahoo:
                //    return String.Format("<a class='crm-yahoo' href='{0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.MSN:
                //    return String.Format("<a class='crm-msn' href='{0}' target='_blank'>${0}</a><span class='textSmallDescribe'> ({1})</span>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.ICQ:
                //    return String.Format("<div class='crm-icq'>{0}<span class='textSmallDescribe'> ({1})</span></div>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.Jabber:
                //    return String.Format("<div class='crm-jabber'>{0}<span class='textSmallDescribe'> ({1})</span></div>", contactInfo.Data, contactInfo.CategoryToString());
                //case ContactInfoType.AIM:
                //    return String.Format("<div class='crm-aim'>{0}<span class='textSmallDescribe'> ({1})</span></div>", contactInfo.Data, contactInfo.CategoryToString());
                case ContactInfoType.Address:
                    var address = JObject.Parse(contactInfo.Data);

                    var street = address[AddressPart.Street.ToString().ToLower()].ToString().HtmlEncode();
                    var city = address[AddressPart.City.ToString().ToLower()].ToString().HtmlEncode();
                    var state = address[AddressPart.State.ToString().ToLower()].ToString().HtmlEncode();
                    var country = address[AddressPart.Country.ToString().ToLower()].ToString().HtmlEncode();
                    var zip = address[AddressPart.Zip.ToString().ToLower()].ToString().HtmlEncode();

                    var text = street;
                    var tmp = String.IsNullOrEmpty(city) ? "" : city + ", ";

                    if (!String.IsNullOrEmpty(state)) { tmp += state + ", "; }
                    if (!String.IsNullOrEmpty(zip)) { tmp += zip; }
                    tmp = tmp.Trim().TrimEnd(',');
                    if (!String.IsNullOrEmpty(tmp)) { text = !String.IsNullOrEmpty(text) ? text + ",<br/>" + tmp : tmp; }
                    text = !String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(country) ? text + ",<br/>" + country : text;
                    
                    var href = "";
                    if (!String.IsNullOrEmpty(street)) { href += street + ", "; }
                    if (!String.IsNullOrEmpty(city)) { href += city + ", "; }
                    if (!String.IsNullOrEmpty(state)) { href += state + ", "; }
                    if (!String.IsNullOrEmpty(zip)) { href += zip + ", "; }
                    if (!String.IsNullOrEmpty(country)) { href += country + ", "; }
                    href = href.Trim().TrimEnd(',');

                    return String.Format("<div class='crm-address'>{0}"
                        + "<span class='textMediumDescribe'> ({2})</span><br/>"
                        + "<a class='linkMedium' style='text-decoration: underline;' href='http://maps.google.com/maps?q={1}' target='_blank'>"
                        + CRMContactResource.ShowOnMap + "</a>"
                        + "</div>", text, href, contactInfo.CategoryToString());

                default:
                    return contactInfo.Data;
            }

        }


        #endregion

    }
}