#if DEBUG
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using ASC.Security.Cryptography;

namespace ASC.Common.Tests.Security.Cryptography
{
    [TestFixture]
    public class Hasher_Test
    {
        [Test]
        public void DoHash()
        {
            string str = "Привет, дядя Ваня!!";
            
            Assert.AreEqual(
                Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
                Hasher.Base64Hash(str,HashAlg.MD5)
                );

            Assert.AreEqual(
               Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
               Hasher.Base64Hash(str, HashAlg.SHA1)
               );

            Assert.AreEqual(
               Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
               Hasher.Base64Hash(str, HashAlg.SHA256)
               );

            Assert.AreEqual(
               Convert.ToBase64String(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
               Hasher.Base64Hash(str, HashAlg.SHA512)
               );

            Assert.AreEqual(
              Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(str))),
              Hasher.Base64Hash(str) //DEFAULT
              );
        }
    }
}
#endif