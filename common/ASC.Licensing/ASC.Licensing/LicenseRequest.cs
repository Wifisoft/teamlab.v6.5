// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="LicenseRequest.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ASC.Licensing.Utils;

namespace ASC.Licensing
{
    public interface ILicenseRequest
    {
        RequestType Type { get; }
        Uri GetUri(IClient client);
        byte[] ClientId { get; }
        byte[] Certificate { get; }
        byte[] LicenseKey { get;}


        string ToOfflineData();
        WebRequest ToWebRequest(IClient client);
    }

    internal sealed class LicenseRequest : ILicenseRequest
    {
        #region RequestType enum

        #endregion

        private LicenseRequest(RequestType type)
        {
            Type = type;
        }

        public LicenseRequest(RequestType type, IClient client, byte[] licenseKey)
            : this(type, client.GetSerialNumber(), client.GetCertificateExports(), licenseKey)
        {
        }

        public LicenseRequest(RequestType type, byte[] clientId, byte[] payload, byte[] licenseKey)
        {
            if (type == RequestType.Activate)
            {
                if (payload == null) throw new ArgumentNullException("payload");
                if (licenseKey == null) throw new ArgumentNullException("licenseKey");
            }

            Type = type;
            Certificate = payload;
            LicenseKey = licenseKey;
            ClientId = clientId;
        }

        public RequestType Type { get; private set; }
        public byte[] Certificate { get; private set; }
        public byte[] LicenseKey { get; private set; }
        public byte[] ClientId { get; private set; }

        public Uri GetUri(IClient client)
        {
            if (client == null) throw new ArgumentNullException("client");

            var builder = new UriBuilder(client.GetLicenseServerUri());
            //build query string
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendFormat("{1}={0}&", Uri.EscapeDataString(Type.ToString()), "type");
            queryBuilder.AppendFormat("{1}={0}&", Uri.EscapeDataString(ClientId.Encode()), "cid");
            //Make sign
            queryBuilder.AppendFormat("{1}={0}",
                                      Uri.EscapeDataString(GetValidationKey(Type.ToString(), ClientId.Encode(),
                                                                            HashSecret.GetSecret())), "sign");
            builder.Query = queryBuilder.ToString();
            return builder.Uri;
        }

