#region usings

using System;
using System.Collections;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Routing;
using ASC.Api.Enums;
using ASC.Api.Exceptions;
using ASC.Api.Interfaces;

#endregion

namespace ASC.Api.Impl
{
    public class ApiHttpHandler : ApiHttpHandlerBase
    {
        #region IApiHttpHandler Members


        public ApiHttpHandler(RouteData routeData)
            : base(routeData)
        {

        }

        protected override void DoProcess(HttpContextBase context)
        {
            Log.Debug("strating request. context: '{0}'", ApiContext);

            //Neeeded to rollback errors
            context.Response.Buffer = true;
            context.Response.BufferOutput = true;

            try
            {
                Log.Debug("method invoke");
                ApiResponce.Count = ApiContext.Count;
                ApiResponce.StartIndex = ApiContext.StartIndex;

                if (Method != null)
                {
                    var responce = ApiManager.InvokeMethod(Method, ApiContext);
                    PostProcessResponse(context, responce);
                }
                else
                {
                    SetError(context, new MissingMethodException("Method not found"), HttpStatusCode.NotFound);
                }
            }
            catch (TargetInvocationException targetInvocationException)
            {
                //Error caling method
                if (targetInvocationException.InnerException is ItemNotFoundException)
                {
                    SetError(context, targetInvocationException.InnerException, HttpStatusCode.NotFound, "The record could not be found");
                }
                else
                {
                    SetError(context, targetInvocationException.InnerException, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                SetError(context, e, HttpStatusCode.InternalServerError);
            }

            Exception responseError;
            try
            {
                RespondTo(Method, context);
                return;
            }
            catch (ThreadAbortException e)
            {
                //Do nothing. someone killing response
                Log.Error(e,"thread aborted. response not sent");
                return;
            }
            catch (HttpException exception)
            {
                responseError = exception;
                SetError(context, exception, (HttpStatusCode)exception.GetHttpCode());//Set the code of throwed exception
            }
            catch (Exception exception)
            {
                responseError = exception;
                SetError(context, exception, HttpStatusCode.InternalServerError);
            }
            Log.Error(responseError, "error happened while sending response. can't be here");
            RespondTo(Method, context);//If we got there then something went wrong
        }

        #endregion
    }
}