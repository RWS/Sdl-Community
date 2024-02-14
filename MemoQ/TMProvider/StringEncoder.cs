using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Security.Cryptography;

namespace TMProvider
{
    // https://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndaelmanaged.aspx
    public static class StringEncoder
    {
        public static string EncryptText(string text)
        {
            if (text == null) return null;
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                // Encrypt the string to an array of bytes. 
                byte[] encrypted = encryptStringToBytes(text, AppData.RijndaelKey, AppData.RijndaelIV); // encryptStringToBytes(text, myRijndael.Key, myRijndael.IV);
                string encodedText = Convert.ToBase64String(encrypted); // Encoding.UTF8.GetString(encrypted);
                return encodedText;
            }
        }

        public static string DecryptText(string encryptedText)
        {
            try
            {
                if (encryptedText == null) return "";
                using (RijndaelManaged myRijndael = new RijndaelManaged())
                {
                    byte[] original = Convert.FromBase64String(encryptedText); // Encoding.UTF8.GetBytes(encryptedText);

                    // Decrypt the bytes to a string. 
                    string decodedText = decryptStringFromBytes(original, AppData.RijndaelKey, AppData.RijndaelIV); // decryptStringFromBytes(original, myRijndael.Key, myRijndael.IV);

                    return decodedText;
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }

        private static byte[] encryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length == 0)
                throw new ArgumentNullException("No text to encode.");
            if (Key == null || Key.Length == 0)
                throw new ArgumentNullException("Missing encryption key.");
            if (IV == null || IV.Length == 0)
                throw new ArgumentNullException("Missing encryption IV");
            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }

        private static string decryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("No text to decode.");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Missing encryption key.");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Missing encryption IV.");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }

    }
}
