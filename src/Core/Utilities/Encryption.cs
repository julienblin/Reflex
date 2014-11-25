// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Encryption.cs" company="CGI">
//   Copyright (c) CGI. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CGI.Reflex.Core.Utilities
{
    public static class Encryption
    {
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("vNHP2JcnRoJLTKxXg4oV");

        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes", Justification = "Classes are resilient to multiple Dispose() calls")]
        public static string Encrypt(string strToEncrypt)
        {
            if (string.IsNullOrEmpty(References.ReferencesConfiguration.EncryptionKey))
                throw new Exception("To use encryption you must initialize an EncryptionKey in ReferencesConfiguration.");

            using (var key = new Rfc2898DeriveBytes(References.ReferencesConfiguration.EncryptionKey, Salt))
            {
                using (var aesAlg = new AesManaged())
                {
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                    var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (var streamEncrypt = new MemoryStream())
                    {
                        using (var streamWriterEncrypt = new StreamWriter(new CryptoStream(streamEncrypt, encryptor, CryptoStreamMode.Write)))
                        {
                            streamWriterEncrypt.Write(strToEncrypt);
                        }

                        return Convert.ToBase64String(streamEncrypt.ToArray());
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes", Justification = "Classes are resilient to multiple Dispose() calls")]
        public static string Decrypt(string strToDecrypt)
        {
            if (string.IsNullOrEmpty(References.ReferencesConfiguration.EncryptionKey))
                throw new Exception("To use encryption you must initialize an EncryptionKey in ReferencesConfiguration.");

            using (var key = new Rfc2898DeriveBytes(References.ReferencesConfiguration.EncryptionKey, Salt))
            {
                using (var aesAlg = new AesManaged())
                {
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    var bytes = Convert.FromBase64String(strToDecrypt);
                    using (var streamDecrypt = new MemoryStream(bytes))
                    {
                        using (var streamReaderDecrypt = new StreamReader(new CryptoStream(streamDecrypt, decryptor, CryptoStreamMode.Read)))
                        {
                            return streamReaderDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string Hash(string value)
        {
            if (string.IsNullOrEmpty(References.ReferencesConfiguration.EncryptionKey))
                throw new Exception("To use encryption you must initialize an EncryptionKey in ReferencesConfiguration.");

            using (var key = new Rfc2898DeriveBytes(References.ReferencesConfiguration.EncryptionKey, Salt))
            {
                using (var sha = new HMACSHA256(key.GetBytes(32)))
                {
                    return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(value)));
                }
            }
        }

        public static string GenerateRandomToken(int maxLength)
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                var randomNumber = new byte[maxLength];
                cryptoProvider.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber).MaxLength(maxLength);
            }
        }
    }
}
