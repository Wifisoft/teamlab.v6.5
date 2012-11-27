using System;
using System.Xml;

namespace BasecampRestAPI
{
	//<person>
	//    <administrator type="boolean">true</administrator>
	//    <client-id type="integer">0</client-id>
	//    <deleted type="boolean">false</deleted>
	//    <email-address>gumby@dammit.com</email-address>
	//    <first-name>Gumby</first-name>
	//    <has-access-to-new-projects type="boolean">false</has-access-to-new-projects>
	//    <id type="integer">3514656</id>
	//    <identity-url nil="true"></identity-url>
	//    <im-handle></im-handle>
	//    <im-service>AOL</im-service>
	//    <last-name>Dammit</last-name>
	//    <password>i'mgumbydammit</password>
	//    <phone-number-fax></phone-number-fax>
	//    <phone-number-home></phone-number-home>
	//    <phone-number-mobile></phone-number-mobile>
	//    <phone-number-office>302.555.1212</phone-number-office>
	//    <phone-number-office-ext>740</phone-number-office-ext>
	//    <title>Leading Clay Toy</title>
	//    <token>32f9a2dd7cbf3c2de1615f8f3636eb2d</token>
	//    <user-name>gumby</user-name>
	//    <uuid>2c1412da-21e0-33fd-a9b5-142f714ca610</uuid>
	//</person>
    //additional data <can-post>1</can-post>
	public class Person : IPerson
	{

		public static IPerson GetInstance(IBaseCamp camp, XmlNode node)
		{
			return new Person(camp, node);
		}
		private Person(IBaseCamp camp, XmlNode node)
		{
			_camp = camp;
			ID = XmlHelpers.ParseInteger(node, "id");
			FirstName = XmlHelpers.ChildNodeText(node, "first-name");
			LastName = XmlHelpers.ChildNodeText(node, "last-name");
			EmailAddress = XmlHelpers.ChildNodeText(node, "email-address");
			PhoneNumberOffice = XmlHelpers.ChildNodeText(node, "phone-number-office");
			PhoneNumberOfficeExt = XmlHelpers.ChildNodeText(node, "phone-number-office-ext");
			PhoneNumberMobile = XmlHelpers.ChildNodeText(node, "phone-number-mobile");
			PhoneNumberHome = XmlHelpers.ChildNodeText(node, "phone-number-home");
			PhoneNumberFax = XmlHelpers.ChildNodeText(node, "phone-number-fax");
			UserName = XmlHelpers.ChildNodeText(node, "user-name");
			Administrator = XmlHelpers.ParseBool(node, "administrator");
			Deleted = XmlHelpers.ParseBool(node, "deleted");
            Title = XmlHelpers.ChildNodeText(node, "title");
            ImService = XmlHelpers.ChildNodeText(node, "im-service");
            ImHandle = XmlHelpers.ChildNodeText(node, "im-handle");

            CanPost = XmlHelpers.ParseInteger(node, "can-post");
            HasAccessToNewProjects = XmlHelpers.ParseBool(node, "has-access-to-new-projects");
            AvatarUrl = XmlHelpers.ChildNodeText(node, "avatar-url");
		}
		private readonly IBaseCamp _camp;

		#region Implementation of IPerson

		public int ID { get; private set; }
		public bool Administrator { get; private set; }
		public bool Deleted { get; private set; }
		public string EmailAddress { get; private set; }
		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public string PhoneNumberFax { get; private set; }
		public string PhoneNumberHome { get; private set; }
		public string PhoneNumberMobile { get; private set; }
		public string PhoneNumberOffice { get; private set; }
		public string PhoneNumberOfficeExt { get; private set; }
		public string UserName { get; private set; }
        public string Title { get; private set; }
        public string ImService { get; private set; }
        public string ImHandle { get; private set; }

        public int CanPost { get; private set; }
        public bool HasAccessToNewProjects { get; private set; }
        public string AvatarUrl { get; private set; }

		#endregion

		public static string PathForId(int id)
		{
			return string.Format("people/{0}.xml", id);
		}
	}
}
