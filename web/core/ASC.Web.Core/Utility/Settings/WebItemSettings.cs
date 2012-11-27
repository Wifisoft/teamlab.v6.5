using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core.Utility.Settings
{
    [Serializable]
    [DataContract]
    public class WebItemSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("{C888CF56-585B-4c78-9E64-FE1093649A62}"); }
        }

        [DataMember(Name = "Settings")]
        public List<WebItemOption> SettingsCollection { get; set; }


        public WebItemSettings()
        {
            SettingsCollection = new List<WebItemOption>();
        }

        
        public ISettings GetDefault()
        {
            var settings = new WebItemSettings();
            WebItemManager.Instance.GetItemsAll().ForEach(w =>
            {
                var opt = new WebItemOption()
                {
                    ItemID = w.ID,
                    SortOrder = WebItemManager.GetSortOrder(w),
                    Disabled = false,
                };
                settings.SettingsCollection.Add(opt);
            });
            return settings;
        }


        [Serializable]
        [DataContract]
        public class WebItemOption
        {
            [DataMember(Name = "Id")]
            public Guid ItemID { get; set; }

            [DataMember(Name = "SortOrder")]
            public int SortOrder { get; set; }

            [DataMember(Name = "Disabled")]
            public bool Disabled { get; set; }
        }
    }
}
