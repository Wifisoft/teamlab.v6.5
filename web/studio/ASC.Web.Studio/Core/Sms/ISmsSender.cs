
namespace ASC.Web.Studio.Core.SMS
{
    public interface ISmsSender
    {
        RequestSender Notify(string message);
    }
}
