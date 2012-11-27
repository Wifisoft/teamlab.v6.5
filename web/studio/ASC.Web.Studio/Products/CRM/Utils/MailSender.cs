﻿#region Import

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Common.Threading.Progress;
using ASC.Common.Utils;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Web.CRM.Resources;
using ASC.Web.Files.Api;
using ASC.Web.Studio.Utility;
using Newtonsoft.Json.Linq;
using log4net;
using System.Runtime.Serialization;
using File = System.IO.File;
using System.Threading;

#endregion

namespace ASC.Web.CRM.Classes
{

    class SendBatchEmailsOperation : IProgressItem, IDisposable
    {
        #region Constructor

        private SendBatchEmailsOperation()
            : this(new List<int>(), new List<int>(), String.Empty, String.Empty, false)
        {

        }

        public SendBatchEmailsOperation(
              List<int> fileID,
              List<int> contactID,
              String subject,
              String bodyTempate,
              bool storeInHistory

            )
        {
            Id = TenantProvider.CurrentTenantID;
            Percentage = 0;
            _fileID = fileID;
            _contactID = contactID;
            _subject = subject;
            _bodyTempate = bodyTempate;

            _log = LogManager.GetLogger("ASC.CRM.MailSender");
            _tenantID = TenantProvider.CurrentTenantID;
            _daoFactory = Global.DaoFactory;
            _SMTPSetting = Global.TenantSettings.SMTPServerSetting;
            _currentUserID = ASC.Core.SecurityContext.CurrentAccount.ID;
            _storeInHistory = storeInHistory;

            Status = new
            {
                RecipientCount = _contactID.Count,
                EstimatedTime = 0,
                DeliveryCount = 0
            };
        }

        #endregion

        #region Members

        private readonly bool _storeInHistory;

        private readonly ILog _log;

        private readonly SMTPServerSetting _SMTPSetting;

        private readonly DaoFactory _daoFactory;

        private readonly Guid _currentUserID;

        private readonly int _tenantID;

        private int historyCategory;

        private readonly List<int> _contactID;

        private readonly String _subject;

        private readonly String _bodyTempate;

        private readonly List<int> _fileID;

        #endregion

        #region Property

        public object Id { get; set; }

        public object Status { get; set; }

        public object Error { get; set; }

        public double Percentage { get; set; }

        public bool IsCompleted { get; set; }

        #endregion

        #region IProgressItem Members

        private bool isValidMail(string e_mail)
        {
            string expr =
              @"^[-a-z0-9!#$%&'*+/=?^_`{|}~]+(?:\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@(?:[a-z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*(?:aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-z][a-z])$";

            Match isMatch =
              Regex.Match(e_mail, expr, RegexOptions.IgnoreCase);

            return isMatch.Success;
        }

        private void AddToHistory(int contactID, String content)
        {

            if (contactID == 0 || String.IsNullOrEmpty(content)) return;

            var historyEvent = new RelationshipEvent();

            historyEvent.ContactID = contactID;
            historyEvent.Content = content;
            historyEvent.CreateBy = _currentUserID;
            historyEvent.CreateOn = TenantUtil.DateTimeNow();

            if (historyCategory == 0)
            {
                using (var listItemDao = _daoFactory.GetListItemDao())
                {
                    // HACK
                    var listItem = listItemDao.GetItems(ListType.HistoryCategory).Find(
                         item => item.AdditionalParams == "event_category_email.png");

                    if (listItem == null)
                        listItemDao.CreateItem(ListType.HistoryCategory, new ListItem
                                                                             {
                                                                                 AdditionalParams = "event_category_email.png",
                                                                                 Title = CRMCommonResource.HistoryCategory_Note
                                                                             });
                    //

                    historyCategory = listItem.ID;

                }

            }

            historyEvent.CategoryID = historyCategory;

            using (var relationshipEventDao = _daoFactory.GetRelationshipEventDao())
            {
                historyEvent = relationshipEventDao.CreateItem(historyEvent);

                if (historyEvent.ID > 0 && _fileID != null && _fileID.Count > 0)
                    relationshipEventDao.AttachFiles(historyEvent.ID, _fileID.ToArray());
            }
        }


