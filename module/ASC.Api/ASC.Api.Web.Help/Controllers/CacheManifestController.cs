using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ASC.Api.Web.Help.Controllers
{
    public class CacheManifestController : Controller
    {
        //
        // GET: /CacheManifest/
        [OutputCache(CacheProfile = "pages")]
        public ActionResult GetCacheManifest()
        {
            if (string.Equals(ConfigurationManager.AppSettings["offline_cache"],bool.TrueString,StringComparison.OrdinalIgnoreCase))
                return new CacheActionResult(MvcApplication.CacheManifest);
            return new HttpNotFoundResult();
        }

    }

    public class HttpNotFoundResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            context.RequestContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
        }
    }

    public class CacheActionResult : ActionResult
    {
        private readonly CacheManifest _mainfest;

        public CacheActionResult(CacheManifest mainfest)
        {
            _mainfest = mainfest;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/cache-manifest";
            _mainfest.Write(context.HttpContext.Response.Output);
        }
    }
}
