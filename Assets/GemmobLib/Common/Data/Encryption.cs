using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Gemmob.Common.Data {
    public static class Encryption {
        // IMPORTANT: Make sure to change this key for each project you use this encryption in to help secure your
        // encrypted values. This key must be exactly 32 characters long (256 bit).

        private static RijndaelManaged provider = null;

        private static void SetupProvider() {
            provider = new RijndaelManaged();

            var identifier = Application.identifier;
            var substring = identifier.Substring(0, Math.Min(identifier.Length - 1, 31));
            var key = substring.PadRight(32, '@');
            Logs.LogFormat("Simple Encrypt key: {0}", key);
            provider.Key = Encoding.ASCII.GetBytes(key);
            provider.Mode = CipherMode.ECB;
        }


        public static string EncryptString(string sourceString) {
            if (provider == null) {
                SetupProvider();
            }

            ICryptoTransform encryptor = provider.CreateEncryptor();

            byte[] sourceBytes = Encoding.UTF8.GetBytes(sourceString);

            byte[] outputBytes = encryptor.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);

            return Convert.ToBase64String(outputBytes);
        }


        public static string DecryptString(string sourceString) {
            if (provider == null) {
                SetupProvider();
            }

            ICryptoTransform decryptor = provider.CreateDecryptor();

            byte[] sourceBytes = Convert.FromBase64String(sourceString);

            byte[] outputBytes = decryptor.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);

            return Encoding.UTF8.GetString(outputBytes);
        }

        public static string EncryptFloat(float value) {
            byte[] bytes = BitConverter.GetBytes(value);
            string base64 = Convert.ToBase64String(bytes);
            return EncryptString(base64);
        }


        public static string EncryptInt(int value) {
            byte[] bytes = BitConverter.GetBytes(value);
            string base64 = Convert.ToBase64String(bytes);
            return EncryptString(base64);
        }


        public static float DecryptFloat(string sourceString) {
            var decryptedString = DecryptString(sourceString);
            var bytes = Convert.FromBase64String(decryptedString);
            return BitConverter.ToSingle(bytes, 0);
        }


        public static int DecryptInt(string sourceString) {
            var decryptedString = DecryptString(sourceString);
            var bytes = Convert.FromBase64String(decryptedString);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}