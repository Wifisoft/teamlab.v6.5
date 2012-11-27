using System;

namespace ASC.Specific
{
    public interface IApiSortableDate
    {
        ApiDateTime Created { get; set; }
        ApiDateTime Updated { get; set; }
    }
}