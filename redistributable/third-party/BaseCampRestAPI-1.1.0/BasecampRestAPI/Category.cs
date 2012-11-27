using System.Xml;

namespace BasecampRestAPI
{
    /*
        <category>
            <id type="integer">#{id}</id>
            <name>#{name}</name>
            <project-id type="integer">#{project_id}</project-id>
            <elements-count type="integer">#{elements_count}</elements-count>
            <type>#{type}</type>
        </category>
    */
    public class Category : ICategory
	{
        private readonly IBaseCamp _camp;

        public static ICategory GetInstance(IBaseCamp camp, XmlNode node)
        {
            return new Category(camp, node);
        }
        private Category(IBaseCamp camp, XmlNode node)
        {
            _camp = camp;
            ID = XmlHelpers.ParseInteger(node, "id");
            Name = XmlHelpers.ChildNodeText(node, "name");
            ProjectID = XmlHelpers.ParseInteger(node, "project-id");
            //ElementsCount = XmlHelpers.ParseInteger(node, "elements-count");
            Type = (XmlHelpers.ChildNodeText(node, "type") == "PostCategory") ?
                CategoryType.Post : CategoryType.Attachment;
        }
        
        #region Implementation of ICategory

		public int ID { get; private set; }
        public string Name { get; private set; }
		public int ProjectID { get; private set; }
		//public int ElementsCount { get; private set; }
		public CategoryType Type { get; private set; }

		#endregion

		static public string PathForId(int id)
		{
			return string.Format("categories/{0}.xml", id);
		}
	}
}
