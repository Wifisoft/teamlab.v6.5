using System.Collections.Generic;

namespace TMResourceData.Model
{
    public class Author
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public List<ResCulture> Langs { get; set; }
        public List<ResProject> Projects { get; set; }

        public Author()
        {}

        public Author(string login)
        {
            Login = login;
            IsAdmin = false;
            Langs = new List<ResCulture>();
            Projects = new List<ResProject>();
        }
    }
}
