using System.Linq;
using ASC.Api.Web.Help.DocumentGenerator;

namespace ASC.Api.Web.Help.Models
{
    public class SectionMethodViewModel
    {
        public MsDocEntryPoint Section { get; set; }
        public MsDocEntryPointMethod Method { get; set; }

        public SectionMethodViewModel(MsDocEntryPoint section, MsDocEntryPointMethod method)
        {
            Section = section;
            Method = method;
        }
    }
}