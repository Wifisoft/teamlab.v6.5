using System.Web.UI.WebControls;

namespace ASC.Web.Studio.Masters
{
    public interface IStudioMaster
    {        
        PlaceHolder ContentHolder { get; }

        PlaceHolder SideHolder { get; }

        PlaceHolder TitleHolder { get; }

        PlaceHolder FooterHolder { get; }

        bool DisabledSidePanel { get; set; }

        bool? LeftSidePanel { get; set; }
    }
}
