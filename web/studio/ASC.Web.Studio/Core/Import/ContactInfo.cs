namespace ASC.Web.Studio.Core.Import
{
    public class ContactInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

		public override bool Equals(object obj)
		{
			try
			{
				if (obj is ContactInfo)
				{
					var o = obj as ContactInfo;
					return Email.Equals(o.Email);
				}
			}
			catch { }
			return false;
		}

		public override int GetHashCode()
		{
			return Email.GetHashCode();
		}
    }
}