using System.Reflection;

namespace ASC.Forum
{
    public class Tag
    {
        public virtual int ID { get; set; }

        public virtual string Name { get; set; }

        public virtual bool IsApproved { get; set; }

        public virtual int TenantID { get; set; }
    }


	public class RankTag : Tag
    {
        

        public virtual int Rank { get; internal set; }

        public RankTag()
        {
            this.Rank = 0;
        }

        public RankTag(int id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.IsApproved = false;
            this.Rank = 0;
        }
    }
}
