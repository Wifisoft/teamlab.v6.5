using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using ASC.Web.Talk.Addon;

namespace ASC.Web.Talk.HttpHandlers
{
  public class UserPhotoHandler : IHttpHandler
  {
    public bool IsReusable
    {
      // To enable pooling, return true here.
      // This keeps the handler in memory.
      get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
      string imagepath = UserPhotoManager.GetMediumPhotoURL(new Guid());
      
      if (!String.IsNullOrEmpty(context.Request["id"]))
      {
        imagepath = UserPhotoManager.GetMediumPhotoURL(CoreContext.UserManager.GetUserByUserName(context.Request["id"].Split('@')[0]).ID);
      }

      if (String.Equals(context.Request["id"], CoreContext.TenantManager.GetCurrentTenant().TenantDomain, StringComparison.InvariantCultureIgnoreCase))
      {
        imagepath = WebImageSupplier.GetAbsoluteWebPath("teamlab.png", TalkAddon.AddonID);
      }

      context.Response.Redirect(imagepath, false);
      context.Response.StatusCode = 301;

      //try
      //{
      //  HttpWebRequest HWRequest = (HttpWebRequest)HttpWebRequest.Create(CommonLinkUtility.GetFullAbsolutePath(imagepath));
      //  HttpWebResponse HWResponse = (HttpWebResponse)HWRequest.GetResponse();
      //  HWResponse.GetResponseStream().StreamCopyTo(context.Response.OutputStream);
      //  context.Response.Cache.SetCacheability(HttpCacheability.Public);
      //  context.Response.ContentType = HWResponse.ContentType;
      //  context.Response.BufferOutput = false;
      //}
      //catch (Exception)
      //{
      //  context.Response.ContentType = "text/html";
      //  context.Response.BufferOutput = false;
      //  context.Response.StatusCode = 404;
      //  context.Response.End();
      //}
    }
  }
}
