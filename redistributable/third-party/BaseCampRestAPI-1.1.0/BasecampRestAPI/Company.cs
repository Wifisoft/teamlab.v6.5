using System.Collections.Generic;
using System.Xml;

namespace BasecampRestAPI
{
	/*
		<company>
			<id type="integer">1776</id>
			<name>Peter Griffin Brewery</name>
			<address-one>123 Main Street</address-one>
			<address-two>Suite 456</address-two>
			<city>Anytown</city>
			<state>OH</state>
			<zip>65445</zip>
			<country>USA</country>
			<web-address>http://petergriffinbrewery.example.com</web-address>
			<phone-number-office>343-555-1212</phone-number-office>
			<phone-number-fax>343-555-1212</phone-number-fax>
			<time-zone-id>PST</time-zone-id>
			<can-see-private type="boolean">true</can-see-private>

			<!-- for non-client companies -->
			<url-name>http://petergriffinbrewery.example.com</url-name>
		</company>
	 */
	public class Company : ICompany
	{
		#region Implementation of ICompany
		public int Id { get; private set; }
		public string Name { get; private set; }
		public string Address1 { get; private set; }
		public string Address2 { get; private set; }
		public string City { get; private set; }
		public string State { get; private set; }
		public string ZipCode { get; private set; }
		public string Country { get; private set; }
		public string WebAddress { get; private set; }
		public string PhoneNumberOffice { get; private set; }
		public string PhoneNumberFax { get; private set; }
		public string TimeZoneId { get; private set; }
		public bool CanSeePrivate { get; private set; }
		public string UrlName { get; private set; }
		public IPerson[] People
		{
			get
			{
				string requestPath = string.Format("projects/{0}/people.xml", Id);
				List<IPerson> people = new List<IPerson>();
				foreach (XmlNode node in _service.GetRequestResponseElement(requestPath).ChildNodes)
				{
					people.Add(Person.GetInstance(_camp, node));
				}
				return people.ToArray();
			}
		}

		#endregion

		public static ICompany GetInstance(IBaseCamp camp, XmlNode node)
		{
			return new Company(camp, camp.Service, node);
		}
		private Company(IBaseCamp camp, IRestWebService service, XmlNode node)
		{
			_camp = camp;
			_service = service;
			Id = XmlHelpers.ParseInteger(node, "id");
			Name = XmlHelpers.ChildNodeText(node, "name");
			Address1 = XmlHelpers.ChildNodeText(node, "address-one");
			Address2 = XmlHelpers.ChildNodeText(node, "address-two");
			City = XmlHelpers.ChildNodeText(node, "city");
			State = XmlHelpers.ChildNodeText(node, "state");
			ZipCode = XmlHelpers.ChildNodeText(node, "zip");
			Country = XmlHelpers.ChildNodeText(node, "country");
			WebAddress = XmlHelpers.ChildNodeText(node, "web-address");
			PhoneNumberOffice = XmlHelpers.ChildNodeText(node, "phone-number-office");
			PhoneNumberFax = XmlHelpers.ChildNodeText(node, "phone-number-fax");
			TimeZoneId = XmlHelpers.ChildNodeText(node, "time-zone-id");
			CanSeePrivate = XmlHelpers.ParseBool(node, "can-see-private");
			UrlName = XmlHelpers.ChildNodeText(node, "url-name");
		}
		private readonly IBaseCamp _camp;
		private readonly IRestWebService _service;

		public static string PathForId(int id)
		{
			return string.Format("companies/{0}.xml", id);
		}
	}
}
