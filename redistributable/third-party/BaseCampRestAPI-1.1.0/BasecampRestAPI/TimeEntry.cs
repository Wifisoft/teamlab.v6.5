using System;
using System.Xml;

namespace BasecampRestAPI
{
	//<time-entry>
	//  <id type="integer">#{id}</id>
	//  <project-id type="integer">#{project-id}</project-id>
	//  <person-id type="integer">#{person-id}</person-id>
	//  <date type="date">#{date}</date>
	//  <hours>#{hours}</hours>
	//  <description>#{description}</description>
	//  <todo-item-id type="integer">#{todo-item-id}</todo-item-id>
	//</time-entry>
	public class TimeEntry : ITimeEntry
	{
		private IBaseCamp Camp { get; set; }
        
        public static TimeEntry GetInstance(IBaseCamp camp, XmlNode node)
		{
			return new TimeEntry(camp, node);
		}
		private TimeEntry(IBaseCamp camp, XmlNode node)
		{
			Camp = camp;
			ID = XmlHelpers.ParseInteger(node, "id");
			ProjectID = XmlHelpers.ParseInteger(node, "project-id");
			PersonID = XmlHelpers.ParseInteger(node, "person-id");
			Date = XmlHelpers.ParseDateTime(node, "date");
			Hours = ConvertToDouble(XmlHelpers.ChildNodeText(node, "hours"));
			Description = XmlHelpers.ChildNodeText(node, "description");
			ToDoItemID = XmlHelpers.ParseInteger(node, "todo-item-id");
		}
		

		#region Implementation of ITimeEntry

		public int ID { get; private set; }
		public DateTime Date { get; private set; }
		public double Hours { get; private set; }
		public string Description { get; private set; }
        public int ProjectID { get; private set; }
        public int PersonID { get; private set; }
        public int ToDoItemID { get; private set; }

		#endregion

        public double ConvertToDouble(string inputString)
        {
            string separator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            double res = 0;

            if (inputString.IndexOf(separator) > 0)
                res = Convert.ToDouble(inputString.Trim());
            else
            {
                switch (separator)
                {
                    case ".":
                        inputString = inputString.Replace(",", ".");
                        res = Convert.ToDouble(inputString.Trim());
                        break;
                    case ",":
                        inputString = inputString.Replace(".", ",");
                        res = Convert.ToDouble(inputString.Trim());
                        break;
                }
            }

            return res;

            //float res;
            //if (!float.TryParse(hours, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out res)) res = (float)Convert.ToDouble(hours);
        }
	}
}
