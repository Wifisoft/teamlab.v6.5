using System;
using System.Collections.Generic;
using ASC.PhotoManager.Data;
using ASC.PhotoManager.Helpers;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.PhotoManager.Controls
{
    public partial class PhotoList : System.Web.UI.UserControl
    {
        #region Constants

        protected int _imagesCount = 11;
        private bool leftBtnEnabled = false;
        private bool rightBtnEnabled = false;

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if(Request.QueryString["start"] != null)
                    hdnStartPosiotion.Value = Request.QueryString["start"];
                
                LoadImages();
            }
        }
        
        #endregion

        #region Methods

        void LoadImages()
        {
            if (Current != null && ItemsList != null && ItemsList.Count != 0)
            {
                ASC.Data.Storage.IDataStore store = ASC.Data.Storage.StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "photo");
        
                
                if (ItemsList.IndexOf(Current) == 0)
                    LeftBtnEnabled = false;
                else { LeftBtnEnabled = true; }

                if(ItemsList.IndexOf(Current) == ItemsList.Count - 1)
                    RightBtnEnabled = false;
                else { RightBtnEnabled = true; }

                int countEmptyRegions = (_imagesCount - 1) / 2;

                for (int i = 0; i < countEmptyRegions; i++)
                {
                    ItemsList.Insert(0, AlbumItem.Empty);
                }
                for (int i = 0; i < countEmptyRegions; i++)
                {
                    ItemsList.Insert(ItemsList.Count, AlbumItem.Empty);
                }

                int imageIndex = ItemsList.IndexOf(Current);
                int totalImageCount = ItemsList.Count;


                int currentPosition = imageIndex - countEmptyRegions;

                ltrPhotoList.Text = "<input id=\"startPosition\" type=\"hidden\" value=\"" + currentPosition + "\" /><table border='0' cellpadding=\"0\" cellspacing=\"0\"  style='margin: 0px 7px;'><tr id=\"imageList\">";
                
                for (int i = 0; i < totalImageCount; i++)
                {
                    if (i >= totalImageCount)
                        break;

                    bool imageSeleted;
                    string imgPath = ASC.PhotoManager.PhotoConst.ImagesPath + ItemsList[i].ExpandedStoreThumb;

                    if (Current == ItemsList[i])
                        imageSeleted = true;
                    else
                        imageSeleted = false;

                    if (i >= currentPosition && i < currentPosition + _imagesCount)
                        ltrPhotoList.Text += ImageHTMLHelper.GetHTMLSmallThumb(ItemsList[i], 46, "onclick=\"loadPhoto('" + ItemsList[i].Id + "', '" + (string.IsNullOrEmpty(SetParam) ? string.Empty : "&set=" + SetParam) + "')\"", imageSeleted, true, store);
                    else
                        ltrPhotoList.Text += ImageHTMLHelper.GetHTMLSmallThumb(ItemsList[i], 46, "onclick=\"loadPhoto('" + ItemsList[i].Id + "', '" + (string.IsNullOrEmpty(SetParam) ? string.Empty : "&set=" + SetParam) + "')\"", imageSeleted, false, store);
                }

                hdnStartPosiotion.Value = currentPosition.ToString();
                ltrPhotoList.Text += "</tr></table>";
            }
        }


        #endregion

        #region Properties

        public bool LeftBtnEnabled
        {
            get { return leftBtnEnabled; }
            set { leftBtnEnabled = value; }
        }
        public bool RightBtnEnabled
        {
            get { return rightBtnEnabled; }
            set { rightBtnEnabled = value; }
        }

        public IList<AlbumItem> ItemsList { get; set; }

        public AlbumItem Current { get; set; }

        public string SetParam  { get; set; }

        #endregion

    }
}