        public void RunJob()
        {
            var smtpClient = GetSmtpClient();

            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(_tenantID);

            var contactCount = _contactID.Count;

            if (contactCount == 0)
            {
                Complete();

                return;
            }

            var from = new MailAddress(_SMTPSetting.SenderEmailAddress, _SMTPSetting.SenderDisplayName, Encoding.UTF8);

            var filePaths = new List<String>();

            using (var fileDao = FilesIntegration.GetFileDao())
                foreach (var fileID in _fileID)
                {

                    var fileObj = fileDao.GetFile(fileID);

                    if (fileObj == null) continue;

                    using (var fileStream = fileDao.GetFileStream(fileObj))
                    {
                        var directoryPath = String.Concat(Path.GetTempPath(), "/teamlab/", _tenantID,
                                                          "/crm/files/mailsender/");

                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);

                        var filePath = String.Concat(directoryPath, fileObj.Title);


                        using (var newFileStream = File.Create(filePath))
                            fileStream.StreamCopyTo(newFileStream);

                        filePaths.Add(filePath);
                    }
                }


            var templateManager = new MailTemplateManager(_daoFactory);

            var deliveryCount = 0;

            try
            {
                Error = String.Empty;

                foreach (var contactID in _contactID)
                {
                    using (var contactInfoDao = _daoFactory.GetContactInfoDao())
                    {

                        var startDate = DateTime.Now;

                        var contactEmails = contactInfoDao.GetList(contactID, ContactInfoType.Email, null, true);

                        if (contactEmails.Count == 0)
                        {
                            continue;
                        }

                        var recipientEmail = contactEmails[0].Data;

                        if (!isValidMail(recipientEmail))
                        {

                            Error += String.Format(CRMCommonResource.MailSender_InvalidEmail,
                                                             recipientEmail) + "<br/>";


                            continue;
                        }

                        var to = new MailAddress(recipientEmail);

                        using (var message = new MailMessage(from, to))
                        {
                            try
                            {

                                message.Subject = _subject;
                                message.Body = templateManager.Apply(_bodyTempate, contactID);
                                message.SubjectEncoding = Encoding.UTF8;
                                message.BodyEncoding = Encoding.UTF8;
                                message.IsBodyHtml = true;

                                foreach (var filePath in filePaths)
                                    message.Attachments.Add(new Attachment(filePath));

                                _log.Debug(GetLoggerRow(message));

                                smtpClient.Send(message);

                                if (_storeInHistory)
                                    AddToHistory(contactID, String.Format(
                                            @"{0}:\n\r{1}\n\r{2}:\n\r\n\r{3}",
                                            CRMCommonResource.MailBody,
                                            HtmlUtil.GetText(message.Body),
                                            CRMCommonResource.MailSubject,
                                            HtmlUtil.GetText(message.Subject)
                                        ));

                                var endDate = DateTime.Now;

                                var waitInterval = endDate.Subtract(startDate);

                                deliveryCount++;

                                var estimatedTime = TimeSpan.FromTicks(waitInterval.Ticks * (_contactID.Count - deliveryCount));

                                Status = new
                                {
                                    RecipientCount = _contactID.Count,
                                    EstimatedTime = new TimeSpan(
                                        estimatedTime.Days,
                                        estimatedTime.Hours,
                                        estimatedTime.Minutes,
                                        estimatedTime.Seconds).ToString(),
                                    DeliveryCount = deliveryCount
                                };


                            }
                            catch (SmtpFailedRecipientsException ex)
                            {

                                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                                {
                                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;

                                    if (status == SmtpStatusCode.MailboxBusy ||
                                        status == SmtpStatusCode.MailboxUnavailable)
                                    {
                                        Error = String.Format(CRMCommonResource.MailSender_MailboxBusyException, 5);

                                        _log.Error(Error);

                                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));

                                        smtpClient.Send(message);

                                        deliveryCount++;

                                    }
                                    else
                                    {
                                        Error += String.Format(CRMCommonResource.MailSender_FailedDeliverException,
                                                               ex.InnerExceptions[i].FailedRecipient) + "<br/>";

                                        _log.Error(Error);

                                    }
                                }
                            }

                            Percentage += 100 / contactCount;

                            if (Percentage > 100)
                                Percentage = 100;
                        }
                    }
                }


            }
            finally
            {
                foreach (var filePath in filePaths)
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }

            Status = new
            {
                RecipientCount = _contactID.Count,
                EstimatedTime = new TimeSpan(0, 0, 0).ToString(),
                DeliveryCount = deliveryCount
            };

            Complete();
        }

        private String GetLoggerRow(MailMessage mailMessage)
        {
            if (mailMessage == null)
                return String.Empty;

            var result = new StringBuilder();

            result.AppendLine("From:" + mailMessage.From.Address);
            result.AppendLine("To:" + mailMessage.To[0].Address);
            result.AppendLine("Subject:" + mailMessage.Subject);
            result.AppendLine("Body:" + mailMessage.Body);
            result.AppendLine("TenantID:" + _tenantID);

            foreach (var attachment in mailMessage.Attachments)
            {
                result.AppendLine("Attachment: " + attachment.Name);
            }

            return result.ToString();
        }

        public object Clone()
        {
            var cloneObj = new SendBatchEmailsOperation();

            cloneObj.Error = Error;
            cloneObj.Id = Id;
            cloneObj.IsCompleted = IsCompleted;
            cloneObj.Percentage = Percentage;
            cloneObj.Status = Status;

            return cloneObj;
        }

        #endregion

        private void DeleteFiles()
        {
            if (_fileID == null || _fileID.Count == 0) return;

            foreach (var fileID in _fileID)
                using (var fileDao = FilesIntegration.GetFileDao())
                {
                    var fileObj = fileDao.GetFile(fileID);

                    if (fileObj == null) continue;


                    fileDao.DeleteFileStream(fileObj.ID);
                    fileDao.DeleteFile(fileObj.ID);
                }
        }

        private SmtpClient GetSmtpClient()
        {
            var smtpClient = new SmtpClient(_SMTPSetting.Host, _SMTPSetting.Port);

            if (_SMTPSetting.RequiredHostAuthentication)
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_SMTPSetting.HostLogin, _SMTPSetting.HostPassword);
            }

            smtpClient.EnableSsl = _SMTPSetting.EnableSSL;

            return smtpClient;
        }

        private void Complete()
        {
            IsCompleted = true;
            Percentage = 100;


            _log.Debug("Completed");

        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is SendBatchEmailsOperation)) return false;

            var curOperation = (SendBatchEmailsOperation)obj;

            if ((curOperation.Id == Id) && (curOperation._tenantID == _tenantID)) return true;

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ _tenantID.GetHashCode();
        }

        #region IDisposable Members

        public void Dispose()
        {
            DeleteFiles();
        }

        #endregion
    }

    public class MailSender
    {
        private static readonly Object _syncObj = new Object();

        private static readonly ProgressQueue _mailQueue = new ProgressQueue(2, TimeSpan.FromSeconds(60), true);

        public static int GetQuotas()
        {

            int quotas;

            if (!int.TryParse(WebConfigurationManager.AppSettings["crm.mailsender.quotas"], out quotas))
                quotas = 1000;

            return quotas;
        }

        public static IProgressItem Start(List<int> fileID, List<int> contactID, String subject,
                                          String bodyTemplate, bool storeInHistory)
        {
            lock (_syncObj)
            {

                var operation = _mailQueue.GetStatus(TenantProvider.CurrentTenantID);

                if (operation == null)
                {

                    if (fileID == null)
                        fileID = new List<int>();

                    if (contactID == null || contactID.Count == 0) return null;
                    if (String.IsNullOrEmpty(subject) || String.IsNullOrEmpty(bodyTemplate)) return null;

                    if (contactID.Count > GetQuotas())
                        contactID = contactID.Take(GetQuotas()).ToList();

                    operation = new SendBatchEmailsOperation(
                          fileID,
                          contactID,
                          subject,
                          bodyTemplate,
                          storeInHistory
                        );

                    _mailQueue.Add(operation);
                }

                if (!_mailQueue.IsStarted)
                    _mailQueue.Start(x => x.RunJob());

                return operation;
            }
        }

        public static IProgressItem GetStatus()
        {
            return _mailQueue.GetStatus(TenantProvider.CurrentTenantID);
        }

        public static void Cancel()
        {
            lock (_syncObj)
            {
                var findedItem = _mailQueue.GetItems().Where(elem => (int)elem.Id == TenantProvider.CurrentTenantID);

                if (findedItem.Any())
                    _mailQueue.Remove(findedItem.ElementAt(0));
            }
        }
    }

    [Serializable]
    [DataContract]
    public class MailTemplateTag
    {
        [DataMember(Name = "sysname")]
        public String SysName { get; set; }

        [DataMember(Name = "display_name")]
        public String DisplayName { get; set; }

        [DataMember(Name = "category")]
        public String Category { get; set; }

        [DataMember(Name = "is_company")]
        public bool isCompany { get; set; }

        [DataMember(Name = "name")]
        public String Name { get; set; }


    }

    public class MailTemplateManager
    {

        #region Members

        private readonly Dictionary<String, IEnumerable<MailTemplateTag>> _templateTagsCache = new Dictionary<String, IEnumerable<MailTemplateTag>>();

        private readonly DaoFactory _daoFactory;

        #endregion

        #region Constructor

        public MailTemplateManager()
        {
            _daoFactory = Global.DaoFactory;
        }

        public MailTemplateManager(DaoFactory daoFactory)
        {
            _daoFactory = daoFactory;
        }

        #endregion

        private IEnumerable<MailTemplateTag> GetTagsFrom(String template)
        {
            if (_templateTagsCache.ContainsKey(template)) return _templateTagsCache[template];

            var tags = GetAllTags();

            var result = new List<MailTemplateTag>();

            var _regex = new Regex("\\$\\((Person|Company)\\.[^<>\\)]*\\)");


            if (!_regex.IsMatch(template))
                return new List<MailTemplateTag>();

            foreach (Match match in _regex.Matches(template))
            {
                var findedTag = tags.Find(item => String.Compare(item.Name, match.Value) == 0);

                if (findedTag == null) continue;

                if (!result.Contains(findedTag))
                    result.Add(findedTag);
            }

            _templateTagsCache.Add(template, result);

            return result;
        }

        private String Apply(String template, IEnumerable<MailTemplateTag> templateTags, int contactID)
        {
            var result = template;


            using (var contactDao = _daoFactory.GetContactDao())
            using (var contactInfoDao = _daoFactory.GetContactInfoDao())
            using (var customFieldDao = _daoFactory.GetCustomFieldDao())
            {
                var contact = contactDao.GetByID(contactID);

                if (contact == null)
                    throw new ArgumentException("contact is null");

                foreach (var tag in templateTags)
                {
                    var tagParts = tag.SysName.Split(new[] { '_' });

                    var source = tagParts[0];

                    var tagValue = String.Empty;

                    switch (source)
                    {
                        case "common":

                            if (contact is Person)
                            {

                                var person = (Person)contact;

                                switch (tagParts[1])
                                {

                                    case "firstName":
                                        tagValue = person.FirstName;

                                        break;
                                    case "lastName":
                                        tagValue = person.LastName;

                                        break;
                                    case "jobTitle":
                                        tagValue = person.JobTitle;
                                        break;
                                    case "companyName":
                                        var relativeCompany = contactDao.GetByID(((Person)contact).CompanyID);

                                        if (relativeCompany != null)
                                            tagValue = relativeCompany.GetTitle();


                                        break;
                                    default:
                                        tagValue = String.Empty;
                                        break;

                                }

                            }
                            else
                            {

                                var company = (Company)contact;

                                switch (tagParts[1])
                                {
                                    case "companyName":
                                        tagValue = company.CompanyName;
                                        break;
                                    default:
                                        tagValue = String.Empty;
                                        break;
                                }
                            }

                            break;
                        case "customField":
                            var tagID = Convert.ToInt32(tagParts[tagParts.Length - 1]);

                            var entityType = contact is Company ? EntityType.Company : EntityType.Person;

                            tagValue = customFieldDao.GetValue(entityType, contactID, tagID);

                            break;
                        case "contactInfo":
                            var contactInfoType = (ContactInfoType)Enum.Parse(typeof(ContactInfoType), tagParts[1]);
                            var category = Convert.ToInt32(tagParts[2]);
                            var contactInfos = contactInfoDao.GetList(contactID, contactInfoType, category, true);

                            if (contactInfos == null || contactInfos.Count == 0) break;

                            var contactInfo = contactInfos[0];

                            if (contactInfoType == ContactInfoType.Address)
                            {
                                var addressPart = (AddressPart)Enum.Parse(typeof(AddressPart), tagParts[3]);

                                tagValue = JObject.Parse(contactInfo.Data)[addressPart.ToString().ToLower()].Value<String>();

                            }
                            else
                                tagValue = contactInfo.Data;

                            break;
                        default:
                            throw new ArgumentException(tag.SysName);
                    }

                    result = result.Replace(tag.Name, tagValue);
                }

            }

            return result;
        }

        public String Apply(String template, int contactID)
        {
            return Apply(template, GetTagsFrom(template), contactID);
        }

        private String ToTagName(String value, bool isCompany)
        {
            return String.Format("$({0}.{1})", isCompany ? "Company" : "Person", value);
        }

        private List<MailTemplateTag> GetAllTags()
        {
            return GetTags(true).Union(GetTags(false)).ToList();
        }

        public List<MailTemplateTag> GetTags(bool isCompany)
        {

            var result = new List<MailTemplateTag>();

            if (isCompany)
            {

                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.CompanyName,
                    SysName = "common_companyName",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = isCompany,
                    Name = ToTagName("Company Name", isCompany)
                });

            }
            else
            {
                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.FirstName,
                    SysName = "common_firstName",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = false,
                    Name = ToTagName("First Name", isCompany)
                });

                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.LastName,
                    SysName = "common_lastName",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = false,
                    Name = ToTagName("Last Name", isCompany)
                });

                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.JobTitle,
                    SysName = "common_jobTitle",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = false,
                    Name = ToTagName("Job Title", isCompany)
                });


                result.Add(new MailTemplateTag
                {
                    DisplayName = CRMContactResource.CompanyName,
                    SysName = "common_companyName",
                    Category = CRMContactResource.GeneralInformation,
                    isCompany = false,
                    Name = ToTagName("Company Name", isCompany)
                });

            }

            #region Contact Infos

            foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
            {

                var localName = String.Format("contactInfo_{0}_{1}", infoTypeEnum, ContactInfo.GetDefaultCategory(infoTypeEnum));
                var localTitle = infoTypeEnum.ToLocalizedString();

                if (infoTypeEnum == ContactInfoType.Address)
                    foreach (AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart)))
                        result.Add(new MailTemplateTag
                                       {
                                           SysName = String.Format(localName + "_{0}_{1}", addressPartEnum, (int)AddressCategory.Work),
                                           DisplayName = String.Format(localTitle + " {0}", addressPartEnum.ToLocalizedString()),
                                           Category = CRMContactResource.GeneralInformation,
                                           isCompany = isCompany,
                                           Name = ToTagName(String.Format("{0} {1}", infoTypeEnum.ToString(), addressPartEnum.ToString()), isCompany)
                                       });
                else
                    result.Add(new MailTemplateTag
                    {
                        SysName = localName,
                        DisplayName = localTitle,
                        Category = CRMContactResource.GeneralInformation,
                        isCompany = isCompany,
                        Name = ToTagName(infoTypeEnum.ToString(), isCompany)
                    });
            }

            #endregion

            #region Custom Fields

            var entityType = isCompany ? EntityType.Company : EntityType.Person;

            var customFieldsDao = Global.DaoFactory.GetCustomFieldDao();

            var customFields = customFieldsDao.GetFieldsDescription(entityType);

            var category = CRMContactResource.GeneralInformation;

            foreach (var customField in customFields)
            {
                if (customField.FieldType == CustomFieldType.SelectBox) continue;
                if (customField.FieldType == CustomFieldType.CheckBox) continue;

                if (customField.FieldType == CustomFieldType.Heading)
                {
                    if (!String.IsNullOrEmpty(customField.Label))
                        category = customField.Label;

                    continue;
                }

                result.Add(new MailTemplateTag
                                 {
                                     SysName = "customField_" + customField.ID,
                                     DisplayName = customField.Label.HtmlEncode(),
                                     Category = category,
                                     isCompany = isCompany,
                                     Name = ToTagName(customField.Label, isCompany)
                                 });
            }

            #endregion

            return result;
        }

    }
}