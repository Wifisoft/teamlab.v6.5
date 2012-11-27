using System;
namespace BasecampRestAPI
{
	public interface IAttachment
	{
        int ByteSize { get; }
        int Vers { get; }
        string DownloadUrl {get;}
        int Collection { get; } 
        //bool Current { get; }
        string Description { get; }
        int ID { get; }
        string Name { get; }
        int OwnerID { get; }
        string OwnerType {get;}
        //bool Private {get;}
        int ProjectID { get; }
        int CategoryID { get; }
        int AuthorID { get; }
        DateTime CreatedOn { get; }        
	}
}