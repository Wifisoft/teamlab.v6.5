using System;

namespace ASC.Web.Studio.Controls.Dashboard.Dao
{
    [Serializable]
    class WidgetState
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

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }


        public WidgetState(Guid id, Guid containerId)
        {
            ID = id;
            ContainerID = containerId;
        }

        public override bool Equals(object obj)
        {
            var ws = obj as WidgetState;
            return ws != null && ID == ws.ID && ContainerID == ws.ContainerID;
        }

        public override int GetHashCode()
        {
            return (int)(ID.GetHashCode() ^ ContainerID.GetHashCode());
        }
    }
}
