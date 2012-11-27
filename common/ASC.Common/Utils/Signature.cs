using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ASC.Security.Cryptography;
using NUnit.Framework;

namespace ASC.Common.Utils
{
    public static class Signature
    {
        public static string Create<T>(T obj)
        {
            return Create<T>(obj, Encoding.UTF8.GetString(MachinePseudoKeys.GetMachineConstant()));
        }

        public static string Create<T>(T obj, string secret)
        {
            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(obj);
            var payload = GetHashBase64(str + secret) + "|" + str;
            return HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(payload));
        }

        private static string GetHashBase64(string str)
        {
            return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        public static T Read<T>(string signature)
        {
            return Read<T>(signature, Encoding.UTF8.GetString(MachinePseudoKeys.GetMachineConstant()));
        }

        public static T Read<T>(string signature, string secret)
        {
            try
            {
                var payloadParts = Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(signature)).Split('|');
                if (GetHashBase64(payloadParts[1] + secret) == payloadParts[0])
                {
                    //Sig correct
                    return new JavaScriptSerializer().Deserialize<T>(payloadParts[1]);
                }
            }
            catch (Exception)
            {
                
            }
            return default(T);
        }
    }

#if (DEBUG)
    [TestFixture]
    public class SignatureTest
    {
        [Test]
        public void TestSignature()
        {
            var validObject = new { expire = DateTime.UtcNow.AddMinutes(15), key = "345jhndfg", ip = "192.168.1.1" };
            var encoded = Signature.Create(validObject, "ThE SeCret Key!");
            Assert.IsNotNull( Signature.Read<object>(encoded, "ThE SeCret Key!"));
            Assert.IsNull(Signature.Read<object>(encoded, "ThE SeCret Key"));
        }
    }
#endif
}