using System;

namespace ASC.Projects.Core.Domain
{
    public abstract class DomainObject<TID> where TID : struct
    {
        public TID ID
        {
            get;
            set;
        }

        public virtual String UniqID
        {
            get { return DoUniqId(GetType(), ID); }
        }

        internal static string DoUniqId(Type type, TID id)
        {
            return String.Format("{0}_{1}", type.Name, id);
        }

        public override int GetHashCode()
        {
            return (GetType().FullName + "|" + ID.GetHashCode()).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as DomainObject<TID>;
            return compareTo != null && (
                (!IsTransient() && !compareTo.IsTransient() && ID.Equals(compareTo.ID)) ||
                ((IsTransient() || compareTo.IsTransient()) && GetHashCode().Equals(compareTo.GetHashCode())));
        }

        private bool IsTransient()
        {
            return ID.Equals(default(TID));
        }
    }
}
