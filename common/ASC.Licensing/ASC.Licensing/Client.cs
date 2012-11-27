// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="Client.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ASC.Licensing.Utils;

namespace ASC.Licensing
{
    public interface IClient
    {
        byte[] GetCertificateExports();
        byte[] GetSerialNumber();
        Uri GetLicenseServerUri();
    }

    internal sealed class Client : IClient
    {
        private readonly X509Certificate2 _clientCertificate;
        private readonly byte[] _clientId;
        private readonly byte[] _export;

        public Client()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var storeCertificates = store.Certificates.Cast<X509Certificate2>()
                .Where(x => CertSubject.Parse(x.Subject).Get(CertSubject.KnownField.CanonicalName) == "www.teamlab.com")
                .Where(x => x.HasPrivateKey)
                .Where(x => x.NotAfter > DateTime.UtcNow)
                .Where(x => x.NotBefore < DateTime.UtcNow)
                .OrderByDescending(x=>x.NotBefore)
                .ThenByDescending(x=>x.NotAfter);

            _clientCertificate = storeCertificates.FirstOrDefault(x=>x.Verify());
            if (_clientCertificate == null)
                throw new LicenseCertificateException("Can't find valid TM cert");


            if (!_clientCertificate.HasPrivateKey)
                throw new LicenseCertificateException("Client certificate should conaint PK");

            _export = _clientCertificate.Export(X509ContentType.Cert);
            //Check
            var test = new X509Certificate2(_export);
            if (test.HasPrivateKey)
                throw new LicenseCertificateException("Exported certificate shouldn't conaint PK");

            _clientId = _clientCertificate.GetSerialNumber();
        }

        public byte[] GetCertificateExports()
        {
            return _export;
        }

        public byte[] GetSerialNumber()
        {
            return _clientId;
        }

        public Uri GetLicenseServerUri()
        {
            return new Uri("http://localhost:4779/license.do"); //TODO:Think where to move it!
        }

        public byte[] Decrypt(byte[] data)
        {
            var rsaProvider = (RSACryptoServiceProvider) _clientCertificate.PrivateKey;
            return new RSASigner(rsaProvider).Decrypt(data);
        }
    }
}