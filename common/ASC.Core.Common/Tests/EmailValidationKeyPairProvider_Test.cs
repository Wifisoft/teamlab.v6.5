#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASC.Common.Tests.Security.Cryptography
{
    using NUnit.Framework;
    using ASC.Security.Cryptography;
    using System.Security.Cryptography;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class EmailValidationKeyPairProvider_Test
    {
        public void PasswordDerivedBytes_Test()
        {

            byte[] randBytes = new byte[5];
            new Random(10032010).NextBytes(randBytes);
            

            var tdes = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
            var pwddb = new System.Security.Cryptography.PasswordDeriveBytes("1", new byte[] { 1 });
            tdes.Key = pwddb.CryptDeriveKey("TripleDES", "SHA1", 192, tdes.IV);
            //string s = Convert.ToBase64String(tdes.Key);

        }

        [Test]
        public void Test_MConst()
        {
            var b1 = EmailValidationKeyProvider.GetMachineConstantInternal();
            var b2 = EmailValidationKeyProvider.GetMachineConstantInternal();

            var s1 = Convert.ToBase64String(b1);
            var s2 = Convert.ToBase64String(b2);

            Assert.That(s1,Is.EqualTo(s2));
        }

        [Test]
        public void GetEmailKey_MillisecondDistanceDifference()
        {
            var k1 = EmailValidationKeyProvider.GetEmailKey("sm_anton@mail.ru");
            System.Threading.Thread.Sleep(15);
            var k2 = EmailValidationKeyProvider.GetEmailKey("sm_anton@mail.ru");

            Assert.That(k1, Is.Not.EqualTo(k2));
        }

        [Test]
        public void ValidateKeyImmediate()
        {
            var k1 = EmailValidationKeyProvider.GetEmailKey("sm_anton@mail.ru");
            Assert.That(EmailValidationKeyProvider.ValidateEmailKey("sm_anton@mail.ru", k1)==EmailValidationKeyProvider.ValidationResult.Ok);
            Assert.That(EmailValidationKeyProvider.ValidateEmailKey("sm_anton@mail.ru2", k1) == EmailValidationKeyProvider.ValidationResult.Invalid);
        }

        [Test]
        public void ValidateKey_Delayed()
        {
            var k1 = EmailValidationKeyProvider.GetEmailKey("sm_anton@mail.ru");
            System.Threading.Thread.Sleep(100);
            Assert.That(EmailValidationKeyProvider.ValidateEmailKey("sm_anton@mail.ru", k1,TimeSpan.FromMilliseconds(150))==EmailValidationKeyProvider.ValidationResult.Ok);
            System.Threading.Thread.Sleep(100);
            Assert.That(EmailValidationKeyProvider.ValidateEmailKey("sm_anton@mail.ru", k1, TimeSpan.FromMilliseconds(150)) == EmailValidationKeyProvider.ValidationResult.Expired);
        }
    }
}
#endif