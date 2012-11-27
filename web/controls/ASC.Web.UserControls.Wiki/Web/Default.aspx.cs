using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.UserControls.Wiki;

public enum Action
{
    None = 0,
    AddNew,
    AddNewFile,
    Edit,
    View
}

public partial class _Default : System.Web.UI.Page
{

    private string _mainPath;
    private string _wikiPage = null;
    protected string WikiPage
    {
        get
        {
            if (_wikiPage == null)
            {
                if (string.IsNullOrEmpty(Request["page"]))
                {
                    _wikiPage = string.Empty;
                }
                else
                {
                    _wikiPage = Request["page"];
                }
            }

            return _wikiPage;
        }
    }


    private Action _action = Action.None;
    protected Action Action
    {
        get
        {
            if (_action.Equals(Action.None))
            {
                if (Request["action"] == null)
                {
                    _action = Action.View;
                }
                else
                {
                    if (Request["action"].Equals("edit", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(WikiPage))
                    {
                        _action = Action.Edit;
                    }
                    else if(Request["action"].Equals("newfile", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _action = Action.AddNewFile;
                    }
                    else
                    {
                        _action = Action.AddNew;
                    }
                }
            }

            return _action;
        }
    }


    protected bool IsFile
    {
        get
        {
            return WikiPage.Split('.').Length > 1 || Action.Equals(Action.AddNewFile);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        _mainPath = Request.ApplicationPath.TrimEnd('/') + "{0}";

        if (!IsPostBack)
        {

            InitListPages();
            LoadWikiPage();



            if (string.IsNullOrEmpty(WikiPage) || Action.Equals(Action.Edit))
            {
                cmdEdit.Enabled = false;
            }

            cmdAddNew.NavigateUrl = ActionHelper.GetAddPagePath(_mainPath);
            cmdEdit.NavigateUrl = ActionHelper.GetEditPagePath(_mainPath, WikiPage);
            cmdAddFile.NavigateUrl = ActionHelper.GetAddFilePath(_mainPath);
            cmdEditFile.NavigateUrl = ActionHelper.GetEditPagePath(_mainPath, WikiPage);

        }
    }


    private void LoadWikiPage()
    {

        if(!IsFile)
        {
            wikiViewFile.Visible = false;
            wikiEditFile.Visible = false;

            if (WikiPage.Equals(string.Empty) || !Action.Equals(Action.View))
            {
                if (!Action.Equals(Action.View))
                {
                    wikiEditPage.Visible = true;
                }
                wikiViewPage.Visible = false;


                if (Action.Equals(Action.Edit))
                {
                    wikiEditPage.PageName = WikiPage;
                }
                else
                {
                    wikiEditPage.PageName = string.Empty;
                }


                Title = string.Format("WIKI - Edit/Add page");
            }
            else
            {
                wikiViewPage.Visible = true;
                wikiEditPage.Visible = false;

                wikiViewPage.PageName = WikiPage;
                Title = string.Format("WIKI - {0}", PageNameUtil.Decode(WikiPage));
            }
        }
        else
        {
            wikiViewPage.Visible = false;
            wikiEditPage.Visible = false;

            if (WikiPage.Equals(string.Empty) || !Action.Equals(Action.View))
            {
                if (!Action.Equals(Action.View))
                {
                    wikiEditFile.Visible = true;

                }
                wikiViewFile.Visible = false;


                if (Action.Equals(Action.Edit))
                {
                    wikiEditFile.FileName = WikiPage;
                }
                else
                {
                    wikiEditFile.FileName = string.Empty;
                }


                Title = string.Format("WIKI - Edit/Add file");
            }
            else
            {
                wikiViewFile.Visible = true;
                wikiEditFile.Visible = false;

                cmdEditFile.Visible = true;

                wikiViewFile.FileName = WikiPage;
                Title = string.Format("WIKI - {0}", PageNameUtil.Decode(WikiPage));
            }
        }
        

    }

    private void InitListPages()
    {
        wikiListPages.mainPath = _mainPath;
        wikiListFiles.mainPath = _mainPath;
    }


}
