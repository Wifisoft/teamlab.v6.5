using System;
using System.Text;
using System.Web.UI;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Controls.Common;

namespace ASC.Web.Files.Controls
{
    public partial class EmptyFolder : UserControl
    {
        #region Property

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("EmptyFolder/EmptyFolder.ascx"); }
        }

        public object FolderIDUserRoot { get; set; }
        public object FolderIDCommonRoot { get; set; }
        public object FolderIDShare { get; set; }
        public object FolderIDTrash { get; set; }
        public object FolderIDCurrentRoot { get; set; }

        #endregion

        #region Members

        protected string ExtsWebPreviewed = string.Join(", ", Studio.Utility.FileUtility.ExtsWebPreviewed.ToArray());
        protected string ExtsWebEdited = string.Join(", ", Studio.Utility.FileUtility.ExtsWebEdited.ToArray());

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            var strGoto = string.Format("<a href='#{1}' class='emptyContainer_goto baseLinkAction'>{0}</a><br/>", FilesUCResource.ButtonGotoMy, FolderIDUserRoot);
            var strUpload =
                !Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context)
                    ? string.Format("<a class='emptyContainer_upload baseLinkAction' >{0}</a><br/>", FilesUCResource.ButtonUpload)
                    : string.Empty;
            var strCreate =
                !Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context)
                    ? string.Format("<a class='emptyContainer_create baseLinkAction' >{0}</a>", FilesUCResource.NewFile)
                    : string.Empty;
            var strDragDrop = string.Format("<span class='emptyContainer_dragDrop' > {0}</span>", FilesUCResource.EmptyScreenDescrDragDrop);

            //my
            if (FolderIDUserRoot != null)
            {
                var myButton = new StringBuilder();
                myButton.Append(strUpload);
                myButton.Append(strCreate);

                var descrMy = string.Format(FilesUCResource.EmptyScreenDescrMy,
                                            //create
                                            "<span class='hintCreate baseLinkAction' >", "</span>",
                                            //upload
                                            "<span class='hintUpload baseLinkAction' >", "</span>",
                                            //open
                                            "<span class='hintOpen baseLinkAction' >", "</span>",
                                            //edit
                                            "<span class='hintEdit baseLinkAction' >", "</span>"
                    );
                descrMy += strDragDrop;

                EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                                                   {
                                                       ID = "emptyContainer_my",
                                                       ImgSrc = PathProvider.GetImagePath("empty_screen_my.png"),
                                                       Header = FilesUCResource.MyFiles,
                                                       HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                                                       Describe = descrMy,
                                                       ButtonHTML = myButton.ToString()
                                                   });
            }

            //forme
            if (FolderIDShare != null)
            {
                EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                                                   {
                                                       ID = "emptyContainer_forme",
                                                       ImgSrc = PathProvider.GetImagePath("empty_screen_forme.png"),
                                                       Header = FilesUCResource.SharedForMe,
                                                       HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                                                       Describe = FilesUCResource.EmptyScreenDescrForme,
                                                       ButtonHTML = strGoto
                                                   });
            }

            //corporate
            if (FolderIDCommonRoot != null)
            {
                var corporateButton = new StringBuilder();
                corporateButton.Append(strGoto);
                corporateButton.Append(strUpload);
                corporateButton.Append(strCreate);

                EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                                                   {
                                                       ID = "emptyContainer_corporate",
                                                       ImgSrc = PathProvider.GetImagePath("empty_screen_corporate.png"),
                                                       Header = FilesUCResource.CorporateFiles,
                                                       HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                                                       Describe = FilesUCResource.EmptyScreenDescrCorporate + strDragDrop,
                                                       ButtonHTML = corporateButton.ToString()
                                                   });
            }

            //trash
            if (FolderIDTrash != null)
            {
                EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                                                   {
                                                       ID = "emptyContainer_trash",
                                                       ImgSrc = PathProvider.GetImagePath("empty_screen_trash.png"),
                                                       Header = FilesUCResource.Trash,
                                                       HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                                                       Describe = FilesUCResource.EmptyScreenDescrTrash,
                                                       ButtonHTML = strGoto
                                                   });
            }

            //project
            if (FolderIDCurrentRoot != null)
            {
                var projectButton = new StringBuilder();
                projectButton.Append(strUpload);
                projectButton.Append(strCreate);

                EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                                                   {
                                                       ID = "emptyContainer_project",
                                                       ImgSrc = PathProvider.GetImagePath("empty_screen_project.png"),
                                                       Header = FilesUCResource.ProjectFiles,
                                                       HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                                                       Describe = FilesUCResource.EmptyScreenDescrProject + strDragDrop,
                                                       ButtonHTML = projectButton.ToString()
                                                   });
            }


            //Filter
            EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                                               {
                                                   ID = "emptyContainer_filter",
                                                   ImgSrc = PathProvider.GetImagePath("empty_screen_filter.png"),
                                                   Header = FilesUCResource.Filter,
                                                   HeaderDescribe = FilesUCResource.EmptyScreenFilter,
                                                   Describe = FilesUCResource.EmptyScreenFilterDescr,
                                                   ButtonHTML = string.Format("<a id='files_clearFilter' class='baseLinkAction' >{0}</a>", FilesUCResource.ButtonClearFilter)
                                               });
        }
    }
}