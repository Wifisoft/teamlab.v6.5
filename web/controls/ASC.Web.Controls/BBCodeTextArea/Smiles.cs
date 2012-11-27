using System.Collections.Generic;

namespace ASC.Web.Controls
{
    public struct Smile
    {
        public string Value;
        public string Img;
        public string Title;
        public string JavascriptValue;
        public Smile(string value, string img)
        {
            Value = value;
            Img = img;
            Title = "";
            JavascriptValue = "";

        }
        public Smile(string value, string img, string title, string javascriptValue)
        {
            Value = value;
            Img = img;
            Title = title;
            JavascriptValue = javascriptValue;

        }
    }
}
