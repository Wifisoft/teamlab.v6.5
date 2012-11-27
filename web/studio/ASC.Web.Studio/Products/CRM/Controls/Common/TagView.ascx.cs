using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;

namespace ASC.Web.CRM.Controls.Common
{
    [AjaxNamespace("AjaxPro.TagView")]
    public partial class TagView : BaseUserControl
    {
        #region Members

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Common/TagView.ascx"); } }
        
        public String[] Tags { get; set; }
        
        protected String[] AllTags { get; set; }

        [AjaxProperty]
        public EntityType TargetEntityType { get; set; }

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(GetType());
            AllTags = Global.DaoFactory.GetTagDao().GetAllTags(TargetEntityType).Where(item => !Tags.Contains(item)).ToArray();
        }
        #endregion

        #region Methods

        #region Ajax Methods
        
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void DeleteTag(int id, EntityType entityType, string tagName)
        {
            if (id == 0 || String.IsNullOrEmpty(tagName)) return;

            Global.DaoFactory.GetTagDao().DeleteTagFromEntity(entityType, id, HttpUtility.HtmlDecode(tagName));

            return;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void AddTag(int id, EntityType entityType, string tagName)
        {
            if (id == 0 || String.IsNullOrEmpty(tagName)) return;

            Global.DaoFactory.GetTagDao().AddTagToEntity(entityType, id, HttpUtility.HtmlDecode(tagName));
        }

        #endregion

        #endregion
    }
}