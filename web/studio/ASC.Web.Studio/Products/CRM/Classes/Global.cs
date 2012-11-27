#region Import

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ASC.Core;
using ASC.Core.Users;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using System.Web.Configuration;

#endregion

namespace ASC.Web.CRM.Classes
{
    public class Global
    {
        public static readonly int EntryCountOnPage = 25;
        public static readonly int VisiblePageCount = 10;

        public static DaoFactory DaoFactory
        {
            get { return new DaoFactory(TenantProvider.CurrentTenantID, CRMConstants.DatabaseId); }
        }

        public static CRMSettings TenantSettings
        {
            get { return SettingsManager.Instance.LoadSettings<CRMSettings>(TenantProvider.CurrentTenantID); }
        }

        public static IDataStore GetStore()
        {
            return StorageFactory.GetStorage(PathProvider.BaseVirtualPath + "web.config",
                                             TenantProvider.CurrentTenantID.ToString(), "crm");
        }

        public static bool DebugVersion
        {
            get
            {
                bool debug;

                if (!bool.TryParse(WebConfigurationManager.AppSettings["crm.debugversion"], out debug))
                    debug = false;

                return debug;
            }
        }

        //Code snippet

        /// <summary>
        /// method for generating a country list, say for populating
        /// a ComboBox, with country options. We return the
        /// values in a Generic List<T>
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCountryList()
        {
            var cultureList = new List<string>();

            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

            foreach (var culture in cultures)
            {
                if (culture.LCID == 127) continue;

                var region = new RegionInfo(culture.Name);

                if (String.IsNullOrEmpty(region.EnglishName)) continue;

                if (!(cultureList.Contains(region.EnglishName)))
                    cultureList.Add(region.EnglishName);

            }            
            return cultureList;
        }

        public static String RenderPrivateItemHeader(String title, EntityType entityType, int entityID)
        {
            var sb = new StringBuilder();

            Dictionary<Guid, String> accessList;

            switch (entityType)
            {

                case EntityType.Contact:
                case EntityType.Person:
                case EntityType.Company:
                    accessList = CRMSecurity.GetAccessSubjectTo(Global.DaoFactory.GetContactDao().GetByID(entityID));
                    break;
                case EntityType.Case:
                    accessList = CRMSecurity.GetAccessSubjectTo(Global.DaoFactory.GetCasesDao().GetByID(entityID));
                    break;
                case EntityType.Opportunity:
                    accessList = CRMSecurity.GetAccessSubjectTo(Global.DaoFactory.GetDealDao().GetByID(entityID));
                    break;
                default:
                    throw new NotImplementedException();

            }

            foreach (var item in accessList)
            {
                sb.AppendFormat("<span class='splitter'></span>{0}",
                    StudioUserInfoExtension.RenderProfileLink(CoreContext.UserManager.GetUsers(item.Key), ProductEntryPoint.ID));
            }

            return String.Format(@"<div class='privateItemMark'></div><div style='margin-left:30px;'>
                                  {2} 
                                  <div style='font-size:12px;margin-bottom:10px;'>{0}:{1}</div>
                                  </div>", CustomNamingPeople.Substitute<CRMCommonResource>("PrivatePanelAccessListLable").HtmlEncode(),
                                        sb,
                                        title
                                      );
        }

    }
}