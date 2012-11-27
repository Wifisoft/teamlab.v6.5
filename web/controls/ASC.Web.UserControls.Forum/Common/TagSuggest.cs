using System;
using System.Collections.Generic;
using AjaxPro;
using ASC.Forum;
using ASC.Web.Studio.Utility;

namespace ASC.Web.UserControls.Forum.Common
{
    [AjaxNamespace("TagSuggest")]
    public class TagSuggest
    {
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse GetSuggest(Guid settingsID, string text, string varName)
        {
            AjaxResponse resp = new AjaxResponse();

            string startSymbols = text;
            int ind = startSymbols.LastIndexOf(",");
            if (ind != -1)
                startSymbols = startSymbols.Substring(ind + 1);

            startSymbols = startSymbols.Trim();

            var tags = new List<Tag>();

            if (!String.IsNullOrEmpty(startSymbols))
                tags = ForumDataProvider.SearchTags(TenantProvider.CurrentTenantID, startSymbols);

            int counter = 0;
            string resNames = "", resHelps = "";

            foreach (var tag in tags)
            {
                if (counter > 10)
                    break;

                resNames += tag.Name + "$";
                resHelps += tag.ID + "$";
                counter++;
            }

            resNames = resNames.TrimEnd('$');
            resHelps = resHelps.TrimEnd('$');
            resp.rs1 = resNames;
            resp.rs2 = resHelps;
            resp.rs3 = text;
            resp.rs4 = varName;

            return resp;
        }

    }
}
