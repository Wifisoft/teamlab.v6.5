using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using ASC.Api.Interfaces;
using ASC.Api.Interfaces.ResponseTypes;

namespace ASC.Api.Impl.Responders
{
    public class DirectResponder : IApiResponder
    {
        #region IApiResponder Members

        public string Name
        {
            get { return "direct"; }
        }

        public IEnumerable<string> GetSupportedExtensions()
        {
            return new string[0];
        }

        public bool CanSerializeType(Type type)
        {
            return false;
        }


        public bool CanRespondTo(IApiStandartResponce responce, HttpContextBase context)
        {
            return responce.Response is IApiDirectResponce;
        }

        public void RespondTo(IApiStandartResponce responce, HttpContextBase context)
        {
            ((IApiDirectResponce) responce.Response).WriteResponce(context.Response);
        }

        #endregion
    }
}