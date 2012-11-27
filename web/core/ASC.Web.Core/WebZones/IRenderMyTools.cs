using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace ASC.Web.Core.WebZones
{
    public interface IRenderMyTools : IRenderWebItem
    {
        Control LoadMyToolsControl(Page page);
        string ParameterName { get; }
        string TabName { get; }
    }

    public interface IFullScreenControl { }
}
