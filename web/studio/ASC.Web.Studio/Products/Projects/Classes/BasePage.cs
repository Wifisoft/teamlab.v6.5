using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Json;
using System.Text;
using ASC.Common.Security;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Web.Studio;

namespace ASC.Web.Projects
{
    public abstract class BasePage : MainPage
    {
        protected BasePage()
        {
            if (CallContext.GetData("CURRENT_ACCOUNT") == null && SecurityContext.IsAuthenticated)
            {
                CallContext.SetData("CURRENT_ACCOUNT", SecurityContext.CurrentAccount.ID);
            }
        }

        public Participant Participant
        {
            get { return Classes.Global.EngineFactory.GetParticipantEngine().GetByID(SecurityContext.CurrentAccount.ID); }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            PageLoad();
        }

        public void JsonPublisher<T>(T data, String jsonClassName) where T : class
        {

            String json;

            using (var stream = new MemoryStream())
            {

                var serializer = new DataContractJsonSerializer(typeof(T));

                serializer.WriteObject(stream, data);

                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            Page.ClientScript.RegisterClientScriptBlock(GetType(),
                                                       Guid.NewGuid().ToString(),
                                                        String.Format(" var {1} = {0};", json, jsonClassName),
                                                        true);
        }

        protected abstract void PageLoad();
    }
}
