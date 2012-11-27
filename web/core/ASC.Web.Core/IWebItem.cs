using System;

namespace ASC.Web.Core
{
    public interface IWebItem
    {
        Guid ID { get; }

        string Name { get; }

        string Description { get; }

        string StartURL { get; }

        WebItemContext Context { get; }
    }
}
