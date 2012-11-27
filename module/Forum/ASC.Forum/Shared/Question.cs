using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ASC.Forum
{
    public enum QuestionType
    {
        SeveralAnswer =0,

        OneAnswer=1
    }

	public class Question
    {
        public virtual int ID { get; set; } 

        public virtual string Name { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual QuestionType Type { get; set; }

        public virtual List<AnswerVariant> AnswerVariants { get; set; }

        public virtual int TopicID { get; set; }

        public virtual int TenantID { get; set; }

        public Question()
        {   
            AnswerVariants = new List<AnswerVariant>(0);
        }
    }   
}
