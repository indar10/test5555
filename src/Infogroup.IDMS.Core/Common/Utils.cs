using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Infogroup.IDMS.Configuration;
using Abp.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using System.Configuration;

namespace Infogroup.IDMS.Common
{
    public class Utils:DomainService
    {
        private readonly IConfigurationRoot _appConfiguration;
        public const string saltValue = "s1234&&8lue";
        public const string hashAlgorithm = "SHA1";
        public const int passwordIterations = 2;
        public const string initVector = "@1B2c3D4e5F6g7H8";
        public const int keySize = 256;

        public Utils(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        /// <summary>
        /// Decrypt a password
        /// </summary>
        /// <param name="cipherText">The text to be decrypted </param>
        /// <returns>Decrypted plain text</returns>
        public string Decrypt(string cipherText)
        {
            var passPhrase = _appConfiguration.GetValue<string>("Utils:passPhrase");

            PasswordDeriveBytes password = null;
            RijndaelManaged symmetricKey = null;
            ICryptoTransform decryptor = null;
            CryptoStream cryptoStream = null;
            MemoryStream memoryStream = null;
            int decryptedByteCount;
            byte[] plainTextBytes;
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                password = new PasswordDeriveBytes(
                   passPhrase,
                   saltValueBytes,
                   hashAlgorithm,
                   passwordIterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);
                symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                decryptor = symmetricKey.CreateDecryptor(
                   keyBytes,
                   initVectorBytes);
                memoryStream = new MemoryStream(cipherTextBytes);
                cryptoStream = new CryptoStream(memoryStream,
                   decryptor,
                   CryptoStreamMode.Read);

                plainTextBytes = new byte[cipherTextBytes.Length];

                decryptedByteCount = cryptoStream.Read(plainTextBytes,
                    0,
                    plainTextBytes.Length);
            }
            finally
            {
                if (memoryStream != null)
                    memoryStream.Close();
                if (cryptoStream != null)
                    cryptoStream.Close();
                if (decryptor != null)
                    decryptor.Dispose();
                if (symmetricKey != null)
                    symmetricKey.Clear();
                symmetricKey = null;
            }

            var plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }

        /// <summary>
        /// Text to encryt
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="passPhrase"></param>
        /// <returns>Encrypted text</returns>
        public string Encrypt(string plainText, string passPhrase)
        {
            // string passPhrase = "Pa23%%pr@se";
            //string passPhrase = ConfigurationManager.AppSettings["SHApass"];
            PasswordDeriveBytes password = null;
            RijndaelManaged symmetricKey = null;
            ICryptoTransform encryptor = null;
            CryptoStream cryptoStream = null;
            MemoryStream memoryStream = null;
            byte[] cipherTextBytes;
            try
            {
                var initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                var saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                using (password = new PasswordDeriveBytes(
                    passPhrase,
                    saltValueBytes,
                    hashAlgorithm,
                    passwordIterations))
                {
                    var keyBytes = password.GetBytes(keySize / 8);
                    symmetricKey = new RijndaelManaged();
                    symmetricKey.Mode = CipherMode.CBC;
                    encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                    memoryStream = new MemoryStream();
                    using (cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memoryStream.ToArray();
                    }
                }
            }
            finally
            {
                if (memoryStream != null)
                    memoryStream.Close();
                if (cryptoStream != null)
                    cryptoStream.Close();
                if (encryptor != null)
                    encryptor.Dispose();
                if (symmetricKey != null)
                    symmetricKey.Clear();
                symmetricKey = null;
            }
            var cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }

        public string IDMSEncrypt(string plainText)
        {
            // string passPhrase = "Pa23%%pr@se";
            //string passPhrase = ConfigurationManager.AppSettings["SHApass"];
            var passPhrase = _appConfiguration.GetValue<string>("Utils:passPhrase");
            string saltValue = "s1234&&8lue";
            string hashAlgorithm = "SHA1";
            int passwordIterations = 2;
            string initVector = "@1B2c3D4e5F6g7H8";
            int keySize = 256;
            PasswordDeriveBytes password = null;
            RijndaelManaged symmetricKey = null;
            ICryptoTransform encryptor = null;
            CryptoStream cryptoStream = null;
            MemoryStream memoryStream = null;
            byte[] cipherTextBytes;
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                password = new PasswordDeriveBytes(
                    passPhrase,
                    saltValueBytes,
                    hashAlgorithm,
                    passwordIterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);

                symmetricKey = new RijndaelManaged();

                symmetricKey.Mode = CipherMode.CBC;

                encryptor = symmetricKey.CreateEncryptor(
                    keyBytes,
                    initVectorBytes);

                memoryStream = new MemoryStream();

                cryptoStream = new CryptoStream(memoryStream,
                    encryptor,
                    CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

                cryptoStream.FlushFinalBlock();

                cipherTextBytes = memoryStream.ToArray();
            }
            finally
            {
                if (memoryStream != null)
                    memoryStream.Close();
                if (cryptoStream != null)
                    cryptoStream.Close();
                if (encryptor != null)
                    encryptor.Dispose();
                if (symmetricKey != null)
                    symmetricKey.Clear();
                symmetricKey = null;
            }
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }

        public static string IDMSDecrypt(string cipherText)
        {
            string passPhrase = "Pa23%%pr@se";
            //string passPhrase = ConfigurationManager.AppSettings["SHApass"];

            string saltValue = "s1234&&8lue";
            string hashAlgorithm = "SHA1";
            int passwordIterations = 2;
            string initVector = "@1B2c3D4e5F6g7H8";
            int keySize = 256;
            PasswordDeriveBytes password = null;
            RijndaelManaged symmetricKey = null;
            ICryptoTransform decryptor = null;
            CryptoStream cryptoStream = null;
            MemoryStream memoryStream = null;
            int decryptedByteCount;
            byte[] plainTextBytes;
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                password = new PasswordDeriveBytes(
                   passPhrase,
                   saltValueBytes,
                   hashAlgorithm,
                   passwordIterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);
                symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                decryptor = symmetricKey.CreateDecryptor(
                   keyBytes,
                   initVectorBytes);
                memoryStream = new MemoryStream(cipherTextBytes);
                cryptoStream = new CryptoStream(memoryStream,
                   decryptor,
                   CryptoStreamMode.Read);

                plainTextBytes = new byte[cipherTextBytes.Length];

                decryptedByteCount = cryptoStream.Read(plainTextBytes,
                    0,
                    plainTextBytes.Length);
            }
            finally
            {
                if (memoryStream != null)
                    memoryStream.Close();
                if (cryptoStream != null)
                    cryptoStream.Close();
                if (decryptor != null)
                    decryptor.Dispose();
                if (symmetricKey != null)
                    symmetricKey.Clear();
                symmetricKey = null;
            }

            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }
    }
}