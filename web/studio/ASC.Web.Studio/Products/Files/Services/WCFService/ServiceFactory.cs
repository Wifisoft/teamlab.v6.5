using System;
using System.ServiceModel;
using ASC.Web.Studio.Core;
using Microsoft.Practices.ServiceLocation;
using Microsoft.ServiceModel.Web;

namespace ASC.Web.Files.Services.WCFService
{
    internal class ServiceFactory : WebServiceHost2Factory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var docService = ServiceLocator.Current.GetInstance<IFileStorageService>();
            var service = new WebServiceHost2(docService, baseAddresses);
            service.MaxMessageSize = SetupInfo.MaxUploadSize != 0 ? SetupInfo.MaxUploadSize : 25*1024*1024;
            return service;
        }
    }
}