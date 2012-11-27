using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using ASC.Api.Interfaces;
using ASC.Api.Interfaces.ResponseTypes;
using ASC.Api.Utils;
using System.Net;

namespace ASC.Api.Impl.Responders
{
    public class ContentResponder : IApiResponder
    {
        #region IApiResponder Members

        public string Name
        {
            get { return "content"; }
        }

        public IEnumerable<string> GetSupportedExtensions()
        {
            return new string[0];
        }

        public bool CanSerializeType(Type type)
        {
            return false;
        }

        public string Serialize(IApiStandartResponce obj, ApiContext context)
        {
            throw new NotSupportedException();
        }

        public bool CanRespondTo(IApiStandartResponce responce, HttpContextBase context)
        {
            return responce.Response is IApiContentResponce;
        }

        public void RespondTo(IApiStandartResponce responce, HttpContextBase context)
        {
            var contentResponce = (IApiContentResponce) responce.Response;
            if (contentResponce.ContentDisposition != null)
            {
                context.Response.AddHeader("Content-Disposition", contentResponce.ContentDisposition.ToString());
            }
            if (contentResponce.ContentType != null)
            {
                context.Response.ContentType = contentResponce.ContentType.ToString();
            }
            if (contentResponce.ContentEncoding != null)
            {
                context.Response.ContentEncoding = contentResponce.ContentEncoding;
            }
            context.Response.WriteStreamToResponce(contentResponce.ContentStream);
        }

        #endregion
    }
}