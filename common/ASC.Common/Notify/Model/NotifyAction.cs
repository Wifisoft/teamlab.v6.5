using System;

namespace ASC.Notify.Model
{
    [Serializable]
    public class NotifyAction : INotifyAction
    {
        public string ID { get; private set; }

        public string Name { get; private set; }


        public NotifyAction(string id, string name)
        {
            if (id == null) throw new ArgumentNullException("id");

            ID = id;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var a = obj as INotifyAction;
            return a != null && a.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("action: {0}", Name);
        }
    }
}