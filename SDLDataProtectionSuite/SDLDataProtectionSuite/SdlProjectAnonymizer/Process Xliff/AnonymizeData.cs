using System;
using System.Security.Cryptography;
using System.Text;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff
{
	public static class AnonymizeData
	{
		public static string EncryptData(string textData, string encryptionkey)
		{
			var objrij = new RijndaelManaged
			{
				//set the mode for operation of the algorithm
				Mode = CipherMode.CBC,
				//set the padding mode used in the algorithm.
				Padding = PaddingMode.PKCS7,
				//set the size, in bits, for the secret key.
				KeySize = 0x80,
				//set the block size in bits for the cryptographic operation.
				BlockSize = 0x80
			};

			//set the symmetric key that is used for encryption & decryption.
			var passBytes = Encoding.UTF8.GetBytes(encryptionkey);
			//set the initialization vector (IV) for the symmetric algorithm
			var encryptionkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

			var len = passBytes.Length;
			if (len > encryptionkeyBytes.Length)
			{
				len = encryptionkeyBytes.Length;
			}

			Array.Copy(passBytes, encryptionkeyBytes, len);

			objrij.Key = encryptionkeyBytes;
			objrij.IV = encryptionkeyBytes;

			//Creates symmetric AES object with the current key and initialization vector IV.
			var objtransform = objrij.CreateEncryptor();
			var textDataByte = Encoding.UTF8.GetBytes(textData);

			//Final transform the test string.
			return Convert.ToBase64String(objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));
		}

		public static string DecryptData(string encryptedText, string encryptionkey)
		{
			var objrij = new RijndaelManaged
			{
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7,
				KeySize = 0x80,
				BlockSize = 0x80
			};

			var encryptedTextByte = Convert.FromBase64String(encryptedText);
			var passBytes = Encoding.UTF8.GetBytes(encryptionkey);
			var encryptionkeyBytes = new byte[0x10];
			var len = passBytes.Length;
			if (len > encryptionkeyBytes.Length)
			{
				len = encryptionkeyBytes.Length;
			}

			Array.Copy(passBytes, encryptionkeyBytes, len);
			objrij.Key = encryptionkeyBytes;
			objrij.IV = encryptionkeyBytes;
			var textByte = objrij.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);

			return Encoding.UTF8.GetString(textByte);  //it will return readable string
		}
	}
}