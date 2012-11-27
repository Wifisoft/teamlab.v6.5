// // --------------------------------------------------------------------------------------------------------------------
// // <copyright company="Ascensio System Limited" file="RSASigner.cs">
// //   
// // </copyright>
// // <summary>
// //   (c) Copyright Ascensio System Limited 2008-2012
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ASC.Licensing.Utils
{
    internal sealed class RSASigner
    {
        private const int Padding = 11;
        private readonly HashAlgorithm _algorithm;
        private readonly int _blockLength = 128;
        private readonly int _maxBlockLengthWithPadding = 128 - Padding;
        private readonly RSACryptoServiceProvider _rsaCrypto = new RSACryptoServiceProvider();
        private readonly object _synchLock = new object();


        public RSASigner(Stream snkStream)
        {
            if (snkStream != null)
            {
                var buffer = new byte[snkStream.Length];
                snkStream.Read(buffer, 0, buffer.Length);
                _rsaCrypto = EncryptionUtils.GetRSAFromSnkBytes(buffer);
            }
            else
            {
                _rsaCrypto = EncryptionUtils.GetPublicKeyFromAssembly(typeof (RSASigner).Assembly);
            }
            _blockLength = _rsaCrypto.ExportParameters(false).Modulus.Length;
            _maxBlockLengthWithPadding = _blockLength - Padding;
            _algorithm = SHA1.Create();
        }

        public RSASigner()
            : this((Stream)null)
        {
        }

        public RSASigner(RSACryptoServiceProvider provider)
        {
            _rsaCrypto = provider;
            _blockLength = _rsaCrypto.ExportParameters(false).Modulus.Length;
            _maxBlockLengthWithPadding = _blockLength - Padding;
            _algorithm = SHA1.Create();
        }

        public byte[] Encrypt(byte[] data)
        {
            lock (_synchLock)
            {
                try
                {
                    if (data.Length > _maxBlockLengthWithPadding)
                    {
                        int iterations = data.Length/_maxBlockLengthWithPadding;
                        var output = new byte[(iterations + 1)*_blockLength];
                        for (int i = 0; i <= iterations; i++)
                        {
                            var tempBytes =
                                new byte[
                                    (data.Length - _maxBlockLengthWithPadding*i > _maxBlockLengthWithPadding)
                                        ? _maxBlockLengthWithPadding
                                        : data.Length - _maxBlockLengthWithPadding*i];
                            Buffer.BlockCopy(data, _maxBlockLengthWithPadding*i, tempBytes, 0, tempBytes.Length);
                            byte[] encryptedBytes = _rsaCrypto.Encrypt(tempBytes, false);
                            // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes after encryption and before decryption.
                            Buffer.BlockCopy(encryptedBytes, 0, output, _blockLength*i, _blockLength);
                        }
                        return output;
                    }
                    return _rsaCrypto.Encrypt(data, false);
                }
                catch (CryptographicException)
                {
                    return new byte[0];
                }
            }
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            int blockSize = _blockLength , index = 0, bytesLeft = encryptedData.Length;

            using (var memDecryptedTextbuffer = new MemoryStream(bytesLeft/2))
            {
                //Split the serialized data into smaller blocks for processing
                while (bytesLeft > 0)
                {
                    //If the blocksize is too large, set it to the required amount
                    if (bytesLeft < blockSize)
                        blockSize = bytesLeft;

                    //Get a block from the encrypted data
                    byte[] block = BlockCopy(encryptedData, index, blockSize);
                    byte[] decryptedBlock = _rsaCrypto.Decrypt(block, false);
                    memDecryptedTextbuffer.Write(decryptedBlock, 0, decryptedBlock.Length);

                    //Update position and size tracking
                    index += blockSize;
                    bytesLeft -= blockSize;
                }
                //Dump the encrypted data to the caller
                memDecryptedTextbuffer.Position = 0;
                return memDecryptedTextbuffer.ToArray();
            }
        }

        private byte[] BlockCopy(byte[] source, int startAt, int size)
        {
            if ((source == null) || (source.Length < (startAt + size)))
                return null;

            var ret = new byte[size];
            Buffer.BlockCopy(source, startAt, ret, 0, size);
            return ret;
        }


        public bool Validate(byte[] payload, byte[] sign)
        {
            return _rsaCrypto.VerifyHash(_algorithm.ComputeHash(payload), null, sign);
        }

        public byte[] Sign(byte[] payload)
        {
            if (payload == null) throw new ArgumentNullException("payload");

            lock (_synchLock)
            {
                return _rsaCrypto.SignHash(_algorithm.ComputeHash(payload), null);
            }
        }

        public static byte[] GetSignBuffer(byte[] data,IEnumerable<byte[]> salts)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (salts == null) throw new ArgumentNullException("salts");
            
            var toSignBuffer = new byte[data.Length + salts.Sum(x=>x.Length)];
            data.CopyTo(toSignBuffer, 0);
            var currentIndex = data.Length;
            foreach (var salt in salts.Where(x=>x.Length>0))
            {
                salt.CopyTo(toSignBuffer, currentIndex);
                currentIndex += salt.Length;
            }
            return toSignBuffer;
        }
    }
}