using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Web.Controls
{
    public class ViewSwitcherCombobox : ViewSwitcherBaseItem
    {

        private IList<ViewSwitcherComboboxItem> _comboboxItems;

        public IList<ViewSwitcherComboboxItem> ComboboxItems
        {
            get
            {
                if (_comboboxItems == null)
                {
                    _comboboxItems = new List<ViewSwitcherComboboxItem>();
                }
                return _comboboxItems;
            }
            set { _comboboxItems = value; }
        }

        public string ComboboxClass { get; set; }

        public override string GetLink()
        {
            var sb = new StringBuilder();

            var dropdownID = Guid.NewGuid().ToString();

            var css = "viewSwitcherCombobox";
            var label = SortLabel;
            var hintText = HintText == String.Empty ? Resources.ViewSwitcherResource.Choose : HintText;
            var isBold = HintText == String.Empty ? "" : "font-weight:bold";
            if (!string.IsNullOrEmpty(ComboboxClass))
            {
                css = string.Empty;
                label = string.Format("<span class='{0}'>{1}</span>", ComboboxClass, SortLabel);
                hintText = string.Empty;
            }

            sb.AppendFormat("<span style='float:left;'>{1}:</span>&nbsp;<div id='viewSwitcherDropdownToggle{0}' onclick='viewSwitcherDropdownToggle(this, \"viewSwitcherDropdownList{0}\");' onselectstart='return false;' ondbclick='return false;' onmousedown='return false;' class='{2}' style='{4}'><span class='baseLinkAction'>{3}</span><small>&nbsp; ▼</small></div>", dropdownID, label, css, hintText, isBold);

            if (ComboboxItems == null || ComboboxItems.Count == 0)
            {
                return sb.ToString();
            }

            sb.AppendFormat("<div id='viewSwitcherDropdownList{0}' class='viewSwitcherDropdown' style='display: none;'>", dropdownID);

            foreach (var item in ComboboxItems)
            {
                sb.AppendFormat("<div class='clearFix viewSwitcherComboItem'><a class='{2}' href=\"{0}\">{1}</a></div>", item.SortUrl, item.SortLabel, item.CssClass ?? string.Empty);
            }

            sb.Append("</div>");

            return sb.ToString();
        }
    }
}