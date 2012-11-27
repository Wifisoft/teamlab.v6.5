using System.Collections.Generic;
using System.IO;

namespace ASC.Web.Studio.Core.Import
{
    public class OutlookCSVUserImporter : TextFileUserImporter
    {
        public OutlookCSVUserImporter(Stream fileStream)
            : base(fileStream)
        {
            HasHeader = true;
            Separators = new[] { ';', ',' };
            NameMapping = new Dictionary<string, string>()
                              {
                                  {"First name", "FirstName"},
                                  {"Last name", "LastName"},
                                  {"Middle Name", ""},
                                  {"Name", ""},
                                  {"Nickname", ""},
                                  {"E-mail Address", "Email"},
								  {"Email", "Email"},
                                  {"Home Street", "PrimaryAddress"},
                                  {"Home City", ""},
                                  {"Home Postal Code", "PostalCode"},
                                  {"Home State", ""},
                                  {"Home Country/Region", ""},
                                  {"Home Phone", "PhoneHome"},
                                  {"Home Fax", ""},
                                  {"Mobile Phone", "PhoneMobile"},
                                  {"Personal Web Page", ""},
                                  {"Business Street", ""},
                                  {"Business City", ""},
                                  {"Business Postal Code", ""},
                                  {"Business State", ""},
                                  {"Business Country/Region", ""},
                                  {"Business Web Page", ""},
                                  {"Business Phone", "PhoneOffice"},
                                  {"Business Fax", ""},
                                  {"Pager", ""},
                                  {"Job Title", "Title"},
                                  {"Department", "Department"},
                                  {"Office Location", ""},
                                  {"Notes", "Notes"}
                              };
        }
    }
}