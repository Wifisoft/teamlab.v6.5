#region Import

using System;
using System.Linq;
using System.Web;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Data.Storage;
using ASC.Web.Controls;
using ASC.Web.Core.Utility;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Resources;
using ASC.Common.Threading.Progress;
using System.Collections.Generic;
using ASC.Web.Studio.Core.Users;
using ASC.Core;


#endregion

namespace ASC.Web.CRM.Controls.Contacts
{
    public class ImportFileHandler : IFileUploadHandler
    {
        public FileUploadResult ProcessUpload(HttpContext context)
        {
            var fileUploadResult = new FileUploadResult();

            if (!ProgressFileUploader.HasFilesToUpload(context)) return fileUploadResult;

            var file = new ProgressFileUploader.FileToUpload(context);

            String assignedPath;

            Global.GetStore().SaveTemp("temp", out assignedPath, file.InputStream);

            file.InputStream.Position = 0;

            var jObject = ImportFromCSV.GetInfo(file.InputStream);
           
            jObject.Add("assignedPath", assignedPath);

            fileUploadResult.Success = true;
            fileUploadResult.Data = jObject.ToString();
         
            return fileUploadResult;
        }
    }


    [AjaxNamespace("AjaxPro.Utils.ImportFromCSV")]
    public partial class ImportFromCSVView : BaseUserControl
    {
        #region Property

        public static String Location { get { return PathProvider.GetFileStaticRelativePath("Contacts/ImportFromCSVView.ascx"); } }

        #endregion

        #region Ajax Methods

        [AjaxMethod]
        public void StartImport(String CSVFileURI, String importSettingsJSON)
        {
            ImportFromCSV.Start(CSVFileURI, importSettingsJSON);

        }

        [AjaxMethod]
        public IProgressItem GetStatus()
        {
            return ImportFromCSV.GetStatus();
        }

        [AjaxMethod]
        public String GetSampleRow(String CSVFileURI, int indexRow)
        {
            if (String.IsNullOrEmpty(CSVFileURI) || indexRow < 1)
                throw new ArgumentException();

            if (!Global.GetStore().IsFile("temp", CSVFileURI))
                throw new ArgumentException();

            var CSVFileStream = Global.GetStore().GetReadStream("temp", CSVFileURI);

            return ImportFromCSV.GetRow(CSVFileStream, indexRow);
        }

        [AjaxMethod]
        public String GetPreviewImportData(String CSVFileURI,
                                                int companyNameColumnIndex,
                                                int firstNameColumnIndex,
                                                int lastNameColumnIndex)
        {

            if (String.IsNullOrEmpty(CSVFileURI))
                throw new ArgumentException();

            if (!Global.GetStore().IsFile("temp", CSVFileURI))
                throw new ArgumentException();

            var CSVFileStream = Global.GetStore().GetReadStream("temp",CSVFileURI);

            return ImportFromCSV.GetFoundedContacts(CSVFileStream, companyNameColumnIndex, firstNameColumnIndex, lastNameColumnIndex);
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {

            Utility.RegisterTypeForAjax(typeof(ImportFromCSVView));

            Page.ClientScript.RegisterClientScriptInclude(GetType(), "ajaxupload_script", WebPath.GetPath("js/ajaxupload.3.5.js"));

            var columnSelectorData = new[]
                                         {
                                             new
                                             {
                                                  name = String.Empty,
                                                  title = CRMContactResource.NoMatchSelect,
                                                  isHeader = false
                                             },
                                             new
                                             {
                                                  name = "-1",
                                                  title = CRMContactResource.DoNotImportThisField,
                                                  isHeader = false
                                             },
                                             new
                                             {
                                                  name = String.Empty,
                                                  title = CRMContactResource.GeneralInformation,
                                                  isHeader = true
                                             },
                                             new
                                             {
                                                  name = "firstName",
                                                  title = CRMContactResource.FirstName,
                                                  isHeader = false
                                             },
                                             new
                                             {
                                                  name = "lastName",
                                                  title = CRMContactResource.LastName,
                                                  isHeader = false
                                             },
                                             new
                                             {
                                                  name = "title",
                                                  title = CRMContactResource.JobTitle,
                                                  isHeader = false
                                             },
                                             new
                                             {
                                                  name = "companyName",
                                                  title = CRMContactResource.CompanyName,
                                                  isHeader = false
                                             },
                                             new
                                             {
                                                  name = "notes",
                                                  title = CRMContactResource.About,
                                                  isHeader = false
                                             },
                                             new
                                             {
                                                  name = "tag",
                                                  title = CRMContactResource.Tags,
                                                  isHeader = false
                                             }
                                         }.ToList();

            foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
                foreach (Enum categoryEnum in Enum.GetValues(ContactInfo.GetCategory(infoTypeEnum)))
                {

                    var localName = String.Format("contactInfo_{0}_{1}", infoTypeEnum, Convert.ToInt32(categoryEnum));
                    var localTitle = String.Format("{1} ({0})", categoryEnum.ToLocalizedString().ToLower(), infoTypeEnum.ToLocalizedString());

                    if (infoTypeEnum == ContactInfoType.Address)
                        foreach (AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart)))
                            columnSelectorData.Add(new
                                                       {
                                                           name = String.Format(localName + "_{0}", addressPartEnum),
                                                           title = String.Format(localTitle + " {0}", addressPartEnum.ToLocalizedString().ToLower()),
                                                           isHeader = false
                                                       });
                    else
                        columnSelectorData.Add(new
                                               {
                                                   name = localName,
                                                   title = localTitle,
                                                   isHeader = false
                                               });
                }


            columnSelectorData.AddRange(Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Contact).FindAll(customField => customField.FieldType == CustomFieldType.TextField || customField.FieldType == CustomFieldType.TextArea || customField.FieldType == CustomFieldType.Heading)
                                                          .ConvertAll(customField => new
                                                                                         {
                                                                                             name = "customField_" + customField.ID,
                                                                                             title = customField.Label.HtmlEncode(),
                                                                                             isHeader = customField.FieldType == CustomFieldType.Heading
                                                                                         }));

            Page.ClientScript.RegisterClientScriptBlock(GetType(),
                                                        Guid.NewGuid().ToString(),
                                                        String.Format(" var {0} = {1}; ", "columnSelectorData", JavaScriptSerializer.Serialize(columnSelectorData)),
                                                        true);

            var privatePanel = (PrivatePanel)Page.LoadControl(PrivatePanel.Location);
            privatePanel.CheckBoxLabel = CRMContactResource.PrivatePanelCheckBoxLabelForContact;
            privatePanel.IsPrivateItem = false;
            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode() };
            privatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            privatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            privatePanel.HideNotifyPanel = true;
            _phPrivatePanel.Controls.Add(privatePanel);

        }

        #endregion

    }
}