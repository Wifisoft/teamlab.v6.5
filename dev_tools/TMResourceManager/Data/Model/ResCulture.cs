namespace TMResourceData.Model
{
    public class ResCulture
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public bool Available { get; set; }

        public override bool Equals(object obj)
        {
            return Title.Equals(((ResCulture) obj).Title);
        }
        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }
    }
}