        public string ToOfflineData()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(memoryStream))
                {
                    var header =(byte)((byte)Type << 7 | ClientId.Length);
                    writer.Write(header);
                    writer.Write(ClientId);
                    if (Type==RequestType.Activate)
                    {
                        //Write cert and license data
                        writer.Write((ushort)Certificate.Length);
                        writer.Write(Certificate);
                        if (LicenseKey.Length > 0)
                        {
                            writer.Write((ushort) LicenseKey.Length);
                            writer.Write(LicenseKey);
                        }
                    }
                    var data = memoryStream.ToArray();
                    var signBuffer = RSASigner.GetSignBuffer(data, new[] {HashSecret.GetSecret()});
                    writer.Write(SHA512.Create().ComputeHash(signBuffer));
                    return DataEncoder.ToHexString(memoryStream.ToArray());
                }
            }
        }

        public static LicenseRequest FromOfflineData(string data)
        {
            using (var memoryStream = new MemoryStream(DataEncoder.FromHexString(data)))
            {
                using (var reader = new BinaryReader(memoryStream))
                {
                    var header = reader.ReadByte();
                    var type = (RequestType)(header>>7);
                    var cidLength = header ^ (byte)type << 7;
                    var request = new LicenseRequest(type) {ClientId = reader.ReadBytes(cidLength)};
                    if (type==RequestType.Activate)
                    {
                        request.Certificate = reader.ReadBytes(reader.ReadUInt16());
                        if (memoryStream.Position < memoryStream.Length - 66/*UInt32+SHA512*/)
                        {
                            var licenseLength = reader.ReadUInt16();
                            if (licenseLength > 0)
                            {
                                request.LicenseKey = reader.ReadBytes(licenseLength);
                            }
                        }
                        else
                        {
                            request.LicenseKey=new byte[0];
                        }
                    }
                    var dataEnd = memoryStream.Position;
                    var sign = reader.ReadBytes(64);
                    //Verify sign
                    memoryStream.Position = 0;
                    var databuffer = reader.ReadBytes((int) dataEnd);
                    var signBuffer = RSASigner.GetSignBuffer(databuffer, new[] { HashSecret.GetSecret() });
                    var signToValidate= SHA512.Create().ComputeHash(signBuffer);
                    if (sign.Where((t, i) => t!=signToValidate[i]).Any())
                    {
                        throw new LicenseValidationException("Signatures doesn't match");
                    }
                    return request;
                }
            }
        }

        public WebRequest ToWebRequest(IClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            //Make a request
            WebRequest request = WebRequest.Create(GetUri(client));
            request.Method = "GET";
            if (Type == RequestType.Activate)
            {
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (Stream requestStream = request.GetRequestStream())
                {
                    using (var writer = new StreamWriter(requestStream))
                    {
                        writer.Write(string.Format("{1}={0}&",
                                                   DataEncoder.ToString(client.GetCertificateExports()),
                                                   "cert"));
                        writer.Write(string.Format("{1}={0}", DataEncoder.ToString(LicenseKey),
                                                   "lid"));
                    }
                }
            }
            return request;
        }

        public static LicenseRequest FromNameValueCollection(NameValueCollection collection)
        {
            string typeString = collection["type"];
            string clientIdString = collection["cid"];
            string key = collection["sign"];

            //Validate key first
            string validationKey = GetValidationKey(typeString, clientIdString, HashSecret.GetSecret());

            if (!string.Equals(validationKey, key, StringComparison.Ordinal))
                throw new SecurityException("Validation keys doesn't match");

            var type = (RequestType)Enum.Parse(typeof(RequestType), typeString, true);
            var clientId = DataEncoder.FromString(clientIdString);


            if (type == RequestType.Activate && (collection["cert"] == null || collection["lid"] == null))
                throw new ArgumentException(
                    "Request form must contain certificate information and license id when activating");

            var licenseRequest = new LicenseRequest(type) { ClientId = clientId };

            if (type == RequestType.Activate)
            {
                licenseRequest.Certificate = DataEncoder.FromString(collection["cert"]);
                licenseRequest.LicenseKey = DataEncoder.FromString(collection["lid"]);
            }
            return licenseRequest;
        }

        public static LicenseRequest FromHttpRequest(HttpRequestBase request)
        {
            string typeString = request["type"];
            if (string.IsNullOrEmpty(typeString))
                throw new ArgumentNullException("request[type]", "RequestType can't be empty");

            var type = (RequestType)Enum.Parse(typeof(RequestType), typeString, true);

            if (type == RequestType.Activate && request.ContentLength == 0)
                throw new ArgumentException("Request body can't be empty when activating");

            if (type == RequestType.Activate && request.Form.Count == 0)
                throw new ArgumentException("Request form can't be empty when activating");

            if (type == RequestType.Activate && (request.Form["cert"] == null || request.Form["lid"] == null))
                throw new ArgumentException(
                    "Request form must contain certificate information and license id when activating");

            var nameValue = new NameValueCollection(request.QueryString);
            nameValue.Add(request.Form);
            return FromNameValueCollection(nameValue);
        }

        private static string GetValidationKey(string typeString, string clientIdString, byte[] secret)
        {
            string toValidate = string.Format("{0}|{1}|{2}|{3}",
                                              typeString,
                                              clientIdString,
                                              DateTime.UtcNow.ToString("MM-yyyy", CultureInfo.InvariantCulture),/*For time shifting on client*/
                                              DataEncoder.ToString(secret));

            SHA1 hash = SHA1.Create();
            string validationKey = DataEncoder.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(toValidate)));
            return validationKey;
        }
    }

    public enum RequestType
    {
        Activate=0,
        License=1
    }
}