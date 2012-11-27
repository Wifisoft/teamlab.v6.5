﻿#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ASC.CRM.Core;
using ASC.Common.Threading.Progress;
using ASC.Web.CRM.Classes;
using AjaxPro;


#endregion

namespace ASC.Web.CRM.Controls.Contacts
{
    [AjaxNamespace("AjaxPro.TestMailSender")]
    public partial class TestMailSender : BaseUserControl
    {

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Contacts/TestMailSender.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            Utility.RegisterTypeForAjax(typeof(TestMailSender));

        }

        [AjaxMethod]
        public IProgressItem Start()
        {

            var contactIDs = Global.DaoFactory.GetContactInfoDao()
            .GetList(0, ContactInfoType.Email, null, true).Select(item => item.ContactID);

            //var subjectTemplate = @"это заголовок сообщения";
            //var bodyTemplate = @"это тело сообщения";
            // return  MailSender.Start(null, new HashSet<int>(contactIDs), subjectTemplate, bodyTemplate);
            throw new NotImplementedException();

        }




        [AjaxMethod]
        public String TestTemplate()
        {
            var temp = new MailTemplateManager();

            return temp.Apply(@"
                                asdfasdf
asdfasdfasdf
asdfasdf
asdf
$(Person.First Name)
asd
f
asdf
as
dfas


                              ", 2328);

        //    throw new NotImplementedException();
            //    return temp.

        }


        [AjaxMethod]
        public IProgressItem GetStatus()
        {
            return MailSender.GetStatus();

        }
        
        [AjaxMethod]
        public IProgressItem Cancel()
        {
             MailSender.Cancel();

            return GetStatus();

        }

    }
}