using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using log4net;
using Microsoft.ServiceModel.Web;

namespace ASC.Web.Files.Services.WCFService
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    internal class ServiceErrorLoggingBehaviorAttribute : Attribute, IServiceBehavior, IErrorHandler
    {
        private static readonly ILog logger = LogManager.GetLogger("ASC.Files");


        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        public bool HandleError(Exception error)
        {
            if (error is WebProtocolException)
            {
                if (logger.IsDebugEnabled)
                {
                    logger.Debug(error.Data.Contains("message") ? error.Data["message"] : error.Message, error);
                }
            }
            else
            {
                logger.Error(error);
            }
            return true;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher d in serviceHostBase.ChannelDispatchers)
            {
                d.ErrorHandlers.Add(this);
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}