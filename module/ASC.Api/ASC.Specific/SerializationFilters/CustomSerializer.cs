using System;
using System.Linq;
using ASC.Api.Attributes;
using ASC.Api.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace ASC.Specific.SerializationFilters
{
    public sealed class CustomSerializer : ApiCallFilter
    {
        public Type SerializerType { get; set; } 

        public CustomSerializer(Type serializerType)
        {
            SerializerType = serializerType;
            Responder = ServiceLocator.Current.GetInstance(SerializerType) as IApiResponder;
        }

        private IApiResponder Responder { get; set; }

        public override void PostMethodCall(IApiMethodCall method, Api.Impl.ApiContext context, object methodResponce)
        {
            method.Responders.Add(Responder); //Resolve type
            //Add serializers to
            base.PostMethodCall(method, context, methodResponce);
        }
    }
}