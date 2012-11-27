using System.IO;
using System.Reflection;
using System.Resources;

namespace ASC.FederatedLogin.Helpers
{
    public class JsCallbackHelper
    {
        public static string GetCallbackPage()
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ASC.FederatedLogin.callback.htm")))
                return reader.ReadToEnd();
        }
    }
}