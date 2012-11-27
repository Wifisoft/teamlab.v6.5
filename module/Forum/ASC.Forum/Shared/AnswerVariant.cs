using System.Reflection;

namespace ASC.Forum
{
	public class AnswerVariant
    {
        public virtual int ID { get; set; }

        public virtual string Name { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual int AnswerCount { get; set; }

        public virtual int QuestionID { get; set; }
        
    }
}
