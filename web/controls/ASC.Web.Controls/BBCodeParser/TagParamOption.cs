
namespace ASC.Web.Controls.BBCodeParser
{
    public class TagParamOption
    {
        public int ParamNumber{get; set;}
      
        public string DefaultValue{get; set;}
     
        public string PreValue{get; set;}
       
        public bool IsUseAnotherParamValue{get; set;}
      
        public int AnotherParamNumber{get; set;}
       
        public TagParamOption()
        {
            DefaultValue = "";
            ParamNumber = 0;
            PreValue = "";
            IsUseAnotherParamValue = false;
            AnotherParamNumber = 0;
        }
    }
}
