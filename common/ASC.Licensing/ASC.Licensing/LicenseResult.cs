// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="LicenseResult.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using ASC.Licensing.Utils;

namespace ASC.Licensing
{
    internal sealed class LicenseResult
    {
        public LicenseResult(byte[] licenseData, Client client)
        {
            try
            {
                var rsaSigner = new RSASigner();
                //We always should have the same format
                using (var sourceStream = new MemoryStream(licenseData))
                using (var reader = new BinaryReader(sourceStream))
                {
                    long dataLength = reader.ReadInt64();
                    byte[] data = reader.ReadBytes((int) dataLength);
                    byte[] sign = reader.ReadBytes((int) (licenseData.Length-8-dataLength));

                    var signBuffer = RSASigner.GetSignBuffer(data,new[]{BitConverter.GetBytes(DateTime.UtcNow.Year),client.GetSerialNumber()});
                    bool valid = rsaSigner.Validate(signBuffer, sign);
                    if (!valid)
                        throw new LicenseValidationException("License sign is invalid (maybe clock shifting)");
                    LicenseData = client.Decrypt(data);
                }
            }
            catch (Exception e)
            {
                throw new LicenseException("License corrupted", e);
            }
        }

        public byte[] LicenseData { get; private set; }

        public string AsString()
        {
            return Encoding.UTF8.GetString(LicenseData);
        }

        public Stream AsStream()
        {
            return new MemoryStream(LicenseData);
        }

        public string ToOfflineString()
        {
            return DataEncoder.ToHexString(LicenseData);
        }

        public static LicenseResult FromOfflineString(string offlineString, Client client)
        {
            return new LicenseResult(DataEncoder.FromHexString(offlineString),client);
        }
    }

   
}