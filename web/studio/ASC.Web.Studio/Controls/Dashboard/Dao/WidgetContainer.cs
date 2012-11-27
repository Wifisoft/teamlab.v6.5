using System;
using System.Collections.Generic;

namespace ASC.Web.Studio.Controls.Dashboard.Dao
{
    [Serializable]
    class WidgetContainer
    {
        public Guid ID
        {
            get;
            private set;
        }

        public Guid ContainerID
        {
            get;
            private set;
        }

        public int TenantID
        {
            get;
            set;
        }

        public Guid UserID
        {
            get;
            set;
        }

        public ColumnSchemaType ColumnSchemaType
        {
            get;
            set;
        }

        public List<WidgetState> States
        {
            get;
            set;
        }

        public WidgetContainer(Guid id, Guid containerId)
        {
            ID = id;
            ContainerID = containerId;
            States = new List<WidgetState>();
        }
    }
}
