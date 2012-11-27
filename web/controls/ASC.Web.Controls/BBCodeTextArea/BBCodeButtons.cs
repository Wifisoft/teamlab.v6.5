using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Web.Controls
{
    public class BBCodeButton
    {
        public string BBCodeTagName { get; set; }

        public string JavascriptAction { get; set; }

        public BBCodeTagType TagType { get; set; }

        public BBCodeButton()
        {
            this.TagType = BBCodeTagType.Normal;
        }

    }

    public class BBCodeCustomButton : BBCodeButton
    {
        public string ImageFileName { get; set; }

        public string AltText { get; set; }

        public bool Disabled { get; set; }

    }

    public class BBCodeComboBoxItem : BBCodeButton
    {
        public string Name { get; set; }

        public bool IsTitle { get; set; }

        public new BBCodeTagType TagType { get { return BBCodeTagType.Normal; } }

        
        public string Style { get; set; }

        public BBCodeComboBoxItem()
        {
            this.IsTitle = false;
        }
    }
}
