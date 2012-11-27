using System.Text.RegularExpressions;

namespace ASC.Web.Controls.BBCodeParser
{   

    public class ExpressionReplacement
    {       
        public ExpressionReplacement(string expression, string replacement)
        {
            this.Expression = expression;
            this.Replacement = replacement;           
        }

        public string Expression{get; set;}


        public string Replacement { get; set; }
        
    }
}
