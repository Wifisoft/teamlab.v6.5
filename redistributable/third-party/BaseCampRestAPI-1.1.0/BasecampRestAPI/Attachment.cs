using System.Xml;
using System;
namespace BasecampRestAPI
{
    /*
    <attachment>
        <id type="integer">#{id}</id>
        <name>#{name}</name>
        <description>#{description}</description>
        <byte-size type="integer">#{byte_size}</byte-size>
        <download-url>#{download_url}</download-url>

        <project-id type="integer">#{project_id}</project-id>
        <category-id type="integer">#{category_id}</category-id>
        <person-id type="integer">#{person_id}</person-id>
        <private type="boolean">#{private}</private>
        <created-on type="datetime">#{created_on}</created-on>

        <!-- if the attachment belongs to a message or comment -->
        <owner-id type="integer">#{owner_id}</owner-id>
        <owner-type>#{owner_type}</owner-type>

        <!-- for attachments with multiple versions, collection specifies
            the id of the "parent" attachment (version 1), and current will
            be true for the most recent version -->
        <collection type="integer">#{collection}</collection>
        <version type="integer">#{version}</version>
        <current type="boolean">#{current}</current>
    </attachment>
    */
    public class Attachment : IAttachment
	{
        private readonly IBaseCamp _camp;
        
        public static IAttachment GetInstance(IBaseCamp camp, XmlNode node)
		{
			return new Attachment(camp, node);
		}
        private Attachment(IBaseCamp camp, XmlNode node)
		{
			_camp = camp;
            ProjectID = XmlHelpers.ParseInteger(node, "project-id");
            CategoryID = XmlHelpers.ParseInteger(node, "category-id");
            AuthorID = XmlHelpers.ParseInteger(node, "person-id");
            CreatedOn = XmlHelpers.ParseDateTime(node, "created-on");
            ByteSize = XmlHelpers.ParseInteger(node, "byte-size");
            Vers = XmlHelpers.ParseInteger(node, "version");
            DownloadUrl = XmlHelpers.ChildNodeText(node, "download-url");
            Collection = XmlHelpers.ParseInteger(node, "collection");
            //Current = XmlHelpers.ParseBool(node, "current");
            Description = XmlHelpers.ChildNodeText(node, "description");
            ID = XmlHelpers.ParseInteger(node, "id");
			Name = XmlHelpers.ChildNodeText(node, "name");
            OwnerID = XmlHelpers.ParseInteger(node, "owner-id");
            OwnerType = XmlHelpers.ChildNodeText(node, "owner-type");
			//Private = XmlHelpers.ParseBool(node, "private");
		}

		#region Implementation of IAttachment

        public int ByteSize { get; private set; }
        public int Vers { get; private set; }
        public string DownloadUrl { get; private set; }
        public int Collection { get; private set; }
        //public bool Current { get; private set; }
        public string Description { get; private set; }
        public int ID { get; private set; }
        public string Name { get; private set; }
        public int OwnerID { get; private set; }
        public string OwnerType { get; private set; }
        //public bool Private { get; private set; }
        public int ProjectID { get; private set; }
        public int CategoryID { get; private set; }
        public int AuthorID { get; private set; }
        public DateTime CreatedOn { get; private set; }

		#endregion
	}
}
